using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Threading.Tasks;

namespace RevoltSharp
{

    /// <summary>
    /// Revolt http/rest methods for server roles.
    /// </summary>
    public static class RoleHelper
    {
        /// <inheritdoc cref="CreateRoleAsync(RevoltRestClient, string, string, Option{int})" />
        public static Task<Role> CreateRoleAsync(this Server server, string name, Option<int> rank = null)
            => CreateRoleAsync(server.Client.Rest, server.Id, name, rank);

        /// <summary>
        /// Create a server role.
        /// </summary>
        /// <returns>
        /// <see cref="Role"/>
        /// </returns>
        /// <exception cref="RevoltArgumentException"></exception>
        /// <exception cref="RevoltRestException"></exception>
        public static async Task<Role> CreateRoleAsync(this RevoltRestClient rest, string serverId, string name, Option<int> rank = null)
        {
            Conditions.ServerIdLength(serverId, nameof(CreateRoleAsync));
            Conditions.RoleNameLength(name, nameof(CreateRoleAsync));
            CreateRoleRequest Req = new CreateRoleRequest
            {
                name = name
            };
            if (rank != null)
                Req.rank = Optional.Some(rank.Value);

            RoleJson Json = await rest.PostAsync<RoleJson>($"/servers/{serverId}/roles", Req);
            return new Role(rest.Client, Json, serverId, Json.Id);
        }

        /// <inheritdoc cref="ModifyRoleAsync(RevoltRestClient, string, string, Option{string}, Option{string}, Option{bool}, Option{int})" />
        public static Task<Role> ModifyAsync(this Role role, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
            => ModifyRoleAsync(role.Client.Rest, role.ServerId, role.Id, name, color, hoist, rank);

        /// <inheritdoc cref="ModifyRoleAsync(RevoltRestClient, string, string, Option{string}, Option{string}, Option{bool}, Option{int})" />
        public static Task<Role> ModifyRoleAsync(this Server server, Role role, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
            => ModifyRoleAsync(server.Client.Rest, server.Id, role.Id, name, color, hoist, rank);

        /// <inheritdoc cref="ModifyRoleAsync(RevoltRestClient, string, string, Option{string}, Option{string}, Option{bool}, Option{int})" />
        public static Task<Role> ModifyRoleAsync(this Server server, string roleId, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
            => ModifyRoleAsync(server.Client.Rest, server.Id, roleId, name, color, hoist, rank);

        /// <summary>
        /// Update a role with properties.
        /// </summary>
        /// <returns>
        /// <see cref="Role"/> 
        /// </returns>
        /// <exception cref="RevoltArgumentException"></exception>
        /// <exception cref="RevoltRestException"></exception>
        public static async Task<Role> ModifyRoleAsync(this RevoltRestClient rest, string serverId, string roleId, Option<string> name = null, Option<string> color = null, Option<bool> hoist = null, Option<int> rank = null)
        {
            Conditions.ServerIdLength(serverId, nameof(ModifyRoleAsync));
            Conditions.RoleIdLength(roleId, nameof(ModifyRoleAsync));

            ModifyRoleRequest Req = new ModifyRoleRequest();
            if (name != null)
            {
                Conditions.RoleNameLength(name.Value, nameof(ModifyRoleAsync));
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

        /// <inheritdoc cref="DeleteRoleAsync(RevoltRestClient, string, string)" />
        public static Task DeleteAsync(this Role role)
          => DeleteRoleAsync(role.Client.Rest, role.ServerId, role.Id);

        /// <inheritdoc cref="DeleteRoleAsync(RevoltRestClient, string, string)" />
        public static Task DeleteRoleAsync(this Server server, string roleId)
            => DeleteRoleAsync(server.Client.Rest, server.Id, roleId);

        /// <inheritdoc cref="DeleteRoleAsync(RevoltRestClient, string, string)" />
        public static Task DeleteRoleAsync(this Server server, Role role)
           => DeleteRoleAsync(server.Client.Rest, server.Id, role.Id);

        /// <summary>
        /// Delete a role from a server.
        /// </summary>
        /// <exception cref="RevoltArgumentException"></exception>
        /// <exception cref="RevoltRestException"></exception>
        public static async Task DeleteRoleAsync(this RevoltRestClient rest, string serverId, string roleId)
        {
            Conditions.ServerIdLength(serverId, nameof(DeleteRoleAsync));
            Conditions.RoleIdLength(roleId, nameof(DeleteRoleAsync));

            await rest.DeleteAsync($"/servers/{serverId}/roles/{roleId}");
        }
    }
}