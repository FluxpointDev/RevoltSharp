using Newtonsoft.Json;
using System;

namespace RevoltSharp
{
    internal class ChannelJson
    {
        [JsonProperty("_id")]
        public string id;
        public string channel_type;
        public string nonce;
        public string user;
        public bool active;
        public string[] recipients;
        public string last_message_id;
        public AttachmentJson icon;
        public string description;
        public string name;
        public string owner;
        public int permissions;
        public int default_permissions;
        public string server;
        public bool nsfw;
        internal Channel ToEntity(RevoltClient client)
        {
            switch (channel_type)
            {
                case "SavedMessages":
                    return new SavedMessagesChannel
                    {
                        Id = id,
                        Type = ChannelType.SavedMessages,
                        User = user,
                        Client = client
                    };
                case "DirectMessage":
                    return new DMChannel
                    {
                        Id = id,
                        Type = ChannelType.DM,
                        Active = active,
                        Recipents = recipients != null ? recipients : new string[0],
                        LastMessageId = last_message_id,
                        Client = client
                    };
                case "Group":
                    return new GroupChannel
                    {
                        Id = id,
                        Type = ChannelType.Group,
                        Recipents = recipients != null ? recipients : new string[0],
                        Description = description,
                        LastMessageId = last_message_id,
                        Name = name,
                        OwnerId = owner,
                        Permissions = permissions,
                        Icon = icon != null ? icon.ToEntity() : null,
                        Client = client
                    };
                case "TextChannel":
                    return new TextChannel
                    {
                        Id = id,
                        Type = ChannelType.Text,
                        DefaultPermissions = default_permissions,
                        Description = description,
                        Name = name,
                        ServerId = server,
                        Icon = icon != null ? icon.ToEntity() : null,
                        LastMessageId = last_message_id,
                        Client = client,
                        IsNsfw = nsfw
                    };
                case "VoiceChannel":
                    return new VoiceChannel
                    {
                        Id = id,
                        Type = ChannelType.Voice,
                        DefaultPermissions = default_permissions,
                        Description = description,
                        Name = name,
                        ServerId = server,
                        Icon = icon != null ? icon.ToEntity() : null,
                        Client = client
                    };
            }
            if (!string.IsNullOrEmpty(server))
                return new ServerUnknownChannel
                {
                    Id = id,
                    ServerId = server,
                    Type = ChannelType.Unknown,
                    Client = client
                };

            return new UnknownChannel
            {
                Id = id,
                Type = ChannelType.Unknown,
                Client = client
            };
        }
    }
}
