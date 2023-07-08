using Optionals;
using RevoltSharp.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Requests;
internal class ReportContentRequest : IRevoltRequest
{
    public SafetyReportedContentJson content { get; set; }
    public Optional<string> additional_context { get; set; }
}
