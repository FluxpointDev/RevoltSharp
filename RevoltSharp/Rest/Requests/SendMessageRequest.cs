using Optional;

namespace RevoltSharp.Rest.Requests
{
    internal class SendMessageRequest : RevoltRequest
    {
        public Option<string> content;
        public Option<string> nonce;
        public Option<string[]> attachments;
        public Option<EmbedJson[]> embeds;
        public Option<MessageMasqueradeJson> masquerade;
        public Option<MessageInteractionsJson> interactions;
    }
}
