using Optionals;
using System.Collections.Generic;

namespace RevoltSharp;

public class ChannelUpdatedProperties
{
    internal ChannelUpdatedProperties(Channel channel, PartialChannelJson json)
    {
        Name = json.Name;
        Active = json.Active;
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

    public Optional<string> Name { get; private set; }
    public Optional<bool> Active { get; private set; }
    public Optional<Attachment?> Icon { get; private set; }
    public Optional<string> Description { get; private set; }
    public Optional<ChannelPermissions> DefaultPermissions { get; private set; }
    public Optional<Dictionary<string, ChannelPermissions>> RolePermissions { get; private set; }
    public Optional<bool> Nsfw { get; private set; }
    public Optional<string> OwnerId { get; private set; }
}
