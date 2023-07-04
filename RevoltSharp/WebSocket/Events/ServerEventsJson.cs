using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket
{

	internal class ServerJoinEventJson
	{
		[JsonProperty("id")]
		public string? ServerId;

		[JsonProperty("server")]
		public ServerJson? ServerJson;

		[JsonProperty("channels")]
		public ChannelJson[]? ChannelsJson;
	}
	internal class ServerDeleteEventJson
	{
		[JsonProperty("id")]
		public string? ServerId;
	}
	internal class ServerUpdateEventJson
	{
		[JsonProperty("id")]
		public string? ServerId;

		[JsonProperty("data")]
		public PartialServerJson? Data;

		[JsonProperty("clear")]
		public Optional<string[]> Clear;
	}

}