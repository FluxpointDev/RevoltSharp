using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp.WebSocket;

internal class MessageEventJson : MessageJson
{
    internal Message ToEntity(RevoltClient client)
    {
        return Message.Create(client, this);
    }
}
internal class MessageDeleteEventJson
{
	[JsonProperty("id")]
	public string? MessageId;

	[JsonProperty("ids")]
	public string[]? MessageIds;

	[JsonProperty("channel")]
	public string? ChannelId;
}
internal class MessageUpdateEventJson
{
	[JsonProperty("id")]
	public string MessageId = null!;

	[JsonProperty("channel")]
	public string ChannelId = null!;

	[JsonProperty("data")]
	public MessageUpdateDataJson Data = null!;
}
internal class MessageUpdateDataJson
{
	[JsonProperty("content")]
	public Optional<string> Content;

	[JsonProperty("embeds")]
	public Optional<EmbedJson[]> Embeds;

	[JsonProperty("edited")]
	public DateTime EditedAt;
}
