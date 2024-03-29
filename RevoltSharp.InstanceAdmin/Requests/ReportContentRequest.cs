﻿using Optionals;
using RevoltSharp.Rest;

namespace RevoltSharp.Requests;
internal class ReportContentRequest : IRevoltRequest
{
    public SafetyReportedContentJson content { get; set; } = null!;
    public Optional<string> additional_context { get; set; }
}
