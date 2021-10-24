using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Collections.Generic;
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

        [Command("perms")]
        public async Task PermTest(string id = "")
        {
            if (id == "")
                id = Context.User.Id;
            ServerMember Member = await Context.Server.GetMemberAsync(id);
            if (Member == null)
            {
                await ReplyAsync("Could not find server member.");
                return;
            }
            List<string> Perms = new List<string>();
            if (Member.Permissions.BanMembers)
                Perms.Add("- Ban Members");
            if (Member.Permissions.ChangeAvatar)
                Perms.Add("- Change Avatar");
            if (Member.Permissions.ChangeNickname)
                Perms.Add("- Change Nickname");
            if (Member.Permissions.CreateInvite)
                Perms.Add("- Create Invite");
            if (Member.Permissions.EmbedLinks)
                Perms.Add("- Embed Links");
            if (Member.Permissions.KickMembers)
                Perms.Add("- Kick Members");
            if (Member.Permissions.ManageAvatars)
                Perms.Add("- Manage Avatars");
            if (Member.Permissions.ManageChannels)
                Perms.Add("- Manage Channels");
            if (Member.Permissions.ManageMessages)
                Perms.Add("- Manage Messages");
            if (Member.Permissions.ManageNicknames)
                Perms.Add("- Manage Nicknames");
            if (Member.Permissions.ManageRoles)
                Perms.Add("- Manage Roles");
            if (Member.Permissions.ManageServer)
                Perms.Add("- Manage Server");
            if (Member.Permissions.SendMessages)
                Perms.Add("- Send Messages");
            if (Member.Permissions.UploadFiles)
                Perms.Add("- Upload Files");
            if (Member.Permissions.ViewChanels)
                Perms.Add("- View Channels");
            if (Member.Permissions.VoiceCall)
                Perms.Add("- Voice Call");
            await ReplyAsync($"{Member.Permissions.RawServer}|{Member.Permissions.RawChannel}");
        }

        [Command("test")]
        public async Task Test()
        {
            await ReplyAsync("Test");
        }
    }
}
