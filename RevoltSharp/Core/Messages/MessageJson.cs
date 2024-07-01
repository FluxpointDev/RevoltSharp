using Newtonsoft.Json;
using Optionals;
using System;
using System.Collections.Generic;

namespace RevoltSharp;


internal class MessageJson
{
    [JsonProperty("_id")]
    public string MessageId { get; set; } = null!;

    [JsonProperty("type")]
    public string? MessageType { get; set; }

    [JsonProperty("nonce")]
    public string? Nonce { get; set; }

    [JsonProperty("channel")]
    public string ChannelId { get; set; } = null!;

    [JsonProperty("author")]
    public string AuthorId { get; set; } = null!;

    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("attachments")]
    public AttachmentJson[]? Attachments { get; set; }

    [JsonProperty("mentions")]
    public string[]? Mentions { get; set; }

    [JsonProperty("replies")]
    public string[]? Replies { get; set; }

    [JsonProperty("embeds")]
    public EmbedJson[]? Embeds { get; set; }

    [JsonProperty("system")]
    public MessageSystemJson? System { get; set; }

    [JsonProperty("webhook")]
    public MessageWebhookJson? Webhook { get; set; }

    [JsonProperty("edited")]
    public Optional<DateTime> EditedAt { get; set; }

    [JsonProperty("reactions")]
    public Optional<Dictionary<string, string[]>> Reactions { get; set; }

    [JsonProperty("masquerade")]
    public MessageMasqueradeJson? Masquerade { get; set; }
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
    public string? SystemType { get; set; }

    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("by")]
    public string? By { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("from")]
    public string? From { get; set; }

    [JsonProperty("to")]
    public string? To { get; set; }

    [JsonProperty("content")]
    public string? Content { get; set; }
}