using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests;

internal class ModifyDefaultPermissionsRequest : IRevoltRequest
{
    public ulong permissions { get; set; }
}
