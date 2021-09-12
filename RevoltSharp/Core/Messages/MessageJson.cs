using Newtonsoft.Json;

namespace RevoltSharp
{
    internal class MessageJson
    {
        [JsonProperty("_id")]
        public string id;
        public string type;
        public string nonce;
        public string channel;
        public string author;
        public string content;
        public AttachmentJson[] attachments;
        public string[] mentions;
        public string[] replies;
    }
}
