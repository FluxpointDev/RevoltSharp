using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for group channels.
/// </summary>
public static class GroupChannelHelper
{
    public static async Task<GroupChannel> CreateGroupChannelAsync(this RevoltRestClient rest, string name, Option<string> description = null, bool isNsfw = false)
    {
        Conditions.NotAllowedForBots(rest, "CreateGroupChannelAsync");
        Conditions.ChannelNameEmpty(name, "CreateGroupChannelAsync");
        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            users = Optional.Some(System.Array.Empty<string>()),
            nsfw = Optional.Some(isNsfw)
        };
        if (description != null)
            Req.description = Optional.Some(description.Value);

        ChannelJson Json = await rest.PostAsync<ChannelJson>("channels/create", Req);
        return (GroupChannel)Channel.Create(rest.Client, Json);
    }

    public static Task<IReadOnlyCollection<User>> GetMembersAsync(this GroupChannel channel)
      => GetGroupChannelMembersAsync(channel.Client.Rest, channel.Id);

    public static async Task<IReadOnlyCollection<User>> GetGroupChannelMembersAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.NotAllowedForBots(rest, "GetGroupChannelMembersAsync");
        Conditions.ChannelIdEmpty(channelId, "GetGroupChannelMembersAsync");

        UserJson[]? List = await rest.GetAsync<UserJson[]>($"channels/{channelId}");
        if (List == null)
            return System.Array.Empty<User>();

        return List.Select(x => new User(rest.Client, x)).ToImmutableArray();
    }

    public static Task<GroupChannel?> GetGroupChannelAsync(this SelfUser user, string channelId)
        => ChannelHelper.GetChannelAsync<GroupChannel>(user.Client.Rest, channelId);

    public static Task<GroupChannel?> GetGroupChannelAsync(this RevoltRestClient rest, string channelId)
        => ChannelHelper.GetChannelAsync<GroupChannel>(rest, channelId);

    public static Task<IReadOnlyCollection<GroupChannel>> GetGroupChannelsAsync(this SelfUser user)
        => GetGroupChannelsAsync(user.Client.Rest);

    public static async Task<IReadOnlyCollection<GroupChannel>> GetGroupChannelsAsync(this RevoltRestClient rest)
    {
        if (rest.Client.WebSocket != null)
            return rest.Client.WebSocket.ChannelCache.Values.Where(x => x.Type == ChannelType.Group).Select(x => (GroupChannel)x).ToArray();

        ChannelJson[]? Channels = await rest.GetAsync<ChannelJson[]>("/users/dms");
        if (Channels == null)
            return System.Array.Empty<GroupChannel>();

        return Channels.Select(x => new GroupChannel(rest.Client, x)).ToImmutableArray();
    }


    public static Task LeaveAsync(this GroupChannel channel)
      => LeaveGroupChannelAsync(channel.Client.Rest, channel.Id);

    public static Task LeaveGroupChannelAsync(this SelfUser user, GroupChannel channel)
      => LeaveGroupChannelAsync(user.Client.Rest, channel.Id);

    public static Task LeaveGroupChannelAsync(this SelfUser user, string channelId)
      => LeaveGroupChannelAsync(user.Client.Rest, channelId);

    public static async Task LeaveGroupChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdEmpty(channelId, "LeaveGroupChannelAsync");

        await rest.DeleteAsync($"/channels/{channelId}");
    }

    public static Task AddUserAsync(this GroupChannel channel, User user)
        => AddUserToGroupChannelAsync(channel.Client.Rest, channel.Id, user.Id);

    public static Task AddUserAsync(this GroupChannel channel, string userId)
        => AddUserToGroupChannelAsync(channel.Client.Rest, channel.Id, userId);

    public static async Task AddUserToGroupChannelAsync(this RevoltRestClient rest, string channelId, string userId)
    {
        Conditions.NotAllowedForBots(rest, "AddUserToGroupChannelAsync");
        Conditions.ChannelIdEmpty(channelId, "AddUserToGroupChannel");
        Conditions.UserIdEmpty(userId, "AddUserToGroupChannel");

        await rest.PutAsync<HttpResponseMessage>($"channels/{channelId}/recipients/{userId}");
    }

    public static Task RemoveUserAsync(this GroupChannel channel, User user)
        => RemoveUserFromGroupChannelAsync(channel.Client.Rest, channel.Id, user.Id);

    public static Task RemoveUserAsync(this GroupChannel channel, string userId)
        => RemoveUserFromGroupChannelAsync(channel.Client.Rest, channel.Id, userId);

    public static async Task RemoveUserFromGroupChannelAsync(this RevoltRestClient rest, string channelId, string userId)
    {
        Conditions.NotAllowedForBots(rest, "RemoveUserFromGroupChannelAsync");
        Conditions.ChannelIdEmpty(channelId, "RemoveUserFromGroupChannelAsync");
        Conditions.UserIdEmpty(userId, "RemoveUserFromGroupChannelAsync");

        await rest.DeleteAsync($"channels/{channelId}/recipients/{userId}");
    }
}
