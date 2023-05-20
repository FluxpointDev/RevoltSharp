using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Threading.Tasks;
using Optionals;
using System.Xml.Linq;

namespace RevoltSharp;

public static class RoleHelper
{
    public static Task<Role> CreateRoleAsync(this Server server, string name, Option<int> rank = null)
        => CreateRoleAsync(server.Client.Rest, server.Id, name, rank);
    public static async Task<Role> CreateRoleAsync(this RevoltRestClient rest, string serverId, string name, Option<int> rank = null)
    {
        Conditions.ServerIdEmpty(serverId, "CreateRoleAsync");
        Conditions.RoleNameEmpty(name, "CreateRoleAsync");
        CreateRoleRequest Req = new CreateRoleRequest
        {
            name = name
        };
        if (rank != null)
            Req.rank = Optional.Some(rank.Value);

        RoleJson Json = await rest.PostAsync<RoleJson>($"/servers/{serverId}/roles", Req);
        return new Role(rest.Client, Json, serverId, Json.Id);
    }

    public static Task<Role> ModifyAsync(this Role role, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
        => ModifyRoleAsync(role.Client.Rest, role.ServerId, role.Id, name, color, hoist, rank);

    public static Task<Role> ModifyRoleAsync(this Server server, Role role, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
        => ModifyRoleAsync(server.Client.Rest, server.Id, role.Id, name, color, hoist, rank);

    public static Task<Role> ModifyRoleAsync(this Server server, string roleId, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
        => ModifyRoleAsync(server.Client.Rest, server.Id, roleId, name, color, hoist, rank);

    public static async Task<Role> ModifyRoleAsync(this RevoltRestClient rest, string serverId, string roleId, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
    {
        Conditions.ServerIdEmpty(serverId, "ModifyRoleAsync");
        Conditions.RoleIdEmpty(roleId, "ModifyRoleAsync");
        
        ModifyRoleRequest Req = new ModifyRoleRequest();
        if (name != null)
        {
            Conditions.RoleNameEmpty(name.Value, "ModifyRoleAsync");
            Req.name = Optional.Some(name.Value);
        }

        if (color != null)
        {
            if (string.IsNullOrEmpty(color.Value))
                Req.RemoveValue("Colour");
            else
                Req.colour = Optional.Some(color.Value);
        }
        if (hoist != null)
            Req.hoist = Optional.Some(hoist.Value);

        if (rank != null)
            Req.rank = Optional.Some(rank.Value);

        return await rest.PatchAsync<Role>($"/servers/{serverId}/roles/{roleId}", Req);
    }

    public static Task DeleteAsync(this Role role)
      => DeleteRoleAsync(role.Client.Rest, role.ServerId, role.Id);

    public static Task DeleteRoleAsync(this Server server, string roleId)
        => DeleteRoleAsync(server.Client.Rest, server.Id, roleId);

    public static Task DeleteRoleAsync(this Server server, Role role)
       => DeleteRoleAsync(server.Client.Rest, server.Id, role.Id);

    public static async Task DeleteRoleAsync(this RevoltRestClient rest, string serverId, string roleId)
    {
        Conditions.ServerIdEmpty(serverId, "DeleteRoleAsync");
        Conditions.RoleIdEmpty(roleId, "DeleteRoleAsync");

        await rest.DeleteAsync($"/servers/{serverId}/roles/{roleId}");
    }
}
