using Optionals;
using RevoltSharp.Requests;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;
public static class AdminHelper
{
    public static async Task<IReadOnlyCollection<UserMessage>> GetMessagesAsync(this AdminClient admin, string channelId = null, string userId = null, string query = null, int messageCount = 100, bool includeAuthor = false, string nearbyMessageId = null, string beforeMessageId = null, string afterMessageId = null)
    {
        Conditions.ChannelIdLength(channelId, nameof(GetMessagesAsync));
        Conditions.MessageSearchCount(messageCount, nameof(GetMessagesAsync));

        AdminGetMessagesRequest Req = new AdminGetMessagesRequest
        {
            limit = messageCount
        };
        if (!string.IsNullOrEmpty(channelId))
            Req.channel = Optional.Some(channelId);

        if (!string.IsNullOrEmpty(userId))
            Req.author = Optional.Some(userId);

        if (!string.IsNullOrEmpty(query))
            Req.query = Optional.Some(query);

        if (!string.IsNullOrEmpty(query))
            Req.query = Optional.Some(query);

        if (!string.IsNullOrEmpty(nearbyMessageId))
            Req.nearby = Optional.Some(nearbyMessageId);

        if (!string.IsNullOrEmpty(beforeMessageId))
            Req.before = Optional.Some(beforeMessageId);

        if (!string.IsNullOrEmpty(afterMessageId))
            Req.after = Optional.Some(afterMessageId);

        AdminMessagesDataJson Data = null;
        try
        {
            Data = await admin.Client.Rest.PostAsync<AdminMessagesDataJson>($"admin/messages", Req);
        }
        catch
        {
            return Array.Empty<UserMessage>();
        }
        Dictionary<string, User> Authors = includeAuthor ? Data.Users.ToDictionary(x => x.Id, x => new User(admin.Client, x)) : new Dictionary<string, User>();

        return Data.Messages.Select(x => new UserMessage(admin.Client, x) { Author = Authors.GetValueOrDefault(x.AuthorId) }).ToImmutableArray();
    }
}
