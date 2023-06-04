using Optionals;
using System;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;

internal class EditMemberRequest : IRevoltRequest
{
    public Optional<string[]> roles { get; internal set; }
    public Optional<string> nickname { get; internal set; }
    public Optional<AttachmentJson> avatar { get; internal set; }
    public Optional<DateTime> timeout { get; internal set; }
    public Optional<List<string>> remove { get; internal set; }

    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = new Optional<List<string>>(new List<string>());

        remove.Value.Add(value);
    }
}
