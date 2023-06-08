using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class GetMessagesRequest : IRevoltRequest
{
    public int limit { get; set; } = 100;
    public Optional<string> before { get; set; }
    public Optional<string> after { get; set; }
    public string sort { get; set; } = "Latest";
    public bool include_users { get; set; } = false;
}
