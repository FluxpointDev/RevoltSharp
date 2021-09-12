namespace RevoltSharp.WebSocket.Events
{
    internal class ReadyEventJson
    {
        public UserJson[] users;
        public ServerJson[] servers;
        public ChannelJson[] channels;
        public ServerMemberJson[] members;
    }
    internal class ServerMemberJson
    {
        public string server;
        public string user;
        public string[] roles;
    }
}
