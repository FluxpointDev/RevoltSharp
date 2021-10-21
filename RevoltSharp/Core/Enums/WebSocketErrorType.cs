using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RevoltSharp
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WebSocketErrorType
    {
        [EnumMember(Value = "LabelMe")]
        Unknown,

        [EnumMember(Value = "InternalError")]
        InternalServerError,

        InvalidSession,

        OnboardingNotFinished,

        AlreadyAuthenticated
    }
}
