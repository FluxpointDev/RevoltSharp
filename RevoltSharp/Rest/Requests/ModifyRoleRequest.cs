using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class ModifyRoleRequest : RevoltRequest
{
    public Optional<string> name;
    public Optional<string> colour;
    public Optional<bool> hoist;
    public Optional<int> rank;
    public Optional<string[]> remove;
}
