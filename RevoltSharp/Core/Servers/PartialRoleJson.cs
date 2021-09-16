using Optional;
using Optional.Unsafe;

namespace RevoltSharp
{
    internal class PartialRoleJson
    {
        public Option<string> name;
        public Option<int[]> permissions;
        public Option<bool> hoist;
        public Option<int> rank;
        public Option<string> colour;

        internal Role ToEntity(RevoltClient client, string serverId, string roleId)
        {
            return new Role
            {
                Id = roleId,
                Name = name.ValueOrDefault(),
                Permissions = permissions.ValueOrDefault(),
                Client = client,
                ServerId = serverId
            };
        }
    }
}
