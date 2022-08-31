using Newtonsoft.Json;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests
{
    internal class ReasonRequest : RevoltRequest
    {
        public Option<string> reason;

    }
}
