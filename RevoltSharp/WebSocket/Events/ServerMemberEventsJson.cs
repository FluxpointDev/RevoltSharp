using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket
{

	internal class ServerMemberJoinEventJson
	{
		[JsonProperty("id")]
		public string? ServerId;

		[JsonProperty("user")]
		public string? UserId;
	}
	internal class ServerMemberLeaveEventJson
	{
		[JsonProperty("id")]
		public string? ServerId;

		[JsonProperty("user")]
		public string? UserId;
	}
	internal class ServerMemberUpdateEventJson
	{
		[JsonProperty("id")]
		public ServerMemberIdsJson? Id;

		[JsonProperty("data")]
		public PartialServerMemberJson? Data;

		[JsonProperty("clear")]
		public Optional<string[]> Clear;
	}
}