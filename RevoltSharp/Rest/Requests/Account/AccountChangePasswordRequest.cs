using RevoltSharp.Rest;

namespace RevoltSharp;

internal class AccountChangePasswordRequest : IRevoltRequest
{
    public string password { get; set; }
    public string current_password { get; set; }
}
