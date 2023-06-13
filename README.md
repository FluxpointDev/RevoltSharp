# Info
RevoltSharp is a bot lib used to connect to [Revolt](https://revolt.chat/) chat app.

This lib supports both bots and user accounts.

### Core Features
- Stable | The lib is designed to handle errors and exceptions with logging and nice error messages to help you build a public bot.
- Easy to Use | RevoltSharp has similar design principals to discord.net with easy to use objects and properties.
- Connection | The WebSocket is error proof and will keep the connection alive even during an outage or unstable network!
- Efficient | You can run a private 1 server bot using 30mb of ram (with command handling, commands and cache) or a public bot in 450+ servers with 210mb of ram that can run on any $3 hosting service with 1GB ram!
- Cache | The lib will cache reuseable data for an easy to use and fast experience, especially with events.
- Requests | You can send custom requests using Client.Rest.SendRequest that uses disposable streams and buffers for memory efficiency.
- Documented | Most of the code is documented with xml code comments or you can see the docs here https://docs.fluxpoint.dev/revoltsharp
- Built-in Command Handling | 

# Support
Join the RevoltSharp server for support and meet other C# developers!

https://rvlt.gg/N33Rf6DE

# Documentation
The library is documented in code and also has a docs site that you can browser with guides!

https://docs.fluxpoint.dev/revoltsharp

# Install
You can download the lib in visual studio/code using this [Nuget Package](https://www.nuget.org/packages/RevoltSharp)

Once it's installed you can use this basic example.
```cs
static void Main(string[] args)
{
    Start().GetAwaiter().GetResult();
}

public static RevoltClient Client;
public static async Task Start()
{
    Client = new RevoltClient("Bot Token", ClientMode.WebSocket);
    
    await Client.StartAsync();
    await Task.Delay(-1);
}
```

# Commands
This lib also includes and easy to use command system for setting up commands and using data.
Big thanks to [Discord.net](https://github.com/discord-net/Discord.Net) for the internal command handler system, this was modified to work with Revolt and the classes.

Here is an example on how to setup commands.
```cs
class Program
{
    static void Main(string[] args)
    {
        Start().GetAwaiter().GetResult();
    }

    public static RevoltClient Client;
    public static async Task Start()
    {
        Client = new RevoltClient("Bot Token", ClientMode.WebSocket);
        await Client.StartAsync();
        CommandHandler Commands = new CommandHandler(Client);
        Commands.Service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        await Task.Delay(-1);
    }
}

public class CommandHandler
{
    public CommandHandler(RevoltClient client)
    {
        Client = client;
        client.OnMessageRecieved += Client_OnMessageRecieved;
    }
    public RevoltClient Client;
    public CommandService Service = new CommandService();
    private void Client_OnMessageRecieved(Message msg)
        {
            UserMessage Message = msg as UserMessage;
            if (Message == null || Message.Author.IsBot)
                return;
            int argPos = 0;
            if (!(Message.HasCharPrefix('!', ref argPos) || Message.HasMentionPrefix(Client.CurrentUser, ref argPos)))
                return;
            CommandContext context = new CommandContext(Client, Message);
            Service.ExecuteAsync(context, argPos, null);
        }
}
```

### Create a command
```cs
public class CmdTest : ModuleBase
{
    [Command("hi")]
    public async Task Hi()
    {
        await ReplyAsync("Hi " + Context.User.Username);
    }

    [Command("say")]
    public async Task Say([Remainder] string text)
    {
        await ReplyAsync(text);
    }
}
```
