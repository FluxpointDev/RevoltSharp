using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateRoleRequest : IRevoltRequest
{
    public string name;
    public Optional<int> rank;
}
