using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class GetMessagesRequest : IRevoltRequest
{
    public int limit { get; internal set; } = 100;
    public Optional<string> before { get; internal set; }
    public Optional<string> after { get; internal set; }
    public string sort { get; internal set; } = "Latest";
    public bool include_users { get; internal set; } = false;
}
