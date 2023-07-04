namespace RevoltSharp
{

    public class MessageReply
    {
        public MessageReply(string messageId, bool isMention)
        {
            MessageId = messageId;
            IsMention = isMention;
        }

        public string MessageId { get; internal set; }
        public bool IsMention { get; internal set; }

        internal MessageReplyJson ToJson()
        {
            return new MessageReplyJson
            {
                messageId = MessageId,
                isMention = IsMention
            };
        }
    }
}