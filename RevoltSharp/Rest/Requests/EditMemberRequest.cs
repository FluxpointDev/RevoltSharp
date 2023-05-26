using Optionals;
using System;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;

internal class EditMemberRequest : IRevoltRequest
{
    public Optional<string[]> roles;
    public Optional<string> nickname;
    public Optional<AttachmentJson> avatar;
    public Optional<DateTime> timeout;
    public Optional<List<string>> remove;

    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = new Optional<List<string>>(new List<string>());

        remove.Value.Add(value);
    }
}
