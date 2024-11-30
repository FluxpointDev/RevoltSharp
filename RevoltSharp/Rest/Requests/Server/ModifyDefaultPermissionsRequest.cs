namespace RevoltSharp.Rest.Requests;

internal class ModifyDefaultPermissionsRequest : IRevoltRequest
{
    public ulong permissions { get; set; }
}
