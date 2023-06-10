using Optionals;
using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TestBot;

class Program
{
    static void Main(string[] args)
    {
        Start().GetAwaiter().GetResult();
    }

    public static RevoltClient Client;

    public static async Task Start()
    {
        // Yes ik i can use json file blah blah :p
        string Token = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RevoltBots/Config.txt");
        Client = new RevoltClient(Token, ClientMode.WebSocket, new ClientConfig
        {
            Debug = new ClientDebugConfig
            {
                LogRestRequestJson = true,
                LogRestResponseJson = false,
                LogRestRequest = false,
                LogWebSocketFull = true,
                LogWebSocketReady = false,
                LogWebSocketError = true,
                LogWebSocketUnknownEvent = true
            },
            Owners = new string[] { "01FE57SEGM0CBQD6Y7X10VZQ49", "01FEYH91F7KQXFM5737YVR1M1N" }
        });
        Client.OnReady += Client_OnReady;
        Client.OnWebSocketError += Client_OnWebSocketError;
        await Client.StartAsync();
        //new EventTests(Client);
        await Client.CurrentUser.ModifySelfAsync(statusText: new Option<string>("LOL"));


        CommandHandler CommandHandler = new CommandHandler(Client);
        _ = CommandHandler.LoadCommands();
        await Task.Delay(-1);
    }


    private static void Client_OnReady(SelfUser value)
    {
        Console.WriteLine("Ready: " + value.Username);
    }

    private static void Client_OnWebSocketError(SocketError value)
    {
        Console.WriteLine("Socket Error: " + value.Message);
    }
}

public class CommandHandler
{
    public CommandHandler(RevoltClient client)
    {
        Client = client;
        Client.OnMessageRecieved += Client_OnMessageRecieved;
        Service.OnCommandExecuted += Service_OnCommandExecuted;
    }
    private RevoltClient Client;
    private CommandService Service = new CommandService();

    // Change this prefix
    public const string Prefix = "!";

    public async Task LoadCommands()
    {
        await Service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
    }


    private void Client_OnMessageRecieved(Message msg)
    {
        UserMessage Message = msg as UserMessage;
        if (Message == null || Message.Author.IsBot)
            return;
        int argPos = 0;
        if (!(Message.HasStringPrefix(Prefix, ref argPos) || Message.HasMentionPrefix(Client.CurrentUser, ref argPos)))
            return;
        CommandContext context = new CommandContext(Client, Message);

        _ = Service.ExecuteAsync(context, argPos, null);
    }

    private void Service_OnCommandExecuted(Optional<CommandInfo> commandinfo, CommandContext context, IResult result)
    {
        if (result.IsSuccess)
            Console.WriteLine("Success command: " + commandinfo.Value.Name);
        else
        {
            if (commandinfo.HasValue)
                context.Channel.SendMessageAsync("Error: " + result.ErrorReason);
        }
    }
}
