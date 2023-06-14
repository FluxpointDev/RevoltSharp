using RevoltSharp.Rest;

namespace RevoltSharp;

/// <summary>
/// Config options for the RevoltSharp lib.
/// </summary>
public class ClientConfig
{
    /// <summary>
    /// The user agent used for REST requests and websockets.
    /// </summary>
    /// <remarks>
    /// If null, a default user agent is used instead.
    /// </remarks>
    public string? UserAgent = null;

    /// <summary>
    /// Do not change this unless you know what you're doing.
    /// </summary>
    public string ApiUrl = "https://api.revolt.chat/";

    /// <summary>
    /// Do not use this unless you know what you're doing.
    /// </summary>
    public ClientDebugConfig Debug = new ClientDebugConfig();

    /// <summary>
    /// Enable this if you want to use the lib with a userbot
    /// </summary>
    public bool UserBot;

    /// <summary>
    /// Useful for owner checks and also used for RequireOwnerAttribute when using the built-in command handler.
    /// </summary>
    public string[] Owners = null;

    /// <summary>
    /// The cf_clearance cookie for Cloudflare.
    /// </summary>
    /// <remarks>
    /// This is only neccesary if Revolt is currently in Under Attack Mode (eg during a DDoS attack).
    /// Please ensure that the user agent and IP used to generate the clearance cookie will be identical to the ones used by your RevoltSharp client, or else CloudFlare will not accept the clearance.
    /// </remarks>
    public string? CfClearance = null;
}

/// <summary>
/// Debug settings for the RevoltSharp lib.
/// </summary>
public class ClientDebugConfig
{
    /// <summary>
    /// This is only used when running Windows OS, if true then RevoltClient will not disable console quick edit mode for command prompt.
    /// </summary>
    public bool EnableConsoleQuickEdit { get; set; }

    /// <summary>
    /// This will be changed once you run Client.StartAsync()
    /// </summary>
    /// <remarks>
    /// Defaults to https://autumn.revolt.chat
    /// </remarks>
    public string UploadUrl { get; internal set; } = "https://autumn.revolt.chat/";

    /// <summary>
    /// This will be changed once you run Client.StartAsync()
    /// </summary>
    /// <remarks>
    /// Defaults to wss://ws.revolt.chat
    /// </remarks>
    public string WebsocketUrl { get; internal set; } = "wss://revolt.chat";

    /// <summary>
    /// Log all websocket events that you get from Revolt.
    /// </summary>
    /// <remarks>
    /// Do not use this in production!
    /// </remarks>
    public bool LogWebSocketFull { get; set; }

    /// <summary>
    /// Log the websocket ready event json data.
    /// </summary>
    public bool LogWebSocketReady { get; set; }

    /// <summary>
    /// Log when the websocket gets an error.
    /// </summary>
    public bool LogWebSocketError { get; set; }

    /// <summary>
    /// Log when the websocket gets an unknown event not used in the lib.
    /// </summary>
    public bool LogWebSocketUnknownEvent { get; set; }

    /// <summary>
    /// Log the internal request used on <see cref="RevoltRestClient.SendRequestAsync(RequestType, string, IRevoltRequest)"/> and <see cref="RevoltRestClient.UploadFileAsync(byte[], string, UploadFileType)"/>
    /// </summary>
    public bool LogRestRequest { get; set; }

    /// <summary>
    /// Log the json content used when sending a http request.
    /// </summary>
    public bool LogRestRequestJson { get; set; }

    /// <summary>
    /// Log the http response content/json when successful.
    /// </summary>
    public bool LogRestResponseJson { get; set; }
}
