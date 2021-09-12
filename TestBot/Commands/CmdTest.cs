using RevoltSharp.Commands;
using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    public class CmdTest : ModuleBase
    {
        [Command("server")]
        public async Task Server()
        {
            if (Context.Server == null)
            {
                await ReplyAsync("You need to use a revolt server for this command.");
                return;
            }
            int MemberCount = 0;
            try
            {
                var Members = await Context.Server.GetMembersAsync();
                MemberCount = Members.Length;
            }
            catch { }
            await ReplyAsync($"**{Context.Server.Name}**\n{Context.Server.Description}\n\nRoles: `{Context.Server.Roles.Keys.Count}` | Channels: `{Context.Server.ChannelIds.Count}` | Members: `{MemberCount}`");
        }
    }
}
