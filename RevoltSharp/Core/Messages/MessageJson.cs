using Newtonsoft.Json;
using Optionals;
using System;
using System.Collections.Generic;

namespace RevoltSharp
{

    internal class MessageJson
    {
        [JsonProperty("_id")]
        public string MessageId = null!;

        [JsonProperty("type")]
        public string? MessageType;

        [JsonProperty("nonce")]
        public string? Nonce;

        [JsonProperty("channel")]
        public string ChannelId = null!;

        [JsonProperty("author")]
        public string AuthorId = null!;

        [JsonProperty("content")]
        public string? Content;

        [JsonProperty("attachments")]
        public AttachmentJson[]? Attachments;

        [JsonProperty("mentions")]
        public string[]? Mentions;

        [JsonProperty("replies")]
        public string[]? Replies;

        [JsonProperty("embeds")]
        public EmbedJson[]? Embeds;

        [JsonProperty("system")]
        public MessageSystemJson? System;

        [JsonProperty("webhook")]
        public MessageWebhookJson? Webhook;

        [JsonProperty("edited")]
        public Optional<DateTime> EditedAt;

        [JsonProperty("reactions")]
        public Optional<Dictionary<string, string[]>> Reactions;

        [JsonProperty("masquerade")]
        public MessageMasqueradeJson? Masquerade;
    }
    internal class MessageMasqueradeJson
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("avatar")]
        public string? AvatarUrl { get; set; }

        [JsonProperty("colour")]
        public Optional<string> Color { get; set; }
    }
    internal class MessageSystemJson
    {
        [JsonProperty("type")]
        public string? SystemType;

        [JsonProperty("id")]
        public string? Id;

        [JsonProperty("by")]
        public string? By;

        [JsonProperty("name")]
        public string? Name;

        [JsonProperty("from")]
        public string? From;

        [JsonProperty("to")]
        public string? To;

        [JsonProperty("content")]
        public string? Content;
    }
}