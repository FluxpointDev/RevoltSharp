using Optionals;

namespace RevoltSharp.Rest.Requests
{
    internal class ModifyChannelRequest : RevoltRequest
    {
        public Optional<string> name;
        public Optional<string> description;
        public Optional<string> icon;
        public Optional<bool> nsfw;
        public Optional<string[]> remove;
        public Optional<string> owner;
    }
}
