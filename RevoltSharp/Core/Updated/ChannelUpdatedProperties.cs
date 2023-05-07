using Newtonsoft.Json;
using Optionals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class ChannelUpdatedProperties
    {
        internal ChannelUpdatedProperties(Channel channel, PartialChannelJson json)
        {
            Name = json.Name;
            if (json.Icon.HasValue)
                Icon = new Optional<Attachment?>(json.Icon.Value != null ? new Attachment(json.Icon.Value) : null);

            Description = json.Description;

            if (channel is ServerChannel SC)
            {
                if (json.DefaultPermissions.HasValue)
                    DefaultPermissions = new Optional<ChannelPermissions>(SC.DefaultPermissions);

                if (json.RolePermissions.HasValue)
                    RolePermissions = new Optional<Dictionary<string, ChannelPermissions>>(SC.InternalRolePermissions);
            }

            Nsfw = json.IsNsfw;
            OwnerId = json.OwnerId;
        }

        public Optional<string> Name { get; set; }
        public Optional<Attachment?> Icon { get; set; }
        public Optional<string> Description { get; set; }
        public Optional<ChannelPermissions> DefaultPermissions { get; set; }
        public Optional<Dictionary<string, ChannelPermissions>> RolePermissions { get; set; }
        public Optional<bool> Nsfw { get; set; }
        public Optional<string> OwnerId { get; set; }
    }
}
