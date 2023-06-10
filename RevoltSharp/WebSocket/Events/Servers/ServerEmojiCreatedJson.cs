using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events;

internal class ServerEmojiCreatedEventJson : EmojiJson
{

}
internal class ServerEmojiDeleteEventJson
{
    [JsonProperty("id")]
    public string? Id;
}
