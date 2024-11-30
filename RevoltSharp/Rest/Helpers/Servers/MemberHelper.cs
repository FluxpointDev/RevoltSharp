using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace RevoltSharp;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>
/// Revolt http/rest methods for server members.
/// </summary>
public static class MemberHelper
{
    /// <inheritdoc cref="AddRoleAsync(RevoltRestClient, ServerMember, string)" />
    public static Task AddRoleAsync(this ServerMember member, Role role)
        => AddRoleAsync(member.Client.Rest, member, role);

    /// <inheritdoc cref="AddRoleAsync(RevoltRestClient, ServerMember, string)" />
    public static Task AddRoleAsync(this ServerMember member, string roleId)
        => AddRoleAsync(member.Client.Rest, member, roleId);

    /// <inheritdoc cref="AddRoleAsync(RevoltRestClient, ServerMember, string)" />
    public static Task AddRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
        => AddRoleAsync(rest, member, role.Id);

    /// <summary>
    /// Add a role to a server member.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task AddRoleAsync(this RevoltRestClient rest, ServerMember member, string roleId)
    {
        Conditions.RoleIdLength(roleId, nameof(AddRoleAsync));
        
        await member.RoleLock.WaitAsync();
        EditMemberRequest request = new EditMemberRequest
        {
            roles = Optional.Some(member.RolesIds.Append(roleId).Select(x => x).ToArray())
        };
        try
        {
            HttpResponseMessage response = await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", request);
            if (response.IsSuccessStatusCode)
            {
                member.RolesIds = request.roles.Value;
                member.InternalRoles = new ConcurrentDictionary<string, Role>(request.roles.Value.ToDictionary(x => x, x => member.Server.InternalRoles[x]));
            }
        }
        finally
        {
            member.RoleLock.Release();
        }
    }

    /// <inheritdoc cref="AddRolesAsync(RevoltRestClient, ServerMember, string[])" />
    public static Task AddRolesAsync(this ServerMember member, Role[] roles)
        => AddRolesAsync(member.Client.Rest, member, roles.Select(x => x.Id).ToArray());

    /// <inheritdoc cref="AddRolesAsync(RevoltRestClient, ServerMember, string[])" />
    public static Task AddRolesAsync(this ServerMember member, string[] roleIds)
        => AddRolesAsync(member.Client.Rest, member, roleIds);

    /// <inheritdoc cref="AddRolesAsync(RevoltRestClient, ServerMember, string[])" />
    public static Task AddRolesAsync(this RevoltRestClient rest, ServerMember member, Role[] roles)
        => AddRolesAsync(rest, member, roles.Select(x => x.Id).ToArray());

    /// <summary>
    /// Add roles to a server member.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task AddRolesAsync(this RevoltRestClient rest, ServerMember member, string[] roleIds)
    {
        Conditions.RoleListEmpty(roleIds, nameof(AddRolesAsync));

        foreach (string r in roleIds)
        {
            Conditions.RoleIdLength(r, nameof(AddRolesAsync));
        }
        await member.RoleLock.WaitAsync();
        EditMemberRequest request = new EditMemberRequest
        {
            roles = Optional.Some(member.RolesIds.Union(roleIds).ToArray())
        };
        try
        {
            HttpResponseMessage response = await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", request);
            if (response.IsSuccessStatusCode)
            {
                member.RolesIds = request.roles.Value;
                member.InternalRoles = new ConcurrentDictionary<string, Role>(request.roles.Value.ToDictionary(x => x, x => member.Server.InternalRoles[x]));
            }
        }
        finally
        {
            member.RoleLock.Release();
        }
        
    }

    /// <inheritdoc cref="RemoveRoleAsync(RevoltRestClient, ServerMember, string)" />
    public static Task RemoveRoleAsync(this ServerMember member, Role role)
        => RemoveRoleAsync(member.Client.Rest, member, role);

    /// <inheritdoc cref="RemoveRoleAsync(RevoltRestClient, ServerMember, string)" />
    public static Task RemoveRoleAsync(this ServerMember member, string roleId)
        => RemoveRoleAsync(member.Client.Rest, member, roleId);

    /// <inheritdoc cref="RemoveRoleAsync(RevoltRestClient, ServerMember, string)" />
    public static Task RemoveRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
        => RemoveRoleAsync(rest, member, role.Id);

    /// <summary>
    /// Remove a role from a server member.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task RemoveRoleAsync(this RevoltRestClient rest, ServerMember member, string roleId)
    {
        Conditions.RoleIdLength(roleId, nameof(RemoveRoleAsync));

        await member.RoleLock.WaitAsync();
        var request = new EditMemberRequest
        {
            roles = Optional.Some(member.RolesIds.Where(x => x != roleId).ToArray())
        };
        try
        {
            HttpResponseMessage response = await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", request);
            if (response.IsSuccessStatusCode)
            {
                member.RolesIds = request.roles.Value;
                member.InternalRoles = new ConcurrentDictionary<string, Role>(request.roles.Value.ToDictionary(x => x, x => member.Server.InternalRoles[x]));
            }
        }
        finally
        {
            member.RoleLock.Release();
        }
    }

    /// <inheritdoc cref="RemoveRolesAsync(RevoltRestClient, ServerMember, string[])" />
    public static Task RemoveRolesAsync(this ServerMember member, Role[] roles)
        => RemoveRolesAsync(member.Client.Rest, member, roles.Select(x => x.Id).ToArray());

    /// <inheritdoc cref="RemoveRolesAsync(RevoltRestClient, ServerMember, string[])" />
    public static Task RemoveRolesAsync(this ServerMember member, string[] roleIds)
        => RemoveRolesAsync(member.Client.Rest, member, roleIds);

    /// <inheritdoc cref="RemoveRolesAsync(RevoltRestClient, ServerMember, string[])" />
    public static Task RemoveRolesAsync(this RevoltRestClient rest, ServerMember member, Role[] roles)
        => RemoveRolesAsync(rest, member, roles.Select(x => x.Id).ToArray());

    /// <summary>
    /// Remove roles from a server member.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task RemoveRolesAsync(this RevoltRestClient rest, ServerMember member, string[] roleIds)
    {
        Conditions.RoleListEmpty(roleIds, nameof(RemoveRolesAsync));
        foreach (string r in roleIds)
        {
            Conditions.RoleIdLength(r, nameof(RemoveRolesAsync));
        }
        await member.RoleLock.WaitAsync();

        EditMemberRequest request = new EditMemberRequest
        {
            roles = Optional.Some(member.RolesIds.Except(roleIds).ToArray())
        };
        try
        {
            HttpResponseMessage response = await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", request);
            if (response.IsSuccessStatusCode)
            {
                member.RolesIds = request.roles.Value;
                member.InternalRoles = new ConcurrentDictionary<string, Role>(request.roles.Value.ToDictionary(x => x, x => member.Server.InternalRoles[x]));
            }
        }
        finally
        {
            member.RoleLock.Release();
        }
    }

    /// <inheritdoc cref="GetMemberAsync(RevoltRestClient, string, string)" />
    public static Task<ServerMember?> GetMemberAsync(this Server server, string userId)
        => GetMemberAsync(server.Client.Rest, server.Id, userId);

    /// <summary>
    /// Get member info from a server.
    /// </summary>
    /// <returns>
    /// <see cref="ServerMember"/> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<ServerMember?> GetMemberAsync(this RevoltRestClient rest, string serverId, string userId)
    {
        Conditions.ServerIdLength(serverId, nameof(GetMemberAsync));
        Conditions.UserIdLength(userId, nameof(GetMemberAsync));

        if (rest.Client.TryGetServer(serverId, out Server Server) && Server.InternalMembers.TryGetValue(userId, out ServerMember sm))
            return sm;

        ServerMemberJson? Member = await rest.GetAsync<ServerMemberJson>($"servers/{serverId}/members/{userId}");
        if (Member == null)
            return null;

        User User = await rest.GetUserAsync(userId);
        if (User == null)
            return null;

        ServerMember SM = new ServerMember(rest.Client, Member, null, User);
        if (rest.Client.WebSocket != null)
        {
            try
            {
                Server.AddMember(SM);
            }
            catch { }
        }
        return SM;
    }

    /// <inheritdoc cref="GetMemberAsync(RevoltRestClient, string, string)" />
    public static Task<IReadOnlyCollection<ServerMember>?> GetMembersAsync(this Server server, bool onlineOnly = false)
       => GetMembersAsync(server.Client.Rest, server.Id, onlineOnly);

    /// <summary>
    /// Get all members from a server.
    /// </summary>
    /// <remarks>
    /// It is recommended to reuse this list or use the server member cache once this has completed.
    /// </remarks>
    /// <returns>
    /// List of <see cref="ServerMember"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<ServerMember>?> GetMembersAsync(this RevoltRestClient rest, string serverId, bool onlineOnly = false)
    {
        Conditions.ServerIdLength(serverId, nameof(GetMembersAsync));

        MembersListJson? List = await rest.GetAsync<MembersListJson>($"servers/{serverId}/members?exclude_offline=" + onlineOnly.ToString());
        if (List == null)
            return System.Array.Empty<ServerMember>();

        HashSet<ServerMember> Members = new HashSet<ServerMember>();
        for (int i = 0; i < List.Members.Length; i++)
        {
            if (!rest.Client.TryGetUser(List.Users[i].Id, out User user))
            {
                user = new User(rest.Client, List.Users[i]);

                if (rest.Client.WebSocket != null)
                    rest.Client.WebSocket.UserCache.TryAdd(List.Users[i].Id, user);
            }


            Members.Add(new ServerMember(rest.Client, List.Members[i], List.Users[i], user));
        }

        if (!onlineOnly && rest.Client.WebSocket != null && rest.Client.TryGetServer(serverId, out Server cachedServer))
            cachedServer.HasAllMembers = true;

        return Members.ToImmutableArray();
    }

    /// <inheritdoc cref="KickMemberAsync(RevoltRestClient, string, string)" />
    public static Task KickMemberAsync(this Server server, string userId)
        => KickMemberAsync(server.Client.Rest, server.Id, userId);

    /// <inheritdoc cref="KickMemberAsync(RevoltRestClient, string, string)" />
    public static Task KickMemberAsync(this Server server, ServerMember member)
        => KickMemberAsync(server.Client.Rest, server.Id, member.Id);

    /// <inheritdoc cref="KickMemberAsync(RevoltRestClient, string, string)" />
    public static Task KickMemberAsync(this Server server, User user)
        => KickMemberAsync(server.Client.Rest, server.Id, user.Id);

    /// <inheritdoc cref="KickMemberAsync(RevoltRestClient, string, string)" />
    public static Task KickAsync(this ServerMember member)
        => KickMemberAsync(member.Client.Rest, member.ServerId, member.Id);

    /// <summary>
    /// Kick a member from a server.
    /// </summary>
    /// <remarks>
    /// Current user/bot account needs <see cref="ServerPermission.KickMembers"/>
    /// </remarks>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task KickMemberAsync(this RevoltRestClient rest, string serverId, string userId)
    {
        Conditions.ServerIdLength(serverId, nameof(KickMemberAsync));
        Conditions.UserIdLength(userId, nameof(KickMemberAsync));

        await rest.DeleteAsync($"servers/{serverId}/members/{userId}");
    }

    /// <inheritdoc cref="BanMemberAsync(RevoltRestClient, string, string, string)" />
    public static Task<ServerBanInfo> BanMemberAsync(this Server server, string userId, string? reason = null)
        => BanMemberAsync(server.Client.Rest, server.Id, userId, reason);

    /// <inheritdoc cref="BanMemberAsync(RevoltRestClient, string, string, string)" />
    public static Task<ServerBanInfo> BanMemberAsync(this Server server, ServerMember member, string? reason = null)
        => BanMemberAsync(server.Client.Rest, server.Id, member.Id, reason);

    /// <inheritdoc cref="BanMemberAsync(RevoltRestClient, string, string, string)" />
    public static Task<ServerBanInfo> BanMemberAsync(this Server server, User user, string? reason = null)
        => BanMemberAsync(server.Client.Rest, server.Id, user.Id, reason);

    /// <inheritdoc cref="BanMemberAsync(RevoltRestClient, string, string, string)" />
    public static Task<ServerBanInfo> BanAsync(this ServerMember member, string? reason = null)
        => BanMemberAsync(member.Client.Rest, member.ServerId, member.Id, reason);

    /// <summary>
    /// Ban a member from a server.
    /// </summary>
    /// <remarks>
    /// Current user/bot account needs <see cref="ServerPermission.BanMembers"/>
    /// </remarks>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<ServerBanInfo> BanMemberAsync(this RevoltRestClient rest, string serverId, string userId, string? reason = null)
    {
        Conditions.ServerIdLength(serverId, nameof(BanMemberAsync));
        Conditions.UserIdLength(userId, nameof(BanMemberAsync));

        ReasonRequest Req = new ReasonRequest();
        if (!string.IsNullOrEmpty(reason))
            Req.reason = Optional.Some(reason);

        ServerBanInfoJson ServerBanJson = await rest.PutAsync<ServerBanInfoJson>($"servers/{serverId}/bans/{userId}", Req);
        return new ServerBanInfo(rest.Client, ServerBanJson);
    }

    /// <inheritdoc cref="UnBanMemberAsync(RevoltRestClient, string, string)" />
    public static Task UnBanMemberAsync(this Server server, string userId)
        => UnBanMemberAsync(server.Client.Rest, server.Id, userId);

    /// <inheritdoc cref="UnBanMemberAsync(RevoltRestClient, string, string)" />
    public static Task UnBanMemberAsync(this Server server, User user)
        => UnBanMemberAsync(server.Client.Rest, server.Id, user.Id);

    /// <summary>
    /// Unban a member from a server.
    /// </summary>
    /// <remarks>
    /// Current user/bot account needs <see cref="ServerPermission.BanMembers"/>
    /// </remarks>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task UnBanMemberAsync(this RevoltRestClient rest, string serverId, string userId)
    {
        Conditions.ServerIdLength(serverId, nameof(UnBanMemberAsync));
        Conditions.MemberIdLength(userId, nameof(UnBanMemberAsync));

        await rest.DeleteAsync($"servers/{serverId}/bans/{userId}");
    }

    /// <inheritdoc cref="ModifyMemberAsync(RevoltRestClient, string, string, Option{string}, Option{Attachment}, Option{DateTime?})" />
    public static Task<ServerMember> ModifyAsync(this ServerMember member, Option<string?>? nickname = null, Option<Attachment?>? avatar = null, Option<DateTime?>? timeout = null)
    {
        return ModifyMemberAsync(member.Client.Rest, member.ServerId, member.Id, nickname, avatar, timeout);
    }

    /// <inheritdoc cref="ModifyMemberAsync(RevoltRestClient, string, string, Option{string}, Option{Attachment}, Option{DateTime?})" />
    public static Task<ServerMember> ModifyMemberAsync(this Server _, ServerMember member, Option<string?>? nickname = null, Option<Attachment?>? avatar = null, Option<DateTime?>? timeout = null)
        => ModifyAsync(member, nickname, avatar, timeout);

    /// <inheritdoc cref="ModifyMemberAsync(RevoltRestClient, string, string, Option{string}, Option{Attachment}, Option{DateTime?})" />
    public static Task<ServerMember> ModifyMemberAsync(this Server server, string memberId, Option<string?>? nickname = null, Option<Attachment?>? avatar = null, Option<DateTime?>? timeout = null)
    {
        return ModifyMemberAsync(server.Client.Rest, server.Id, memberId, nickname, avatar, timeout);
    }

    /// <inheritdoc cref="ModifyMemberAsync(RevoltRestClient, string, string, Option{string}, Option{Attachment}, Option{DateTime?})" />
    public static Task<ServerMember> ModifyMemberAsync(this RevoltRestClient rest, Server server, string memberId, Option<string?>? nickname = null, Option<Attachment?>? avatar = null, Option<DateTime?>? timeout = null)
    {
        return ModifyMemberAsync(rest, server.Id, memberId, nickname, avatar, timeout);
    }

    /// <summary>
    /// Modify a server member.
    /// </summary>
    /// <remarks>
    /// This will not return a full user object!
    /// </remarks>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<ServerMember> ModifyMemberAsync(this RevoltRestClient rest, string serverId, string memberId, Option<string?>? nickname = null, Option<Attachment?>? avatar = null, Option<DateTime?>? timeout = null)
    {
        Conditions.ServerIdLength(serverId, nameof(ModifyMemberAsync));
        Conditions.MemberIdLength(memberId, nameof(ModifyMemberAsync));
        EditMemberRequest Req = new EditMemberRequest();
        if (nickname != null)
        {
            if (string.IsNullOrEmpty(nickname.Value))
                Req.RemoveValue("Nickname");
            else
                Req.nickname = Optional.Some(nickname.Value);
        }

        if (avatar != null)
        {
            if (avatar.Value == null)
                Req.RemoveValue("Avatar");
            else
                Req.avatar = Optional.Some(avatar.Value.ToJson());
        }

        if (timeout != null)
        {
            if (!timeout.Value.HasValue)
                Req.RemoveValue("Timeout");
            else
                Req.timeout = Optional.Some(timeout.Value.Value);
        }
        ServerMemberJson member = await rest.PatchAsync<ServerMemberJson>($"servers/{serverId}/members/{memberId}", Req);

        return new ServerMember(rest.Client, member, new UserJson { Id = member.Id.User }, null);
    }
}