using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp.WebSocket.Events;

internal class MessageUpdateEventJson
{
    [JsonProperty("id")]
    public string MessageId = null!;

    [JsonProperty("channel")]
    public string ChannelId = null!;

    [JsonProperty("data")]
    public MessageUpdatedJson Data = null!;
}
internal class MessageUpdatedJson
{
    [JsonProperty("content")]
    public Optional<string> Content;

    [JsonProperty("embeds")]
    public Optional<EmbedJson[]> Embeds;

    [JsonProperty("edited")]
    public DateTime EditedAt;
}
