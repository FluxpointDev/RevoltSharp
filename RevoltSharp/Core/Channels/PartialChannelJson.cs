using Optional;

namespace RevoltSharp
{
    internal class PartialChannelJson
    {
        public Option<string> name { get; set; }
        public Option<AttachmentJson> icon { get; set; }
        public Option<string> description { get; set; }
        public Option<int> default_permissions { get; set; }
    }
}
