using RevoltSharp.Core.Servers;
using RevoltSharp.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class ServerHelper
{
    public static Task<Server> GetServerAsync(this SelfUser user, string serverId)
        => GetServerAsync(user.Client.Rest, serverId);

    public static async Task<Server> GetServerAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "GetServerAsync");

        ServerJson Server = await rest.SendRequestAsync<ServerJson>(RequestType.Get, $"/servers/{serverId}");
        return new Server(rest.Client, Server);
    }

    public static Task<ServerBan[]> GetBansAsync(this Server server)
        => GetBansAsync(server.Client.Rest, server.Id);

    public static async Task<ServerBan[]> GetBansAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "GetBansAsync");

        ServerBansJson Bans = await rest.SendRequestAsync<ServerBansJson>(RequestType.Get, $"/servers/{serverId}/bans");
        IEnumerable<ServerBan> BanList = Bans.Users.Select(x => new ServerBan(rest.Client) { Id = x.Id, Avatar = x.Avatar == null ? null : new Attachment(x.Avatar), Username = x.Username, Reason = Bans.Bans.Where(b => b.Ids.UserId == x.Id).FirstOrDefault().Reason} );
        return BanList.ToArray();
    }

    public static Task<HttpResponseMessage> LeaveAsync(this Server server)
        => LeaveServerAsync(server.Client.Rest, server.Id);

    public static Task<HttpResponseMessage> LeaveServerAsync(this SelfUser user, Server server)
       => LeaveServerAsync(user.Client.Rest, server.Id);
    public static Task<HttpResponseMessage> LeaveServerAsync(this SelfUser user, string serverId)
       => LeaveServerAsync(user.Client.Rest, serverId);
    public static async Task<HttpResponseMessage> LeaveServerAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "LeaveServerAsync");

        return await rest.SendRequestAsync(RequestType.Delete, $"/servers/{serverId}");
    }
}
