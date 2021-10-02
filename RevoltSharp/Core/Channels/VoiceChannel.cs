using Optional.Unsafe;

namespace RevoltSharp
{
    public class VoiceChannel : Channel
    {
        public string ServerId { get; internal set; }
        public int DefaultPermissions { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Attachment Icon { get; internal set; }

        public VoiceChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
            ServerId = model.Id;
            DefaultPermissions = model.DefaultPermissions;
            Name = model.Name;
            Description = model.Description;
            Icon = new Attachment(client, model.Icon);
        }

        internal override void Update(PartialChannelJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();

            if (json.Icon.HasValue)
                Icon = json.Icon.ValueOrDefault() != null ? new Attachment(Client, json.Icon.ValueOrDefault()) : null;

            if (json.DefaultPermissions.HasValue)
                DefaultPermissions = json.DefaultPermissions.ValueOrDefault();

            if (json.Description.HasValue)
                Description = json.Description.ValueOrDefault();
        }
    }
}
