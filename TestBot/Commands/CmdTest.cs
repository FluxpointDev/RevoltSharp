using RevoltSharp.Commands;
using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    public class CmdTest : ModuleBase
    {
        [Command("test")]
        public async Task Test()
        {
            await ReplyAsync("Test");
        }
    }
}
