using Optionals;
using RevoltSharp.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp;
internal class AdminGetMessagesRequest : IRevoltRequest
{
    public Optional<string> nearby { get; set; }

    public Optional<string> before { get; set; }

    public Optional<string> after { get; set; }

    public Optional<string> sort { get; set; }

    public int limit { get; set; }

    public Optional<string> channel { get; set; }

    public Optional<string> author { get; set; }

    public Optional<string> query { get; set; }
}
