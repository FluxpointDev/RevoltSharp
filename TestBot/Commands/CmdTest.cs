using Newtonsoft.Json;
using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    [RequireOwner]
    public class CmdTest : ModuleBase
    {
        [Command("emoji")]
        public async Task Emoji(string id)
        {
            Emoji emoji = Context.Client.GetEmoji(id);
            await ReplyAsync($"Exists: {emoji != null}");
        }

        [Command("role")]
        public async Task Role(string id)
        {
            Role role = Context.Client.GetRole(id);
            await ReplyAsync($"Exists: {role != null}");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(role, Formatting.Indented));
        }

        [Command("emojimsg")]
        public async Task EmojiMsg()
        {
            UserMessage Msg = await Context.Channel.SendMessageAsync("Emoji test", interactions: new MessageInteractions
            {
                Reactions = new Emoji[] { new Emoji(":01GBP83S4WT512ET704ACPVPQW:") },
                RestrictReactions = true
            });
            Console.WriteLine(Context.Server.GetCachedMember(Context.Client.CurrentUser.Id).Permissions.ManageMessages.ToString());
            MessageId = Msg.Id;

        }

        [Command("removeemoji")]
        public async Task EmojiRemove(string id)
        {
            Console.WriteLine(MessageId);
            await Context.Client.Rest.RemoveMessageReactionAsync(Context.Channel.Id, MessageId, "01GBP83S4WT512ET704ACPVPQW", id);
        }

        public static string MessageId;

        [Command("members")]
        public async Task RoleAll()
        {
            await ReplyAsync("Running");
            ServerMember[] Members = await Context.Server.GetMembersAsync();
            Console.WriteLine("Members: " + Members.Length);
            foreach (ServerMember m in Members)
            {
                if (m.IsBot)
                {
                    Console.WriteLine("BOT: " + m.Username);
                    continue;
                }
                if (m.GetRole("01FESEE54DDDSEM217NX9GH4KG") == null)
                {
                    Console.WriteLine("NO ROLE: " + m.User.Username);

                }
                else
                {
                    Console.WriteLine("HAS ROLE: " + m.User.Username);
                }
            }
        }

        [Command("retry")]
        public async Task Retry()
        {
            int Count = 0;
            while (Count != 12)
            {
                try
                {
                    await ReplyAsync($"Testing {Count} :)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Count += 1;
            }
        }


        [Command("testclass")]
        public async Task TestClass(string type)
        {
            Console.WriteLine($"--- --- --- {type}");
            switch (type)
            {
                case "user":
                    Console.WriteLine("Got user");
                    try
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(Context.User, new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;
                case "server":
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Context.Server, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                    break;
                case "channel":
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Context.Channel, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                    break;
                case "member":
                    if (Context.Member == null)
                        Console.WriteLine("NULL MEMBER!");
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Context.Member, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                    break;
                case "message":
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Context.Message, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        MaxDepth = 1,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                    break;
            }
            Console.WriteLine("--- --- ---");
        }

        [Command("test")]
        public async Task Test()
        {
            Console.WriteLine("-- Roles ---");
            Role Role = await Context.Server.CreateRoleAsync("Test");
            Console.WriteLine("- Create");
            await Role.DeleteAsync();
            Console.WriteLine("- Delete");
            Console.WriteLine("-- Channels ---");
            TextChannel Channel = await Context.Server.CreateTextChannelAsync("T", "AHH");
            Console.WriteLine("- Create");
            await Channel.DeleteChannelAsync();
            Console.WriteLine("- Delete");
            Console.WriteLine("-- Msg ---");
            UserMessage Msg = await Context.Channel.SendMessageAsync("Ahh", new Embed[] { new EmbedBuilder
            {
                Title = "T",
                Description = "L",
                Color = new RevoltColor(0, 0, 1),
                Url = "https://fluxpoint.dev"
            }.Build() }, masquerade: new MessageMasquerade("Ah"));
            Console.WriteLine("- Create");
            await Context.Client.Rest.AddMessageReactionAsync(Msg.ChannelId, Msg.Id, "01GC7SSTBN4D4AGH6KDHMGFHFV");
            Console.WriteLine("- Add reaction");
            await Msg.EditMessageAsync(new Option<string>("T"));
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
                $"View Channels: {Perms.ViewChannels} | Manage Webhooks: {Perms.ManageWebhooks} | Manage Channels: {Perms.ManageChannels}");
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
            FileAttachment Attach = await Context.Client.Rest.UploadFileAsync("/Downloads/blob-b61714e8-fc2e-49db-8ca1-d6366784ef64.png", RevoltSharp.Rest.RevoltRestClient.UploadFileType.Attachment);
            
        }

        [Command("chan")]
        public async Task Channel()
        {
            await ReplyAsync(Context.Channel.Id);
        }

        [Command("roles")]
        public async Task Role()
        {
            await Context.Channel.SendMessageAsync("", new Embed[]
            {
                new EmbedBuilder
                {
                    Description = String.Join("\n", Context.Server.Roles.Select(x => $"{x.Name} - {x.Color} ({x.Id})"))
                }.Build()
            }); 
            foreach(Role Role in Context.Server.Roles)
            {
                Console.WriteLine($"[ {Role.Name} ] {Role.Permissions.AssignRoles}:{Role.Permissions.BanMembers}:{Role.Permissions.ManageMessages}:{Role.Permissions.SendMessages}");
            }
        }

        [Command("addrole")]
        public async Task AddRole()
        {
            ServerMember Member = await Context.Server.GetMemberAsync("01G3BHHPN05RTFDGB99YRYC8QN");
            System.Net.Http.HttpResponseMessage Req = await Context.Client.Rest.AddRoleAsync(Member, Context.Server.GetRole("01FESEE54DDDSEM217NX9GH4KG"));
            await ReplyAsync(Req.IsSuccessStatusCode.ToString());
        
        }


        [Command("memberroles")]
        public async Task Roles(string id)
        {
            ServerMember Member = await Context.Server.GetMemberAsync(id);
            await ReplyAsync(String.Join(", ", Member.Roles.Select(x => x.Name)));
        }

        [Command("removerole")]
        public async Task RemoveRole()
        {
            ServerMember Member = await Context.Server.GetMemberAsync("01G3BHHPN05RTFDGB99YRYC8QN");
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
            await Context.Channel.SendMessageAsync("Hello", new RevoltSharp.Embed[]
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
                    Color = new RevoltColor(0, 1, 0)
                }.Build(),
            });
        }
        
        // replies: space separated list of strings like so:
        // +01GX79H9S8FZERHVA5S5E68AAY -01GX79H9S8FZERHVA5S5E68AAY
        // + means mention, - means do not
        [Command("reply")]
        public async Task Reply([Remainder] string replies)
        {
            await Context.Channel.SendMessageAsync("Hello", replies: replies.Split(" ").Select(id => new MessageReply() {
                id = id[1..],
                mention = id[0] == '+'
            }).ToArray());
        }

        [Command("error")]
        public async Task Error()
        {
            throw new Exception("This is a test error :)");
        }
        
        [Command("countdown")]
        public async Task Countdown() 
        {
            UserMessage message = await Context.Channel.SendMessageAsync("3");
            await Task.Delay(TimeSpan.FromSeconds(1));
            await message.EditMessageAsync(new Option<string>("2"));
            await Task.Delay(TimeSpan.FromSeconds(1));
            await message.EditMessageAsync(new Option<string>("1"));
        }
    }
}
