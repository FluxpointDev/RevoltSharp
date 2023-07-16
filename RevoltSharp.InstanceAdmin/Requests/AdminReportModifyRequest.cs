using Optionals;
using RevoltSharp.Rest;
using System;

namespace RevoltSharp.Requests;
internal class AdminReportModifyRequest : IRevoltRequest
{
    public AdminReportModifyStatusRequest? status;

    public Optional<string> notes;
}
internal class AdminReportModifyStatusRequest
{
    public string? status;
    public Optional<string> rejection_reason;
    public Optional<DateTime> closed_at;
}