using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{

    internal class ErrorEventJson
    {
        [JsonProperty("error")]
        public RevoltErrorType Error { get; set; }

        [JsonProperty("msg")]
        public string? Message { get; set; }
    }
}