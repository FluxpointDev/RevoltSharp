using Newtonsoft.Json;
using RevoltSharp.Rest;
using RevoltSharp.WebSocket;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class RevoltClient : ClientEvents
    {
        public RevoltClient(string token, ClientMode mode, ClientConfig config = null)
        {
            try
            {
                DisableConsoleQuickEdit.Go();
            }
            catch { }
            Token = token;
            Config = config ?? new ClientConfig();
            Serializer = new JsonSerializer();
            Serializer.Converters.Add(new OptionConverter());
            Rest = new RevoltRestClient(this);
            if (mode == ClientMode.WebSocket)
                WebSocket = new RevoltSocketclient(this);

        }

        public string Token { get; internal set; }

        internal JsonSerializer Serializer { get; set; }

        public ClientConfig Config { get; internal set; }

        public RevoltRestClient Rest { get; internal set; }

        internal RevoltSocketclient WebSocket;

        public async Task StartAsync()
        {
            if (WebSocket == null)
                throw new RevoltException("Client is in http-only mode.");

            if (WebSocket.WebSocket != null)
                return;
            WebSocket.SetupWebsocket();
            while (WebSocket.WebSocket == null || WebSocket.WebSocket.State != System.Net.WebSockets.WebSocketState.Open) { }
        }

        public async Task StopAsync()
        {
            if (WebSocket == null)
                throw new RevoltException("Client is in http-only mode.");

            if (WebSocket.WebSocket != null)
            {
                await WebSocket.WebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "", WebSocket.CancellationToken);
            }
        }

        /// <summary>
        /// Get the current bot user if you are using websocket.
        /// </summary>
        public User CurrentUser
            => WebSocket != null ? WebSocket.CurrentUser : null;
    }

    public enum ClientMode
    {
        Http, WebSocket
    }
}
