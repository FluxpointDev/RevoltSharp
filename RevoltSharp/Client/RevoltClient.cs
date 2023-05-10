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
        Serializer.Converters.Add(new OptionConverter());
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
            Config.Owners = new string[0];

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
    public string Version { get; } = "5.1.1";

    internal bool UserBot { get; set; }

    internal JsonSerializer Serializer { get; set; }

    /// <summary>
    /// Client config options for user-agent and debug options including self-host support.
    /// </summary>
    public ClientConfig Config { get; internal set; }

    /// <summary>
    /// Internal rest/http client used to connect to the Revolt API.
    /// </summary>
    /// <remarks>
    /// You can also make custom requests with <see cref="RevoltRestClient.SendRequestAsync(RequestType, string, RevoltRequest)"/> and json class based on <see cref="RevoltRequest"/>
    /// </remarks>
    public RevoltRestClient Rest { get; internal set; }

    internal RevoltSocketClient? WebSocket;

    internal bool FirstConnection = true;
    internal bool IsConnected = false;

    public SelfUser? CurrentUser { get; internal set; }

    /// <summary>
    /// Start the Rest and Websocket to be used for the lib.
    /// </summary>
    /// <remarks>
    /// Will throw a <see cref="RevoltException"/> if the token is incorrect or failed to login for the current user/bot.
    /// </remarks>
    /// <exception cref="RevoltException"></exception>
    public async Task StartAsync()
    {
        if (FirstConnection)
        {
            FirstConnection = false;
            QueryRequest Query = await Rest.SendRequestAsync<QueryRequest>(RequestType.Get, "/");
            if (Query == null)
            {
                Console.WriteLine("[RevoltSharp] Client failed to connect to the revolt api at " + Config.ApiUrl);
                throw new RevoltException("Client failed to connect to the revolt api at " + Config.ApiUrl);
            }

            if (!Uri.IsWellFormedUriString(Query.serverFeatures.imageServer.url, UriKind.Absolute))
                throw new RevoltException("[RevoltSharp] Server Image server url is an invalid format.");

            Config.Debug.WebsocketUrl = Query.websocketUrl;
            Config.Debug.UploadUrl = Query.serverFeatures.imageServer.url;

            if (!Config.Debug.UploadUrl.EndsWith('/'))
                Config.Debug.UploadUrl += '/';

            UserJson SelfUser = await Rest.SendRequestAsync<UserJson>(RequestType.Get, "/users/@me");
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
    /// Will throw a <see cref="RevoltException"/> if <see cref="ClientMode.Http"/>
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
    /// Get a list of Servers from the websocket client 
    /// </summary>
    public IReadOnlyCollection<Server> Servers
        => WebSocket != null ? (IReadOnlyCollection<Server>)WebSocket.ServerCache.Values : new ReadOnlyCollection<Server>(new List<Server>());

    public IReadOnlyCollection<User> Users
       => WebSocket != null ? (IReadOnlyCollection<User>)WebSocket.UserCache.Values : new ReadOnlyCollection<User>(new List<User>());


    public User? GetUser(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.UserCache.TryGetValue(id, out User User))
            return User;
        return null;
    }

    public Channel? GetChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan))
            return Chan;
        return null;
    }

    public GroupChannel? GetGroupChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is GroupChannel GC)
            return GC;
        return null;
    }

    public DMChannel? GetDMChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is DMChannel DM)
            return DM;
        return null;
    }

    public TextChannel? GetTextChannel(Optional<string> channelId)
    {
        if (WebSocket != null && channelId.HasValue && !string.IsNullOrEmpty(channelId.Value) && WebSocket.ChannelCache.TryGetValue(channelId.Value, out Channel Chan) && Chan is TextChannel TC)
            return TC;
        return null;
    }

    public TextChannel? GetTextChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is TextChannel TC)
            return TC;
        return null;
    }

    public VoiceChannel? GetVoiceChannel(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan) && Chan is VoiceChannel VC)
            return VC;
        return null;
    }

    public Server? GetServer(string id)
    {
        if (WebSocket != null && !string.IsNullOrEmpty(id) && WebSocket.ServerCache.TryGetValue(id, out Server Server))
            return Server;
        return null;
    }

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
