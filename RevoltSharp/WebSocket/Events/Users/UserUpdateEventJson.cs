using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class UserUpdateEventJson
    {
        public string id;
        public PartialUserJson data;
        public Option<string> clear;
    }
}
