using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests
{
    internal class BulkDeleteMessagesRequest : RevoltRequest
    {
        public string[] ids;
    }
}
