using Optional.Unsafe;

namespace RevoltSharp
{
    /// <summary>
    /// If you would like to get a specific channel please cast to TextChannel or VoiceChannel
    /// </summary>
    public abstract class ServerChannel : Channel
    {
        public ServerChannel(RevoltClient client, ChannelJson model)
            : base (client)
        {
            Id = model.Id;
            ServerId = model.Server;
            DefaultPermissions = model.DefaultPermissions;
            Name = model.Name;
            Description = model.Description;
            Icon = model.Icon != null ? new Attachment(client, model.Icon) : null;
        }

        public string ServerId { get; internal set; }

        public Server Server
            => Client.GetServer(ServerId);
        public int DefaultPermissions { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Attachment Icon { get; internal set; }

        internal override void Update(PartialChannelJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();

            if (json.Icon.HasValue)
                Icon = new Attachment(Client, json.Icon.ValueOrDefault());

            if (json.DefaultPermissions.HasValue)
                DefaultPermissions = json.DefaultPermissions.ValueOrDefault();

            if (json.Description.HasValue)
                Description = json.Description.ValueOrDefault();
        }
    }
}
