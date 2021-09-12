namespace RevoltSharp.Rest.Requests
{
    internal class SendMessageRequest : RevoltRequest
    {
        public string content;
        public string nonce;
        public string[] attachments;
    }
}
