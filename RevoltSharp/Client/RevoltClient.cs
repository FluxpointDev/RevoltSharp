using Newtonsoft.Json;
using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using RevoltSharp.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
    /// Version of the current RevoltSharp lib installed.
    /// </summary>
    public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);


    /// <summary>
    /// Create a Revolt client that can be used for user or bot accounts.
    /// </summary>
    /// <param name="token">Bot token to connect with.</param>
    /// <param name="mode">Use http for http requests only with no websocket.</param>
    /// <param name="config">Optional config stuff for the bot and lib.</param>
    /// <exception cref="RevoltArgumentException"></exception>
    public RevoltClient(ClientMode mode, ClientConfig? config = null)
    {
        Config = config ??= new ClientConfig();
        ConfigSafetyChecks();

        if (!Config.Debug.EnableConsoleQuickEdit)
        {
            try
            {
                DisableConsoleQuickEdit.Go();
            }
            catch { }
        }

        Logger = new RevoltLogger("RevoltSharp", config.LogMode);
        Logger.AllowOptionals = true;
        Rest = new RevoltRestClient(this);
        Admin = new AdminClient(this);
        Mode = mode;
        if (Mode == ClientMode.WebSocket)
            WebSocket = new RevoltSocketClient(this);
    }

    /// <summary>
    /// Set the voice client to use for the lib.
    /// </summary>
    /// <param name="client"></param>
    /// <exception cref="RevoltArgumentException"></exception>
    public void SetVoiceClient(IVoiceClient client)
    {
        if (client == null)
        {
            Logger.LogMessage("Voice client can't be empty.", RevoltLogSeverity.Error);
            throw new RevoltArgumentException("Voice client can't be empty.");
        }

        VoiceClient = client;
    }

    /// <summary>
    /// The current voice client in use with RevoltSharp.
    /// </summary>
    public IVoiceClient VoiceClient { get; internal set; } = null;

    /// <summary>
    /// The current client mode that RevoltSharp is using either Http or WebSocket
    /// </summary>
    public ClientMode Mode { get; internal set; }

    private void ConfigSafetyChecks()
    {
        if (string.IsNullOrEmpty(Config.ApiUrl))
        {
            Logger.LogMessage("Config API Url is missing.", RevoltLogSeverity.Error);
            throw new RevoltArgumentException("Config API Url is missing");
        }

        if (!Config.ApiUrl.EndsWith('/'))
            Config.ApiUrl += "/";

        Config.UserAgent ??= $"RevoltSharp v{Version} ({Config.ClientName})";
        Config.Owners ??= Array.Empty<string>();
        Config.Debug ??= new ClientDebugConfig();
    }


    /// <summary>
    /// Revolt bot token used for http requests and websocket.
    /// </summary>
    public string Token { get; internal set; }

    /// <summary>
    /// The current version of the revolt instance connected to.
    /// </summary>
    /// <remarks>
    /// This will be empty of you do not use <see cref="StartAsync" />.
    /// </remarks>
    public string? RevoltVersion { get; internal set; }

    public bool IsUserAccount { get; internal set; }
    public bool IsLoginComplete => CurrentUser != null;

    /// <summary>
    /// The json serializer that is used with RevoltSharp.
    /// </summary>
    public static JsonSerializer Serializer { get; internal set; } = new JsonSerializer
    {
        ContractResolver = new RevoltContractResolver()
    };

    /// <summary>
    /// The json serializer that is used with RevoltSharp with pretty print formatting.
    /// </summary>
    public static JsonSerializer SerializerPretty { get; internal set; } = new JsonSerializer
    {
        ContractResolver = new RevoltContractResolver(),
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// The json serializer that is used with RevoltSharp.
    /// </summary>
    public static JsonSerializer Deserializer { get; internal set; } = CreateDes();

    internal static JsonSerializer CreateDes()
    {
        JsonSerializer des = new JsonSerializer();
        des.Converters.Add(new OptionalDeserializerConverter());
        return des;
    }

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

    /// <summary>
    /// This is for self-hosted revolt instances that have global admin access.
    /// </summary>
    public AdminClient Admin { get; internal set; }

    internal RevoltLogger Logger;

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
    /// The current query info of the connected Revolt instance.
    /// </summary>
    /// <remarks>
    /// This will be <see langword="null" /> of you do not use <see cref="StartAsync" />.
    /// </remarks>
    public Query? CurrentQuery { get; internal set; }

    /// <summary>
    /// The current user/bot account's private notes message channel.
    /// </summary>
    /// <remarks>
    /// This will be <see langword="null" /> if you have not created the channel from <see cref="BotHelper.GetSavedMessagesChannelAsync(RevoltRestClient)" /> once.
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
            if (string.IsNullOrEmpty(Token))
                throw new RevoltException("You need to login with the account first using LoginAsync()");

            if (!IsLoginComplete)
                throw new RevoltException("This account has not properly logged in.");

            InvokeLog("Starting...", RevoltLogSeverity.Debug);

            FirstConnection = false;
            try
            {
                CurrentQuery = await Rest.GetQueryAsync(true);
            }
            catch (Exception ex)
            {
                InvokeLogAndThrowException($"Client failed to connect to the Revolt API at {Config.ApiUrl}. {ex.Message}");
            }

            if (!Uri.IsWellFormedUriString(CurrentQuery.ImageServerUrl, UriKind.Absolute))
                InvokeLogAndThrowException("Image server url is an invalid format.");

            RevoltVersion = CurrentQuery.RevoltVersion;
            Config.Debug.WebsocketUrl = CurrentQuery.WebsocketUrl;
            Config.Debug.UploadUrl = CurrentQuery.ImageServerUrl;

            Config.Debug.VoiceServerUrl = CurrentQuery.VoiceApiUrl;
            Config.Debug.VoiceWebsocketUrl = CurrentQuery.VoiceWebsocketUrl;

            InvokeLog($"Started: {CurrentUser.Username} ({CurrentUser.Id})", RevoltLogSeverity.Info);
            InvokeStarted(CurrentUser);

            if (VoiceClient != null)
                _ = VoiceClient.StartAsync();
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

    public async Task<AccountLogin> LoginAsync(string email, string password, string sessionName = null)
    {
        AccountLogin Response = null;
        try
        {
            var Json = await Rest.PostAsync<AccountLoginJson>("auth/session/login", new AccountLoginRequest
            {
                email = email,
                password = password,
                friendly_name = sessionName
            });
            Response = new AccountLogin(Json);
        }
        catch
        {
            return new AccountLogin(null)
            {
                ResponseType = LoginResponseType.Failed
            };
        }

        if (Response.ResponseType != LoginResponseType.Success)
            return Response;

        AccountLogin LoginState = await LoginAsync(Response.Token, AccountType.User);
        if (LoginState.ResponseType != LoginResponseType.Success)
            return LoginState;

        return Response;
    }

    public async Task<AccountLogin> LoginMFAAsync(string ticket, string code, MFAType type, string sessionName = null)
    {
        AccountLogin Response = null;
        try
        {
            var Req = new AccountLoginMFARequest
            {
                mfa_ticket = ticket,
                friendly_name = sessionName
            };
            if (type == MFAType.Recovery)
                Req.mfa_response.recovery_code = code;
            else
                Req.mfa_response.totp_code = code;

            var Json = await Rest.PostAsync<AccountLoginJson>("auth/session/login", Req);
            Response = new AccountLogin(Json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new AccountLogin(null)
            {
                ResponseType = LoginResponseType.Failed
            };
        }

        if (Response.ResponseType != LoginResponseType.Success)
            return Response;


        AccountLogin LoginState = await LoginAsync(Response.Token, AccountType.User);
        if (LoginState.ResponseType != LoginResponseType.Success)
            return LoginState;

        return Response;
    }

    public async Task<AccountLogin> LoginAsync(string token, AccountType type)
    {
        if (string.IsNullOrEmpty(token))
        {
            Logger.LogMessage("Client token is missing!", RevoltLogSeverity.Error);
            throw new RevoltArgumentException("Client token is missing!");
        }

        Token = token;
        IsUserAccount = type == AccountType.User;
        Rest.Http.DefaultRequestHeaders.Add(IsUserAccount ? "x-session-token" : "x-bot-token", Token);
        Rest.FileHttpClient.DefaultRequestHeaders.Add(IsUserAccount ? "x-session-token" : "x-bot-token", Token);

        // Check onboarding status
        if (IsUserAccount)
        {
            OnboardStatus Status = await Rest.GetAccountOnboardingStatus();

            if (Status.IsOnboardingRequired)
                return new AccountLogin(null)
                {
                    ResponseType = LoginResponseType.OnboardingRequired,
                };
        }

        UserJson? SelfUser = null;
        try
        {
            SelfUser = await Rest.GetAsync<UserJson>("users/@me", null, true);
        }
        catch (RevoltRestException re)
        {
            if (re.Code == 401)
                throw new RevoltRestException("The token is invalid.", re.Code, re.Type);

            throw;
        }
        catch (Exception ex)
        {
            InvokeLogAndThrowException($"Failed to login. {ex.Message}");
        }
        CurrentUser = new SelfUser(this, SelfUser);

        if (CurrentUser.IsBot == IsUserAccount)
            throw new Exception("Login session is using the wrong account type.");

        InvokeLogin(CurrentUser);

        return new AccountLogin(null)
        {
            ResponseType = LoginResponseType.Success
        };
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
        if (Mode == ClientMode.Http)
            throw new RevoltException("Client is in HTTP-only mode.");

        if (WebSocket.WebSocket != null)
        {
            WebSocket.StopWebSocket = true;
            await WebSocket.WebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "", WebSocket.CancellationToken);
            WebSocket.ClearAllCache();

        }
        Dictionary<string, string> test = new Dictionary<string, string>();
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
        if (channelId.HasValue && !string.IsNullOrEmpty(channelId.Value) && this.TryGetTextChannel(channelId.Value, out TextChannel Chan))
            return Chan;
        return null;
    }

    #region Log Event

    /// <summary>
    /// Called to display information, events, and errors originating from the <see cref="RevoltClient"/>.
    /// </summary>
    /// <remarks>
    /// By default, RevoltSharp will log its events to the <see cref="Console"/>. Adding a subscriber to this event overrides this behavior.
    /// </remarks>
    public event LogEvent? OnLog;

    internal void InvokeLog(string message, RevoltLogSeverity severity)
    {
        if (Config.LogMode != RevoltLogSeverity.None)
            Logger.LogMessage(message, severity);

        OnLog?.Invoke(message, severity);
    }

    internal void InvokeLogAndThrowException(string message)
    {
        InvokeLog(message, RevoltLogSeverity.Error);
        throw new RevoltException(message);
    }

    #endregion
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