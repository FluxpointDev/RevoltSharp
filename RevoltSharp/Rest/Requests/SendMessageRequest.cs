using Optionals;

namespace RevoltSharp.Rest.Requests
{
    internal class SendMessageRequest : RevoltRequest
    {
        public Optional<string> content;
        public Optional<string> nonce;
        public Optional<string[]> attachments;
        public Optional<EmbedJson[]> embeds;
        public Optional<MessageMasqueradeJson> masquerade;
        public Optional<MessageInteractionsJson> interactions;
        public Optional<MessageReply[]> replies;
    }
}
