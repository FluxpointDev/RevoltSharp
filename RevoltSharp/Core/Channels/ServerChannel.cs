using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    /// <summary>
    /// Base channel for all servers that can be casted to <see cref="TextChannel" /> <see cref="VoiceChannel" /> or <see cref="UnknownServerChannel" />
    /// </summary>
    public class ServerChannel : Channel
    {
        internal ServerChannel(RevoltClient client, ChannelJson model) : base(client)
        {
            Id = model.Id;
            ServerId = model.Server;
            DefaultPermissions = new ChannelPermissions(model.DefaultPermissions);
            RolePermissions = model.RolePermissions != null ? model.RolePermissions.ToDictionary(x => x.Key, x => new ChannelPermissions(x.Value)) : new Dictionary<string, ChannelPermissions>();
            Name = model.Name;
            Description = model.Description;
            Icon = model.Icon != null ? new Attachment(client, model.Icon) : null;
        }

        /// <summary>
        /// If of the parent server
        /// </summary>
        public string ServerId { get; internal set; }

        /// <summary>
        /// Parent server of the channel
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null" /> if using http mode
        /// </remarks>
        public Server Server
            => Client.GetServer(ServerId);

        /// <summary>
        /// Default permissions for all members in the channel
        /// </summary>
        public ChannelPermissions DefaultPermissions { get; internal set; }

        /// <summary>
        /// Role permission for the channel that wil override default permissions
        /// </summary>
        public Dictionary<string, ChannelPermissions> RolePermissions { get; internal set; }

        /// <summary>
        /// Name of the channel
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Description of the channel
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Icon attachment of the channel
        /// </summary>
        /// <remarks>
        /// This may be <see langword="null" />
        /// </remarks>
        public Attachment Icon { get; internal set; }

        /// <summary>
        /// Check if a member has a permission for the channel
        /// </summary>
        /// <param name="member"></param>
        /// <param name="permission"></param>
        /// <returns><see langword="true" /> if member has permission</returns>
        public bool HasPermission(ServerMember member, ChannelPermission permission)
        {
            bool HasDefault = DefaultPermissions.Has(permission);
            if (HasDefault)
                return true;
            foreach (var c in RolePermissions)
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
            Console.WriteLine("Update Channel");

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
                foreach (var i in json.RolePermissions.ValueOrDefault())
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
