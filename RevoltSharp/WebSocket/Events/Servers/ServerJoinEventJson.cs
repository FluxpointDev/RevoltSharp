using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerJoinEventJson
{
    [JsonProperty("id")]
    public string? Id;

    [JsonProperty("server")]
    public ServerJson? Server;

    [JsonProperty("channels")]
    public ChannelJson[]? Channels;
}
