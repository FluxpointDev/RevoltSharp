using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    public class CmdTest : ModuleBase
    {
        [Command("test")]
        public async Task Test()
        {
            Console.WriteLine("MSG: test");
            await Context.Channel.SendMessageAsync("Test");
        }

        [Command("chan")]
        public async Task Channel()
        {
            await ReplyAsync(Context.Channel.Id);
        }

        [Command("role")]
        public async Task Role(string id)
        {
            foreach(var Role in Context.Server.GetRoles())
            {
                Console.WriteLine($"[ {Role.Name} ] {Role.Permissions.AssignRoles}:{Role.Permissions.BanMembers}:{Role.Permissions.ManageMessages}:{Role.Permissions.SendMessages}");
            }
            
        }

        [Command("embed")]
        public async Task Embed()
        {
            await Context.Channel.SendMessageAsync("t", null, new RevoltSharp.Embed[]
            {
                new EmbedBuilder
                {
                    Title = "Title",
                    Description = "Desc",
                    Color = new RevoltColor("#ffffff"),
                    IconUrl = "https://img.fluxpoint.dev/2531914271424512.png",
                    Url = "https://fluxpoint.dev"
                }.Build()
            });
        }
    }
}
