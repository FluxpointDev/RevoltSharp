using Newtonsoft.Json;
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
    public string Notes;

    [JsonProperty("rejection_reason")]
    public string RejectionReason;

    [JsonProperty("closed_at")]
    public DateTime ClosedAt;
}
internal class SafetyReportedContentJson
{
    [JsonProperty("type")]
    public string Type;

    [JsonProperty("id")]
    public string Id;

    [JsonProperty("message_id")]
    public string MessageId;

    [JsonProperty("report_reason")]
    public string Reason;
}
