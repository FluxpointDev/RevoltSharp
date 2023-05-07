using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Net.Http;
using System.Threading.Tasks;
using Optionals;

namespace RevoltSharp;

public static class RoleHelper
{
    public static Task<Role> CreateRoleAsync(this Server server, string roleName)
        => CreateRoleAsync(server.Client.Rest, server.Id, roleName);
    public static async Task<Role> CreateRoleAsync(this RevoltRestClient rest, string serverId, string roleName)
    {
        Conditions.ServerIdEmpty(serverId, "CreateRoleAsync");
        Conditions.RoleNameEmpty(roleName, "CreateRoleAsync");

        RoleJson Json = await rest.SendRequestAsync<RoleJson>(RequestType.Post, $"/servers/{serverId}/roles", new CreateRoleRequest { name = roleName });
        if (Json == null)
            return null;
        return new Role(rest.Client, Json, serverId, Json.Id);
    }

    public static Task<Role> ModifyAsync(this Role role, Option<string> name, Option<string> color, Option<bool> hoist, Option<int> rank)
        => ModifyRoleAsync(role.Client.Rest, role.ServerId, role.Id, name, color, hoist, rank);

    public static Task<Role> ModifyRoleAsync(this Server server, Role role, Option<string> name, Option<string> color, Option<bool> hoist, Option<int> rank)
        => ModifyRoleAsync(server.Client.Rest, server.Id, role.Id, name, color, hoist, rank);

    public static Task<Role> ModifyRoleAsync(this Server server, string roleId, Option<string> name, Option<string> color, Option<bool> hoist, Option<int> rank)
        => ModifyRoleAsync(server.Client.Rest, server.Id, roleId, name, color, hoist, rank);

    public static async Task<Role> ModifyRoleAsync(this RevoltRestClient rest, string serverId, string roleId, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
    {
        Conditions.ServerIdEmpty(serverId, "ModifyRoleAsync");
        Conditions.RoleIdEmpty(roleId, "ModifyRoleAsync");
        
        ModifyRoleRequest Req = new ModifyRoleRequest();
        if (name != null)
            Req.name = Optional.Some(name.Value);
        if (color != null)
        {
            if (string.IsNullOrEmpty(color.Value))
                Req.remove = Optional.Some(new string[] { "Color" });
            else
                Req.colour = Optional.Some(color.Value);
        }
        if (hoist != null)
            Req.hoist = Optional.Some(hoist.Value);
        if (rank != null)
            Req.rank = Optional.Some(rank.Value);

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
        Conditions.ServerIdEmpty(serverId, "DeleteRoleAsync");
        Conditions.RoleIdEmpty(roleId, "DeleteRoleAsync");

        return await rest.SendRequestAsync(RequestType.Delete, $"/servers/{serverId}/roles/{roleId}");
    }
}
