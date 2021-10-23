using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    public class CmdTest : ModuleBase
    {
        [Command("mutualtest")]
        public async Task MutualTest()
        {
            ServerMember SM = await Context.Server.GetMemberAsync("01FEYH91F7KQXFM5737YVR1M1N");
            User User = Context.Client.GetUser("01FEYH91F7KQXFM5737YVR1M1N");
            GroupChannel Group = (GroupChannel)Context.Client.GetChannel("01FJN2GRVQYF2VZ6KAD8253682");
            if (Group.Users.ContainsKey("01FEYH91F7KQXFM5737YVR1M1N"))
                Console.WriteLine($"--- Group ---\nUsers: {Group.Recipents.Count()}:{Group.Users.Keys.Count()}");
            else
                Console.WriteLine($"--- No Group ---\nUsers: {Group.Recipents.Count()}:{Group.Users.Keys.Count()}");

            if (SM == null)
                Console.WriteLine($"--- No Server ---\nMembers: {Context.Server.Members.Keys.Count()}");
            else
            {
                Console.WriteLine($"--- Server ---\nRoles: {SM.RolesIds.Count()}:{SM.Roles.Keys.Count()}\nMembers: {Context.Server.Members.Keys.Count()}\n");
                foreach(Role r in SM.Roles.Values)
                {
                    Console.WriteLine("- " + r.Name);
                }
            }
            foreach(var m in Context.Server.Members.Values)
            {
                Console.Write("* " + m.User.Username);
            }
            if (User == null)
                Console.WriteLine($"--- No User ---\nClient Users: {Context.Client.Users.Count()}");
            else
            {
                Console.Write($"--- User ---\nClient Users: {Context.Client.Users.Count()}\nMutuals: G {User.MutualGroups.Keys.Count()}:S {User.MutualServers.Keys.Count()}\n");
                foreach(GroupChannel GC in User.MutualGroups.Values)
                {
                    Console.WriteLine("- " + GC.Name);
                }
                foreach(Server S in User.MutualServers.Values)
                {
                    Console.WriteLine("> " + S.Name);
                }
            }
        }

        [Command("permtest")]
        public async Task PermTest()
        {
            Console.WriteLine("PERM TEST");
            try
            {
                var Role = Context.Server.Members[Context.User.Id].Roles.Values.OrderByDescending(x => x.Rank).FirstOrDefault();
                await ReplyAsync(Role.Name + " " + Role.hasPerm(ServerPermission.BanMembers));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Command("test")]
        public async Task Test()
        {
            await ReplyAsync("Test");
        }
    }
}
