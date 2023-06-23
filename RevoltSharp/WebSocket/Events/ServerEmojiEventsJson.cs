using Newtonsoft.Json;

namespace RevoltSharp.WebSocket;

internal class ServerEmojiCreatedEventJson : EmojiJson
{

}
internal class ServerEmojiDeleteEventJson
{
	[JsonProperty("id")]
	public string? EmojiId;
}