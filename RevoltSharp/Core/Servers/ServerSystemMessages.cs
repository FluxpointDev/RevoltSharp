using Optionals;

namespace RevoltSharp
{
    public class ServerSystemMessages
    {
        public Optional<string> UserJoined { get; set; }
        public Optional<string> UserLeft { get; set; }
        public Optional<string> UserKicked { get; set; }
        public Optional<string> UserBanned { get; set; }
    }
}
