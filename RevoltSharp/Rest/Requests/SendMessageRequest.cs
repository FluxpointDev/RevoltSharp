using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class SendMessageRequest : IRevoltRequest
{
    public Optional<string> content { get; set; }
    public string nonce { get; set; }
    public Optional<string[]> attachments { get; set; }
    public Optional<EmbedJson[]> embeds { get; set; }
    public Optional<MessageMasqueradeJson> masquerade { get; set; }
    public Optional<MessageInteractionsJson> interactions { get; set; }
    public Optional<MessageReplyJson[]> replies { get; set; }
}
