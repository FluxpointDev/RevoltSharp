using Optionals;

namespace RevoltSharp.Rest.Requests;

internal class CreateChannelRequest : RevoltRequest
{
    public string name;
    public Optional<string> type;
    public Optional<string> description;
    public Optional<string[]> users;
    public Optional<bool> nsfw;

}
