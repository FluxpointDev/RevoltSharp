using Newtonsoft.Json;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests
{
    internal class CreateChannelRequest : RevoltRequest
    {
        public string name;
        [JsonProperty("type")]
        public string Type;
        public Option<string> description;
        public Option<string[]> users;
        public Option<bool> nsfw;

    }
}
