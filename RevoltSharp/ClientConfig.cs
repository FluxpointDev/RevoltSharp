using RevoltSharp.Rest;
using System.Net.Http;

namespace RevoltSharp
{
    public class ClientConfig
    {
        public string UserAgent = "Revolt Bot (RevoltSharp)";

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
        /// Enabled by default, rest requests for POST/PUT/DELETE will throw a RevoltRestException
        /// </summary>
        public bool RestThrowException = true;

        public string[] Owners = null;
    }
    public class ClientDebugConfig
    {
        // This will be set once you run Client.StartAsync()
        public string UploadUrl { get; internal set; } = "https://autumn.revolt.chat/";

        // This will be set once you run Client.StartAsync()
        public string WebsocketUrl { get; internal set; } = "wss://ws.revolt.chat";

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
        /// Log the internal request used on <see cref="RevoltRestClient.SendRequestAsync(Rest.RequestType, string)"/> and <see cref="RevoltRestClient.UploadFileAsync(byte[], string, RevoltRestClient.UploadFileType)"/>
        /// </summary>
        public bool LogRestRequest { get; set; }

        /// <summary>
        /// Check all requests sent, if they are not successful enable this to throw an exception.
        /// </summary>
        /// <remarks>
        /// By default the lib will return <see langword="null"/> for get requests, <see cref="RevoltRestException"/> for post/put/delete or <see cref="HttpResponseMessage"/> if you disable <see cref="ClientConfig.RestThrowException"/>.
        /// </remarks>
        public bool CheckRestRequest { get; set; }

        /// <summary>
        /// Log the json content used when sending a http request.
        /// </summary>
        public bool LogRestRequestJson { get; set; }

        /// <summary>
        /// Log the http response content/json when successful.
        /// </summary>
        public bool LogRestResponseJson { get; set; }
    }

}
