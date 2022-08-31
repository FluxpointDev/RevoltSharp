using Newtonsoft.Json;
using Optional;
using System;

namespace RevoltSharp
{
    internal class MessageJson
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

        [JsonProperty("embeds")]
        public Embed[] Embeds;

        [JsonProperty("system")]
        public MessageSystemJson System;

        [JsonProperty("edited")]
        public Option<DateTime> Edited;
    }
    internal class MessageSystemJson
    {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("by")]
        public string By;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("from")]
        public string From;

        [JsonProperty("to")]
        public string To;

        [JsonProperty("content")]
        public string Content;
    }
}
