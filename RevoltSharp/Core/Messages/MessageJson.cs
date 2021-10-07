using Newtonsoft.Json;

namespace RevoltSharp
{
    public class MessageJson
    {
        [JsonProperty("_id")]
        public string Id;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("nonce")]
        public string Nonce;

        [JsonProperty("channel")]
        public string Channel;

        [JsonProperty("author")]
        public string Author;

        // So if its a system message we get a json object
        // this would cause deserialization to a string to fail
        [JsonProperty("content")]
        public object Content;

        [JsonProperty("attachments")]
        public AttachmentJson[] Attachments;

        [JsonProperty("mentions")]
        public string[] Mentions;

        [JsonProperty("replies")]
        public string[] Replies;
    }
}
