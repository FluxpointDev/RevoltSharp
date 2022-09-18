using Optional;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class MemberHelper
    {
        public static async Task<HttpResponseMessage> AddRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
        {
            if (!member.Roles.Any(x => x.Id == role.Id))
                return await rest.SendRequestAsync(RequestType.Patch, $"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
                {
                    roles = Option.Some(member.Roles.Append(role).Select(x => x.Id).ToArray())
                });
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        public static async Task<HttpResponseMessage> RemoveRoleAsync(this RevoltRestClient rest, ServerMember member, Role role)
        {
            if (member.Roles.Any(x => x.Id == role.Id))
                return await rest.SendRequestAsync(RequestType.Patch, $"servers/{member.ServerId}/members/{member.Id}", new EditMemberRequest
                {
                    roles = Option.Some(member.Roles.Except(new Role[] { role }).Select(x => x.Id).ToArray())
                });
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        public static Task<ServerMember> GetMemberAsync(this Server server, string userId)
            => GetMemberAsync(server.Client.Rest, server.Id, userId);

        public static async Task<ServerMember> GetMemberAsync(this RevoltRestClient rest, string serverId, string userId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(userId))
                throw new RevoltArgumentException("User id can't be empty for this request.");

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

        public static Task<ServerMember[]> GetMembersAsync(this Server server)
           => GetMembersAsync(server.Client.Rest, server.Id);

        public static async Task<ServerMember[]> GetMembersAsync(this RevoltRestClient rest, string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            MembersListJson List = await rest.SendRequestAsync<MembersListJson>(RequestType.Get, $"servers/{serverId}/members");
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
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(userId))
                throw new RevoltArgumentException("User id can't be empty for this request.");

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
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(userId))
                throw new RevoltArgumentException("User id can't be empty for this request.");
            ReasonRequest Req = new ReasonRequest();
            if (!string.IsNullOrEmpty(reason))
                Req.reason = Optional.Option.Some(reason);

            return await rest.SendRequestAsync(RequestType.Put, $"servers/{serverId}/bans/{userId}", Req);
        }
        public static Task<HttpResponseMessage> UnbanMemberAsync(this Server server, string userId)
            => UnbanMemberAsync(server.Client.Rest, server.Id, userId);
        public static async Task<HttpResponseMessage> UnbanMemberAsync(this RevoltRestClient rest, string serverId, string userId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(userId))
                throw new RevoltArgumentException("User id can't be empty for this request.");
            
            return await rest.SendRequestAsync(RequestType.Delete, $"servers/{serverId}/bans/{userId}");
        }

    }
}
