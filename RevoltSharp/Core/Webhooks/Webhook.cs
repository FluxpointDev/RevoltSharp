namespace RevoltSharp;

public class Webhook : CreatedEntity
{
    internal Webhook(RevoltClient client, WebhookJson model) : base(client, model.Id)
    {
        Name = model.Name;
        Avatar = model.Avatar != null ? new Attachment(Client, model.Avatar) : null;
        ChannelId = model.ChannelId;
        Permissions = model.Permissions.HasValue ? model.Permissions.Value : 0;
        Token = model.Token;
    }

    public string Name;

    public Attachment? Avatar;

    public string ChannelId;

    public ulong Permissions;

    public string Token;
}
