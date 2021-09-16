using Optional.Unsafe;

namespace RevoltSharp
{
    public class GroupChannel : Channel
    {
        public int Permissions { get; internal set; }
        public string[] Recipents { get; internal set; }
        public string LastMessageId { get; internal set; }
        public string OwnerId { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Attachment Icon { get; internal set; }

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
