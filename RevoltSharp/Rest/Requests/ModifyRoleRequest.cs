using Optionals;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;


internal class ModifyRoleRequest : IRevoltRequest
{
    public Optional<string> name { get; set; }
    public Optional<string> colour { get; set; }
    public Optional<bool> hoist { get; set; }
    public Optional<int> rank { get; set; }
    public Optional<List<string>> remove { get; set; }

    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = Optional.Some(new List<string>());

        remove.Value.Add(value);
    }
}