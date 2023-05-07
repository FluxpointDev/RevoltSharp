using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ReactionRemovedEventJson
{
    [JsonProperty("id")]
    public string MessageId;

    [JsonProperty("channel_id")]
    public string ChannelId;

    [JsonProperty("user_id")]
    public string UserId;

    [JsonProperty("emoji_id")]
    public string EmojiId;
}
