using Newtonsoft.Json;

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
        public string[] recipents;
        public object last_message;
        public AttachmentJson icon;
        public string description;
        public string name;
        public string owner;
        public int permissions;
        public int default_permissions;
        public string server;
        internal Channel ToEntity(RevoltClient client)
        {
            string LastMessage = "";
            if (last_message != null)
            {
                if (last_message is string lms)
                    LastMessage = lms;
                else if (last_message is LastMessageJson js)
                {
                    LastMessage = js.id;
                }
            }
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
                        Recipents = recipents,
                        LastMessageId = LastMessage,
                        Client = client
                    };
                case "Group":
                    return new GroupChannel
                    {
                        Id = id,
                        Type = ChannelType.Group,
                        Recipents = recipents,
                        Description = description,
                        LastMessageId = LastMessage,
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
                        LastMessageId = LastMessage,
                        Client = client
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
