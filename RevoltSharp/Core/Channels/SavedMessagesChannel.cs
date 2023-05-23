namespace RevoltSharp;

public class SavedMessagesChannel : Channel
{
    internal SavedMessagesChannel(RevoltClient client, ChannelJson model) : base(client, model)
    {
        Id = model.Id;
        Type = ChannelType.SavedMessages;
    }

    internal override void Update(PartialChannelJson json)
    {

    }
}
