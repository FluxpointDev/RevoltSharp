using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
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
            ChannelJson Channel = await rest.SendRequestAsync<ChannelJson>(RequestType.Get, $"/channels/{channelId}");
            return (TValue)RevoltSharp.Channel.Create(rest.Client, Channel);
        }

        public static Task<Channel> EditAsync(this Channel channel, Optional<string> name, Optional<string> desc, Optional<string> iconId, Optional<bool> nsfw)
            => EditChannelAsync(channel.Client.Rest, channel.Id, name, desc, iconId, nsfw);

        public static Task<Channel> EditChannelAsync(this Server server, string channelId, Optional<string> name, Optional<string> desc, Optional<string> iconId, Optional<bool> nsfw)
            => EditChannelAsync(server.Client.Rest, channelId, name, desc, iconId, nsfw);
        
        public static async Task<Channel> EditChannelAsync(this RevoltRestClient rest, string channelId, Optional<string> name, Optional<string> desc, Optional<string> iconId, Optional<bool> nsfw)
        {
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
            return await rest.SendRequestAsync(RequestType.Delete, $"/channels/{channelId}");
        }

        
    }
}
