using Optionals;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;

internal class ModifyChannelRequest : RevoltRequest
{
    public Optional<string> name;
    public Optional<string> description;
    public Optional<string> icon;
    public Optional<bool> nsfw;
    public Optional<List<string>> remove;
    public Optional<string> owner;

    

    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = new Optional<List<string>>(new List<string>());

        remove.Value.Add(value);
    }
}
