using Optional.Unsafe;

namespace RevoltSharp
{
    public class GroupChannel : Channel
    {
        public int Permissions;
        public string[] Recipents;
        public string LastMessageId;
        public string OwnerId;
        public string Name;
        public string Description;
        public Attachment Icon;

        internal override void Update(PartialChannelJson json)
        {
            if (json.name.HasValue)
                Name = json.name.ValueOrDefault();
            if (json.icon.HasValue)
                Icon = json.icon.ValueOrDefault() != null ? json.icon.ValueOrDefault().ToEntity() : null;
            if (json.description.HasValue)
                Description = json.description.ValueOrDefault();
        }
    }
}
