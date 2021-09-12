using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp.Rest
{
    public static class ServerHelper
    {
        public static async Task<Server> GetServerAsync(this RevoltRestClient rest, string serverId)
        {
            ServerJson Server = await rest.SendRequestAsync<ServerJson>(RequestType.Get, $"/servers/{serverId}");
            return Server.ToEntity(rest.Client);
        }

        public static async Task<Channel> GetChannelAsync(this RevoltRestClient rest, string channelId)
        {
            ChannelJson Channel = await rest.SendRequestAsync<ChannelJson>(RequestType.Get, $"/channels/{channelId}");
            return Channel.ToEntity(rest.Client);
        }

        public static Task<ServerMember[]> GetMembersAsync(this Server server)
           => GetMembersAsync(server.Client.Rest, server.Id);

        public static async Task<ServerMember[]> GetMembersAsync(this RevoltRestClient rest, string serverId)
        {
            MembersListJson List = await rest.SendRequestAsync<MembersListJson>(RequestType.Get, $"servers/{serverId}/members");
            HashSet<ServerMember> Members = new HashSet<ServerMember>();
            for (int i = 0; i < List.members.Length; i++)
            {
                Members.Add(ServerMember.Create(List.members[i], List.users[i]));
            }
            return Members.ToArray();
        }
    }
}
