using RevoltSharp.Rest;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class GroupChannelHelper
    {
        public static Task<User[]> GetMembersAsync(this GroupChannel channel)
          => GetGroupChannelMembersAsync(channel.Client.Rest, channel.Id);

        public static async Task<User[]> GetGroupChannelMembersAsync(this RevoltRestClient rest, string channelId)
        {
            Conditions.ChannelIdEmpty(channelId);

            UserJson[] List = await rest.SendRequestAsync<UserJson[]>(RequestType.Get, $"channels/{channelId}");
            
            return List.Select(x => new User(rest.Client, x)).ToArray();
        }

        public static Task<GroupChannel> GetGroupChannelAsync(this SelfUser user, string channelId)
            => ChannelHelper.GetChannelAsync<GroupChannel>(user.Client.Rest, channelId);

        public static Task<GroupChannel> GetGroupChannelAsync(this RevoltRestClient rest, string channelId)
            => ChannelHelper.GetChannelAsync<GroupChannel>(rest, channelId);

        public static Task<GroupChannel[]> GetGroupChannelsAsync(this SelfUser user)
            => GetGroupChannelsAsync(user.Client.Rest);

        public static async Task<GroupChannel[]> GetGroupChannelsAsync(this RevoltRestClient rest)
        {
            ChannelJson[] Channels = await rest.SendRequestAsync<ChannelJson[]>(RequestType.Get, "/users/dms");
            return Channels.Select(x => new GroupChannel(rest.Client, x)).ToArray();
        }


        public static Task<HttpResponseMessage> LeaveAsync(this GroupChannel channel)
          => LeaveGroupChannelAsync(channel.Client.Rest, channel.Id);

        public static Task<HttpResponseMessage> LeaveGroupChannelAsync(this SelfUser user, GroupChannel channel)
          => LeaveGroupChannelAsync(user.Client.Rest, channel.Id);

        public static Task<HttpResponseMessage> LeaveGroupChannelAsync(this SelfUser user, string channelId)
          => LeaveGroupChannelAsync(user.Client.Rest, channelId);

        public static async Task<HttpResponseMessage> LeaveGroupChannelAsync(this RevoltRestClient rest, string channelId)
        {
            Conditions.ChannelIdEmpty(channelId);

            return await rest.SendRequestAsync(RequestType.Delete, $"/channels/{channelId}");
        }

    }
}
