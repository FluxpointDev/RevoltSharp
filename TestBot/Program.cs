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
            string Token = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RevoltBots/Config.txt");
            Client = new RevoltClient(Token, ClientMode.WebSocket);
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
            if (msg.Author.IsBot)
                return;
            int argPos = 0;
            if (!(msg.HasCharPrefix('!', ref argPos) || msg.HasMentionPrefix(Client.CurrentUser, ref argPos)))
                return;
            CommandContext context = new CommandContext(Client, msg);
            Service.ExecuteAsync(context, argPos, null);
        }
    }
}
