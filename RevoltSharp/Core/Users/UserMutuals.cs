using System;
using System.Collections.Generic;

namespace RevoltSharp;

public class UserMutuals
{
    internal UserMutuals(UserMutualsJson? json)
    {
        if (json == null || json.users == null)
            Users = Array.Empty<string>();
        else
            Users = json.users;

        if (json == null || json.servers == null)
            Servers = Array.Empty<string>();
        else
            Servers = json.servers;
    }

    public IReadOnlyCollection<string> Users { get; internal set; }
    public IReadOnlyCollection<string> Servers { get; internal set; }
}
