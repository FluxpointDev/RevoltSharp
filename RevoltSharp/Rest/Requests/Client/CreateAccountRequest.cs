using RevoltSharp.Rest;

namespace RevoltSharp;

internal class CreateAccountRequest : IRevoltRequest
{
    public string email { get; set; }
    public string password { get; set; }
    public string? invite { get; set; }
    public string? captcha { get; set; }
}
