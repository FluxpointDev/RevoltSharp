using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RevoltSharp
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChannelType
    {
        Unknown,
        [EnumMember(Value = "TextChannel")]
        Text,

        [EnumMember(Value = "VoiceChannel")]
        Voice,

        SavedMessages,

        Group,

        [EnumMember(Value = "DirectMessage")]
        Dm
    }
}