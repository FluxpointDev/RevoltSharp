using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class MemberHelper
{

    public static Task AddRoleAsync(this ServerMember member, Role role)
        => AddRoleAsync(member.Client.Rest, member, role);

    public static Task AddRoleAsync(this ServerMember member, string roleId)
        => AddRoleAsync(member.Client.Rest, member, roleId);

    public static Task AddRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
        => AddRoleAsync(rest, member, role.Id);

    public static async Task AddRoleAsync(this RevoltRestClient rest, ServerMember member, string roleId)
    {
        Conditions.RoleIdEmpty(roleId, "AddRoleAsync");
        Conditions.OwnerModifyCheck(member, "AddRoleAsync");

        if (!member.RolesIds.Any(x => x == roleId))
        {
            await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
            {
                roles = Optional.Some(member.RolesIds.Append(roleId).Select(x => x).ToArray())
            });
        }
    }

    public static Task AddRolesAsync(this ServerMember member, Role[] roles)
        => AddRolesAsync(member.Client.Rest, member, roles.Select(x => x.Id).ToArray());

    public static Task AddRolesAsync(this ServerMember member, string[] roleIds)
        => AddRolesAsync(member.Client.Rest, member, roleIds);

    public static Task AddRolesAsync(this RevoltRestClient rest, ServerMember member, Role[] roles)
        => AddRolesAsync(rest, member, roles.Select(x => x.Id).ToArray());

    public static async Task AddRolesAsync(this RevoltRestClient rest, ServerMember member, string[] roleIds)
    {
        Conditions.OwnerModifyCheck(member, "AddRolesAsync");
        Conditions.RoleListEmpty(roleIds, "AddRolesAsync");

        foreach (string r in roleIds)
        {
            Conditions.RoleIdEmpty(r, "AddRolesAsync");
        }

        await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
        {
            roles = Optional.Some(member.RolesIds.Union(roleIds).ToArray())
        });

    }



    public static Task RemoveRoleAsync(this ServerMember member, Role role)
        => RemoveRoleAsync(member.Client.Rest, member, role);

    public static Task RemoveRoleAsync(this ServerMember member, string roleId)
        => RemoveRoleAsync(member.Client.Rest, member, roleId);

    public static Task RemoveRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
        => RemoveRoleAsync(rest, member, role.Id);

    public static async Task RemoveRoleAsync(this RevoltRestClient rest, ServerMember member, string roleId)
    {
        Conditions.RoleIdEmpty(roleId, "RemoveRoleAsync");
        Conditions.OwnerModifyCheck(member, "RemoveRoleAsync");

        if (member.Roles.Any(x => x.Id == roleId))
        {
            await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
            {
                roles = Optional.Some(member.RolesIds.Where(x => x != roleId).ToArray())
            });
        }
    }

    public static Task RemoveRolesAsync(this ServerMember member, Role[] roles)
        => RemoveRolesAsync(member.Client.Rest, member, roles.Select(x => x.Id).ToArray());

    public static Task RemoveRolesAsync(this ServerMember member, string[] roleIds)
        => RemoveRolesAsync(member.Client.Rest, member, roleIds);

    public static Task RemoveRolesAsync(this RevoltRestClient rest, ServerMember member, Role[] roles)
        => RemoveRolesAsync(rest, member, roles.Select(x => x.Id).ToArray());

    public static async Task RemoveRolesAsync(this RevoltRestClient rest, ServerMember member, string[] roleIds)
    {
        Conditions.OwnerModifyCheck(member, "RemoveRolesAsync");
        Conditions.RoleListEmpty(roleIds, "RemoveRolesAsync");
        foreach (string r in roleIds)
        {
            Conditions.RoleIdEmpty(r, "RemoveRolesAsync");
        }

        await rest.PatchAsync<HttpResponseMessage>($"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
        {
            roles = Optional.Some(member.RolesIds.Except(roleIds).ToArray())
        });

    }

    public static Task<ServerMember?> GetMemberAsync(this Server server, string userId)
        => GetMemberAsync(server.Client.Rest, server.Id, userId);

    public static async Task<ServerMember?> GetMemberAsync(this RevoltRestClient rest, string serverId, string userId)
    {
        Conditions.ServerIdEmpty(serverId, "GetMemberAsync");
        Conditions.UserIdEmpty(userId, "GetMemberAsync");

        if (rest.Client.WebSocket != null && rest.Client.WebSocket.ServerCache.TryGetValue(serverId, out Server Server) && Server.InternalMembers.TryGetValue(userId, out ServerMember sm))
            return sm;

        ServerMemberJson? Member = await rest.GetAsync<ServerMemberJson>($"servers/{serverId}/members/{userId}");
        if (Member == null)
            return null;

        User User = await rest.GetUserAsync(userId);
        if (User == null)
            return null;
        ServerMember SM = new ServerMember(rest.Client, Member, null, User);
        if (rest.Client.WebSocket != null)
            rest.Client.WebSocket.ServerCache[serverId].AddMember(SM);
        return SM;
    }

    public static Task<ServerMember[]?> GetMembersAsync(this Server server, bool onlineOnly = false)
       => GetMembersAsync(server.Client.Rest, server.Id);

    public static async Task<ServerMember[]?> GetMembersAsync(this RevoltRestClient rest, string serverId, bool onlineOnly = false)
    {
        Conditions.ServerIdEmpty(serverId, "GetMembersAsync");

        MembersListJson? List = await rest.GetAsync<MembersListJson>($"servers/{serverId}/members?exclude_offline=" + onlineOnly.ToString());
        if (List == null)
            return null;

        HashSet<ServerMember> Members = new HashSet<ServerMember>();
        for (int i = 0; i < List.Members.Length; i++)
        {
            Members.Add(new ServerMember(rest.Client, List.Members[i], List.Users[i], rest.Client.GetUser(List.Users[i].Id)));
        }
        return Members.ToArray();
    }


    public static Task KickMemberAsync(this Server server, string userId)
        => KickMemberAsync(server.Client.Rest, server.Id, userId);
    public static Task KickMemberAsync(this Server server, ServerMember member)
        => KickMemberAsync(server.Client.Rest, server.Id, member.Id);
    public static Task KickMemberAsync(this Server server, User user)
        => KickMemberAsync(server.Client.Rest, server.Id, user.Id);
    public static Task KickAsync(this ServerMember member)
        => KickMemberAsync(member.Client.Rest, member.ServerId, member.Id);

    public static async Task KickMemberAsync(this RevoltRestClient rest, string serverId, string userId)
    {
        Conditions.ServerIdEmpty(serverId, "KickMemberAsync");
        Conditions.UserIdEmpty(userId, "KickMemberAsync");

        await rest.DeleteAsync($"servers/{serverId}/members/{userId}");
    }
    public static Task<ServerBan> BanMemberAsync(this Server server, string userId, string reason = "")
        => BanMemberAsync(server.Client.Rest, server.Id, userId, reason);
    public static Task<ServerBan> BanMemberAsync(this Server server, ServerMember member, string reason = "")
        => BanMemberAsync(server.Client.Rest, server.Id, member.Id, reason);
    public static Task<ServerBan> BanMemberAsync(this Server server, User user, string reason = "")
        => BanMemberAsync(server.Client.Rest, server.Id, user.Id, reason);
    public static Task<ServerBan> BanAsync(this ServerMember member, string reason = "")
        => BanMemberAsync(member.Client.Rest, member.ServerId, member.Id, reason);

    public static async Task<ServerBan> BanMemberAsync(this RevoltRestClient rest, string serverId, string userId, string reason = "")
    {
        Conditions.ServerIdEmpty(serverId, "BanMemberAsync");
        Conditions.UserIdEmpty(userId, "BanMemberAsync");

        ReasonRequest Req = new ReasonRequest();
        if (!string.IsNullOrEmpty(reason))
            Req.reason = Optional.Some(reason);

        await rest.PutAsync<ServerBan>($"servers/{serverId}/bans/{userId}", Req);
        return new ServerBan(rest.Client, null, null) { Id = userId, Reason = reason };
    }
    public static Task UnBanMemberAsync(this Server server, string userId)
        => UnBanMemberAsync(server.Client.Rest, server.Id, userId);

    public static Task UnBanMemberAsync(this Server server, User user)
        => UnBanMemberAsync(server.Client.Rest, server.Id, user.Id);

    public static async Task UnBanMemberAsync(this RevoltRestClient rest, string serverId, string userId)
    {
        Conditions.ServerIdEmpty(serverId, "UnbanMemberAsync");
        Conditions.MemberIdEmpty(userId, "UnbanMemberAsync");

        await rest.DeleteAsync($"servers/{serverId}/bans/{userId}");
    }


    public static Task ModifyAsync(this ServerMember member, Option<string> nickname = null, Option<Attachment> avatar = null, Option<DateTime?> timeout = null)
    {
        Conditions.OwnerModifyCheck(member, "ModifyMemberAsync");
        return ModifyMemberAsync(member.Client.Rest, member.ServerId, member.Id, nickname, avatar, timeout);
    }

    public static Task ModifyMemberAsync(this Server server, ServerMember member, Option<string> nickname = null, Option<Attachment> avatar = null, Option<DateTime?> timeout = null)
        => ModifyAsync(member, nickname, avatar, timeout);

    public static Task ModifyMemberAsync(this Server server, string memberId, Option<string> nickname = null, Option<Attachment> avatar = null, Option<DateTime?> timeout = null)
        => ModifyMemberAsync(server.Client.Rest, server.Id, memberId, nickname, avatar, timeout);

    public static Task ModifyMemberAsync(this RevoltRestClient rest, Server server, string memberId, Option<string> nickname = null, Option<Attachment> avatar = null, Option<DateTime?> timeout = null)
        => ModifyMemberAsync(rest, server.Id, memberId, nickname, avatar, timeout);

    public static async Task ModifyMemberAsync(this RevoltRestClient rest, string serverId, string memberId, Option<string> nickname = null, Option<Attachment> avatar = null, Option<DateTime?> timeout = null)
    {
        Conditions.ServerIdEmpty(serverId, "ModifyMemberAsync");
        Conditions.MemberIdEmpty(memberId, "ModifyMemberAsync");

        EditMemberRequest Req = new EditMemberRequest();
        if (nickname != null)
        {
            if (string.IsNullOrEmpty(nickname.Value))
                Req.RemoveValue("Nickname");
            else
                Req.nickname = new Optional<string>(nickname.Value);
        }

        if (avatar != null)
        {
            if (avatar.Value == null)
                Req.RemoveValue("Avatar");
            else
                Req.avatar = new Optional<AttachmentJson>(avatar.Value.ToJson());
        }

        if (timeout != null)
        {
            if (!timeout.Value.HasValue)
                Req.RemoveValue("Timeout");
            else
                Req.timeout = new Optional<DateTime>(timeout.Value.Value);
        }

        await rest.PatchAsync<HttpResponseMessage>($"servers/{serverId}/members/{memberId}", Req);
    }
}
