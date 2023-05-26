using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class GetMessagesRequest : IRevoltRequest
{
    public int limit = 100;
    public Optional<string> before;
    public Optional<string> after;
    public string sort = "Latest";
    public bool include_users = false;
}
