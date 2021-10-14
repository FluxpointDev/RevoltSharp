using Optional.Unsafe;

namespace RevoltSharp
{
    public class VoiceChannel : ServerChannel
    {
        public VoiceChannel(RevoltClient client, ChannelJson model)
            : base(client, model)
        {
           
        }

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
