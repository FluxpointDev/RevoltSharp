using Optionals;

namespace RevoltSharp;

public class ServerSystemMessages
{
    public Optional<string> UserJoined { get; internal set; }
    public Optional<string> UserLeft { get; internal set; }
    public Optional<string> UserKicked { get; internal set; }
    public Optional<string> UserBanned { get; internal set; }
}
