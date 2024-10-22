using Optionals;
using System.Collections.Generic;

namespace RevoltSharp.Rest.Requests;
internal class ModifyServerRequest : IRevoltRequest
{
    public Optional<string> name { get; set; }

    public Optional<string> description { get; set; }

    public Optional<string> icon { get; set; }

    public Optional<string> banner { get; set; }

    public Optional<List<CategoryJson>> categories { get; set; }

    public Optional<ModifyServerSystemChannels> system_messages { get; set; }

    public Optional<List<string>> remove { get; set; }

    public void RemoveValue(string value)
    {
        if (!remove.HasValue)
            remove = Optional.Some(new List<string>());

        remove.Value.Add(value);
    }
}
internal class ModifyServerSystemChannels
{
    public Optional<string?> user_joined { get; set; }
    public Optional<string?> user_left { get; set; }
    public Optional<string?> user_kicked { get; set; }
    public Optional<string?> user_banned { get; set; }
}
