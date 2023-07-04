using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RevoltSharp
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RevoltErrorType
    {
        [EnumMember(Value = "LabelMe")]
        Unknown,
        AlreadyOnboarded,
        UsernameTaken,
        InvalidUsername,
        UnknownUser,
        AlreadyFriends,
        AlreadySentRequest,
        Blocked,
        BlockedByOther,
        NotFriends,
        UnknownChannel,
        UnknownAttachment,
        UnknownMessage,
        CannotEditMessage,
        CannotJoinCall,
        TooManyAttachments,
        TooManyReplies,
        EmptyMessage,
        PayloadTooLarge,
        CannotRemoveYourself,
        GroupTooLarge,
        AlreadyInGroup,
        NotInGroup,
        UnknownServer,
        InvalidRole,
        Banned,
        TooManyServers,
        TooManyEmoji,
        ReachedMaximumBots,
        [EnumMember(Value = "IsBot")]
        NotAllowedForBots,
        NotAllowedForUsers,
        BotIsPrivate,
        CannotReportYourself,
        MissingPermission,
        MissingUserPermission,
        NotElevated,
        NotPrivileged,
        CannotGiveMissingPermissions,
        NotOwner,
        [EnumMember(Value = "DatabaseError")]
        RevoltDatabaseError,
        [EnumMember(Value = "InternalError")]
        InternalServerError,
        InvalidOperation,
        InvalidCredentials,
        InvalidProperty,
        InvalidSession,
        DuplicateNonce,
        VosoUnavailable,
        NotFound,
        NoEffect,
        FailedValidation,
        OnboardingNotFinished,
        AlreadyAuthenticated
    }
}