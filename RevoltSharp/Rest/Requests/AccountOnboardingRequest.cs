using RevoltSharp.Rest;

namespace RevoltSharp;
internal class AccountOnboardingRequest : IRevoltRequest
{
    public string username { get; set; }
}
