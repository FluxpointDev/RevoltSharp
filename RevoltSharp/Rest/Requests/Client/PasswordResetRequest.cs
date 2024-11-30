using RevoltSharp.Rest;

namespace RevoltSharp;

internal class PasswordResetRequest : IRevoltRequest
{
    public string email {  get; set; }
    public string captcha { get; set; }
}
