using Optional.Unsafe;

namespace RevoltSharp
{
    public class VoiceChannel : Channel
    {
        public string ServerId { get { return base.ServerId; } internal set { base.ServerId = value; } }
        public int DefaultPermissions { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Attachment Icon { get; internal set; }

        internal override void Update(PartialChannelJson json)
        {
            if (json.name.HasValue)
                Name = json.name.ValueOrDefault();

            if (json.icon.HasValue)
                Icon = json.icon.ValueOrDefault() != null ? json.icon.ValueOrDefault().ToEntity() : null;

            if (json.default_permissions.HasValue)
                DefaultPermissions = json.default_permissions.ValueOrDefault();

            if (json.description.HasValue)
                Description = json.description.ValueOrDefault();
        }
    }
}
