using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class ChannelHelper
{
    public static Task<TextChannel> GetTextChannelAsync(this Server server, string channelId)
        => GetChannelAsync<TextChannel>(server.Client.Rest, channelId);

    public static Task<TextChannel> GetTextChannelAsync(this RevoltRestClient rest, string channelId)
        => GetChannelAsync<TextChannel>(rest, channelId);

    public static Task<VoiceChannel> GetVoiceChannelAsync(this Server server, string channelId)
        => GetChannelAsync<VoiceChannel>(server.Client.Rest, channelId);

    public static Task<VoiceChannel> GetVoiceChannelAsync(this RevoltRestClient rest, string channelId)
        => GetChannelAsync<VoiceChannel>(rest, channelId);

    public static Task<Channel> GetChannelAsync(this Server server, string channelId)
        => GetChannelAsync<Channel>(server.Client.Rest, channelId);

    public static Task<Channel> GetChannelAsync(this RevoltRestClient rest, string channelId)
        => GetChannelAsync<Channel>(rest, channelId);

    internal static async Task<TValue> GetChannelAsync<TValue>(this RevoltRestClient rest, string channelId)
        where TValue : Channel
    {
        Conditions.ChannelIdEmpty(channelId, "GetChannelAsync");

        ChannelJson Channel = await rest.SendRequestAsync<ChannelJson>(RequestType.Get, $"/channels/{channelId}");
        return (TValue)RevoltSharp.Channel.Create(rest.Client, Channel);
    }


    public static Task<TextChannel> CreateTextChannelAsync(this Server server, string name, string description, bool nsfw = false)
        => CreateTextChannelAsync(server.Client.Rest, server.Id, name, description, nsfw);

    public static async Task<TextChannel> CreateTextChannelAsync(this RevoltRestClient rest, string serverId, string name, string description, bool nsfw = false)
    {
        Conditions.ServerIdEmpty(serverId, "CreateTextChannelAsync");
        Conditions.ChannelNameEmpty(name, "CreateTextChannelAsync");

        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            Type = "Text"
        };
        if (!string.IsNullOrEmpty(description))
            Req.description = Optional.Some(description);
        if (nsfw)
            Req.nsfw = Optional.Some(true);

        ChannelJson Json = await rest.SendRequestAsync<ChannelJson>(RequestType.Post, $"/servers/{serverId}/channels", Req);
        return new TextChannel(rest.Client, Json);
    }

    public static Task<VoiceChannel> CreateVoiceChannelAsync(this Server server, string name, string description)
        => CreateVoiceChannelAsync(server.Client.Rest, server.Id, name, description);

    public static async Task<VoiceChannel> CreateVoiceChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = "")
    {
        Conditions.ServerIdEmpty(serverId, "CreateVoiceChannelAsync");
        Conditions.ChannelNameEmpty(name, "CreateVoiceChannelAsync");

        CreateChannelRequest Req = new CreateChannelRequest
        {
            name = name,
            Type = "Voice"
        };
        if (!string.IsNullOrEmpty(description))
            Req.description = Optional.Some(description);

        ChannelJson Json = await rest.SendRequestAsync<ChannelJson>(RequestType.Post, $"/servers/{serverId}/channels", Req);
        return new VoiceChannel(rest.Client, Json);
    }


    public static Task<Channel> ModifyAsync(this TextChannel channel, Option<string> name, Option<string> desc, Option<string> iconId, Option<bool> nsfw)
        => ModifyChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, null);

    public static Task<Channel> ModifyAsync(this VoiceChannel channel, Option<string> name, Option<string> desc, Option<string> iconId, Option<bool> nsfw)
        => ModifyChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, null);

    public static Task<Channel> ModifyAsync(this GroupChannel channel, Option<string> name, Option<string> desc, Option<string> iconId, Option<bool> nsfw, Option<string> owner)
        => ModifyChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw, owner);

    public static Task<Channel> ModifyChannelAsync(this Server server, string channelId, Option<string> name, Option<string> desc, Option<string> iconId, Option<bool> nsfw)
        => ModifyChannelAsync(server.Client.Rest, channelId, name, desc, iconId, nsfw, null);
    
    public static async Task<Channel> ModifyChannelAsync(this RevoltRestClient rest, string channelId, Option<string> name, Option<string> desc, Option<string> iconId, Option<bool> nsfw, Option<string> owner)
    {
        Conditions.ChannelIdEmpty(channelId, "ModifyChannelAsync");

        ModifyChannelRequest Req = new ModifyChannelRequest();
        if (name != null)
        {
            if (string.IsNullOrEmpty(name.Value))
                throw new RevoltException("Channel modify name can not be empty.");
            Req.name = Optional.Some(name.Value);
        }
        if (desc != null)
           Req.description = Optional.Some(desc.Value);

        if (iconId != null)
        {
            if (string.IsNullOrEmpty(iconId.Value))
                Req.remove = Optional.Some(new string[] { "Icon" });
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

        return await rest.SendRequestAsync<Channel>(RequestType.Patch, $"/channels/{channelId}", Req);
    }

    public static Task<HttpResponseMessage> DeleteChannelAsync(this ServerChannel channel)
        => DeleteChannelAsync(channel.Client.Rest, channel.Id);

    public static Task<HttpResponseMessage> DeleteChannelAsync(this Server server, string channelId)
        => DeleteChannelAsync(server.Client.Rest, channelId);

    public static async Task<HttpResponseMessage> DeleteChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdEmpty(channelId, "DeleteChannelAsync");

        return await rest.SendRequestAsync(RequestType.Delete, $"/channels/{channelId}");
    }

    public static Task<HttpResponseMessage> DeleteMessagesAsync(this Channel channel, Message[] messages)
        => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messages.Select(x => x.Id).ToArray());

    public static Task<HttpResponseMessage> DeleteMessagesAsync(this Channel channel, string[] messageIds)
        => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messageIds);

    public static async Task<HttpResponseMessage> DeleteMessagesAsync(this RevoltRestClient rest, string channelId, string[] messageIds)
    {
        Conditions.ChannelIdEmpty(channelId, "DeleteMessagesAsync");
        Conditions.MessageIdEmpty(messageIds, "DeleteMessagesAsync");


        return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/bulk", new BulkDeleteMessagesRequest
        {
            ids = messageIds
        });
    }


}
