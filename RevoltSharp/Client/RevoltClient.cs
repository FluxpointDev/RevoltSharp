using Newtonsoft.Json;
using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using RevoltSharp.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt client used to connect to the Revolt chat API and WebSocket with a user or bot token.
/// </summary>
/// <remarks>
/// Docs: <see href="https://docs.fluxpoint.dev/revoltsharp"/>
/// </remarks>
public class RevoltClient : ClientEvents
{
    /// <summary>
    /// Create a Revolt client that can be used for user or bot accounts.
    /// </summary>
    /// <param name="token">Bot token to connect with.</param>
    /// <param name="mode">Use http for http requests only with no websocket.</param>
    /// <param name="config">Optional config stuff for the bot and lib.</param>
    public RevoltClient(string token, ClientMode mode, ClientConfig? config = null)
    {
        if (string.IsNullOrEmpty(token))
            throw new RevoltArgumentException("Client token is missing!");

        Token = token;
        Config = config ?? new ClientConfig();
        ConfigSafetyChecks();

        if (!Config.Debug.EnableConsoleQuickEdit)
        {
            try
            {
                DisableConsoleQuickEdit.Go();
            }
            catch { }
        }
        UserBot = Config.UserBot;
        Serializer = new JsonSerializer();
        SerializerSettings = new JsonSerializerSettings { ContractResolver = new RevoltContractResolver() };
        Serializer.ContractResolver = SerializerSettings.ContractResolver;

        Deserializer = new JsonSerializer();
        DeserializerSettings = new JsonSerializerSettings();

        OptionalDeserializerConverter Converter = new OptionalDeserializerConverter();
        DeserializerSettings.Converters.Add(Converter);
        Deserializer.Converters.Add(Converter);

        Rest = new RevoltRestClient(this);
        if (mode == ClientMode.WebSocket)
            WebSocket = new RevoltSocketClient(this);
    }

    private void ConfigSafetyChecks()
    {
        if (string.IsNullOrEmpty(Config.ApiUrl))
            throw new RevoltException("Config API Url is missing");

        if (!Config.ApiUrl.EndsWith('/'))
            Config.ApiUrl += "/";

        if (string.IsNullOrEmpty(Config.UserAgent))
            throw new RevoltException("Config UserAgent is missing");

        if (Config.Owners == null)
            Config.Owners = Array.Empty<string>();

        if (Config.Debug == null)
            Config.Debug = new ClientDebugConfig();
    }


    /// <summary>
    /// Revolt bot token used for http requests and websocket.
    /// </summary>
    public string Token { get; internal set; }

    /// <summary>
    /// Version of the current RevoltSharp lib installed.
    /// </summary>
    public string Version { get; } = "6.0.0";

    /// <summary>
    /// The current version of the revolt instance connected to.
    /// </summary>
    /// <remarks>
    /// This will be empty of you do not use <see cref="StartAsync" />.
    /// </remarks>
    public string? RevoltVersion { get; internal set; }

    internal bool UserBot { get; set; }

    public JsonSerializer Serializer { get; internal set; }
    public JsonSerializerSettings SerializerSettings { get; internal set; }
    public JsonSerializer Deserializer { get; internal set; }
    public JsonSerializerSettings DeserializerSettings { get; internal set; }

    /// <summary>
    /// Client config options for user-agent and debug options including self-host support.
    /// </summary>
    public ClientConfig Config { get; internal set; }

    /// <summary>
    /// Internal rest/http client used to connect to the Revolt API.
    /// </summary>
    /// <remarks>
    /// You can also make custom requests with <see cref="RevoltRestClient.SendRequestAsync(RequestType, string, IRevoltRequest)"/> and json class based on <see cref="IRevoltRequest"/>
    /// </remarks>
    public RevoltRestClient Rest { get; internal set; }

    internal RevoltSocketClient? WebSocket;

    internal bool FirstConnection = true;
    internal bool IsConnected = false;

    /// <summary>
    /// The current logged in user/bot account.
    /// </summary>
    /// <remarks>
    /// This will be <see langword="null" /> of you do not use <see cref="StartAsync" />.
    /// </remarks>
    public SelfUser? CurrentUser { get; internal set; }

    /// <summary>
    /// The current user/bot account's private notes message channel.
    /// </summary>
    /// <remarks>
    /// This will be <see langword="null" /> if you have not created the channel from <see cref="BotHelper.GetOrCreateSavedMessageChannelAsync(RevoltRestClient)" /> once.
    /// </remarks>
    public SavedMessagesChannel? SavedMessagesChannel { get; internal set; }

    /// <summary>
    /// Start the Rest and Websocket to be used for the lib.
    /// </summary>
    /// <remarks>
    /// Will throw a <see cref="RevoltException"/> if the token is incorrect or failed to login for the current user/bot.
    /// </remarks>
    /// <exception cref="RevoltException"></exception>
    /// <exception cref="RevoltArgumentException"></exception>
    public async Task StartAsync()
    {
        if (FirstConnection)
        {
            InvokeLog("Starting...", RevoltLogSeverity.Verbose);

            FirstConnection = false;
            QueryRequest? Query = await Rest.GetAsync<QueryRequest>("/");
            if (Query == null)
                InvokeLogAndThrowException($"Client failed to connect to the revolt api at {Config.ApiUrl}");

            if (!Uri.IsWellFormedUriString(Query.serverFeatures.imageServer.url, UriKind.Absolute))
                throw new RevoltException("Server Image server url is an invalid format.");

            RevoltVersion = Query.revoltVersion;
            Config.Debug.WebsocketUrl = Query.websocketUrl;
            Config.Debug.UploadUrl = Query.serverFeatures.imageServer.url;

            if (!Config.Debug.UploadUrl.EndsWith('/'))
                Config.Debug.UploadUrl += '/';

            UserJson? SelfUser = await Rest.GetAsync<UserJson>("/users/@me");
            if (SelfUser == null)
                throw new RevoltException("Failed to login to user account.");

            CurrentUser = new SelfUser(this, SelfUser);
            InvokeLog($"Started: {SelfUser.Username} ({SelfUser.Id})", RevoltLogSeverity.Standard);
            InvokeStarted(CurrentUser);
        }

        if (WebSocket != null)
        {
            TaskCompletionSource tcs = new TaskCompletionSource();

            void HandleConnected() => tcs.SetResult();
            void HandleError(SocketError error) => tcs.SetException(new RevoltException(error.Message));

            this.OnConnected += HandleConnected;
            this.OnWebSocketError += HandleError;

            _ = WebSocket.SetupWebsocket();

            await tcs.Task;
            this.OnConnected -= HandleConnected;
            this.OnWebSocketError -= HandleError;
        }
    }

    /// <summary>
    /// Stop the WebSocket connection to Revolt.
    /// </summary>
    /// <remarks>
    /// Will throw a <see cref="RevoltException"/> if <see cref="ClientMode.Http"/>.
    /// </remarks>
    /// <exception cref="RevoltException"></exception>
    public async Task StopAsync()
    {
        if (WebSocket == null)
            throw new RevoltException("Client is in http-only mode.");

        if (WebSocket.WebSocket != null)
        {
            WebSocket.StopWebSocket = true;
            await WebSocket.WebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "", WebSocket.CancellationToken);

        }
    }

    /// <summary>
    /// Get a list of <see cref="Server" />s from the websocket client.
    /// </summary>
    /// <remarks>
    /// Will be empty if <see cref="ClientMode.Http"/>.
    /// </remarks>
    public IReadOnlyCollection<Server> Servers
        => WebSocket != null ? (IReadOnlyCollection<Server>)WebSocket.ServerCache.Values : new ReadOnlyCollection<Server>(new List<Server>());

    /// <summary>
    /// Get a list of <see cref="User" />s from the websocket client.
    /// </summary>
    /// <remarks>
    /// Will be empty if <see cref="ClientMode.Http"/>.
    /// </remarks>
    public IReadOnlyCollection<User> Users
       => WebSocket != null ? (IReadOnlyCollection<User>)WebSocket.UserCache.Values : new ReadOnlyCollection<User>(new List<User>());

    /// <summary>
    /// Get a list of <see cref="Channel" />s from the websocket client.
    /// </summary>
    /// <remarks>
    /// Will be empty if <see cref="ClientMode.Http"/>.
    /// </remarks>
    public IReadOnlyCollection<Channel> Channels
        => WebSocket != null ? (IReadOnlyCollection<Channel>)WebSocket.ChannelCache.Values : new ReadOnlyCollection<Channel>(new List<Channel>());

    /// <summary>
    /// Get a list of <see cref="Emoji" />s from the websocket client.
    /// </summary>
    /// <remarks>
    /// Will be empty if <see cref="ClientMode.Http"/>.
    /// </remarks>
    public IReadOnlyCollection<Emoji> Emojis
        => WebSocket != null ? (IReadOnlyCollection<Emoji>)WebSocket.EmojiCache.Values : new ReadOnlyCollection<Emoji>(new List<Emoji>());




    internal TextChannel? GetTextChannel(Optional<string> channelId)
    {
        if (WebSocket != null && channelId.HasValue && !string.IsNullOrEmpty(channelId.Value) && WebSocket.ChannelCache.TryGetValue(channelId.Value, out Channel Chan) && Chan is TextChannel TC)
            return TC;
        return null;
    }






}

/// <summary>
/// Run the client with Http requests only or use websocket to get cached data such as servers, channels and users instead of just ids.
/// </summary>
/// <remarks>
/// Using <see cref="ClientMode.Http"/> means that some data will be <see langword="null"/> such as <see cref="Message.Author"/> and will only contain ids <see cref="Message.AuthorId"/>
/// </remarks>
public enum ClientMode
{
    /// <summary>
    /// Client will only use the http/rest client of Revolt and removes any usage/memory of websocket stuff. 
    /// </summary>
    Http,
    /// <summary>
    /// Will use both WebSocket and http/rest client so you can get cached data for <see cref="User"/>, <see cref="Server"/> and <see cref="Channel"/>
    /// </summary>
    WebSocket
}
