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

    }
    public class ClientDebugConfig
    {
        public string UploadUrl = "https://autumn.revolt.chat/";
        public string WebsocketUrl = "wss://ws.revolt.chat";

        /// <summary>
        /// Log all websocket events that you get from Revolt.
        /// </summary>
        /// <remarks>
        /// Do not use this in production!
        /// </remarks>
        public bool LogWebSocketFull { get; set; }

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
        /// By default the lib will return a <see cref="HttpResponseMessage"/> or <see langword="null"/> for <see langword="GetServerAsync"/>, <see langword="GetUserAsync"/>, ect.
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
