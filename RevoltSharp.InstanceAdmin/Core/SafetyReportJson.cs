using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp;

internal class SafetyReportJson
{
    [JsonProperty("_id")]
    public string Id;

    [JsonProperty("status")]
    public string Status;

    [JsonProperty("author_id")]
    public string AuthorId;

    [JsonProperty("content")]
    public SafetyReportedContentJson Content;

    [JsonProperty("additional_context")]
    public string AdditionalContext;

    [JsonProperty("notes")]
    public string Note;

    [JsonProperty("rejection_reason")]
    public string RejectionReason;

    [JsonProperty("closed_at")]
    public DateTime ClosedAt;
}
internal class SafetyReportedContentJson
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("message_id")]
    public Optional<string> MessageId { get; set; }

    [JsonProperty("report_reason")]
    public string Reason { get; set; }
}
