using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class ChannelHelper
{
    public static Task<TextChannel?> GetTextChannelAsync(this Server server, string channelId)
        => GetChannelAsync<TextChannel>(server.Client.Rest, channelId);

    public static Task<TextChannel?> GetTextChannelAsync(this RevoltRestClient rest, string channelId)
        => GetChannelAsync<TextChannel>(rest, channelId);

    public static Task<VoiceChannel?> GetVoiceChannelAsync(this Server server, string channelId)
        => GetChannelAsync<VoiceChannel>(server.Client.Rest, channelId);

    public static Task<VoiceChannel?> GetVoiceChannelAsync(this RevoltRestClient rest, string channelId)
        => GetChannelAsync<VoiceChannel>(rest, channelId);

    public static Task<Channel?> GetChannelAsync(this Server server, string channelId)
        => GetChannelAsync<Channel>(server.Client.Rest, channelId);

    public static Task<Channel?> GetChannelAsync(this RevoltRestClient rest, string channelId)
        => GetChannelAsync<Channel>(rest, channelId);

    internal static async Task<TValue?> GetChannelAsync<TValue>(this RevoltRestClient rest, string channelId)
        where TValue : Channel
    {
        Conditions.ChannelIdEmpty(channelId, "GetChannelAsync");

        if (rest.Client.WebSocket != null)
        {
            if (rest.Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel chan))
                return (TValue)chan;
            return null;
        }
            

        ChannelJson? Channel = await rest.GetAsync<ChannelJson>($"/channels/{channelId}");
        if (Channel == null)
            return null;

        return (TValue)RevoltSharp.Channel.Create(rest.Client, Channel);
    }


    public static Task<TextChannel> CreateTextChannelAsync(this Server server, string name, string description = null, bool nsfw = false)
        => CreateTextChannelAsync(server.Client.Rest, server.Id, name, description, nsfw);

    public static async Task<TextChannel> CreateTextChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = null, bool nsfw = false)
    {
        Conditions.ServerIdEmpty(serverId, "CreateTextChannelAsync");
        Conditions.ChannelNameEmpty(name, "CreateTextChannelAsync");

        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            type = Optional.Some("Text")
        };
        if (!string.IsNullOrEmpty(description))
            Req.description = Optional.Some(description);
        if (nsfw)
            Req.nsfw = Optional.Some(true);

        ChannelJson Json = await rest.PostAsync<ChannelJson>($"/servers/{serverId}/channels", Req);
        return new TextChannel(rest.Client, Json);
    }

    public static Task<VoiceChannel> CreateVoiceChannelAsync(this Server server, string name, string description = null)
        => CreateVoiceChannelAsync(server.Client.Rest, server.Id, name, description);

    public static async Task<VoiceChannel> CreateVoiceChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = null)
    {
        Conditions.ServerIdEmpty(serverId, "CreateVoiceChannelAsync");
        Conditions.ChannelNameEmpty(name, "CreateVoiceChannelAsync");

        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            type = Optional.Some("Voice")
        };
        if (!string.IsNullOrEmpty(description))
            Req.description = Optional.Some(description);

        ChannelJson Json = await rest.PostAsync<ChannelJson>($"/servers/{serverId}/channels", Req);
        return new VoiceChannel(rest.Client, Json);
    }

    public static Task<Channel> ModifyAsync(this TextChannel channel, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null)
        => ModifyChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, null);

    public static Task<Channel> ModifyAsync(this VoiceChannel channel, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null)
        => ModifyChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, null);

    public static Task<Channel> ModifyAsync(this GroupChannel channel, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null, Option<string> owner = null)
        => ModifyChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, owner);

    public static Task<Channel> ModifyChannelAsync(this Server server, string channelId, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null)
        => ModifyChannelAsync(server.Client.Rest, channelId, name, desc, iconId, nsfw, null);
    
    public static async Task<Channel> ModifyChannelAsync(this RevoltRestClient rest, string channelId, Option<string> name = null, Option<string> desc = null, Option<string> iconId = null, Option<bool> nsfw = null, Option<string> owner = null)
    {
        Conditions.ChannelIdEmpty(channelId, "ModifyChannelAsync");

        ModifyChannelRequest Req = new ModifyChannelRequest();
        if (name != null)
        {
            Conditions.ChannelNameEmpty(name.Value, "ModifyChannelAsync");

            Req.name = Optional.Some(name.Value);
        }
        if (desc != null)
        {
            if (string.IsNullOrEmpty(desc.Value))
                Req.RemoveValue("Description");
            else
                Req.description = Optional.Some(desc.Value);
        }

        if (iconId != null)
        {
            if (string.IsNullOrEmpty(iconId.Value))
                Req.RemoveValue("Icon");
            else
                Req.icon = Optional.Some(iconId.Value);
        }

        if (nsfw != null)
            Req.nsfw = Optional.Some(nsfw.Value);

        if (owner != null)
        {
            Conditions.UserIdEmpty(owner.Value, "ModifyChannelAsync");
            Req.owner = Optional.Some(owner.Value);
        }
        ChannelJson Json = await rest.PatchAsync<ChannelJson>($"/channels/{channelId}", Req);
        return Channel.Create(rest.Client, Json);
    }

    public static Task DeleteChannelAsync(this ServerChannel channel)
        => DeleteChannelAsync(channel.Client.Rest, channel.Id);

    public static Task DeleteChannelAsync(this Server server, string channelId)
        => DeleteChannelAsync(server.Client.Rest, channelId);

    public static async Task DeleteChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdEmpty(channelId, "DeleteChannelAsync");

        await rest.DeleteAsync($"/channels/{channelId}");
    }

    public static Task DeleteMessagesAsync(this Channel channel, Message[] messages)
        => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messages.Select(x => x.Id).ToArray());

    public static Task DeleteMessagesAsync(this Channel channel, string[] messageIds)
        => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messageIds);

    public static async Task DeleteMessagesAsync(this RevoltRestClient rest, string channelId, string[] messageIds)
    {
        Conditions.ChannelIdEmpty(channelId, "DeleteMessagesAsync");
        Conditions.MessageIdEmpty(messageIds, "DeleteMessagesAsync");


        await rest.DeleteAsync($"channels/{channelId}/messages/bulk", new BulkDeleteMessagesRequest
        {
            ids = messageIds
        });
    }

    
}
