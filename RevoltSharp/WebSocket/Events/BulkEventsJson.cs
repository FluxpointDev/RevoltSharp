using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.WebSocket.Events;
internal class BulkEventsJson
{
    [JsonProperty("v")]
    public dynamic[] Events;
}
