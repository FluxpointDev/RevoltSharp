using System.Collections.Generic;
using Optional.Unsafe;

namespace RevoltSharp
{
    public class GroupChannel : Channel
    {
        public int Permissions { get; internal set; }
        public IReadOnlyList<string> Recipents { get; internal set; }
        public string LastMessageId { get; internal set; }
        public string OwnerId { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Attachment Icon { get; internal set; }

        internal GroupChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
            Recipents = model.Recipients != null ? model.Recipients : new string[0];
            Description = model.Description;
            LastMessageId = model.LastMessageId;
            Name = model.Name;
            OwnerId = model.Owner;
            Permissions = model.Permissions;
            Icon = model.Icon != null ? new Attachment(client, model.Icon) : null;
        }

        internal override void Update(PartialChannelJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();
            if (json.Icon.HasValue)
                Icon = new Attachment(Client, json.Icon.ValueOrDefault());
            if (json.Description.HasValue)
                Description = json.Description.ValueOrDefault();
        }
    }
}
