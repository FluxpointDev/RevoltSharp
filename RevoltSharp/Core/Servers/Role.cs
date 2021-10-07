using Optional.Unsafe;
using System.Numerics;

namespace RevoltSharp
{
    public class Role : Entity
    {
        public string Id { get; internal set; }

        public string ServerId { get; internal set; }

        public Server Server { get; internal set; }

        public string Name { get; internal set; }

        public int[] Permissions { get; internal set; }

        public bool IsHoisted { get; internal set; }

        public BigInteger Rank { get; internal set; }

        public string Color { get; internal set; }

        public Role(RevoltClient client, RoleJson model, string serverId, string roleId)
            : base(client)
        {

            Id = roleId;
            Color = model.Colour;
            IsHoisted = model.Hoist;
            Name = model.Name;
            Permissions = model.Permissions;
            Rank = model.Rank;
            ServerId = serverId;
            Server = client.GetServer(ServerId);
        }

        internal Role(RevoltClient client, PartialRoleJson model, string serverId, string roleId)
            : base(client)
        {
            Id = roleId;
            Name = model.Name.ValueOrDefault();
            Permissions = model.Permissions.ValueOrDefault();
            ServerId = serverId;
        }

        internal void Update(PartialRoleJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();

            if (json.Permissions.HasValue)
                Permissions = json.Permissions.ValueOrDefault();

            if (json.Hoist.HasValue)
                IsHoisted = json.Hoist.ValueOrDefault();

            if (json.Rank.HasValue)
                Rank = json.Rank.ValueOrDefault();

            if (json.Colour.HasValue)
                Color = json.Colour.ValueOrDefault();
        }

        public Role CreateFromPartial(RevoltClient client, PartialRoleJson model, string serverId, string roleId)
        {
            return new Role(client, model, serverId, roleId);
        }

        internal Role Clone()
        {
            return (Role) this.MemberwiseClone();
        }
    }
}
