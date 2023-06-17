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
    /// <summary>
    /// Create a group channel.
    /// </summary>
    /// <returns>
    /// <see cref="GroupChannel"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<GroupChannel> CreateGroupChannelAsync(this RevoltRestClient rest, string name, Option<string> description = null, bool isNsfw = false)
    {
        Conditions.NotAllowedForBots(rest, nameof(CreateGroupChannelAsync));
        Conditions.ChannelNameEmpty(name, nameof(CreateGroupChannelAsync));
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


    /// <inheritdoc cref="GetGroupChannelUsersAsync(RevoltRestClient, string)" />
    public static Task<IReadOnlyCollection<User>> GetUsersAsync(this GroupChannel channel)
      => GetGroupChannelUsersAsync(channel.Client.Rest, channel.Id);


    /// <summary>
    /// Get a list of users for the group channel.
    /// </summary>
    /// <returns>
    /// List of <see cref="User"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<User>> GetGroupChannelUsersAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.NotAllowedForBots(rest, nameof(GetGroupChannelUsersAsync));
        Conditions.ChannelIdEmpty(channelId, nameof(GetGroupChannelUsersAsync));

        UserJson[]? List = await rest.GetAsync<UserJson[]>($"channels/{channelId}");
        if (List == null)
            return System.Array.Empty<User>();

        return List.Select(x => new User(rest.Client, x)).ToImmutableArray();
    }

    /// <summary>
    /// Get a group channel.
    /// </summary>
    /// <returns>
    /// <see cref="GroupChannel"/> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<GroupChannel?> GetGroupChannelAsync(this RevoltRestClient rest, string channelId)
        => ChannelHelper.InternalGetChannelAsync<GroupChannel>(rest, channelId);

    /// <summary>
    /// Get a list of group channels the current user/bot account is in.
    /// </summary>
    /// <returns>
    /// List of <see cref="GroupChannel"/>
    /// </returns>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<GroupChannel>> GetGroupChannelsAsync(this RevoltRestClient rest)
    {
        if (rest.Client.WebSocket != null)
            return rest.Client.WebSocket.ChannelCache.Values.Where(x => x.Type == ChannelType.Group).Select(x => (GroupChannel)x).ToArray();

        ChannelJson[]? Channels = await rest.GetAsync<ChannelJson[]>("/users/dms");
        if (Channels == null)
            return System.Array.Empty<GroupChannel>();

        return Channels.Select(x => new GroupChannel(rest.Client, x)).ToImmutableArray();
    }

    /// <inheritdoc cref="LeaveGroupChannelAsync(RevoltRestClient, string)" />
    public static Task LeaveAsync(this GroupChannel channel)
      => LeaveGroupChannelAsync(channel.Client.Rest, channel.Id);

    /// <summary>
    /// Leave a group channel or delete if you are the last user.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task LeaveGroupChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdEmpty(channelId, nameof(LeaveGroupChannelAsync));

        await rest.DeleteAsync($"/channels/{channelId}");
    }

    /// <inheritdoc cref="AddUserToGroupChannelAsync(RevoltRestClient, string, string)" />
    public static Task AddUserAsync(this GroupChannel channel, User user)
        => AddUserToGroupChannelAsync(channel.Client.Rest, channel.Id, user.Id);

    /// <inheritdoc cref="AddUserToGroupChannelAsync(RevoltRestClient, string, string)" />
    public static Task AddUserAsync(this GroupChannel channel, string userId)
        => AddUserToGroupChannelAsync(channel.Client.Rest, channel.Id, userId);

    /// <summary>
    /// Add a user to the group channel.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task AddUserToGroupChannelAsync(this RevoltRestClient rest, string channelId, string userId)
    {
        Conditions.NotAllowedForBots(rest, nameof(AddUserToGroupChannelAsync));
        Conditions.ChannelIdEmpty(channelId, nameof(AddUserToGroupChannelAsync));
        Conditions.UserIdEmpty(userId, nameof(AddUserToGroupChannelAsync));

        await rest.PutAsync<HttpResponseMessage>($"channels/{channelId}/recipients/{userId}");
    }

    /// <inheritdoc cref="RemoveUserFromGroupChannelAsync(RevoltRestClient, string, string)" />
    public static Task RemoveUserAsync(this GroupChannel channel, User user)
        => RemoveUserFromGroupChannelAsync(channel.Client.Rest, channel.Id, user.Id);

    /// <inheritdoc cref="RemoveUserFromGroupChannelAsync(RevoltRestClient, string, string)" />
    public static Task RemoveUserAsync(this GroupChannel channel, string userId)
        => RemoveUserFromGroupChannelAsync(channel.Client.Rest, channel.Id, userId);

    /// <summary>
    /// Remove a user from the group channel.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task RemoveUserFromGroupChannelAsync(this RevoltRestClient rest, string channelId, string userId)
    {
        Conditions.NotAllowedForBots(rest, nameof(RemoveUserFromGroupChannelAsync));
        Conditions.ChannelIdEmpty(channelId, nameof(RemoveUserFromGroupChannelAsync));
        Conditions.UserIdEmpty(userId, nameof(RemoveUserFromGroupChannelAsync));

        await rest.DeleteAsync($"channels/{channelId}/recipients/{userId}");
    }

    /// <summary>
    /// Update a group channel.
    /// </summary>
    /// <returns>
    /// <see cref="GroupChannel"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static Task<GroupChannel> ModifyAsync(this GroupChannel channel, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null, Option<string> owner = null)
        => ChannelHelper.InternalModifyChannelAsync<GroupChannel>(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, owner);

}
