using Optional.Linq;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class RoleHelper
    {
        public static Task<Role> CreateRoleAsync(this Server server, string roleName)
            => CreateRoleAsync(server.Client.Rest, server.Id, roleName);
        public static async Task<Role> CreateRoleAsync(this RevoltRestClient rest, string serverId, string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new RevoltException("Role cannot be created with a empty name.");
            RoleJson Json = await rest.SendRequestAsync<RoleJson>(RequestType.Post, $"/servers/{serverId}/roles", new CreateRoleRequest { name = roleName });
            if (Json == null)
                return null;
            return Json.ToEntity(rest.Client, serverId, Json?.Id);
        }

        public static Task<HttpResponseMessage> ModifyRoleAsync(this Role role, Optional<string> name, Optional<string> color, Optional<bool> hoist, Optional<int> rank)
            => ModifyRoleAsync(role.Client.Rest, role.ServerId, role.Id, name, color, hoist, rank);

        public static Task<HttpResponseMessage> ModifyRoleAsync(this Server server, string roleId, Optional<string> name, Optional<string> color, Optional<bool> hoist, Optional<int> rank)
            => ModifyRoleAsync(server.Client.Rest, server.Id, roleId, name, color, hoist, rank);

        public static async Task<HttpResponseMessage> ModifyRoleAsync(this RevoltRestClient rest, string serverId, string roleId, Optional<string> name = null, Optional<string> color = null, Optional<bool> hoist = null, Optional<int> rank = null)
        {
            ModifyRoleRequest Req = new ModifyRoleRequest();
            if (name != null)
                Req.name = Optional.Option.Some(name.Value);
            if (color != null)
            {
                if (string.IsNullOrEmpty(color.Value))
                    Req.remove = Optional.Option.Some("Color");
                else
                    Req.colour = Optional.Option.Some(color.Value);
            }
            if (hoist != null)
                Req.hoist = Optional.Option.Some(hoist.Value);
            if (rank != null)
                Req.rank = Optional.Option.Some(rank.Value);

            return await rest.SendRequestAsync(RequestType.Patch, $"/servers/{serverId}/roles/{roleId}", Req);
        }

        public static Task<HttpResponseMessage> DeleteRoleAsync(this Role role)
          => DeleteRoleAsync(role.Client.Rest, role.ServerId, role.Id);

        public static Task<HttpResponseMessage> DeleteRoleAsync(this Server server, string roleId)
            => DeleteRoleAsync(server.Client.Rest, server.Id, roleId);

        public static async Task<HttpResponseMessage> DeleteRoleAsync(this RevoltRestClient rest, string serverId, string roleId)
        {
            return await rest.SendRequestAsync(RequestType.Delete, $"/servers/{serverId}/roles/{roleId}");
        }
    }
}
