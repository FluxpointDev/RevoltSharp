using Newtonsoft.Json;

namespace RevoltSharp.Requests;
internal class AdminMessagesDataJson
{
    [JsonProperty("messages")]
    public MessageJson[] Messages;

    [JsonProperty("users")]
    public UserJson[] Users;
}
