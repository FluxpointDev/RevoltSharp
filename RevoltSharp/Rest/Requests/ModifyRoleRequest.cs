using Optional;

namespace RevoltSharp.Rest.Requests
{
    internal class ModifyRoleRequest : RevoltRequest
    {
        public Option<string> name;
        public Option<string> colour;
        public Option<bool> hoist;
        public Option<int> rank;
        public Option<string[]> remove;
    }
}
