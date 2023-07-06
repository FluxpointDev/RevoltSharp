namespace RevoltSharp;

public class MessageWebhook : CreatedEntity
{
    internal MessageWebhook(RevoltClient client, MessageWebhookJson model) : base(client, model.Id)
    {
        Name = model.Name;
        Avatar = Attachment.Create(client, model.Avatar);
    }

    public string Id;

    public string Name;

    public Attachment? Avatar;
}