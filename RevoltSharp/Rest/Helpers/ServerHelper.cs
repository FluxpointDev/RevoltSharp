using RevoltSharp.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class ServerHelper
    {
        public static Task<Server> GetServerAsync(this SelfUser user, string serverId)
            => GetServerAsync(user.Client.Rest, serverId);

        public static async Task<Server> GetServerAsync(this RevoltRestClient rest, string serverId)
        {
            ServerJson Server = await rest.SendRequestAsync<ServerJson>(RequestType.Get, $"/servers/{serverId}");
            return new Server(rest.Client, Server);
        }

        public static Task<ServerMember> GetMemberAsync(this Server server, string userId)
            => GetMemberAsync(server.Client.Rest, server.Id, userId);

        public static async Task<ServerMember> GetMemberAsync(this RevoltRestClient rest, string serverId, string userId)
        {
            if (rest.Client.WebSocket != null && rest.Client.WebSocket.ServerCache.TryGetValue(serverId, out Server Server) && Server.Members.TryGetValue(userId, out ServerMember sm))
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
            MembersListJson List = await rest.SendRequestAsync<MembersListJson>(RequestType.Get, $"servers/{serverId}/members");
            HashSet<ServerMember> Members = new HashSet<ServerMember>();
            for (int i = 0; i < List.Members.Length; i++)
            {
                Members.Add(new ServerMember(rest.Client, List.Members[i], List.Users[i], rest.Client.GetUser(List.Users[i].Id)));
            }
            return Members.ToArray();
        }

        public static Task<HttpResponseMessage> LeaveServerAsync(this Server server)
            => LeaveServerAsync(server.Client.Rest, server.Id);
        public static Task<HttpResponseMessage> LeaveServerAsync(this SelfUser user, Server server)
           => LeaveServerAsync(user.Client.Rest, server.Id);
        public static Task<HttpResponseMessage> LeaveServerAsync(this SelfUser user, string serverId)
           => LeaveServerAsync(user.Client.Rest, serverId);
        public static async Task<HttpResponseMessage> LeaveServerAsync(this RevoltRestClient rest, string serverId)
        {
            return await rest.SendRequestAsync(RequestType.Delete, $"/servers/{serverId}");
        }
    }
}
