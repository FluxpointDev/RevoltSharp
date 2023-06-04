using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateRoleRequest : IRevoltRequest
{
    public string name { get; internal set; }
    public Optional<int> rank { get; internal set; }
}
