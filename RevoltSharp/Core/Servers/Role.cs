using Optional.Unsafe;
using System.Numerics;

namespace RevoltSharp
{
    /// <summary>
    /// Server role
    /// </summary>
    public class Role : Entity
    {
        public string Id { get; internal set; }

        public string ServerId { get; internal set; }

        public Server Server { get; internal set; }

        public string Name { get; internal set; }

        public ServerPermissions Permissions { get; internal set; }

        public bool IsHoisted { get; internal set; }

        public BigInteger Rank { get; internal set; }

        public RevoltColor Color { get; internal set; }

        internal Role(RevoltClient client, RoleJson model, string serverId, string roleId)
            : base(client)
        {

            Id = roleId;
            
            Color = new RevoltColor(model.Colour);
            IsHoisted = model.Hoist;
            Name = model.Name;
            Permissions = new ServerPermissions(model.Permissions.Allowed);
            Rank = model.Rank;
            ServerId = serverId;
            Server = client.GetServer(ServerId);
        }

        internal Role(RevoltClient client, PartialRoleJson model, string serverId, string roleId)
            : base(client)
        {
            Id = roleId;
            Name = model.Name.ValueOrDefault();
            Permissions = new ServerPermissions(model.Permissions.ValueOrDefault().Allowed);
            ServerId = serverId;
        }

        internal void Update(PartialRoleJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();

            if (json.Permissions.HasValue)
                Permissions = new ServerPermissions(json.Permissions.ValueOrDefault().Allowed);

            if (json.Hoist.HasValue)
                IsHoisted = json.Hoist.ValueOrDefault();

            if (json.Rank.HasValue)
                Rank = json.Rank.ValueOrDefault();

            if (json.Colour.HasValue)
                Color = new RevoltColor(json.Colour.ValueOrDefault());
        }

        internal Role Clone()
        {
            return (Role) this.MemberwiseClone();
        }
    }
}
