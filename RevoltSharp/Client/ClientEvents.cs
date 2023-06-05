using System;

namespace RevoltSharp;

/// <summary>
/// Do not use this class! only used for <see cref="RevoltClient"/>
/// </summary>
public class ClientEvents
{
    /// <summary>
    /// Event used for RevoltSharp lib.
    /// </summary>
    public delegate void RevoltEvent();

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void SelfUserEvent<SelfUser>(SelfUser selfuser);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void SocketErrorEvent<SocketError>(SocketError error);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void MessageEvent<Message>(Message message);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void MessagesBulkDeletedEvent<Channel, Messages>(Channel channel,Messages messages);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void UserEvent<User>(User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void RoleEvent<Role>(Role role);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void RoleUpdatedEvent<OldRole, NewRole, Properties>(OldRole old_role, NewRole new_role, Properties updated_props);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void UserUpdatedEvent<OldUser, NewUser>(OldUser old_user, NewUser new_user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerEvent<Server>(Server server);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerEmojiEvent<Server, Emoji>(Server server, Emoji emoji);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerUserEvent<Server, User>(Server server, User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerMemberEvent<Server, Member>(Server server, Member member);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void MessageUpdatedEvent<Message>(Message message);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelMessageIdEvent<Channel, MessageId>(Channel channel, MessageId message_id);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelEvent<Channel>(Channel channel);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelUserEvent<Channel, User>(Channel channel, User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelUpdatedEvent<OldChannel, NewChannel, Properties>(OldChannel old_channel, NewChannel new_channel, Properties updated_props);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerUpdatedEvent<OldServer, NewServer, Properties>(OldServer old_server, NewServer new_server, Properties updated_props);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ReactionEvent<Emoji, Channel, UserCache, MessageCache>(Emoji emoji, Channel channel, UserCache user_cache, MessageCache message_cache);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ReactionBulkRemovedEvent<Emoji, Channel, MessageCache>(Emoji emoji, Channel channel, MessageCache message_cache);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void UserPlatformRemovedEvent<UserId, User>(UserId user_id, User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void LogEvent(string message, LogSeverity severity);

    #region WebSocket Events

    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> WebSocket has fully loaded with cached data and <see cref="RevoltClient.CurrentUser"/> is set.
    /// </summary>
    public event UserEvent<SelfUser>? OnReady;
    internal void InvokeReady(SelfUser user)
    {
        OnReady?.Invoke(user);
    }


    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> WebSocket has encountered an error.
    /// </summary>
    public event SocketErrorEvent<SocketError>? OnWebSocketError;
    internal void InvokeWebSocketError(SocketError error)
    {
        InvokeLog(error.Message, LogSeverity.Error);
        OnWebSocketError?.Invoke(error);
    }


    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> has loaded with the <see cref="RevoltClient.CurrentUser"/>.
    /// </summary>
    /// <remarks>
    /// You can use this with <see cref="ClientMode.Http" /> and <see cref="ClientMode.WebSocket" />
    /// </remarks>
    public event SelfUserEvent<SelfUser>? OnStarted;
    internal void InvokeStarted(SelfUser user)
    {
        OnStarted?.Invoke(user);
    }

    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> WebSocket has connected successfully.
    /// </summary>
    public event RevoltEvent? OnConnected;
    internal void InvokeConnected()
    {
        OnConnected?.Invoke();
    }

    #endregion

    #region Message Events

    /// <summary>
    /// Receive message events from websocket in a <see cref="TextChannel"/>, <seealso cref="GroupChannel"/>, <seealso cref="DMChannel"/> or <seealso cref="SavedMessagesChannel"/>
    /// </summary>
    public event MessageEvent<Message>? OnMessageRecieved;
    internal void InvokeMessageRecieved(Message msg)
    {
        OnMessageRecieved?.Invoke(msg);
    }

    /// <summary>
    /// Receive message updated event with properties of the updated message. (not last message sadly)
    /// </summary>
    public event MessageUpdatedEvent<MessageUpdatedProperties>? OnMessageUpdated;
    internal void InvokeMessageUpdated(MessageUpdatedProperties props)
    {
        OnMessageUpdated?.Invoke(props);
    }

    /// <summary>
    /// Receive message deleted event with the <see cref="Channel" /> and message id.
    /// </summary>
    public event ChannelMessageIdEvent<Channel, string>? OnMessageDeleted;
    internal void InvokeMessageDeleted(Channel chan, string msg)
    {
        OnMessageDeleted?.Invoke(chan, msg);
    }

    /// <summary>
    /// Receieve a list of deleted message ids with the <see cref="Channel" />.
    /// </summary>
    public event MessagesBulkDeletedEvent<Channel, string[]>? OnMessagesBulkDeleted;

    internal void InvokeMessagesBulkDeleted(Channel chan, string[] messages)
    {
        OnMessagesBulkDeleted?.Invoke(chan, messages);
    }

    #endregion

    #region Channel Events
    /// <summary>
    /// A <see cref="DMChannel" /> has been opened or become active again for the <see cref="User" />.
    /// </summary>

    public event ChannelEvent<DMChannel>? OnDMChannelOpened;
    internal void InvokeDMChannelOpened(DMChannel chan)
    {
        OnDMChannelOpened?.Invoke(chan);
    }

    /// <summary>
    /// A <see cref="ServerChannel" /> has been created in a <see cref="Server" />.
    /// </summary>
    public event ChannelEvent<ServerChannel>? OnChannelCreated;
    internal void InvokeChannelCreated(ServerChannel chan)
    {
        OnChannelCreated?.Invoke(chan);
    }

    /// <summary>
    /// A channel has been updated with <see cref="ChannelUpdatedProperties" />.
    /// </summary>
    public event ChannelUpdatedEvent<Channel, Channel, ChannelUpdatedProperties>? OnChannelUpdated;
    internal void InvokeChannelUpdated(Channel old, Channel newc, ChannelUpdatedProperties props)
    {
        OnChannelUpdated?.Invoke(old, newc, props);
    }

    /// <summary>
    /// A <see cref="Channel" /> has been been deleted, this does not include <see cref="DMChannel" /> or <see cref="SavedMessagesChannel" />
    /// </summary>
    public event ChannelEvent<Channel>? OnChannelDeleted;
    internal void InvokeChannelDeleted(Channel chan)
    {
        OnChannelDeleted?.Invoke(chan);
    }

    /// <summary>
    /// The current user/bot account has joined a <see cref="GroupChannel" />.
    /// </summary>
    public event ChannelUserEvent<GroupChannel, SelfUser>? OnGroupJoined;
    internal void InvokeGroupJoined(GroupChannel chan, SelfUser user)
    {
        OnGroupJoined?.Invoke(chan, user);
    }

    /// <summary>
    /// The current user/bot account has left a <see cref="GroupChannel" />.
    /// </summary>
    public event ChannelUserEvent<GroupChannel, SelfUser>? OnGroupLeft;
    internal void InvokeGroupLeft(GroupChannel chan, SelfUser user)
    {
        OnGroupLeft?.Invoke(chan, user);
    }

    /// <summary>
    /// A <see cref="User" /> has joined the <see cref="GroupChannel" />.
    /// </summary>
    public event ChannelUserEvent<GroupChannel, User>? OnGroupUserJoined;
    internal void InvokeGroupUserJoined(GroupChannel chan, User user)
    {
        OnGroupUserJoined?.Invoke(chan, user);
    }

    /// <summary>
    /// A <see cref="User" /> has left or been removed from the <see cref="GroupChannel" />
    /// </summary>
    public event ChannelUserEvent<GroupChannel, User>? OnGroupUserLeft;
    internal void InvokeGroupUserLeft(GroupChannel chan, User user)
    {
        OnGroupUserLeft?.Invoke(chan, user);
    }

    #endregion

    #region Server Events
    /// <summary>
    /// A <see cref="Server" /> has been updated with <see cref="ServerUpdatedProperties" />.
    /// </summary>

    public event ServerUpdatedEvent<Server, Server, ServerUpdatedProperties>? OnServerUpdated;
    internal void InvokeServerUpdated(Server old, Server news, ServerUpdatedProperties props)
    {
        OnServerUpdated?.Invoke(old, news, props);
    }

    /// <summary>
    /// The current user/bot account has joined a <see cref="Server" />.
    /// </summary>
    public event ServerUserEvent<Server, SelfUser>? OnServerJoined;
    internal void InvokeServerJoined(Server server, SelfUser user)
    {
        OnServerJoined?.Invoke(server, user);
    }

    /// <summary>
    /// The current user/bot account has left a <see cref="Server" />.
    /// </summary>
    public event ServerEvent<Server>? OnServerLeft;
    internal void InvokeServerLeft(Server server)
    {
        OnServerLeft?.Invoke(server);
    }

    /// <summary>
    /// A new <see cref="ServerMember" /> has joined the <see cref="Server" />.
    /// </summary>
    public event ServerMemberEvent<Server, ServerMember>? OnMemberJoined;
    internal void InvokeMemberJoined(Server server, ServerMember user)
    {
        OnMemberJoined?.Invoke(server, user);
    }

    /// <summary>
    /// A <see cref="ServerMember" /> has left the <see cref="Server" />
    /// </summary>

    public event ServerMemberEvent<Server, ServerMember>? OnMemberLeft;
    internal void InvokeMemberLeft(Server server, ServerMember user)
    {
        OnMemberLeft?.Invoke(server, user);
    }


    /// <summary>
    /// A new server <see cref="Role" /> has been created.
    /// </summary>
    public event RoleEvent<Role>? OnRoleCreated;
    internal void InvokeRoleCreated(Role role)
    {
        OnRoleCreated?.Invoke(role);
    }

    /// <summary>
    /// A server <see cref="Role" /> has been deleted.
    /// </summary>
    public event RoleEvent<Role>? OnRoleDeleted;
    internal void InvokeRoleDeleted(Role role)
    {
        OnRoleDeleted?.Invoke(role);
    }

    /// <summary>
    /// A server <see cref="Role" /> has been updated with <see cref="RoleUpdatedProperties" />.
    /// </summary>
    public event RoleUpdatedEvent<Role, Role, RoleUpdatedProperties>? OnRoleUpdated;
    internal void InvokeRoleUpdated(Role old, Role newr, RoleUpdatedProperties props)
    {
        OnRoleUpdated?.Invoke(old, newr, props);
    }

    /// <summary>
    /// A server <see cref="Emoji" /> has been created.
    /// </summary>
    public event ServerEmojiEvent<Server, Emoji>? OnEmojiCreated;

    internal void InvokeEmojiCreated(Server server, Emoji emoji)
    {
        OnEmojiCreated?.Invoke(server, emoji);
    }

    /// <summary>
    /// A server <see cref="Emoji" /> has been deleted.
    /// </summary>
    public event ServerEmojiEvent<Server, Emoji>? OnEmojiDeleted;

    internal void InvokeEmojiDeleted(Server server, Emoji emoji)
    {
        OnEmojiDeleted?.Invoke(server, emoji);
    }

    #endregion

    #region User Events

    /// <summary>
    /// A <see cref="User" /> has been updated.
    /// </summary>

    public event UserUpdatedEvent<User, User>? OnUserUpdated;
    internal void InvokeUserUpdated(User old, User newu)
    {
        OnUserUpdated?.Invoke(old, newu);
    }

    /// <summary>
    /// The current user/bot account has been updated.
    /// </summary>
    public event UserUpdatedEvent<SelfUser, SelfUser>? OnCurrentUserUpdated;
    internal void InvokeCurrentUserUpdated(SelfUser old, SelfUser newu)
    {
        OnCurrentUserUpdated?.Invoke(old, newu);
    }

    /// <summary>
    /// A <see cref="User" /> has been banned from the Revolt instance.
    /// </summary>
    /// <remarks>
    /// <see cref="User" /> may be null if not cached.
    /// </remarks>
    public event UserPlatformRemovedEvent<string, User?>? OnUserPlatformRemoved;

    internal void InvokeUserPlatformRemoved(string userid, User? user)
    {
        OnUserPlatformRemoved?.Invoke(userid, user);
    }

    #endregion

    #region Reaction Events

    /// <summary>
    /// A <see cref="Emoji" /> reaction has been added to a <see cref="Message" />.
    /// </summary>
    /// <remarks>
    /// Contains message id or <see cref="Message" /> that can be downloaded.
    /// </remarks>
    public event ReactionEvent<Emoji, Channel, Downloadable<string, User>, Downloadable<string, Message>>? OnReactionAdded;

    internal void InvokeReactionAdded(Emoji emoji, Channel channel, Downloadable<string, User> member, Downloadable<string, Message> messageDownload)
    {
        OnReactionAdded?.Invoke(emoji, channel, member, messageDownload);
    }

    /// <summary>
    /// A <see cref="Emoji" /> reaction has been removed from a <see cref="Message" />.
    /// </summary>
    /// <remarks>
    /// Contains message id or <see cref="Message" /> that can be downloaded.
    /// </remarks>
    public event ReactionEvent<Emoji, Channel, Downloadable<string, User>, Downloadable<string, Message>>? OnReactionRemoved;

    internal void InvokeReactionRemoved(Emoji emoji, Channel channel, Downloadable<string, User> member, Downloadable<string, Message> messageDownload)
    {
        OnReactionRemoved?.Invoke(emoji, channel, member, messageDownload);
    }

    /// <summary>
    /// All <see cref="Emoji" /> reactions have been removed from a <see cref="Message" />.
    /// </summary>
    /// <remarks>
    /// Contains message id or <see cref="Message" /> that can be downloaded.
    /// </remarks>
    public event ReactionBulkRemovedEvent<Emoji, Channel, Downloadable<string, Message>>? OnReactionBulkRemoved;

    internal void InvokeReactionBulkRemoved(Emoji emoji, Channel channel, Downloadable<string, Message> messageDownload)
    {
        OnReactionBulkRemoved?.Invoke(emoji, channel, messageDownload);
    }

    #endregion

    #region Log Event

    /// <summary>
    /// Called to display information, events, and errors originating from the <see cref="RevoltClient"/>.
    /// </summary>
    /// <remarks>By default, RevoltSharp will log its events to the <see cref="Console"/>. Adding a subscriber to this event overrides this behavior.</remarks>
    public event LogEvent? OnLog;

    internal void InvokeLog(string message, LogSeverity severity)
    {
        if (OnLog == null)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            switch (severity)
            {
                case LogSeverity.Verbose: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case LogSeverity.Standard: Console.ForegroundColor = ConsoleColor.Gray; break;
                case LogSeverity.Error: Console.ForegroundColor = ConsoleColor.Red; break;
            }
            Console.WriteLine($"[RevoltSharp] {message}"); // Default implementation
            Console.ForegroundColor = prevColor;
        }
        else
            OnLog.Invoke(message, severity);
    }

    internal void InvokeLogAndThrowException(string message)
    {
        InvokeLog(message, LogSeverity.Error);
        throw new RevoltException(message);
    }

    #endregion
}