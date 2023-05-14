using Optionals;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;

internal class ModifyRoleRequest : RevoltRequest
{
    public Optional<string> name;
    public Optional<string> colour;
    public Optional<bool> hoist;
    public Optional<int> rank;
    public Optional<List<string>> remove;

    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = new Optional<List<string>>(new List<string>());

        remove.Value.Add(value);
    }
}
