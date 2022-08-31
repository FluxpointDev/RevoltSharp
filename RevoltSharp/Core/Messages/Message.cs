using System;

namespace RevoltSharp
{
    /// <summary>
    /// Revolt chat message with author, attachments, mentions and optional server.
    /// </summary>
    public abstract class Message : Entity
    {
        public Message(RevoltClient client)
            : base(client)
        { }

        internal static Message Create(RevoltClient client, MessageJson model)
        {
            if (model.Author == "00000000000000000000000000")
            {
                if (model.System != null)
                {
                    switch (model.System.Type)
                    {
                        case "text":
                            return new SystemMessage<SystemText>(client, model);
                        case "user_added":
                            return new SystemMessage<SystemUserAdded>(client, model);
                        case "user_remove":
                            return new SystemMessage<SystemUserRemoved>(client, model);
                        case "user_joined":
                            return new SystemMessage<SystemUserJoined>(client, model);
                        case "user_left":
                            return new SystemMessage<SystemUserLeft>(client, model);
                        case "user_kicked":
                            return new SystemMessage<SystemUserKicked>(client, model);
                        case "user_banned":
                            return new SystemMessage<SystemUserBanned>(client, model);
                        case "channel_renamed":
                            return new SystemMessage<SystemChannelRenamed>(client, model);
                        case "channel_description_changed":
                            return new SystemMessage<SystemChannelDescriptionChanged>(client, model);
                        case "channel_icon_changed":
                            return new SystemMessage<SystemChannelIconChanged>(client, model);
                        case "channel_ownership_changed":
                            return new SystemMessage<SystemChannelOwnershipChanged>(client, model);
                    }
                }
                return new SystemMessage<SystemUnknown>(client, model);
            }

            return new UserMessage(client, model);
        }

        /// <summary>
        /// Id of the message.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Channel id of the message.
        /// </summary>
        public string ChannelId { get; internal set; }

        public Channel Channel { get; internal set; }

        /// <summary>
        /// Id of the user who posted the message.
        /// </summary>
        public string AuthorId { get; internal set; }

        public User Author { get; internal set; }

        
    }
}
