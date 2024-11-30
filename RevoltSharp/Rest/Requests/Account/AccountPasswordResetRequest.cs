using RevoltSharp.Rest;

namespace RevoltSharp;

internal class AccountPasswordResetRequest : IRevoltRequest
{
    public string token {  get; set; }
    public string password { get; set; }
    public bool remove_sessions { get; set; }
}
