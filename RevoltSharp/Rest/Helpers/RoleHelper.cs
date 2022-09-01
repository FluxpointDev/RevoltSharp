using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class RoleHelper
    {
        public static Task<Role> CreateRoleAsync(this Server server, string roleName)
            => CreateRoleAsync(server.Client.Rest, server.Id, roleName);
        public static async Task<Role> CreateRoleAsync(this RevoltRestClient rest, string serverId, string roleName)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(roleName))
                throw new RevoltException("Role cannot be created with a empty name.");

            RoleJson Json = await rest.SendRequestAsync<RoleJson>(RequestType.Post, $"/servers/{serverId}/roles", new CreateRoleRequest { name = roleName });
            if (Json == null)
                return null;
            return new Role(rest.Client, Json, serverId, Json.Id);
        }

        public static Task<Role> ModifyAsync(this Role role, Optional<string> name, Optional<string> color, Optional<bool> hoist, Optional<int> rank)
            => ModifyRoleAsync(role.Client.Rest, role.ServerId, role.Id, name, color, hoist, rank);

        public static Task<Role> ModifyRoleAsync(this Server server, Role role, Optional<string> name, Optional<string> color, Optional<bool> hoist, Optional<int> rank)
            => ModifyRoleAsync(server.Client.Rest, server.Id, role.Id, name, color, hoist, rank);

        public static Task<Role> ModifyRoleAsync(this Server server, string roleId, Optional<string> name, Optional<string> color, Optional<bool> hoist, Optional<int> rank)
            => ModifyRoleAsync(server.Client.Rest, server.Id, roleId, name, color, hoist, rank);

        public static async Task<Role> ModifyRoleAsync(this RevoltRestClient rest, string serverId, string roleId, Optional<string> name = null, Optional<string> color = null, Optional<bool> hoist = null, Optional<int> rank = null)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(roleId))
                throw new RevoltArgumentException("Role id can't be empty for this request.");

            ModifyRoleRequest Req = new ModifyRoleRequest();
            if (name != null)
                Req.name = Optional.Option.Some(name.Value);
            if (color != null)
            {
                if (string.IsNullOrEmpty(color.Value))
                    Req.remove = Optional.Option.Some(new string[] { "Color" });
                else
                    Req.colour = Optional.Option.Some(color.Value);
            }
            if (hoist != null)
                Req.hoist = Optional.Option.Some(hoist.Value);
            if (rank != null)
                Req.rank = Optional.Option.Some(rank.Value);

            return await rest.SendRequestAsync<Role>(RequestType.Patch, $"/servers/{serverId}/roles/{roleId}", Req);
        }

        public static Task<HttpResponseMessage> DeleteAsync(this Role role)
          => DeleteRoleAsync(role.Client.Rest, role.ServerId, role.Id);

        public static Task<HttpResponseMessage> DeleteRoleAsync(this Server server, string roleId)
            => DeleteRoleAsync(server.Client.Rest, server.Id, roleId);

        public static Task<HttpResponseMessage> DeleteRoleAsync(this Server server, Role role)
           => DeleteRoleAsync(server.Client.Rest, server.Id, role.Id);

        public static async Task<HttpResponseMessage> DeleteRoleAsync(this RevoltRestClient rest, string serverId, string roleId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(roleId))
                throw new RevoltArgumentException("Role id can't be empty for this request.");

            return await rest.SendRequestAsync(RequestType.Delete, $"/servers/{serverId}/roles/{roleId}");
        }
    }
}
