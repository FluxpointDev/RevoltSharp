using RevoltSharp.Rest;

namespace RevoltSharp;

internal class AccountVerificationRequest : IRevoltRequest
{
    public string email { get; set; }
    public string captcha { get; set; }
}
