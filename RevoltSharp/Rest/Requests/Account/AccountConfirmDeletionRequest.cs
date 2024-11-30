using RevoltSharp.Rest;

namespace RevoltSharp;

internal class AccountConfirmDeletionRequest : IRevoltRequest
{
    public string token { get; set; }
}
