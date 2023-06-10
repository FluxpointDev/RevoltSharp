using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RevoltSharp;

/// <summary>
/// Type of channel that is <see cref="TextChannel" />, <see cref="VoiceChannel" />, <see cref="GroupChannel" />, <see cref="UnknownServerChannel" /> or <see cref="UnknownChannel" />
/// </summary>
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
    DM
}