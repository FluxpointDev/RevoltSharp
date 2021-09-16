using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests
{
    internal class ModifyChannelRequest : RevoltRequest
    {
        public Option<string> name;
        public Option<string> description;
        public Option<string> icon;
        public Option<bool> nsfw;
        public Option<string> remove;
    }
}
