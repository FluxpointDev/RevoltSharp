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
    public delegate void SelfUserEvent(SelfUser selfuser);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void SocketErrorEvent(SocketError error);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void MessageEvent(Message message);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void MessagesBulkDeletedEvent(Channel channel, string[] messages);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void UserEvent(User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void RoleEvent(Role role);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void RoleUpdatedEvent(Role old_role, Role new_role, RoleUpdatedProperties properties);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void UserUpdatedEvent(User old_user, User new_user, UserUpdatedProperties properties);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void SelfUserUpdatedEvent(SelfUser old_user, SelfUser new_user, SelfUserUpdatedProperties properties);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerEvent(Server server);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerEmojiEvent(Server server, Emoji emoji);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerUserEvent(Server server, User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerSelfEvent(Server server, SelfUser user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerMemberEvent(Server server, ServerMember member);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerMemberLeftEvent(Server server, string userId, User? user, ServerMember? member);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void MessageUpdatedEvent(Downloadable<string, Message> message_cache, MessageUpdatedProperties message);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelMessageIdEvent(Channel channel, string message_id);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelEvent(Channel channel);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelUserEvent(Channel channel, User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void GroupChannelSelfEvent(GroupChannel channel, SelfUser user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void GroupChannelUserEvent(GroupChannel channel, User user);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ChannelUpdatedEvent(Channel old_channel, Channel new_channel, ChannelUpdatedProperties properties);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ServerUpdatedEvent(Server old_server, Server new_server, ServerUpdatedProperties properties);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ReactionEvent(Emoji emoji, Channel channel, Downloadable<string, User> user_cache, Downloadable<string, Message> message_cache);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void ReactionBulkRemovedEvent(Emoji emoji, Channel channel, Downloadable<string, Message> message_cache);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void UserPlatformRemovedEvent(string user_id, User user, UserFlags flags);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void WebhookEvent(Webhook webhook);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void UserRelationshipUpdated(User user, UserRelationship relationship);

    /// <inheritdoc cref="RevoltEvent" />
    public delegate void LogEvent(string message, RevoltLogSeverity severity);

    #region WebSocket Events

    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> WebSocket has fully loaded with cached data and <see cref="RevoltClient.CurrentUser"/> is set.
    /// </summary>
    public event SelfUserEvent? OnReady;
    internal void InvokeReady(SelfUser user)
    {
        OnReady?.Invoke(user);
    }


    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> WebSocket has encountered an error.
    /// </summary>
    public event SocketErrorEvent? OnWebSocketError;
    internal void InvokeWebSocketError(RevoltClient client, SocketError error)
    {
        client.InvokeLog(error.Message, RevoltLogSeverity.Error);
        OnWebSocketError?.Invoke(error);
    }


    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> has loaded with the <see cref="RevoltClient.CurrentUser"/>.
    /// </summary>
    /// <remarks>
    /// You can use this with <see cref="ClientMode.Http" /> and <see cref="ClientMode.WebSocket" />
    /// </remarks>
    public event SelfUserEvent? OnStarted;
    internal void InvokeStarted(SelfUser user)
    {
        OnStarted?.Invoke(user);
    }

    /// <summary>
    /// Event used when the <see cref="RevoltClient"/> has logged in with the <see cref="RevoltClient.CurrentUser"/>.
    /// </summary>
    /// <remarks>
    /// You can use this with <see cref="ClientMode.Http" /> and <see cref="ClientMode.WebSocket" />
    /// </remarks>
    public event SelfUserEvent? OnLogin;
    internal void InvokeLogin(SelfUser user)
    {
        OnLogin?.Invoke(user);
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
    public event MessageEvent? OnMessageRecieved;
    internal void InvokeMessageRecieved(Message msg)
    {
        OnMessageRecieved?.Invoke(msg);
    }

    /// <summary>
    /// Receive message updated event with properties of the updated message. (not last message sadly)
    /// </summary>
    public event MessageUpdatedEvent? OnMessageUpdated;
    internal void InvokeMessageUpdated(Downloadable<string, Message> message_cache, MessageUpdatedProperties props)
    {
        OnMessageUpdated?.Invoke(message_cache, props);
    }

    /// <summary>
    /// Receive message deleted event with the <see cref="Channel" /> and message id.
    /// </summary>
    public event ChannelMessageIdEvent? OnMessageDeleted;
    internal void InvokeMessageDeleted(Channel chan, string msg)
    {
        OnMessageDeleted?.Invoke(chan, msg);
    }

    /// <summary>
    /// Receieve a list of deleted message ids with the <see cref="Channel" />.
    /// </summary>
    public event MessagesBulkDeletedEvent? OnMessagesBulkDeleted;

    internal void InvokeMessagesBulkDeleted(Channel chan, string[] messages)
    {
        OnMessagesBulkDeleted?.Invoke(chan, messages);
    }

    #endregion

    #region Channel Events
    /// <summary>
    /// A <see cref="DMChannel" /> has been opened or become active again for the <see cref="User" />.
    /// </summary>

    public event ChannelEvent? OnDMChannelOpened;
    internal void InvokeDMChannelOpened(DMChannel chan)
    {
        OnDMChannelOpened?.Invoke(chan);
    }

    /// <summary>
    /// A <see cref="ServerChannel" /> has been created in a <see cref="Server" />.
    /// </summary>
    public event ChannelEvent? OnChannelCreated;
    internal void InvokeChannelCreated(ServerChannel chan)
    {
        OnChannelCreated?.Invoke(chan);
    }

    /// <summary>
    /// A channel has been updated with <see cref="ChannelUpdatedProperties" />.
    /// </summary>
    public event ChannelUpdatedEvent? OnChannelUpdated;
    internal void InvokeChannelUpdated(Channel old, Channel newc, ChannelUpdatedProperties props)
    {
        OnChannelUpdated?.Invoke(old, newc, props);
    }

    /// <summary>
    /// A <see cref="Channel" /> has been been deleted, this does not include <see cref="DMChannel" /> or <see cref="SavedMessagesChannel" />
    /// </summary>
    public event ChannelEvent? OnChannelDeleted;
    internal void InvokeChannelDeleted(Channel chan)
    {
        OnChannelDeleted?.Invoke(chan);
    }

    /// <summary>
    /// The current user/bot account has joined a <see cref="GroupChannel" />.
    /// </summary>
    public event GroupChannelSelfEvent? OnGroupJoined;
    internal void InvokeGroupJoined(GroupChannel chan, SelfUser user)
    {
        OnGroupJoined?.Invoke(chan, user);
    }

    /// <summary>
    /// The current user/bot account has left a <see cref="GroupChannel" />.
    /// </summary>
    public event GroupChannelSelfEvent? OnGroupLeft;
    internal void InvokeGroupLeft(GroupChannel chan, SelfUser user)
    {
        OnGroupLeft?.Invoke(chan, user);
    }

    /// <summary>
    /// A <see cref="User" /> has joined the <see cref="GroupChannel" />.
    /// </summary>
    public event GroupChannelUserEvent? OnGroupUserJoined;
    internal void InvokeGroupUserJoined(GroupChannel chan, User user)
    {
        OnGroupUserJoined?.Invoke(chan, user);
    }

    /// <summary>
    /// A <see cref="User" /> has left or been removed from the <see cref="GroupChannel" />
    /// </summary>
    public event GroupChannelUserEvent? OnGroupUserLeft;
    internal void InvokeGroupUserLeft(GroupChannel chan, User user)
    {
        OnGroupUserLeft?.Invoke(chan, user);
    }

    #endregion

    #region Server Events
    /// <summary>
    /// A <see cref="Server" /> has been updated with <see cref="ServerUpdatedProperties" />.
    /// </summary>

    public event ServerUpdatedEvent? OnServerUpdated;
    internal void InvokeServerUpdated(Server old, Server news, ServerUpdatedProperties props)
    {
        OnServerUpdated?.Invoke(old, news, props);
    }

    /// <summary>
    /// The current user/bot account has joined a <see cref="Server" />.
    /// </summary>
    public event ServerSelfEvent? OnServerJoined;
    internal void InvokeServerJoined(Server server, SelfUser user)
    {
        OnServerJoined?.Invoke(server, user);
    }

    /// <summary>
    /// The current user/bot account has left a <see cref="Server" />.
    /// </summary>
    public event ServerEvent? OnServerLeft;
    internal void InvokeServerLeft(Server server)
    {
        OnServerLeft?.Invoke(server);
    }

    /// <summary>
    /// A new <see cref="ServerMember" /> has joined the <see cref="Server" />.
    /// </summary>
    public event ServerMemberEvent? OnMemberJoined;
    internal void InvokeMemberJoined(Server server, ServerMember user)
    {
        OnMemberJoined?.Invoke(server, user);
    }

    /// <summary>
    /// A <see cref="ServerMember" /> has left the <see cref="Server" />
    /// </summary>

    public event ServerMemberLeftEvent? OnMemberLeft;
    internal void InvokeMemberLeft(Server server, string userId, User? user, ServerMember? member)
    {
        OnMemberLeft?.Invoke(server, userId, user, member);
    }


    /// <summary>
    /// A new server <see cref="Role" /> has been created.
    /// </summary>
    public event RoleEvent? OnRoleCreated;
    internal void InvokeRoleCreated(Role role)
    {
        OnRoleCreated?.Invoke(role);
    }

    /// <summary>
    /// A server <see cref="Role" /> has been deleted.
    /// </summary>
    public event RoleEvent? OnRoleDeleted;
    internal void InvokeRoleDeleted(Role role)
    {
        OnRoleDeleted?.Invoke(role);
    }

    /// <summary>
    /// A server <see cref="Role" /> has been updated with <see cref="RoleUpdatedProperties" />.
    /// </summary>
    public event RoleUpdatedEvent? OnRoleUpdated;
    internal void InvokeRoleUpdated(Role old, Role newr, RoleUpdatedProperties props)
    {
        OnRoleUpdated?.Invoke(old, newr, props);
    }

    /// <summary>
    /// A server <see cref="Emoji" /> has been created.
    /// </summary>
    public event ServerEmojiEvent? OnEmojiCreated;

    internal void InvokeEmojiCreated(Server server, Emoji emoji)
    {
        OnEmojiCreated?.Invoke(server, emoji);
    }

    /// <summary>
    /// A server <see cref="Emoji" /> has been deleted.
    /// </summary>
    public event ServerEmojiEvent? OnEmojiDeleted;

    internal void InvokeEmojiDeleted(Server server, Emoji emoji)
    {
        OnEmojiDeleted?.Invoke(server, emoji);
    }

    #endregion

    #region User Events

    /// <summary>
    /// A <see cref="User" /> has been updated.
    /// </summary>

    public event UserUpdatedEvent? OnUserUpdated;
    internal void InvokeUserUpdated(User old, User newu, UserUpdatedProperties props)
    {
        OnUserUpdated?.Invoke(old, newu, props);
    }

    /// <summary>
    /// The current user/bot account has been updated.
    /// </summary>
    public event SelfUserUpdatedEvent? OnCurrentUserUpdated;
    internal void InvokeCurrentUserUpdated(SelfUser old, SelfUser newu, SelfUserUpdatedProperties props)
    {
        OnCurrentUserUpdated?.Invoke(old, newu, props);
    }

    /// <summary>
    /// A <see cref="User" /> has been banned from the Revolt instance.
    /// </summary>
    /// <remarks>
    /// <see cref="User" /> may be null if not cached.
    /// </remarks>
    public event UserPlatformRemovedEvent? OnUserPlatformRemoved;

    internal void InvokeUserPlatformRemoved(string userid, User? user, UserFlags flags)
    {
        OnUserPlatformRemoved?.Invoke(userid, user, flags);
    }

    #endregion

    /// <summary>
    /// A <see cref="Webhook" /> has been created in a channel.
    /// </summary>
    /// <remarks>
    /// <see cref="Webhook.Token" /> will always be empty.
    /// </remarks>
    public event WebhookEvent? OnWebhookCreated;

    internal void InvokeWebhookCreated(Webhook webhook)
    {
        OnWebhookCreated?.Invoke(webhook);
    }

    /// <summary>
    /// The current client has logged out.
    /// </summary>
    public event RevoltEvent? OnClientLogout;

    internal void InvokeLogout()
    {
        OnClientLogout?.Invoke();
    }

    /// <summary>
    /// A relationship for the current client has changed such as Blocked.
    /// </summary>
    public event UserRelationshipUpdated? OnUserRelationshipUpdated;

    internal void InvokeUserRelationshipUpdated(User user, UserRelationship relationship)
    {
        OnUserRelationshipUpdated?.Invoke(user, relationship);
    }

    #region Reaction Events

    /// <summary>
    /// A <see cref="Emoji" /> reaction has been added to a <see cref="Message" />.
    /// </summary>
    /// <remarks>
    /// Contains message id or <see cref="Message" /> that can be downloaded.
    /// </remarks>
    public event ReactionEvent? OnReactionAdded;

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
    public event ReactionEvent? OnReactionRemoved;

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
    public event ReactionBulkRemovedEvent? OnReactionBulkRemoved;

    internal void InvokeReactionBulkRemoved(Emoji emoji, Channel channel, Downloadable<string, Message> messageDownload)
    {
        OnReactionBulkRemoved?.Invoke(emoji, channel, messageDownload);
    }

    #endregion


}