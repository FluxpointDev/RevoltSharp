using Optionals;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;

internal class ModifyChannelRequest : IRevoltRequest
{
    public Optional<string> name { get; internal set; }
    public Optional<string> description { get; internal set; }
    public Optional<string> icon { get; internal set; }
    public Optional<bool> nsfw { get; internal set; }
    public Optional<List<string>> remove { get; internal set; }
    public Optional<string> owner { get; internal set; }



    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = Optional.Some(new List<string>());

        remove.Value.Add(value);
    }
}
