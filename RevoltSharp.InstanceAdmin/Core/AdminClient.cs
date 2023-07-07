using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp;

public class AdminClient
{
    internal AdminClient(RevoltClient client)
    {
        Client = client;
    }
    internal RevoltClient Client;
}
