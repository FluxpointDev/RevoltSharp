namespace RevoltSharp.Rest.Requests;

internal class AccountLoginRequest : IRevoltRequest
{
    public string email { get; set; }
    public string password { get; set; }
    public string friendly_name { get; set; }
}
