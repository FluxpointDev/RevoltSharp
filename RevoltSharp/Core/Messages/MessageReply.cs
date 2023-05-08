namespace RevoltSharp;

public class MessageReply
{
	public MessageReply(string messageId, bool isMention)
	{
        MessageId = messageId;
        IsMention = isMention;

    }
    public string MessageId { get; }
    public bool IsMention { get; }
}
