using Optional;

namespace RevoltSharp.Rest.Requests
{
    internal class ModifyChannelRequest : RevoltRequest
    {
        public Option<string> name;
        public Option<string> description;
        public Option<string> icon;
        public Option<bool> nsfw;
        public Option<string> remove;
    }
}
