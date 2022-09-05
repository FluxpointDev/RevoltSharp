using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests
{
    internal class DeleteReactionRequest : RevoltRequest
    {
        public string user_id;
        public bool remove_all;
    }
}
