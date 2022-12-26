using RevoltSharp.Rest;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class InviteHelper
    {
        public static Task<HttpResponseMessage> DeleteInviteAsync(this Server server, string inviteId)
            => DeleteInviteAsync(server.Client.Rest, inviteId);

        public static async Task<HttpResponseMessage> DeleteInviteAsync(this RevoltRestClient rest, string inviteId)
        {
            if (string.IsNullOrEmpty(inviteId))
                throw new RevoltArgumentException("Invite id can't be empty for this request.");

            return await rest.SendRequestAsync(RequestType.Delete, $"/invites/{inviteId}");
        }

        public static Task<Invite[]> GetInvitesAsync(this Server server)
            => GetInvitesAsync(server.Client.Rest, server.Id);

        public static async Task<Invite[]> GetInvitesAsync(this RevoltRestClient rest, string serverId)
        {
            Conditions.ServerIdEmpty(serverId);

            InviteJson[] Json = await rest.SendRequestAsync<InviteJson[]>(RequestType.Get, $"/servers/{serverId}/invites");
            return Json.Select(x => new Invite(x)).ToArray();
        }


        /// <summary>
        /// Only user accounts can create invites
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Task<CreatedInvite> CreateInviteAsync(this TextChannel channel)
            => CreateInviteAsync(channel.Client.Rest, channel.Id);

        /// <summary>
        /// Only user accounts can create invites
        /// </summary>
        /// <param name="rest"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        /// <exception cref="RevoltArgumentException"></exception>
        public static async Task<CreatedInvite> CreateInviteAsync(this RevoltRestClient rest, string channelId)
        {
            Conditions.ChannelIdEmpty(channelId);

            CreateInviteJson Json = await rest.SendRequestAsync<CreateInviteJson>(RequestType.Post, $"/channels/{channelId}/invites");
            return new CreatedInvite(Json);
        }
    }
}
