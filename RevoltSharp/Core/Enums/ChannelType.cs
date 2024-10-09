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
    /// <summary>
    /// Unknown channel type that could not be parsed.
    /// </summary>
    Unknown,
    /// <summary>
    /// A Text channel that can send and recieve messages.
    /// </summary>
    [EnumMember(Value = "TextChannel")]
    Text,
    /// <summary>
    /// A Voice channel that can be used to talk or show video calls with others.
    /// </summary>
    [EnumMember(Value = "VoiceChannel")]
    Voice,
    /// <summary>
    /// A private notes channels for the current user.
    /// </summary>
    SavedMessages,
    /// <summary>
    /// A private group of users.
    /// </summary>
    Group,
    /// <summary>
    /// A private channel between another user.
    /// </summary>
    [EnumMember(Value = "DirectMessage")]
    DM
}