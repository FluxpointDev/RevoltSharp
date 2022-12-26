using Optionals;
using System;

namespace RevoltSharp.Rest.Requests
{
    internal class EditMemberRequest : RevoltRequest
    {
        public Optional<string[]> roles;
        public Optional<string> nickname;
        public Optional<AttachmentJson> avatar;
        public Optional<DateTime> timeout;
    }
}
