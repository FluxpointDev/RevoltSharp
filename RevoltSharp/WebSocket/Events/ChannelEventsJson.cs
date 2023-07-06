using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket;


	internal class ChannelEventJson : ChannelJson
	{
	}
	internal class ChannelDeleteEventJson
	{
		[JsonProperty("id")]
		public string? ChannelId;
	}
	internal class ChannelUpdateEventJson
	{
		[JsonProperty("id")]
		public string? ChannelId;

		[JsonProperty("data")]
		public PartialChannelJson? Data;

		[JsonProperty("clear")]
		public Optional<string[]> Clear;
	}