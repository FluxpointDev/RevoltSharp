using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;


/// <summary>
/// Revolt http/rest methods for servers.
/// </summary>
public static class ServerHelper
{
    /// <summary>
    /// Get a server.
    /// </summary>
    /// <returns>
    /// <see cref="Server"/> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Server?> GetServerAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdLength(serverId, nameof(GetServerAsync));

        if (rest.Client.WebSocket != null && rest.Client.WebSocket.ServerCache.TryGetValue(serverId, out Server server))
            return server;

        ServerJson? ServerJson = await rest.GetAsync<ServerJson>($"/servers/{serverId}");
        if (ServerJson == null)
            return null;

        Server Server = new Server(rest.Client, ServerJson);

        if (rest.Client.WebSocket != null)
            rest.Client.WebSocket.ServerCache.TryAdd(serverId, Server);

        return Server;
    }

    /// <inheritdoc cref="GetBansAsync(RevoltRestClient, string)" />
    public static Task<IReadOnlyCollection<ServerBan>> GetBansAsync(this RevoltRestClient rest, Server server)
        => GetBansAsync(rest, server.Id);

    /// <inheritdoc cref="GetBansAsync(RevoltRestClient, string)" />
    public static Task<IReadOnlyCollection<ServerBan>> GetBansAsync(this Server server)
        => GetBansAsync(server.Client.Rest, server.Id);

    /// <summary>
    /// Get a list of banned users for a server.
    /// </summary>
    /// <returns>
    /// List of <see cref="ServerBan"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<ServerBan>> GetBansAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdLength(serverId, nameof(GetBansAsync));

        ServerBansJson? Bans = await rest.GetAsync<ServerBansJson>($"/servers/{serverId}/bans");
        if (Bans == null)
            return System.Array.Empty<ServerBan>();

        return Bans.Users.Select(x => new ServerBan(rest.Client, x, Bans.Bans.Where(b => b.Id.UserId == x.Id).FirstOrDefault())).ToImmutableArray();

    }

    /// <inheritdoc cref="LeaveServerAsync(RevoltRestClient, string)" />
    public static Task LeaveAsync(this Server server)
        => LeaveServerAsync(server.Client.Rest, server.Id);

    /// <inheritdoc cref="LeaveServerAsync(RevoltRestClient, string)" />
    public static Task LeaveAsync(this RevoltRestClient rest, Server server)
        => LeaveServerAsync(rest, server.Id);

    /// <summary>
    /// Leave server or delete it if owned.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task LeaveServerAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdLength(serverId, nameof(LeaveServerAsync));

        await rest.DeleteAsync($"/servers/{serverId}");
    }

    /// <inheritdoc cref="ModifyDefaultPermissions(RevoltRestClient, string, ServerPermissions)" />
    public static Task ModifyDefaultPermissions(this Server server, ServerPermissions permissions)
        => LeaveServerAsync(server.Client.Rest, server.Id);

    /// <inheritdoc cref="ModifyDefaultPermissions(RevoltRestClient, string, ServerPermissions)" />
    public static Task ModifyDefaultPermissions(this RevoltRestClient rest, Server server, ServerPermissions permissions)
        => LeaveServerAsync(rest, server.Id);

    /// <summary>
    /// Modify the default member permissions for the server.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Server> ModifyDefaultPermissions(this RevoltRestClient rest, string serverId, ServerPermissions permissions)
    {
        Conditions.ServerIdLength(serverId, nameof(ModifyDefaultPermissions));

        return await rest.PutAsync<Server>($"/servers/{serverId}/permissions/default", new ModifyDefaultPermissionsRequest
        {
            permissions = permissions.Raw
        });
    }
}