using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RevoltSharp;

/// <summary>
/// The error type for this request/function
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum RevoltErrorType
{
    /// <summary>
    /// Unknown error type.
    /// </summary>
    [EnumMember(Value = "LabelMe")]
    Unknown,
    /// <summary>
    /// User is already onboarded
    /// </summary>
    AlreadyOnboarded,
    /// <summary>
    /// Username is already taken.
    /// </summary>
    UsernameTaken,
    /// <summary>
    /// Invalid username that can't be used.
    /// </summary>
    InvalidUsername,
    /// <summary>
    /// Unknown user or not found.
    /// </summary>
    UnknownUser,
    /// <summary>
    /// You are already friends with this user.
    /// </summary>
    AlreadyFriends,
    /// <summary>
    /// You already send a friend request to this user.
    /// </summary>
    AlreadySentRequest,
    /// <summary>
    /// You have blocked this user.
    /// </summary>
    Blocked,
    /// <summary>
    /// This user has you blocked.
    /// </summary>
    BlockedByOther,
    /// <summary>
    /// You are not friends with this user.
    /// </summary>
    NotFriends,
    /// <summary>
    /// Unknown channel or not found.
    /// </summary>
    UnknownChannel,
    /// <summary>
    /// Unknown attachment or not found.
    /// </summary>
    UnknownAttachment,
    /// <summary>
    /// Unknown message or not found.
    /// </summary>
    UnknownMessage,
    /// <summary>
    /// You can't edit other user's messages.
    /// </summary>
    CannotEditMessage,
    /// <summary>
    /// You can't join this voice channel.
    /// </summary>
    CannotJoinCall,
    /// <summary>
    /// Too many attachments for the sent message.
    /// </summary>
    TooManyAttachments,
    /// <summary>
    /// Too many replies for the sent message.
    /// </summary>
    TooManyReplies,
    /// <summary>
    /// Message is empty or has no data.
    /// </summary>
    EmptyMessage,
    /// <summary>
    /// Request payload is too large.
    /// </summary>
    PayloadTooLarge,
    /// <summary>
    /// You cannot remove yourself from the server?
    /// </summary>
    CannotRemoveYourself,
    /// <summary>
    /// Group has hit the max limit of users.
    /// </summary>
    GroupTooLarge,
    /// <summary>
    /// You are already in this group.
    /// </summary>
    AlreadyInGroup,
    /// <summary>
    /// You or a user is not in the group?
    /// </summary>
    NotInGroup,
    /// <summary>
    /// Unknown server or not found.
    /// </summary>
    UnknownServer,
    /// <summary>
    /// Unknown role or not found.
    /// </summary>
    InvalidRole,
    /// <summary>
    /// You are banned from this server.
    /// </summary>
    Banned,
    /// <summary>
    /// You have hit the max server limit for your account.
    /// </summary>
    TooManyServers,
    /// <summary>
    /// This server has hit the max emojis limit.
    /// </summary>
    TooManyEmoji,
    /// <summary>
    /// You have hit the max bots created for your account.
    /// </summary>
    ReachedMaximumBots,
    /// <summary>
    /// This request/function is not allowed for bot accounts.
    /// </summary>
    [EnumMember(Value = "IsBot")]
    NotAllowedForBots,
    /// <summary>
    /// This request/function is not allowed for normal user accounts.
    /// </summary>
    NotAllowedForUsers,
    /// <summary>
    /// You can't invite this bot because it's private.
    /// </summary>
    BotIsPrivate,
    /// <summary>
    /// You can't report your own messages.
    /// </summary>
    CannotReportYourself,
    /// <summary>
    /// You are missing permissions for this request/function to use.
    /// </summary>
    MissingPermission,
    /// <summary>
    /// You are missing permissions for this request/function to use.
    /// </summary>
    MissingUserPermission,
    /// <summary>
    /// You need to have global instance admin for this request/function to use.
    /// </summary>
    NotElevated,
    /// <summary>
    /// You need to have global instance admin for this request/function to use.
    /// </summary>
    NotPrivileged,
    /// <summary>
    /// You can't modify permissions that you don't have.
    /// </summary>
    CannotGiveMissingPermissions,
    /// <summary>
    /// You are not the owner of this group/server
    /// </summary>
    NotOwner,
    /// <summary>
    /// A Revolt instance database issue has occured.
    /// </summary>
    [EnumMember(Value = "DatabaseError")]
    RevoltDatabaseError,
    /// <summary>
    /// A Revolt instance internal issue has occured.
    /// </summary>
    [EnumMember(Value = "InternalError")]
    InternalServerError,
    /// <summary>
    /// You can't use thie request/function
    /// </summary>
    InvalidOperation,
    /// <summary>
    /// You are not logged in or authorized correctly.
    /// </summary>
    InvalidCredentials,
    /// <summary>
    /// Invalid object property for the Revolt instance.
    /// </summary>
    InvalidProperty,
    /// <summary>
    /// You are not logged in correctly.
    /// </summary>
    InvalidSession,
    /// <summary>
    /// Duplicate message nonce/unique message.
    /// </summary>
    DuplicateNonce,
    /// <summary>
    /// Voice server is unavailable.
    /// </summary>
    VosoUnavailable,
    /// <summary>
    /// This object is missing or not found.
    /// </summary>
    NotFound,
    /// <summary>
    /// This request/function has no effect.
    /// </summary>
    NoEffect,
    /// <summary>
    /// Failed validation for the request/function
    /// </summary>
    FailedValidation,
    /// <summary>
    /// You have not finished onboarding for the current user.
    /// </summary>
    OnboardingNotFinished,
    /// <summary>
    /// You are already authenticated.
    /// </summary>
    AlreadyAuthenticated
}