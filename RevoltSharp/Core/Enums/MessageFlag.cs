using System;

namespace RevoltSharp;

/// <summary>
/// List of message flags.
/// </summary>
[Flags]
public enum MessageFlag : ulong
{
    /// <summary>
    /// Message has no flags.
    /// </summary>
    None = 0,
    /// <summary>
    /// Message has supressed notifications for other users.
    /// </summary>
    SupressNotifications = 1L << 0
}
