using Optionals;
using System;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests
{

    internal class EditMemberRequest : IRevoltRequest
    {
        public Optional<string[]> roles { get; set; }
        public Optional<string> nickname { get; set; }
        public Optional<AttachmentJson> avatar { get; set; }
        public Optional<DateTime> timeout { get; set; }
        public Optional<List<string>> remove { get; set; }

        public void RemoveValue(string value)
        {
            if (!remove.HasValue)
                remove = Optional.Some(new List<string>());

            remove.Value.Add(value);
        }
    }
}
