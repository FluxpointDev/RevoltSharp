namespace RevoltSharp.WebSocket.Events
{
    internal class MessageEventJson : MessageJson
    {
        internal Message ToEntity(RevoltClient client)
        {
            return Message.Create(client, this);
        }
    }
}
