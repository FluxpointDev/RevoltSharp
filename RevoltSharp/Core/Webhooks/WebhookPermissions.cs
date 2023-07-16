namespace RevoltSharp;

/// <summary>
/// Permissions for the webhook that can be used.
/// </summary>
public class WebhookPermissions
{
    internal WebhookPermissions(ulong rawPermission)
    {
        Raw = rawPermission;
    }

    /// <summary>
    /// Raw permissions number for the webhook.
    /// </summary>
    public ulong Raw { get; internal set; }

    /// <summary>
    /// The webhook can send messages.
    /// </summary>
    public bool SendMessages => Has(WebhookPermission.SendMessages);

    /// <summary>
    /// The webhook can send embeds.
    /// </summary>
    public bool SendEmbeds => Has(WebhookPermission.SendEmbeds);

    /// <summary>
    /// The webhook can modify the message author and avatar sending the message.
    /// </summary>
    public bool Masquerade => Has(WebhookPermission.Masquerade);

    /// <summary>
    /// The webhook can add reactions to its own messages.
    /// </summary>
    public bool AddReactions => Has(WebhookPermission.AddReactions);

    /// <summary>
    /// Check if the webhook has a specific permission.
    /// </summary>
    /// <returns><see langword="bool" /></returns>
    public bool Has(WebhookPermission permission)
    {
        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
