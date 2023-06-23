namespace RevoltSharp;
public class MessageWebhook : CreatedEntity
{
    internal MessageWebhook(RevoltClient client, MessageWebhookJson model) : base(client, model.Id)
    {
        Name = model.Name;
        Avatar = model.Avatar != null ? new Attachment(client, model.Avatar) : null;
    }

    public string Id;

    public string Name;

    public Attachment? Avatar;
}
