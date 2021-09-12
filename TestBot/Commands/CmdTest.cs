using RevoltSharp.Commands;
using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    public class CmdTest : ModuleBase
    {
        [Command("hi")]
        public async Task Hi()
        {
            await ReplyAsync("Hi " + Context.User.Username);
        }
    }
}
