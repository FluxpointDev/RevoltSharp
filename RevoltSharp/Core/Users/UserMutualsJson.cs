using System.Collections.Generic;

namespace RevoltSharp;

internal class UserMutualsJson
{
    public HashSet<string> users { get; set; }
    public HashSet<string> servers { get; set; }
}
