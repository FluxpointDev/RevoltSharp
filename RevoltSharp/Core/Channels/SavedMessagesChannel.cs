namespace RevoltSharp;

internal class SavedMessagesChannel : Channel
{
    internal SavedMessagesChannel(RevoltClient client, ChannelJson model) : base(client, model)
    {
        Id = model.Id;
        Type = ChannelType.SavedMessages;
        User = model.User;
    }

    public string User { get; }

    internal override void Update(PartialChannelJson json)
    {
    }
}
