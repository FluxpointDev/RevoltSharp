using System;

namespace RevoltSharp;

/// <summary>
/// List of message flags.
/// </summary>
[Flags]
public enum MessageFlag : ulong
{
    None = 0,
    SupressNotifications = 1L << 0
}
