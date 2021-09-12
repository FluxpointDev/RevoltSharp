using Optional;

namespace RevoltSharp.WebSocket.Events
{
    internal class ServerRoleUpdateEventJson
    {
        public string id;
        public string role_id;
        public PartialRoleJson data;
        public Option<string> clear;
    }
}
