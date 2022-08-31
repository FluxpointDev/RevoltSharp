using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Core.Servers
{
    public class ServerBan : Entity
    {
        public ServerBan(RevoltClient client) : base(client)
        {

        }

        public string Id { get; internal set; }
        public string Username { get; internal set; }
        public string Reason { get; internal set; }
        public Attachment Avatar { get; internal set; }
    }
}
