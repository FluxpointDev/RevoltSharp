using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ErrorEventJson
    {
        [JsonProperty("error")]
        public WebSocketErrorType Error { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}
