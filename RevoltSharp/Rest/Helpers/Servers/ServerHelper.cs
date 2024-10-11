using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
    public static Task<IReadOnlyCollection<ServerBan>?> GetBansAsync(this RevoltRestClient rest, Server server)
        => GetBansAsync(rest, server.Id);

    /// <inheritdoc cref="GetBansAsync(RevoltRestClient, string)" />
    public static Task<IReadOnlyCollection<ServerBan>?> GetBansAsync(this Server server)
        => GetBansAsync(server.Client.Rest, server.Id);

    /// <summary>
    /// Get a list of banned users for a server.
    /// </summary>
    /// <returns>
    /// List of <see cref="ServerBan"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<ServerBan>?> GetBansAsync(this RevoltRestClient rest, string serverId)
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

    /// <inheritdoc cref="ModifyDefaultPermissionsAsync(RevoltRestClient, string, ServerPermissions)" />
    public static Task ModifyDefaultPermissionsAsync(this Server server, ServerPermissions permissions)
        => ModifyDefaultPermissionsAsync(server.Client.Rest, server.Id, permissions);

    /// <inheritdoc cref="ModifyDefaultPermissionsAsync(RevoltRestClient, string, ServerPermissions)" />
    public static Task ModifyDefaultPermissionsAsync(this RevoltRestClient rest, Server server, ServerPermissions permissions)
        => ModifyDefaultPermissionsAsync(rest, server.Id, permissions);

    /// <summary>
    /// Modify the default member permissions for the server.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Server> ModifyDefaultPermissionsAsync(this RevoltRestClient rest, string serverId, ServerPermissions permissions)
    {
        Conditions.ServerIdLength(serverId, nameof(ModifyDefaultPermissionsAsync));

        ServerJson server = await rest.PutAsync<ServerJson>($"/servers/{serverId}", new ModifyDefaultPermissionsRequest
        {
            permissions = permissions.Raw
        });
        return new Server(rest.Client, server);
    }

    /// <inheritdoc cref="ModifyServerAsync(RevoltRestClient, string, Option{string}?, Option{string?}, Option{string?}?, Option{string?}?)" />
    public static Task ModifyAsync(this Server server, Option<string>? name = null, Option<string?> description = null, Option<string?>? icon = null, Option<string?>? banner = null)
        => ModifyServerAsync(server.Client.Rest, server.Id, name, description, icon, banner);

    /// <inheritdoc cref="ModifyServerAsync(RevoltRestClient, string, Option{string}?, Option{string?}, Option{string?}?, Option{string?}?)" />
    public static Task ModifyServerAsync(this RevoltRestClient rest, Server server, Option<string>? name = null, Option<string?> description = null, Option<string?>? icon = null, Option<string?>? banner = null)
        => ModifyServerAsync(rest, server.Id, name, description, icon, banner);

    /// <summary>
    /// Modify the properties of a Revolt server such as name, description, icon and banner.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Server> ModifyServerAsync(this RevoltRestClient rest, string serverId, Option<string>? name = null, Option<string?> description = null, Option<string?>? icon = null, Option<string?>? banner = null)
    {
        Conditions.ServerIdLength(serverId, nameof(ModifyServerAsync));
        ModifyServerRequest Req = new ModifyServerRequest();
        if (name != null)
        {
            Conditions.ServerNameLength(name.Value, nameof(ModifyServerAsync));
            Req.name = Optional.Some(name.Value);
        }
        if (description != null)
        {
            if (string.IsNullOrEmpty(description.Value))
            {
                Req.RemoveValue("Description");
            }
            else
            {
                Conditions.ServerDescriptionLength(description.Value, nameof(ModifyServerAsync));
                Req.description = Optional.Some(description.Value);
            }
        }

        if (icon != null)
        {
            if (string.IsNullOrEmpty(icon.Value))
                Req.RemoveValue("Icon");
            else
            {
                Conditions.AttachmentIdLength(icon.Value, nameof(ModifyServerAsync));
                Req.icon = Optional.Some(icon.Value);
            }
        }

        if (banner != null)
        {
            if (string.IsNullOrEmpty(banner.Value))
                Req.RemoveValue("Banner");
            else
            {
                Conditions.AttachmentIdLength(banner.Value, nameof(ModifyServerAsync));
                Req.banner = Optional.Some(banner.Value);
            }
                
        }

        ServerJson server = await rest.PatchAsync<ServerJson>($"/servers/{serverId}", Req);
        return new Server(rest.Client, server);
    }

    /// <inheritdoc cref="ModifyServerSystemMessagesAsync(RevoltRestClient, string, Option{string?}?, Option{string?}?, Option{string?}?, Option{string?}?)" />
    public static Task ModifySystemMessagesAsync(this Server server, Option<string?>? userJoinedChannelId = null, Option<string?>? userLeftChannelId = null, Option<string?>? userKickedChannelId = null, Option<string?>? userBannedChannelId = null)
        => ModifyServerSystemMessagesAsync(server.Client.Rest, server.Id, userJoinedChannelId, userLeftChannelId, userKickedChannelId, userBannedChannelId);

    /// <inheritdoc cref="ModifyServerSystemMessagesAsync(RevoltRestClient, string, Option{string?}?, Option{string?}?, Option{string?}?, Option{string?}?)" />
    public static Task ModifyServerSystemMessagesAsync(this RevoltRestClient rest, Server server, Option<string?>? userJoinedChannelId = null, Option<string?>? userLeftChannelId = null, Option<string?>? userKickedChannelId = null, Option<string?>? userBannedChannelId = null)
        => ModifyServerSystemMessagesAsync(rest, server.Id, userJoinedChannelId, userLeftChannelId, userKickedChannelId, userBannedChannelId);

    /// <summary>
    /// Modify the properties of a Revolt server such as name, description, icon and banner.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Server> ModifyServerSystemMessagesAsync(this RevoltRestClient rest, string serverId, Option<string?>? userJoinedChannelId = null, Option<string?>? userLeftChannelId = null, Option<string?>? userKickedChannelId = null, Option<string?>? userBannedChannelId = null)
    {
        Conditions.ServerIdLength(serverId, nameof(ModifyServerAsync));
        ModifyServerRequest Req = new ModifyServerRequest();
        if (userJoinedChannelId != null)
        {
            if (!Req.system_messages.HasValue)
                Req.system_messages = Optional.Some(new ModifyServerSystemChannels());

            Req.system_messages.Value.user_joined = Optional.Some(userJoinedChannelId.Value);
        }

        if (userLeftChannelId != null)
        {
            if (!Req.system_messages.HasValue)
                Req.system_messages = Optional.Some(new ModifyServerSystemChannels());

            Req.system_messages.Value.user_left = Optional.Some(userLeftChannelId.Value);
        }

        if (userKickedChannelId != null)
        {
            if (!Req.system_messages.HasValue)
                Req.system_messages = Optional.Some(new ModifyServerSystemChannels());

            Req.system_messages.Value.user_kicked = Optional.Some(userKickedChannelId.Value);
        }

        if (userBannedChannelId != null)
        {
            if (!Req.system_messages.HasValue)
                Req.system_messages = Optional.Some(new ModifyServerSystemChannels());

            Req.system_messages.Value.user_banned = Optional.Some(userBannedChannelId.Value);
        }

        ServerJson server = await rest.PatchAsync<ServerJson>($"/servers/{serverId}", Req);
        return new Server(rest.Client, server);
    }
}