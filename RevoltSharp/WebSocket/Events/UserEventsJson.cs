using Newtonsoft.Json;
using Optionals;

namespace RevoltSharp.WebSocket;

internal class UserUpdateEventJson
{
    [JsonProperty("id")]
    public string? Id;

    [JsonProperty("data")]
    public PartialUserJson? Data;

    [JsonProperty("clear")]
    public Optional<string[]> Clear;
}
internal class UserRelationshipEventJson
{
	[JsonProperty("id")]
	public string? Id;

	[JsonProperty("user")]
	public UserJson? User;

	[JsonProperty("status")]
	public string? Status;
}
internal class UserPlatformWipedEventJson
{
	[JsonProperty("user_id")]
	internal string UserId = null!;
}