using Newtonsoft.Json;
using Optional;

namespace RevoltSharp.Rest.Requests
{
    internal class CreateChannelRequest : RevoltRequest
    {
        public string name;
        [JsonProperty("type")]
        public string Type;
        public Option<string> description;
        public Option<string[]> users;
        public Option<bool> nsfw;

    }
}
