using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class MemberHelper
    {

        public static Task<HttpResponseMessage> AddRoleAsync(this ServerMember member, Role role)
            => AddRoleAsync(member.Client.Rest, member, role);

        public static Task<HttpResponseMessage> AddRoleAsync(this ServerMember member, string roleId)
            => AddRoleAsync(member.Client.Rest, member, roleId);

        public static Task<HttpResponseMessage> AddRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
            => AddRoleAsync(rest, member, role.Id);

        public static async Task<HttpResponseMessage> AddRoleAsync(this RevoltRestClient rest, ServerMember member, string roleId)
        {
            Conditions.RoleIdEmpty(roleId, "AddRoleAsync");

            if (!member.RolesIds.Any(x => x == roleId))
                return await rest.SendRequestAsync(RequestType.Patch, $"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
                {
                    roles = Optional.Some(member.RolesIds.Append(roleId).Select(x => x).ToArray())
                });
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        public static Task<HttpResponseMessage> RemoveRoleAsync(this ServerMember member, Role role)
            => RemoveRoleAsync(member.Client.Rest, member, role);

        public static Task<HttpResponseMessage> RemoveRoleAsync(this ServerMember member, string roleId)
            => RemoveRoleAsync(member.Client.Rest, member, roleId);

        public static Task<HttpResponseMessage> RemoveRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
            => RemoveRoleAsync(rest, member, role.Id);

        public static async Task<HttpResponseMessage> RemoveRoleAsync(this RevoltRestClient rest, ServerMember member, string roleId)
        {
            Conditions.RoleIdEmpty(roleId, "RemoveRoleAsync");

            if (member.Roles.Any(x => x.Id == roleId))
                return await rest.SendRequestAsync(RequestType.Patch, $"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
                {
                    roles = Optional.Some(member.RolesIds.Where(x => x != roleId).ToArray())
                });
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        public static Task<ServerMember> GetMemberAsync(this Server server, string userId)
            => GetMemberAsync(server.Client.Rest, server.Id, userId);

        public static async Task<ServerMember> GetMemberAsync(this RevoltRestClient rest, string serverId, string userId)
        {
            Conditions.ServerIdEmpty(serverId, "GetMemberAsync");
            Conditions.UserIdEmpty(userId, "GetMemberAsync");

            if (rest.Client.WebSocket != null && rest.Client.WebSocket.ServerCache.TryGetValue(serverId, out Server Server) && Server.InternalMembers.TryGetValue(userId, out ServerMember sm))
                return sm;

            ServerMemberJson Member = await rest.SendRequestAsync<ServerMemberJson>(RequestType.Get, $"servers/{serverId}/members/{userId}");
            if (Member == null)
                return null;
            User User = await rest.GetUserAsync(userId);
            ServerMember SM = new ServerMember(rest.Client, Member, null, User);
            if (rest.Client.WebSocket != null)
                rest.Client.WebSocket.ServerCache[serverId].AddMember(SM);
            return SM;
        }

        public static Task<ServerMember[]> GetMembersAsync(this Server server, bool onlineOnly = false)
           => GetMembersAsync(server.Client.Rest, server.Id);

        public static async Task<ServerMember[]> GetMembersAsync(this RevoltRestClient rest, string serverId, bool onlineOnly = false)
        {
            Conditions.ServerIdEmpty(serverId, "GetMembersAsync");

            MembersListJson List = await rest.SendRequestAsync<MembersListJson>(RequestType.Get, $"servers/{serverId}/members?exclude_offline=" + onlineOnly.ToString());
            HashSet<ServerMember> Members = new HashSet<ServerMember>();
            for (int i = 0; i < List.Members.Length; i++)
            {
                Members.Add(new ServerMember(rest.Client, List.Members[i], List.Users[i], rest.Client.GetUser(List.Users[i].Id)));
            }
            return Members.ToArray();
        }


        public static Task<HttpResponseMessage> KickMemberAsync(this Server server, string userId)
            => KickMemberAsync(server.Client.Rest, server.Id, userId);
        public static Task<HttpResponseMessage> KickMemberAsync(this Server server, ServerMember member)
            => KickMemberAsync(server.Client.Rest, server.Id, member.Id);
        public static Task<HttpResponseMessage> KickAsync(this ServerMember member)
            => KickMemberAsync(member.Client.Rest, member.ServerId, member.Id);

        public static async Task<HttpResponseMessage> KickMemberAsync(this RevoltRestClient rest, string serverId, string userId)
        {
            Conditions.ServerIdEmpty(serverId, "KickMemberAsync");
            Conditions.UserIdEmpty(userId, "KickMemberAsync");

            return await rest.SendRequestAsync(RequestType.Delete, $"servers/{serverId}/members/{userId}");
        }
        public static Task<HttpResponseMessage> BanMemberAsync(this Server server, string userId, string reason = "")
            => BanMemberAsync(server.Client.Rest, server.Id, userId, reason);
        public static Task<HttpResponseMessage> BanMemberAsync(this Server server, ServerMember member, string reason = "")
            => BanMemberAsync(server.Client.Rest, server.Id, member.Id, reason);
        public static Task<HttpResponseMessage> BanAsync(this ServerMember member, string reason = "")
            => BanMemberAsync(member.Client.Rest, member.ServerId, member.Id, reason);

        public static async Task<HttpResponseMessage> BanMemberAsync(this RevoltRestClient rest, string serverId, string userId, string reason = "")
        {
            Conditions.ServerIdEmpty(serverId, "BanMemberAsync");
            Conditions.UserIdEmpty(userId, "BanMemberAsync");

            ReasonRequest Req = new ReasonRequest();
            if (!string.IsNullOrEmpty(reason))
                Req.reason = Optional.Some(reason);

            return await rest.SendRequestAsync(RequestType.Put, $"servers/{serverId}/bans/{userId}", Req);
        }
        public static Task<HttpResponseMessage> UnbanMemberAsync(this Server server, string userId)
            => UnbanMemberAsync(server.Client.Rest, server.Id, userId);
        public static async Task<HttpResponseMessage> UnbanMemberAsync(this RevoltRestClient rest, string serverId, string userId)
        {
            Conditions.ServerIdEmpty(serverId, "UnbanMemberAsync");
            Conditions.MemberIdEmpty(userId, "UnbanMemberAsync");

            return await rest.SendRequestAsync(RequestType.Delete, $"servers/{serverId}/bans/{userId}");
        }


        public static Task<HttpResponseMessage> ModifyAsync(this ServerMember member, Option<string> nickname, Option<Attachment> avatar, Option<DateTime> timeout)
            => ModifyMemberAsync(member.Client.Rest, member.ServerId, member.Id, nickname, avatar, timeout);

        public static Task<HttpResponseMessage> ModifyMemberAsync(this Server server, ServerMember member, Option<string> nickname, Option<Attachment> avatar, Option<DateTime> timeout)
            => ModifyMemberAsync(server.Client.Rest, server.Id, member.Id, nickname, avatar, timeout);

        public static Task<HttpResponseMessage> ModifyMemberAsync(this Server server, string memberId, Option<string> nickname, Option<Attachment> avatar, Option<DateTime> timeout)
            => ModifyMemberAsync(server.Client.Rest, server.Id, memberId, nickname, avatar, timeout);

        public static Task<HttpResponseMessage> ModifyMemberAsync(this RevoltRestClient rest, Server server, string memberId, Option<string> nickname, Option<Attachment> avatar, Option<DateTime> timeout)
            => ModifyMemberAsync(rest, server.Id, memberId, nickname, avatar, timeout);

        public static async Task<HttpResponseMessage> ModifyMemberAsync(this RevoltRestClient rest, string serverId, string memberId, Option<string> nickname, Option<Attachment> avatar, Option<DateTime> timeout)
        {
            Conditions.ServerIdEmpty(serverId, "ModifyMemberAsync");
            Conditions.MemberIdEmpty(memberId, "ModifyMemberAsync");

            EditMemberRequest Req = new EditMemberRequest();
            if (nickname != null)
                Req.nickname = new Optional<string>(nickname.Value);

            if (avatar != null)
                Req.avatar = new Optional<AttachmentJson>(avatar.Value.ToJson());

            if (timeout != null)
                Req.timeout = new Optional<DateTime>(timeout.Value);

            return await rest.SendRequestAsync(RequestType.Patch, $"servers/{serverId}/members/{memberId}", Req);
        }
    }
}
