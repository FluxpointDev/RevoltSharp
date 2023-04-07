using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TestBot
{
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
            Client = new RevoltClient(Token, ClientMode.WebSocket);
            await Client.StartAsync();
            CommandHandler Commands = new CommandHandler(Client);
            await Commands.Service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await Task.Delay(-1);
        }
    }

    public class CommandHandler
    {
        public CommandHandler(RevoltClient client)
        {
            Client = client;
            client.OnMessageRecieved += message => {
                Client_OnMessageRecieved(message).GetAwaiter().GetResult();
            };
        }
        public RevoltClient Client;
        public CommandService Service = new CommandService();
        private async Task Client_OnMessageRecieved(Message msg)
        {
            UserMessage Message = msg as UserMessage;
            if (Message == null || Message.Author.IsBot)
                return;
            int argPos = 0;
            if (!(Message.HasCharPrefix('!', ref argPos) || Message.HasMentionPrefix(Client.CurrentUser, ref argPos)))
                return;
            CommandContext context = new CommandContext(Client, Message);
            var result = await Service.ExecuteAsync(context, argPos, null);
            Console.WriteLine(result);
            if (result is ExecuteResult er) {
                Console.WriteLine(er.Exception);
            }
        }
    }
}
