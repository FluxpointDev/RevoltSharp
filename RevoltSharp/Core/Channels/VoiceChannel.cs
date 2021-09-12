using Optional.Unsafe;

namespace RevoltSharp
{
    public class VoiceChannel : Channel
    {
        public string ServerId { get { return base.ServerId; } internal set { base.ServerId = value; } }
        public int DefaultPermissions;
        public string Name;
        public string Description;
        public Attachment Icon;

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
