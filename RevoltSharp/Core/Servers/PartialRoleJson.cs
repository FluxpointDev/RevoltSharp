using Optional;
using Optional.Unsafe;
using System.Numerics;

namespace RevoltSharp
{
    internal class PartialRoleJson
    {
        public Option<string> name;
        public Option<int[]> permissions;
        public Option<bool> hoist;
        public Option<BigInteger> rank;
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
