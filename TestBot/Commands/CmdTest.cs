using Optional;
using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    public class CmdTest : ModuleBase
    {
        [Command("test")]
        public async Task Test()
        {
            Console.WriteLine("-- Roles ---");
            var Role = await Context.Server.CreateRoleAsync("Test");
            Console.WriteLine("- Create");
            await Role.DeleteAsync();
            Console.WriteLine("- Delete");
            Console.WriteLine("-- Channels ---");
            var Channel = await Context.Server.CreateTextChannelAsync("T", "AHH");
            Console.WriteLine("- Create");
            await Channel.DeleteChannelAsync();
            Console.WriteLine("- Delete");
            Console.WriteLine("-- Msg ---");
            var Msg = await Context.Channel.SendMessageAsync("Ahh", null, new Embed[] { new RevoltSharp.EmbedBuilder
            {
                Title = "T",
                Description = "L",
                Color = new RevoltColor(0, 0, 1),
                Url = "https://fluxpoint.dev"
            }.Build() }, new MessageMasquerade("Ah"));
            Console.WriteLine("- Create");
            await Context.Client.Rest.AddMessageReactionAsync(Msg.ChannelId, Msg.Id, Context.Client.GetEmoji("01GC7SSTBN4D4AGH6KDHMGFHFV").Id);
            Console.WriteLine("- Add reaction");
            await Msg.EditMessageAsync(new Optional<string>("T"));
            Console.WriteLine("- Edit");
            await Msg.DeleteMessageAsync();
            Console.WriteLine("- Delete");
        }

        [Command("perms")]
        public async Task Perms(string role = "")
        {
            ServerPermissions Perms = !string.IsNullOrEmpty(role) ? Context.Server.GetRole(role).Permissions : Context.Server.GetCachedMember(Context.Client.CurrentUser.Id).Permissions;
            if (string.IsNullOrEmpty(role))
                Console.WriteLine("User Perms");
            else
                Console.WriteLine("Role Perms");

            Console.WriteLine($"--- Messages ---\n" +
                $"Upload Files: {Perms.UploadFiles} | Send Messages: {Perms.SendMessages} | Add Reaction: {Perms.AddReactions} | Embed Links: {Perms.EmbedLinks} | Masquerade: {Perms.Masquerade} | Manage Messages: {Perms.ManageMessages}");
            Console.WriteLine("--- Channel ---\n" +
                $"View Channels: {Perms.ViewChannels} | Read Messages: {Perms.ReadMessageHistory} | Manage Webhooks: {Perms.ManageWebhooks} | Manage Channels: {Perms.ManageChannels}");
            Console.WriteLine("--- Server ---\n" +
                $"Change Avatar: {Perms.ChangeAvatar} | Change Nickname: {Perms.ChangeNickname} | Create Invites: {Perms.CreateInvites}");
            Console.WriteLine("--- Mod ---\n" +
                $"Assign Roles: {Perms.AssignRoles} | Timeout Members: {Perms.TimeoutMembers} | Manage Avatars: {Perms.ManageAvatars} | Ban Members: {Perms.BanMembers} | Kick Members: {Perms.KickMembers}");
            Console.WriteLine("--- Admin ---\n" +
                $"Manage Custom: {Perms.ManageCustomisation} | Manage Nicknames: {Perms.ManageNicknames} | Manage Server: {Perms.ManageServer} | Manage Roles: {Perms.ManageRoles} | Manage Permissions: {Perms.ManagePermissions}");
        }


        [Command("upload")]
        public async Task Upload()
        {
            var Attach = await Context.Client.Rest.UploadFileAsync("C:/Users/Brandan/Downloads/blob-b61714e8-fc2e-49db-8ca1-d6366784ef64.png", RevoltSharp.Rest.RevoltRestClient.UploadFileType.Attachment);
            
        }

        [Command("chan")]
        public async Task Channel()
        {
            await ReplyAsync(Context.Channel.Id);
        }

        [Command("roles")]
        public async Task Role()
        {
            await Context.Channel.SendMessageAsync("", null, new Embed[]
            {
                new EmbedBuilder
                {
                    Description = String.Join("\n", Context.Server.Roles.Select(x => $"{x.Name} - {x.Color} ({x.Id})"))
                }.Build()
            }); 
            foreach(var Role in Context.Server.Roles)
            {
                Console.WriteLine($"[ {Role.Name} ] {Role.Permissions.AssignRoles}:{Role.Permissions.BanMembers}:{Role.Permissions.ManageMessages}:{Role.Permissions.SendMessages}");
            }
        }

        [Command("addrole")]
        public async Task AddRole()
        {
            var Member = await Context.Server.GetMemberAsync("01G3BHHPN05RTFDGB99YRYC8QN");
            var Req = await Context.Client.Rest.AddRoleAsync(Member, Context.Server.GetRole("01FESEE54DDDSEM217NX9GH4KG"));
            await ReplyAsync(Req.IsSuccessStatusCode.ToString());
        
        }

        [Command("memberroles")]
        public async Task Roles()
        {
            var Member = await Context.Server.GetMemberAsync("01G3BHHPN05RTFDGB99YRYC8QN");
            await ReplyAsync(String.Join(", ", Member.Roles.Select(x => x.Name)));
        }

        [Command("removerole")]
        public async Task RemoveRole()
        {
            var Member = await Context.Server.GetMemberAsync("01G3BHHPN05RTFDGB99YRYC8QN");
            await Context.Client.Rest.RemoveRoleAsync(Member, Context.Server.GetRole("01FESEE54DDDSEM217NX9GH4KG"));
        }

        [Command("testreaction")]
        public async Task TestReaction()
        {
            await Context.Channel.SendMessageAsync("Test", interactions: new MessageInteractions
            {
                Reactions = new Emoji[]
                {
                    Context.Server.GetEmoji("01GBP83S4WT512ET704ACPVPQW")
                },
                RestrictReactions = true
            });
        }

        [Command("embed")]
        public async Task Embed()
        {
            await Context.Channel.SendMessageAsync("Hello", null, new RevoltSharp.Embed[]
            {
                new EmbedBuilder
                {
                    Title = "Title",
                    Description = "Desc",
                    IconUrl = "https://img.fluxpoint.dev/2531914271424512.png",
                    Url = "https://fluxpoint.dev",
                    Image = "https://img.fluxpoint.dev/2531914271424512.png",
                    Color = new RevoltColor(0, 1, 0)
                }.Build(),
                new EmbedBuilder
                {
                    Title = "Title Anime",
                    Image = "C:/Users/Brandan/Downloads/blob-b61714e8-fc2e-49db-8ca1-d6366784ef64.png",
                    Color = new RevoltColor(0, 1, 0)
                }.Build(),
            });
        }
    }
}
