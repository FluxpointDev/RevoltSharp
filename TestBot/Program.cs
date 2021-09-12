using RevoltSharp;
using System;
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
            Console.WriteLine("Starting bot.");
            await Client.StartAsync();
            Console.WriteLine("Bot connected.");
            await Task.Delay(-1);
        }
    }
}
