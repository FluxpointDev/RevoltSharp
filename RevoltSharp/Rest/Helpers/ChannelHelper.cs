using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
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
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            ChannelJson Channel = await rest.SendRequestAsync<ChannelJson>(RequestType.Get, $"/channels/{channelId}");
            return (TValue)RevoltSharp.Channel.Create(rest.Client, Channel);
        }


        public static Task<TextChannel> CreateTextChannelAsync(this Server server, string name, string description, bool nsfw = false)
            => CreateTextChannelAsync(server.Client.Rest, server.Id, name, description, nsfw);

        public static async Task<TextChannel> CreateTextChannelAsync(this RevoltRestClient rest, string serverId, string name, string description, bool nsfw = false)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(name))
                throw new RevoltArgumentException("Channel name can't be empty for this request.");
            CreateChannelRequest Req = new CreateChannelRequest
            {
                name = name,
                Type = "Text"
            };
            if (!string.IsNullOrEmpty(description))
                Req.description = Optional.Option.Some(description);
            if (nsfw)
                Req.nsfw = Optional.Option.Some(true);

            ChannelJson Json = await rest.SendRequestAsync<ChannelJson>(RequestType.Post, $"/servers/{serverId}/channels", Req);
            return new TextChannel(rest.Client, Json);
        }

        public static Task<VoiceChannel> CreateVoiceChannelAsync(this Server server, string name, string description)
            => CreateVoiceChannelAsync(server.Client.Rest, server.Id, name, description);

        public static async Task<VoiceChannel> CreateVoiceChannelAsync(this RevoltRestClient rest, string serverId, string name, string description = "")
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(name))
                throw new RevoltArgumentException("Channel name can't be empty for this request.");
            CreateChannelRequest Req = new CreateChannelRequest
            {
                name = name,
                Type = "Voice"
            };
            if (!string.IsNullOrEmpty(description))
                Req.description = Optional.Option.Some(description);

            ChannelJson Json = await rest.SendRequestAsync<ChannelJson>(RequestType.Post, $"/servers/{serverId}/channels", Req);
            return new VoiceChannel(rest.Client, Json);
        }

        public static Task<Channel> EditAsync(this Channel channel, Optional<string> name, Optional<string> desc, Optional<string> iconId, Optional<bool> nsfw)
            => EditChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw);

        public static Task<Channel> EditChannelAsync(this Server server, string channelId, Optional<string> name, Optional<string> desc, Optional<string> iconId, Optional<bool> nsfw)
            => EditChannelAsync(server.Client.Rest, channelId, name, desc, iconId, nsfw);
        
        public static async Task<Channel> EditChannelAsync(this RevoltRestClient rest, string channelId, Optional<string> name, Optional<string> desc, Optional<string> iconId, Optional<bool> nsfw)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            ModifyChannelRequest Req = new ModifyChannelRequest();
            if (name != null)
            {
                if (string.IsNullOrEmpty(name.Value))
                    throw new RevoltException("Channel modify name can not be empty.");
                Req.name = Optional.Option.Some(name.Value);
            }
            if (desc != null)
               Req.description = Optional.Option.Some(desc.Value);

            if (iconId != null)
            {
                if (string.IsNullOrEmpty(iconId.Value))
                    Req.remove = Optional.Option.Some(new string[] { "Icon" });
                else
                    Req.icon = Optional.Option.Some(iconId.Value);
            }

            if (nsfw != null)
                Req.nsfw = Optional.Option.Some(nsfw.Value);
            return await rest.SendRequestAsync<Channel>(RequestType.Patch, $"/channels/{channelId}", Req);
        }

        public static Task<HttpResponseMessage> DeleteChannelAsync(this ServerChannel channel)
            => DeleteChannelAsync(channel.Client.Rest, channel.Id);

        public static Task<HttpResponseMessage> DeleteChannelAsync(this Server server, string channelId)
            => DeleteChannelAsync(server.Client.Rest, channelId);

        public static async Task<HttpResponseMessage> DeleteChannelAsync(this RevoltRestClient rest, string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            return await rest.SendRequestAsync(RequestType.Delete, $"/channels/{channelId}");
        }

        public static Task<HttpResponseMessage> DeleteMessagesAsync(this Channel channel, Message[] messages)
            => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messages.Select(x => x.Id).ToArray());

        public static Task<HttpResponseMessage> DeleteMessagesAsync(this Channel channel, string[] messageIds)
            => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messageIds);

        public static async Task<HttpResponseMessage> DeleteMessagesAsync(this RevoltRestClient rest, string channelId, string[] messageIds)
        {
            if (string.IsNullOrEmpty(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");

            if (messageIds == null || messageIds.Length == 0)
                throw new RevoltArgumentException("Message id can't be empty for this request.");


            return await rest.SendRequestAsync(RequestType.Delete, $"channels/{channelId}/messages/bulk", new BulkDeleteMessagesRequest
            {
                ids = messageIds
            });
        }


    }
}
