# Info
RevoltSharp is a bot lib used to connect to [Revolt](https://revolt.chat/) chat app.
This lib only supports bots for now and not userbots!

# Support
Join our Revolt server for more information or help with the lib.
https://app.revolt.chat/invite/J5Ras1J3

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
    // You don't need to run start if client mode is http.
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
            if (Message.Author.IsBot)
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
