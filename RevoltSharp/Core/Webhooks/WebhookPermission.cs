using System;

namespace RevoltSharp;

/// <summary>
/// List of webhook permissions.
/// </summary>
[Flags]
public enum WebhookPermission : ulong
{
    /// <summary>
    /// Send messages permission.
    /// </summary>
    SendMessages = 1L << 22,
    /// <summary>
    /// Send embeds permission.
    /// </summary>
    SendEmbeds = 1L << 26,
    /// <summary>
    /// Masquerade messages permission.
    /// </summary>
    Masquerade = 1L << 28,
    /// <summary>
    /// Add reactions to messages permission.
    /// </summary>
    AddReactions = 1L << 29,
}
