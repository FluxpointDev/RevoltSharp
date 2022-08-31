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
        public Option<string> description;
        public string[] users;
        public Option<bool> nsfw;

    }
}
