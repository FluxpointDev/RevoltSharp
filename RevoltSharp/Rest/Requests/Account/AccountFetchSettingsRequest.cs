using RevoltSharp.Rest;

namespace RevoltSharp;
internal class AccountFetchSettingsRequest : IRevoltRequest
{
    public string[] keys { get; set; }
}
