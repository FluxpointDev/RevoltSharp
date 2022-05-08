using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerJoinEventJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("server")]
        public Server Server;

        [JsonProperty("channels")]
        public ServerChannel[] Channels;
    }
}
