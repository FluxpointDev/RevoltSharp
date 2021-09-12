using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerUpdateEventJson
    {
        public string id;
        public PartialServerJson data;
        public Option<string> clear;
    }
}
