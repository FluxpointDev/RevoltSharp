using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ErrorEventJson
{
    [JsonProperty("error")]
    public SocketErrorType Error { get; set; }

    [JsonProperty("msg")]
    public string Message { get; set; }
}
