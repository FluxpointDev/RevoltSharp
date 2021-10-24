using Optional.Unsafe;
using System.Collections.Generic;
using System.Linq;

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
            DefaultPermissions = new ChannelPermissions(model.DefaultPermissions);
            RolePermissions = model.RolePermissions != null ? model.RolePermissions.ToDictionary(x => x.Key, x => new ChannelPermissions(x.Value)) : new Dictionary<string, ChannelPermissions>();
            Name = model.Name;
            Description = model.Description;
            Icon = model.Icon != null ? new Attachment(client, model.Icon) : null;
        }

        public string ServerId { get; internal set; }

        public Server Server
            => Client.GetServer(ServerId);
        public ChannelPermissions DefaultPermissions { get; internal set; }
        public Dictionary<string, ChannelPermissions> RolePermissions { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Attachment Icon { get; internal set; }

        public bool HasPermission(ServerMember member, ChannelPermission permission)
        {
            bool HasDefault = DefaultPermissions.Has(permission);
            if (HasDefault)
                return true;
            foreach(var c in RolePermissions)
            {
                if (member.Roles.ContainsKey(c.Key))
                {
                    bool HasRole = c.Value.Has(permission);
                    if (HasRole)
                        return true;
                }
            }
            return false;
        }

        internal override void Update(PartialChannelJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();

            if (json.Icon.HasValue)
                Icon = new Attachment(Client, json.Icon.ValueOrDefault());

            if (json.DefaultPermissions.HasValue)
                DefaultPermissions = new ChannelPermissions(json.DefaultPermissions.ValueOrDefault());

            if (json.Description.HasValue)
                Description = json.Description.ValueOrDefault();

            if (json.RolePermissions.HasValue)
            {
                foreach(var i in json.RolePermissions.ValueOrDefault())
                {
                    if (RolePermissions.ContainsKey(i.Key))
                        RolePermissions[i.Key] = new ChannelPermissions(i.Value);
                    else
                        RolePermissions.Add(i.Key, new ChannelPermissions(i.Value));
                }
            }
        }
    }
}
