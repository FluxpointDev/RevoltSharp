using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateChannelRequest : IRevoltRequest
{
    public string name { get; internal set; }
    public Optional<string> type { get; internal set; }
    public Optional<string> description { get; internal set; }
    public Optional<string[]> users { get; internal set; }
    public Optional<bool> nsfw { get; internal set; }

}
