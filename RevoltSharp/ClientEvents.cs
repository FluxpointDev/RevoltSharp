namespace RevoltSharp
{
    /// <summary>
    /// Do not use this class! only used for <see cref="RevoltClient"/>
    /// </summary>
    public class ClientEvents
    {
        public delegate void RevoltEvent<TValue>(TValue value);
        public delegate void RevoltEvent<TValue, TValue2>(TValue value, TValue2 value2);

        /// <summary>
        /// Receive message events from websocket in a <see cref="TextChannel"/> or <seealso cref="GroupChannel"/>
        /// </summary>
        public event RevoltEvent<Message> OnMessageRecieved;
        internal void InvokeMessageRecieved(Message msg)
        {
            OnMessageRecieved?.Invoke(msg);
        }

        /// <summary>
        /// Event used when the <see cref="RevoltClient"/> WebSocket has fully loaded with cached data and <see cref="RevoltClient.CurrentUser"/> is set.
        /// </summary>
        public event RevoltEvent<User> OnReady;
        internal void InvokeReady(User user)
        {
            OnReady?.Invoke(user);
        }

        public event RevoltEvent<Message> OnMessageUpdated;
        internal void InvokeMessageUpdated(Message msg)
        {
            OnMessageUpdated?.Invoke(msg);
        }

        public event RevoltEvent<Channel, string> OnMessageDeleted;
        internal void InvokeMessageDeleted(Channel chan, string msg)
        {
            OnMessageDeleted?.Invoke(chan, msg);
        }

        public event RevoltEvent<Channel> OnChannelCreated;
        internal void InvokeChannelCreated(Channel chan)
        {
            OnChannelCreated?.Invoke(chan);
        }

        public event RevoltEvent<Channel, Channel> OnChannelUpdated;
        internal void InvokeChannelUpdated(Channel old, Channel newc)
        {
            OnChannelUpdated?.Invoke(old, newc);
        }

        public event RevoltEvent<Channel> OnChannelDeleted;
        internal void InvokeChannelDeleted(Channel chan)
        {
            OnChannelDeleted?.Invoke(chan);
        }


        public event RevoltEvent<GroupChannel, string> OnGroupJoined;
        internal void InvokeGroupJoined(GroupChannel chan, string user)
        {
            OnGroupJoined?.Invoke(chan, user);
        }

        public event RevoltEvent<GroupChannel, string> OnGroupLeft;
        internal void InvokeGroupLeft(GroupChannel chan, string user)
        {
            OnGroupLeft?.Invoke(chan, user);
        }


        public event RevoltEvent<Server, Server> OnServerUpdated;
        internal void InvokeServerUpdated(Server old, Server news)
        {
            OnServerUpdated?.Invoke(old, news);
        }

        public event RevoltEvent<Server> OnServerJoined;
        internal void InvokeServerJoined(Server server)
        {
            OnServerJoined?.Invoke(server);
        }

        public event RevoltEvent<Server> OnServerLeft;
        internal void InvokeServerLeft(Server server)
        {
            OnServerLeft?.Invoke(server);
        }

        public event RevoltEvent<Server, string> OnMemberJoined;
        internal void InvokeMemberJoined(Server server, string user)
        {
            OnMemberJoined?.Invoke(server, user);
        }

        public event RevoltEvent<Server, string> OnMemberLeft;
        internal void InvokeMemberLeft(Server server, string user)
        {
            OnMemberLeft?.Invoke(server, user);
        }

        public event RevoltEvent<Role> OnRoleCreated;
        internal void InvokeRoleCreated(Role role)
        {
            OnRoleCreated?.Invoke(role);
        }

        public event RevoltEvent<Role> OnRoleDeleted;
        internal void InvokeRoleDeleted(Role role)
        {
            OnRoleDeleted?.Invoke(role);
        }

        public event RevoltEvent<Role, Role> OnRoleUpdated;
        internal void InvokeRoleUpdated(Role old, Role newr)
        {
            OnRoleUpdated?.Invoke(old, newr);
        }
    }
}
