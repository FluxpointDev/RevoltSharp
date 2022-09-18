using Optional;

namespace RevoltSharp.Rest.Requests
{
    internal class EditMemberRequest : RevoltRequest
    {
        public Option<string[]> roles;
    }
}
