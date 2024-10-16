using Newtonsoft.Json;

namespace RevoltSharp.Core.Messages;

internal class BulkMessagesJson
{
    [JsonProperty("messages")]
    public MessageJson[] Messages { get; set; }

    [JsonProperty("users")]
    public UserJson[] Users { get; set; }

    [JsonProperty("members")]
    public ServerMemberJson[] Members { get; set; }
}
