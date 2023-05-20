using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateRoleRequest : RevoltRequest
{
    public string name;
    public Optional<int> rank;
}
