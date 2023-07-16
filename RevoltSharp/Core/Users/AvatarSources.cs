using System;

namespace RevoltSharp;


[Flags]
public enum AvatarSources
{
    /// <summary>
    /// Default Revolt avatar given to the user.
    /// </summary>
    Default = 1,

    /// <summary>
    /// The user's own avatar set.
    /// </summary>
    User = 2,

    /// <summary>
    /// The user's own server avatar set.
    /// </summary>
    Server = 4,

    /// <summary>
    /// The user's own avatar or fallback to default avatar.
    /// </summary>
    UserOrDefault = Default | User,

    /// <summary>
    /// The user's own server avatar or fallback to user avatar.
    /// </summary>
    ServerOrUser = User | Server,

    /// <summary>
    /// Use any of the server avatar, user avatar or default avatar.
    /// </summary>
    Any = Server | User | Default,
}