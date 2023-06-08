using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateRoleRequest : IRevoltRequest
{
    public string name { get; set; }
    public Optional<int> rank { get; set; }
}
