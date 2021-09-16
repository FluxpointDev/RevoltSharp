using RevoltSharp.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class InviteHelper
    {
        public static Task<HttpResponseMessage> DeleteInviteAsync(this Server server, string inviteId)
            => DeleteInviteAsync(server.Client.Rest, inviteId);

        public static async Task<HttpResponseMessage> DeleteInviteAsync(this RevoltRestClient rest, string inviteId)
        {
            return await rest.SendRequestAsync(RequestType.Delete, $"/invites/{inviteId}");
        }

        public static Task<Invite[]> GetInvitesAsync(this Server server)
            => GetInvitesAsync(server.Client.Rest, server.Id);

        public static async Task<Invite[]> GetInvitesAsync(this RevoltRestClient rest, string serverId)
        {
            InviteJson[] Json = await rest.SendRequestAsync<InviteJson[]>(RequestType.Get, $"/servers/{serverId}/invites");
            return Json.Select(x => new Invite { Code = x.code }).ToArray();
        }
    }
}
