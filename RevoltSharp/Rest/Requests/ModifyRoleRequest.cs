using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests
{
    internal class ModifyRoleRequest : RevoltRequest
    {
        public Option<string> name;
        public Option<string> colour;
        public Option<bool> hoist;
        public Option<int> rank;
        public Option<string> remove;
    }
}
