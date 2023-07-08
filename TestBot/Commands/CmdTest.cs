#pragma warning disable CS1998

using Newtonsoft.Json;
using RevoltSharp;
using RevoltSharp.Commands;
using RevoltSharp.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestBot.Commands;


[RequireOwner]
public class CmdTest : ModuleBase
{
    [Command("text")]
    public async Task Text()
    {
        int Count = Context.Server.TextChannels.Count();
    }
    [Command("vc")]
    public async Task VC()
    {
        Console.WriteLine("RUN");
        try
        {
            var VCR = await Context.Server.GetVoiceChannel("01H4A2KP85J5R21M01C1MNYJ5X").JoinChannelAsync();
            string File = "C:\\Users\\Brandan\\Downloads\\toads.mp4";

            _ = Task.Run(async () =>
            {
                return;
                using (var ffmpeg = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-hide_banner -loglevel panic -i \"{File}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }))
                {
                    //using (var output = ffmpeg.StandardOutput.BaseStream)
                    //using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
                    //{
                    //	try { await output.CopyToAsync(discord); }
                    //	finally { await discord.FlushAsync(); }
                    //}
                }


            });

            await ReplyAsync(VCR.Channel.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    [Command("vcdata")]
    public async Task VCData()
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(new InitilizeTransportRequest(), Formatting.Indented));
    }

    [Command("webhooks")]
    public async Task Webhooks()
    {
        var Webhooks = await (Context.Channel as TextChannel).GetWebhooksAsync();
        foreach (var w in Webhooks)
        {
            Console.WriteLine($"WH: {w.Name}");
        }
    }

    [Command("testlog")]
    public async Task TestLog()
    {
        Context.Client.InvokeLog("Info", RevoltLogSeverity.Info);
        Context.Client.InvokeLog("Warn", RevoltLogSeverity.Warn);
        Context.Client.InvokeLog("Error", RevoltLogSeverity.Error);
        Context.Client.InvokeLog("Debug", RevoltLogSeverity.Verbose);
    }

    [Command("bans")]
    public async Task Messages()
    {
        var Messages = await Context.Server.GetBansAsync();

    }

    [Command("permtest")]
    public async Task PermTest()
    {
        await ReplyAsync(Context.Member.Permissions.Has(ServerPermission.BanMembers).ToString());
    }

    [Command("tag")]
    public async Task Tag()
    {
        await ReplyAsync($"Hi {Context.User.Tag}");
    }
    [Command("owner")]
    public async Task Owner()
    {
        await (Context.Channel as GroupChannel).ModifyAsync(owner: new Option<string>("01FE57SEGM0CBQD6Y7X10VZQ49"));
    }

    [Command("myperm")]
    public async Task MyPerm()
    {

        await ReplyAsync($"F: {Context.Member.Permissions.AddReactions} H: {Context.Member.Permissions.Has(ChannelPermission.AddReactions)}\n\n" +
            $"Roles:\n{string.Join("\n", Context.Member.Roles.Select(x => x.Name))}");
    }

    [Command("perm")]
    public async Task Perm()
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Context.Member.Permissions, Formatting.Indented));
        //TextChannel GC = (TextChannel)Context.Channel;
        //await ReplyAsync($"Allow: SP: {GC.DefaultPermissions.Server != null} C: {GC.DefaultPermissions.AddReactions} F: {GC.DefaultPermissions.Has(ChannelPermission.AddReactions)} S: {Context.Server.DefaultPermissions.AddReactions}");

    }

    [Command("dm")]
    public async Task DM()
    {
        DMChannel DM = await Context.User.GetDMChannelAsync();
        await DM.SendMessageAsync("Hi :)");
        await Task.Delay(new TimeSpan(0, 0, 3));
        await DM.CloseAsync();
    }

    [Command("mystatus")]
    public async Task MyStatus()
    {
        string Status = Context.User.Status.Type.ToString();
        await Context.Channel.SendMessageAsync("Status: " + Status);
    }

    [Command("datetest")]
    public async Task DateTest(string id)
    {
        Ulid ulid = Ulid.Parse(id);
        await ReplyAsync($"Time: {ulid.Time.ToString()}");
    }

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
        Console.WriteLine(JsonConvert.SerializeObject(role, Formatting.Indented));
    }

    [Command("emojimsg")]
    public async Task EmojiMsg()
    {
        UserMessage Msg = await Context.Channel.SendMessageAsync("Emoji test",
            interactions: new MessageInteractions(new Emoji[] { new Emoji(":01GBP83S4WT512ET704ACPVPQW:") }, true));
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
        IReadOnlyCollection<ServerMember> Members = await Context.Server.GetMembersAsync();
        Console.WriteLine("Members: " + Members.Count);
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
                Console.WriteLine(JsonConvert.SerializeObject(Context.Server, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
                break;
            case "channel":
                Console.WriteLine(JsonConvert.SerializeObject(Context.Channel, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
                break;
            case "perms":
                {
                    Console.WriteLine("Default");
                    Console.WriteLine(JsonConvert.SerializeObject((Context.Channel as TextChannel).DefaultPermissions, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                    Console.WriteLine("Server");
                    Console.WriteLine(JsonConvert.SerializeObject(Context.Server.DefaultPermissions, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                }
                break;
            case "member":
                if (Context.Member == null)
                    Console.WriteLine("NULL MEMBER!");
                Console.WriteLine(JsonConvert.SerializeObject(Context.Member, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
                break;
            case "message":
                Console.WriteLine(JsonConvert.SerializeObject(Context.Message, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    MaxDepth = 1,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
                break;
        }
        Console.WriteLine("--- --- ---");
    }

    public static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

    [Command("test")]
    public async Task Test()
    {
        Console.WriteLine(semaphoreSlim.CurrentCount);
        await semaphoreSlim.WaitAsync();
        try
        {

            await ReplyAsync("Test");
            await Task.Delay(30000);
            Console.WriteLine("Done delay");
        }
        finally
        {
            Console.WriteLine("Release");
            semaphoreSlim.Release();
        }
    }

    [Command("testcontext")]
    public async Task TestContext()
    {
        Console.WriteLine("-- Roles ---");
        Role Role = await Context.Server.CreateRoleAsync("Test");
        Console.WriteLine("- Create");
        await Role.DeleteAsync();
        Console.WriteLine("- Delete");
        Console.WriteLine("-- Channels ---");
        TextChannel Channel = await Context.Server.CreateTextChannelAsync("T", "AHH");
        Console.WriteLine("- Create");
        await Channel.DeleteAsync();
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
        await Msg.DeleteAsync();
        Console.WriteLine("- Delete");
    }

    [Command("perms"), RequireUserPermission(ServerPermission.ManageRoles)]
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
        FileAttachment Attach = await Context.Client.Rest.UploadFileAsync("/Downloads/blob-b61714e8-fc2e-49db-8ca1-d6366784ef64.png", UploadFileType.Attachment);

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
        foreach (Role Role in Context.Server.Roles)
        {
            Console.WriteLine($"[ {Role.Name} ] {Role.Permissions.AssignRoles}:{Role.Permissions.BanMembers}:{Role.Permissions.ManageMessages}:{Role.Permissions.SendMessages}");
        }
    }

    [Command("addrole")]
    public async Task AddRole()
    {
        ServerMember Member = await Context.Server.GetMemberAsync("01G3BHHPN05RTFDGB99YRYC8QN");
        await Member.AddRoleAsync(Context.Server.GetRole("01FESEE54DDDSEM217NX9GH4KG"));
    }


    [Command("memberroles")]
    public async Task Roles(string id)
    {
        ServerMember Member = await Context.Server.GetMemberAsync(id);
        await ReplyAsync(String.Join(", ", Member.Roles.Select(x => x.Name)));
    }

    [Command("removerole"), RequireOwner]
    public async Task RemoveRole()
    {
        ServerMember Member = await Context.Server.GetMemberAsync("01G3BHHPN05RTFDGB99YRYC8QN");
        await Context.Client.Rest.RemoveRoleAsync(Member, Context.Server.GetRole("01FESEE54DDDSEM217NX9GH4KG"));
    }

    [Command("testreaction")]
    public async Task TestReaction()
    {
        await Context.Channel.SendMessageAsync("Test", interactions: new MessageInteractions(new Emoji[]
            {
            Context.Server.GetEmoji("01GBP83S4WT512ET704ACPVPQW")
            }, true));
    }

    [Command("embed")]
    public async Task Embed()
    {
        await Context.Channel.SendMessageAsync("Hello", new Embed[]
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
        await Context.Channel.SendMessageAsync("Hello", replies: replies.Split(" ").Select(id => new MessageReply(id = id[1..],
            id[0] == '+')).ToArray());
    }

    [Command("error")]
    public async Task Error()
    {
        throw new Exception("This is a test error :)");
    }

    [Command("FullTestClient")]
    public async Task FullTestClient()
    {
        Console.WriteLine("Running self user tests");
        Profile SP = await Context.Client.CurrentUser.GetProfileAsync();
        if (SP == null)
            Console.WriteLine("- Profile null");

        await Context.Client.CurrentUser.ModifySelfAsync(statusText: new Option<string>("Status here"), statusType: new Option<UserStatusType>(UserStatusType.Busy));

        Console.WriteLine("Running user tests");
        User User = Context.Client.GetUser("01FE57SEGM0CBQD6Y7X10VZQ49");
        if (User == null)
            throw new Exception("Failed to get user");
        await User.BlockAsync();
        await User.UnBlockAsync();
        DMChannel DM = await User.GetDMChannelAsync();
        if (DM == null)
            throw new Exception("Failed to get DM");

        Profile UP = await User.GetProfileAsync();
        if (UP == null)
            throw new Exception("Failed to get DM");

        Console.WriteLine("Running dm tests");
        UserMessage DMM = await DM.SendMessageAsync("Hi");

        await DMM.DeleteAsync();
        await DM.CloseAsync();


        Console.WriteLine("Running group tests");

        GroupChannel GC = Context.Client.GetGroupChannel("01G2GCGG376T4E3AV41S6ADGPQ");
        if (GC == null)
            throw new Exception("Failed to get Group");

        UserMessage GCM = await GC.SendMessageAsync("Hi");
        await GCM.DeleteAsync();

        //var GCMembers = await GC.GetMembersAsync();
        //if (!GCMembers.Any())
        //    throw new Exception("Failed to get Group members");

        await GC.ModifyAsync(new Option<string>("Tags Test"), new Option<string>("Desc here"));

        Console.WriteLine("Running saved message tests");
        SavedMessagesChannel Saved = await Context.Client.Rest.GetOrCreateSavedMessageChannelAsync();
        if (Saved == null)
            throw new Exception("Failed to get saved message channel");

        UserMessage SM = await Saved.SendMessageAsync("Hi");
        await SM.DeleteAsync();
    }

    [Command("fulltestserver")]
    public async Task FullTestServer()
    {

        Server Server = Context.Client.GetServer("01G2RNRDXXEZP3WEHQZEY4GE79");
        UserMessage MSG = await Context.Channel.SendMessageAsync("Hi", new Embed[]
        {
        new EmbedBuilder
        {
            Title = "Test",
            Description = "Desc"
        }.Build()
        });
        await MSG.AddReactionAsync(new Emoji(":01GZYQS64JEW1KTX7K8PPGMVA5:"));
        await MSG.RemoveReactionAsync(new Emoji("01GZYQS64JEW1KTX7K8PPGMVA5"), Context.Client.CurrentUser);
        await MSG.EditMessageAsync(new Option<string>("Content here"), new Option<Embed[]>(null));
    }
}