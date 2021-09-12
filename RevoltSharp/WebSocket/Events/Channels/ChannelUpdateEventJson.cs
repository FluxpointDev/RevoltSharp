using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class ChannelUpdateEventJson
    {
        public string id;
        public PartialChannelJson data;
        public Option<string> clear;
    }
}
