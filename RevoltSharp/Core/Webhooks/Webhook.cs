namespace RevoltSharp;

/// <summary>
/// Revolt channel webhook to use with 3rd party services and APIs.
/// </summary>
public class Webhook : CreatedEntity
{
    internal Webhook(RevoltClient client, WebhookJson model) : base(client, model.Id)
    {
        Name = model.Name!;
        Avatar = Attachment.Create(Client, model.Avatar);
        ChannelId = model.ChannelId!;
        Permissions = new WebhookPermissions(model.Permissions ?? 0);
        Token = model.Token;
    }

    /// <summary>
    /// Name of the webhook.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// Avatar attachment of the webhook.
    /// </summary>
    public Attachment? Avatar { get; internal set; }

    /// <summary>
    /// Channel id of the webhook.
    /// </summary>
    public string ChannelId { get; internal set; }

    /// <summary>
    /// Channel of the webhook.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Channel? Channel => Client.GetChannel(ChannelId);

    /// <summary>
    /// Permissions the webhook can use.
    /// </summary>
    public WebhookPermissions Permissions { get; internal set; }

    /// <summary>
    /// The token used when sending messages as the webhook.
    /// </summary>
    public string? Token { get; internal set; }

    /// <summary>
    /// If the webhook object is authorized to use the token for requests.
    /// </summary>
    public bool HasToken => !string.IsNullOrEmpty(Token);

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Webhook name </returns>
    public override string ToString()
    {
        return Name;
    }
}