using Optionals;

namespace RevoltSharp.Rest.Requests;

public class GetMessagesRequest : RevoltRequest
{
    public int limit = 100;
    public Optional<string> before;
    public Optional<string> after;
    public string sort = "Latest";
    public bool include_users = false;
}
