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
            var File = await ReplyFileAsync("C:/Users/Brandan/Downloads/0154.jpeg");
            await ReplyAsync("Test");
        }
    }
}
