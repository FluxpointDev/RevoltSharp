using RevoltSharp.Rest;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class BotHelper
    {
        public static Task<HttpResponseMessage> ModifySelfAsync(this SelfUser user, Optional<string> avatar = null, Optional<string> statusText = null, Optional<UserStatusType> statusType = null, Optional<string> profileBio = null, Optional<string> profileBackground = null)
           => ModifySelfAsync(user.Client.Rest, avatar, statusText, statusType, profileBio, profileBackground);

        public static async Task<HttpResponseMessage> ModifySelfAsync(this RevoltRestClient rest, Optional<string> avatar = null, Optional<string> statusText = null, Optional<UserStatusType> statusType = null, Optional<string> profileBio = null, Optional<string> profileBackground = null)
        {
            return await rest.SendRequestAsync(RequestType.Patch, $"users/@me", ModifySelfRequest.Create(avatar, statusText, statusType, profileBio, profileBackground));
        }

        public static Task<GroupChannel> GetGroupChannel(this SelfUser user, string channelId)
            => ChannelHelper.GetChannelAsync<GroupChannel>(user.Client.Rest, channelId);

        public static Task<GroupChannel> GetGroupChannel(this RevoltRestClient rest, string channelId)
            => ChannelHelper.GetChannelAsync<GroupChannel>(rest, channelId);

        public static Task<GroupChannel[]> GetGroupChannelsAsync(this SelfUser user)
            => GetGroupChannelsAsync(user.Client.Rest);

        public static async Task<GroupChannel[]> GetGroupChannelsAsync(this RevoltRestClient rest)
        {
            ChannelJson[] Channels = await rest.SendRequestAsync<ChannelJson[]>(RequestType.Get, "/users/dms");
            return Channels.Select(x => new GroupChannel(rest.Client, x)).ToArray();
        }

        public static Task<HttpResponseMessage> LeaveServerAsync(this Server server)
            => LeaveServerAsync(server.Client.Rest, server.Id);

        public static Task<HttpResponseMessage> LeaveServerAsync(this SelfUser user, string serverId)
           => LeaveServerAsync(user.Client.Rest, serverId);


        public static async Task<HttpResponseMessage> LeaveServerAsync(this RevoltRestClient rest, string serverId)
        {
            return await rest.SendRequestAsync(RequestType.Delete, $"/servers/{serverId}");
        }
    }
}
