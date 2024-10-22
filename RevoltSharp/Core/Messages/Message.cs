using System;
using System.Linq;

namespace RevoltSharp;


/// <summary>
/// Base chat messages that can be casted to <see cref="UserMessage"/> or <see cref="SystemMessage{Type}"/>
/// </summary>
public abstract class Message : CreatedEntity
{
    internal Message(RevoltClient client, MessageJson model, UserJson[]? users = null, ServerMemberJson[]? members = null)
        : base(client, model.MessageId)
    {
        ChannelId = model.ChannelId;
        if (model.Webhook != null)
        {
            Type = MessageType.Webhook;
            AuthorId = model.Webhook.Id;
            Author = new User(Client, model.Webhook);
        }
        else
        {
            AuthorId = model.AuthorId;
            if (users != null)
                Author = new User(client, users.FirstOrDefault(x => x.Id == AuthorId));
            else
                Author = client.GetUser(model.AuthorId);

            if (Author != null)
                Type = Author.IsBot ? MessageType.Bot : MessageType.User;

        }
        Channel = client.GetChannel(model.ChannelId);

        if (Channel != null && Channel is ServerChannel SC)
        {
            ServerId = SC.ServerId;
            if (client.WebSocket != null && model.AuthorId != User.SystemUserId)
            {
                if (Server.InternalMembers.TryGetValue(model.AuthorId, out var member))
                    Member = member;
                else if (members != null)
                    Member = new ServerMember(client, members.FirstOrDefault(x => x.Id.User == AuthorId), null, Author);
            }
        }
    }

    internal static Message Create(RevoltClient client, MessageJson model, UserJson[]? users = null, ServerMemberJson[]? members = null)
    {
        if (model.AuthorId == User.SystemUserId)
        {
            if (model.System != null)
            {
                switch (model.System.SystemType)
                {
                    case "text":
                        return new SystemMessage<SystemDataText>(client, model, new SystemDataText(), SystemType.Text);
                    case "user_added":
                        return new SystemMessage<SystemDataUserAdded>(client, model, new SystemDataUserAdded(), SystemType.GroupUserAdded);
                    case "user_remove":
                        return new SystemMessage<SystemDataUserRemoved>(client, model, new SystemDataUserRemoved(), SystemType.GroupUserRemoved);
                    case "user_joined":
                        return new SystemMessage<SystemDataUserJoined>(client, model, new SystemDataUserJoined(), SystemType.ServerUserJoined);
                    case "user_left":
                        return new SystemMessage<SystemDataUserLeft>(client, model, new SystemDataUserLeft(), SystemType.ServerUserLeft);
                    case "user_kicked":
                        return new SystemMessage<SystemDataUserKicked>(client, model, new SystemDataUserKicked(), SystemType.ServerUserKicked);
                    case "user_banned":
                        return new SystemMessage<SystemDataUserBanned>(client, model, new SystemDataUserBanned(), SystemType.ServerUserBanned);
                    case "channel_renamed":
                        return new SystemMessage<SystemDataChannelRenamed>(client, model, new SystemDataChannelRenamed(), SystemType.GroupNameChanged);
                    case "channel_description_changed":
                        return new SystemMessage<SystemDataChannelDescriptionChanged>(client, model, new SystemDataChannelDescriptionChanged(), SystemType.GroupDescriptionChanged);
                    case "channel_icon_changed":
                        return new SystemMessage<SystemDataChannelIconChanged>(client, model, new SystemDataChannelIconChanged(), SystemType.GroupIconChanged);
                    case "channel_ownership_changed":
                        return new SystemMessage<SystemDataChannelOwnershipChanged>(client, model, new SystemDataChannelOwnershipChanged(), SystemType.GroupOwnerChanged);
                }
            }
            return new SystemMessage<SystemDataUnknown>(client, model, new SystemDataUnknown(), SystemType.Unknown);
        }

        return new UserMessage(client, model, users, members);
    }

    /// <summary>
    /// Id of the message.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Content of the message.
    /// </summary>
    public string? Content { get; internal set; }

    /// <summary>
    /// Date of when the message was created.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

    /// <summary>
    /// Parent channel id of the message
    /// </summary>
    public string ChannelId { get; internal set; }

    /// <summary>
    /// Channel that the Message is from.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Channel? Channel { get; internal set; }

    public string? ServerId { get; internal set; }

    /// <summary>
    /// Server that the Message is from.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/> or invalid channel context.
    /// </remarks>
    public Server? Server => Client.GetServer(ServerId);

    /// <summary>
    /// Id of the user who posted the message
    /// </summary>
    public string AuthorId { get; internal set; }

    /// <summary>
    /// User who posted the message
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if client is rest mode or system/webhook messages.
    /// </remarks>
    public User? Author { get; internal set; }

    /// <summary>
    /// Member who posted the message
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if client is rest mode or system/webhook messages.
    /// </remarks>
    public ServerMember? Member { get; internal set; }

    /// <summary>
    /// Get the type of message this is.
    /// </summary>
    public MessageType Type { get; internal set; } = MessageType.User;

    public MessageFlag Flags { get; internal set; }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> User/bot/system message </returns>
    public override string ToString()
    {
        return Type.ToString() + " message";
    }
}

/// <summary>
/// The type of message this is.
/// </summary>
public enum MessageType
{
    User, Bot, System, Webhook
}