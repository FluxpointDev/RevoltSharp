namespace RevoltSharp;


public class SavedMessagesChannel : Channel
{
    internal SavedMessagesChannel(RevoltClient client, ChannelJson model) : base(client, model)
    {
        Type = ChannelType.SavedMessages;
        LastMessageId = model.LastMessageId;
    }

    /// <summary>
    /// The last message id sent in this DM channel.
    /// </summary>
    public string? LastMessageId { get; internal set; }

    internal override void Update(PartialChannelJson json)
    {

    }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> CurrentUser#0001 </returns>
    public override string ToString()
    {
        return Client.CurrentUser.Tag;
    }
}
