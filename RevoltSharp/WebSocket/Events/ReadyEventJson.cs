using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;


internal class ReadyEventJson
{
    [JsonProperty("users")]
    public UserJson[]? Users;

    [JsonProperty("servers")]
    public ServerJson[]? Servers;

    [JsonProperty("channels")]
    public ChannelJson[]? Channels;

    [JsonProperty("members")]
    public ServerMemberJson[]? Members;

    [JsonProperty("emojis")]
    public EmojiJson[]? Emojis;
}