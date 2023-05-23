using System;

namespace RevoltSharp;

/// <summary>
/// System flags set for the user.
/// </summary>
public class UserFlags
{
    internal UserFlags(ulong value)
    {
        Raw = value;
        Types = (UserFlagType)Raw;
    }

    /// <summary>
    /// Not recommended to use, use <see cref="Has(UserFlagType)"/> instead.
    /// </summary>
    public ulong Raw { get; internal set; }

    /// <summary>
    /// Check if the user has a flag.
    /// </summary>
    /// <param name="type">The type of system flag to check</param>
    /// <returns><see langword="true" /> if user has the flag otherwise <see langword="false" /></returns>
    public bool Has(UserFlagType type) => Types.HasFlag(type);

    internal UserFlagType Types;
}

/// <summary>
/// System flags for the Revolt instance.
/// </summary>
[Flags]
public enum UserFlagType
{
    /// <summary>
    /// User has been suspended from using Revolt.
    /// </summary>
    Suspended = 1,

    /// <summary>
    /// User has been deleted from the Revolt instance.
    /// </summary>
    Deleted = 2,

    /// <summary>
    /// User has been banned from the Revolt instance.
    /// </summary>
    Banned = 4
}