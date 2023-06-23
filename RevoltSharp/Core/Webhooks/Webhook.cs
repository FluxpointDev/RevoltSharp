namespace RevoltSharp;

public class Webhook : CreatedEntity
{
    internal Webhook(RevoltClient client, WebhookJson model) : base(client, model.Id)
    {
        Name = model.Name;
        Avatar = Attachment.Create(Client, model.Avatar);
        ChannelId = model.ChannelId;
        Permissions = model.Permissions.HasValue ? model.Permissions.Value : 0;
        Token = model.Token;
    }

    public string Name { get; internal set; }

    public Attachment? Avatar { get; internal set; }

	public string ChannelId { get; internal set; }

	public Channel? Channel => Client.GetChannel(ChannelId); 

    public ulong Permissions { get; internal set; }

	public string Token { get; internal set; }
}
