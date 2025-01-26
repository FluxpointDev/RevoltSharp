using Microsoft.Extensions.DependencyInjection;
using Optionals;
using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace TestBot;


class Program
{
    static void Main()
    {
        Start().GetAwaiter().GetResult();
    }

    public static RevoltClient Client;

    public static async Task Start()
    {
        // Telrik fiddle proxy to log requests
        //WebRequest.DefaultWebProxy = new WebProxy("127.0.0.1", 8888);

        // Yes ik i can use json file blah blah :p
        string Token = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RevoltBots/Config.txt");


        Client = new RevoltClient(ClientMode.WebSocket, new ClientConfig
        {
            LogMode = RevoltLogSeverity.Debug,
            //ApiUrl = "http://local.revolt.chat/api/",
            Debug = new ClientDebugConfig
            {
                LogRestRequestJson = false,
                LogRestResponseJson = false,
                LogRestRequest = false,
                LogWebSocketFull = false,
                LogWebSocketReady = false,
                LogWebSocketError = true,
                LogWebSocketUnknownEvent = true
            },
            RestProxy = WebRequest.DefaultWebProxy,
            Owners = new string[] { "01FE57SEGM0CBQD6Y7X10VZQ49", "01FEYH91F7KQXFM5737YVR1M1N" }
        });

        string UserToken = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RevoltBots/UserToken.txt");


        Client.OnReady += Client_OnReady;
        Client.OnWebSocketError += Client_OnWebSocketError;
        await Client.LoginAsync(Token, AccountType.Bot);
        await Client.StartAsync();
        Client.OnReady += Client_OnReady1;
        _ = new EventTests(Client);
        //await Client.CurrentUser.ModifySelfAsync(statusText: new Option<string>("LOL"));

        CommandHandler CommandHandler = new CommandHandler(Client);
        _ = CommandHandler.LoadCommands();
        await Task.Delay(-1);
    }

    private static void Client_OnReady1(SelfUser selfuser)
    {
        _ = Task.Run(async () =>
        {
            Console.WriteLine("Getting sessions");
            try
            {
                var Sessions = await Client.Rest.GetAccountSessionsAsync();
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Sessions, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });
    }

    public static void Client_OnReady(SelfUser value)
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
    public static CommandService Service = new CommandService();

    // Change this prefix
    public const string Prefix = "!";

    public async Task LoadCommands()
    {
        var Services = new ServiceCollection().AddSingleton(Service).BuildServiceProvider();

        await Service.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
    }


    private void Client_OnMessageRecieved(Message msg)
    {
        if (msg is not UserMessage Message || Message.Type != MessageType.User)
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
            if (commandinfo.HasValue && !result.IsSuccess)
            {
                Console.WriteLine(((ExecuteResult)result).Exception);
                context.Channel.SendMessageAsync("Error: " + result.ErrorReason);

            }
        }
    }
}