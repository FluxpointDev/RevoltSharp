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
    public string Version { get; } = "5.6.0";

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
            FirstConnection = false;
            QueryRequest? Query = await Rest.GetAsync<QueryRequest>("/");
            if (Query == null)
            {
                Console.WriteLine("[RevoltSharp] Client failed to connect to the revolt api at " + Config.ApiUrl);
                throw new RevoltException("Client failed to connect to the revolt api at " + Config.ApiUrl);
            }

            if (!Uri.IsWellFormedUriString(Query.serverFeatures.imageServer.url, UriKind.Absolute))
                throw new RevoltException("[RevoltSharp] Server Image server url is an invalid format.");

            RevoltVersion = Query.revoltVersion;
            Config.Debug.WebsocketUrl = Query.websocketUrl;
            Config.Debug.UploadUrl = Query.serverFeatures.imageServer.url;

            if (!Config.Debug.UploadUrl.EndsWith('/'))
                Config.Debug.UploadUrl += '/';

            UserJson? SelfUser = await Rest.GetAsync<UserJson>("/users/@me");
            if (SelfUser == null)
                throw new RevoltException("Failed to login to user account.");

            CurrentUser = new SelfUser(this, SelfUser);
            Console.WriteLine($"[RevoltSharp] Started: {SelfUser.Username} ({SelfUser.Id})");
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

    /// <summary>
    /// Get a <see cref="User" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="User" /> or <see langword="null" /></returns>
    public User? GetUser(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.UserCache.TryGetValue(id, out User User))
            return User;
        return null;
    }

    /// <summary>
    /// Get a <see cref="Channel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="Channel" /> or <see langword="null" /></returns>
    public Channel? GetChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan))
            return Chan;
        return null;
    }

    /// <summary>
    /// Get a <see cref="GroupChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="GroupChannel" /> or <see langword="null" /></returns>
    public GroupChannel? GetGroupChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is GroupChannel GC)
            return GC;
        return null;
    }

    /// <summary>
    /// Get a <see cref="DMChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="DMChannel" /> or <see langword="null" /></returns>
    public DMChannel? GetDMChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is DMChannel DM)
            return DM;
        return null;
    }

    
    internal TextChannel? GetTextChannel(Optional<string> channelId)
    {
        if (WebSocket != null && channelId.HasValue && !string.IsNullOrEmpty(channelId.Value) && WebSocket.ChannelCache.TryGetValue(channelId.Value, out Channel Chan) && Chan is TextChannel TC)
            return TC;
        return null;
    }

    /// <summary>
    /// Get a server <see cref="TextChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="TextChannel" /> or <see langword="null" /></returns>
    public TextChannel? GetTextChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is TextChannel TC)
            return TC;
        return null;
    }

    /// <summary>
    /// Get a server <see cref="VoiceChannel" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="VoiceChannel" /> or <see langword="null" /></returns>
    public VoiceChannel? GetVoiceChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is VoiceChannel VC)
            return VC;
        return null;
    }

    /// <summary>
    /// Get a <see cref="Server" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="Server" /> or <see langword="null" /></returns>
    public Server? GetServer(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ServerCache.TryGetValue(id, out Server Server))
            return Server;
        return null;
    }

    /// <summary>
    /// Get a server <see cref="Role" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="Role" /> or <see langword="null" /></returns>
    public Role? GetRole(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id))
        {
            foreach(Server s in WebSocket.ServerCache.Values)
            {
                Role role = s.GetRole(id);
                if (role != null)
                    return role;
            }
        }
        return null;
    }

    /// <summary>
    /// Get a server <see cref="Emoji" /> from the websocket cache.
    /// </summary>
    /// <returns><see cref="Emoji" /> or <see langword="null" /></returns>
    public Emoji? GetEmoji(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id))
        {
            if (WebSocket.EmojiCache.TryGetValue(id, out Emoji emoji))
                return emoji;
        }
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
