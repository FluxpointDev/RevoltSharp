using RevoltSharp.Rest;

namespace RevoltSharp;

internal class AccountChangeEmailRequest : IRevoltRequest
{
    public string email { get; set; }
    public string current_password { get; set; }
}
