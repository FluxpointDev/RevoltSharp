using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class SendMessageRequest : IRevoltRequest
{
    public Optional<string> content { get; internal set; }
    public Optional<string> nonce { get; internal set; }
    public Optional<string[]> attachments { get; internal set; }
    public Optional<EmbedJson[]> embeds { get; internal set; }
    public Optional<MessageMasqueradeJson> masquerade { get; internal set; }
    public Optional<MessageInteractionsJson> interactions { get; internal set; }
    public Optional<MessageReply[]> replies { get; internal set; }
}
