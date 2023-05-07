namespace RevoltSharp;

/// <summary>
/// Do not use this class! only used for <see cref="RevoltClient"/>
/// </summary>
public class ClientEvents
{
    public delegate void RevoltEvent();
    public delegate void SelfUserEvent<SelfUser>(SelfUser selfuser);
    public delegate void SocketErrorEvent<SocketError>(SocketError error);
    public delegate void MessageEvent<Message>(Message message);
    public delegate void UserEvent<User>(User user);
    public delegate void RoleEvent<Role>(Role role);
    public delegate void RoleUpdatedEvent<OldRole, NewRole, Properties>(OldRole old_role, NewRole new_role, Properties updated_props);
    public delegate void UserUpdatedEvent<OldUser, NewUser>(OldUser old_user, NewUser new_user);
    public delegate void ServerEvent<Server>(Server server);
    public delegate void ServerEmojiEvent<Server, Emoji>(Server server, Emoji emoji);
    public delegate void ServerUserEvent<Server, User>(Server server, User user);
    public delegate void ServerMemberEvent<Server, Member>(Server server, Member member);
    public delegate void MessageUpdatedEvent<Message>(Message message);
    public delegate void ChannelMessageIdEvent<Channel, MessageId>(Channel channel, MessageId message_id);
    public delegate void ChannelEvent<Channel>(Channel channel);
    public delegate void ChannelUserEvent<Channel, User>(Channel channel, User user);
    public delegate void ChannelUpdatedEvent<OldChannel, NewChannel, Properties>(OldChannel old_channel, NewChannel new_channel, Properties updated_props);
    public delegate void ServerUpdatedEvent<OldServer, NewServer, Properties>(OldServer old_server, NewServer new_server, Properties updated_props);
    public delegate void ReactionEvent<Emoji, Channel, UserCache, MessageCache>(Emoji emoji, Channel channel, UserCache user_cache, MessageCache message_cache);

    /// <summary>
    /// Receive message events from websocket in a <see cref="TextChannel"/> or <seealso cref="GroupChannel"/>
    /// </summary>
    public event MessageEvent<Message> OnMessageRecieved;
    internal void InvokeMessageRecieved(Message msg)
    {
        OnMessageRecieved?.Invoke(msg);
    }

    
    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> WebSocket has fully loaded with cached data and <see cref="RevoltClient.CurrentUser"/> is set.
    /// </summary>
    public event UserEvent<SelfUser> OnReady;
    internal void InvokeReady(SelfUser user)
    {
        OnReady?.Invoke(user);
    }

    
    public event MessageUpdatedEvent<MessageUpdatedProperties> OnMessageUpdated;
    internal void InvokeMessageUpdated(MessageUpdatedProperties props)
    {
        OnMessageUpdated?.Invoke(props);
    }

    public event ChannelMessageIdEvent<Channel, string> OnMessageDeleted;
    internal void InvokeMessageDeleted(Channel chan, string msg)
    {
        OnMessageDeleted?.Invoke(chan, msg);
    }

    public event ChannelEvent<DMChannel> OnDMChannelOpened;
    internal void InvokeDMChannelOpened(DMChannel chan)
    {
        OnDMChannelOpened?.Invoke(chan);
    }

    public event ChannelEvent<ServerChannel> OnChannelCreated;
    internal void InvokeChannelCreated(ServerChannel chan)
    {
        OnChannelCreated?.Invoke(chan);
    }

    
    public event ChannelUpdatedEvent<Channel, Channel, ChannelUpdatedProperties> OnChannelUpdated;
    internal void InvokeChannelUpdated(Channel old, Channel newc, ChannelUpdatedProperties props)
    {
        OnChannelUpdated?.Invoke(old, newc, props);
    }

    public event ChannelEvent<Channel> OnChannelDeleted;
    internal void InvokeChannelDeleted(Channel chan)
    {
        OnChannelDeleted?.Invoke(chan);
    }


    public event ChannelUserEvent<GroupChannel, SelfUser> OnGroupJoined;
    internal void InvokeGroupJoined(GroupChannel chan, SelfUser user)
    {
        OnGroupJoined?.Invoke(chan, user);
    }

    public event ChannelUserEvent<GroupChannel, SelfUser> OnGroupLeft;
    internal void InvokeGroupLeft(GroupChannel chan, SelfUser user)
    {
        OnGroupLeft?.Invoke(chan, user);
    }

    public event ChannelUserEvent<GroupChannel, User> OnGroupUserJoined;
    internal void InvokeGroupUserJoined(GroupChannel chan, User user)
    {
        OnGroupUserJoined?.Invoke(chan, user);
    }

    public event ChannelUserEvent<GroupChannel, User> OnGroupUserLeft;
    internal void InvokeGroupUserLeft(GroupChannel chan, User user)
    {
        OnGroupUserLeft?.Invoke(chan, user);
    }


    public event ServerUpdatedEvent<Server, Server, ServerUpdatedProperties> OnServerUpdated;
    internal void InvokeServerUpdated(Server old, Server news, ServerUpdatedProperties props)
    {
        OnServerUpdated?.Invoke(old, news, props);
    }

    public event ServerUserEvent<Server, SelfUser> OnServerJoined;
    internal void InvokeServerJoined(Server server, SelfUser user)
    {
        OnServerJoined?.Invoke(server, user);
    }

    public event ServerEvent<Server> OnServerLeft;
    internal void InvokeServerLeft(Server server)
    {
        OnServerLeft?.Invoke(server);
    }

    public event ServerMemberEvent<Server, ServerMember> OnMemberJoined;
    internal void InvokeMemberJoined(Server server, ServerMember user)
    {
        OnMemberJoined?.Invoke(server, user);
    }

    public event ServerMemberEvent<Server, ServerMember> OnMemberLeft;
    internal void InvokeMemberLeft(Server server, ServerMember user)
    {
        OnMemberLeft?.Invoke(server, user);
    }

    public event RoleEvent<Role> OnRoleCreated;
    internal void InvokeRoleCreated(Role role)
    {
        OnRoleCreated?.Invoke(role);
    }

    public event RoleEvent<Role> OnRoleDeleted;
    internal void InvokeRoleDeleted(Role role)
    {
        OnRoleDeleted?.Invoke(role);
    }

    public event RoleUpdatedEvent<Role, Role, RoleUpdatedProperties> OnRoleUpdated;
    internal void InvokeRoleUpdated(Role old, Role newr, RoleUpdatedProperties props)
    {
        OnRoleUpdated?.Invoke(old, newr, props);
    }

    public event SocketErrorEvent<SocketError> OnWebSocketError;
    internal void InvokeWebSocketError(SocketError error)
    {
        OnWebSocketError?.Invoke(error);
    }

    public event SelfUserEvent<SelfUser> OnStarted;
    internal void InvokeStarted(SelfUser user)
    {
        OnStarted?.Invoke(user);
    }

    public event RevoltEvent OnConnected;
    internal void InvokeConnected() {
        OnConnected?.Invoke();
    }

    public event UserUpdatedEvent<User, User> OnUserUpdated;
    internal void InvokeUserUpdated(User old, User newu)
    {
        OnUserUpdated?.Invoke(old, newu);
    }

    public event UserUpdatedEvent<SelfUser, SelfUser> OnCurrentUserUpdated;
    internal void InvokeCurrentUserUpdated(SelfUser old, SelfUser newu)
    {
        OnCurrentUserUpdated?.Invoke(old, newu);
    }

    public event ServerEmojiEvent<Server, Emoji> OnEmojiCreated;

    internal void InvokeEmojiCreated(Server server, Emoji emoji)
    {
        OnEmojiCreated?.Invoke(server, emoji);
    }

    public event ServerEmojiEvent<Server, Emoji> OnEmojiDeleted;

    internal void InvokeEmojiDeleted(Server server, Emoji emoji)
    {
        OnEmojiDeleted?.Invoke(server, emoji);
    }

    public event ReactionEvent<Emoji, Channel, Downloadable<string, User>, Downloadable<string, Message>> OnReactionAdded;

    internal void InvokeReactionAdded(Emoji emoji, Channel channel, Downloadable<string, User> member, Downloadable<string, Message> messageDownload)
    {
        OnReactionAdded?.Invoke(emoji, channel, member, messageDownload);
    }

    public event ReactionEvent<Emoji, Channel, Downloadable<string, User>, Downloadable<string, Message>> OnReactionRemoved;

    internal void InvokeReactionRemoved(Emoji emoji, Channel channel, Downloadable<string, User> member, Downloadable<string, Message> messageDownload)
    {
        OnReactionRemoved?.Invoke(emoji, channel, member, messageDownload);
    }
}
