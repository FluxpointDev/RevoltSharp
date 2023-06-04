using Optionals;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;

internal class ModifyRoleRequest : IRevoltRequest
{
    public Optional<string> name { get; internal set; }
    public Optional<string> colour { get; internal set; }
    public Optional<bool> hoist { get; internal set; }
    public Optional<int> rank { get; internal set; }
    public Optional<List<string>> remove { get; internal set; }

    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = new Optional<List<string>>(new List<string>());

        remove.Value.Add(value);
    }
}
