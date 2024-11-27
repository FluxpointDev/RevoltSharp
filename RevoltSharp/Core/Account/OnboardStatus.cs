using Newtonsoft.Json;

namespace RevoltSharp;
public class OnboardStatus
{
    [JsonProperty("onboarding")]
    public bool IsOnboardingRequired { get; set; }
}
