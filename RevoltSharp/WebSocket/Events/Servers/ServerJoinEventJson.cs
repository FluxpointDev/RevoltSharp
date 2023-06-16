using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerJoinEventJson
{
    [JsonProperty("id")]
    public string? ServerId;

    [JsonProperty("server")]
    public ServerJson? ServerJson;

    [JsonProperty("channels")]
    public ChannelJson[]? ChannelsJson;
}
