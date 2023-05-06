namespace RevoltSharp
{
    public static class Conditions
    {
        public static void ChannelIdEmpty(string channelId, string request)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                throw new RevoltArgumentException($"Channel id can't be empty for the {request} request.");
        }

        public static void ChannelNameEmpty(string channelname, string request)
        {
            if (string.IsNullOrWhiteSpace(channelname))
                throw new RevoltArgumentException($"Channel name can't be empty for the {request} request.");
        }

        public static void AttachmentIdEmpty(string attachmentid, string request)
        {
            if (string.IsNullOrWhiteSpace(attachmentid))
                throw new RevoltArgumentException($"Attachment id can't be empty for the {request} request.");
        }

        public static void InviteIdEmpty(string inviteId, string request)
        {
            if (string.IsNullOrWhiteSpace(inviteId))
                throw new RevoltArgumentException($"Invite id can't be empty for the {request} request.");
        }

        public static void MessageIdEmpty(string messageId, string request)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException($"Message id can't be empty for the {request} request.");
        }

        public static void MessageIdEmpty(string[] messageId, string request)
        {
            if (messageId == null || messageId.Length == 0)
                throw new RevoltArgumentException($"Message id can't be empty for the {request} request.");
        }

        public static void EmojiIdEmpty(string emojiId, string request)
        {
            if (string.IsNullOrEmpty(emojiId))
                throw new RevoltArgumentException($"Emoji id can't be empty for the {request} request.");
        }

        public static void FileBytesEmpty(byte[] bytes, string request)
        {
            if (bytes == null || bytes.Length == 0)
                throw new RevoltArgumentException($"File data can't be empty for the {request} request.");
        }

        public static void FileNameEmpty(string imagename, string request)
        {
            if (string.IsNullOrEmpty(imagename))
                throw new RevoltArgumentException($"File name can't be empty for the {request} request.");
        }

        public static void EmojiNameEmpty(string emojiname, string request)
        {
            if (string.IsNullOrEmpty(emojiname))
                throw new RevoltArgumentException($"Emoji name can't be empty for the {request} request.");
        }

        public static void ServerIdEmpty(string serverId, string request)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException($"Server id can't be empty for the {request} request.");
        }

        public static void UserIdEmpty(string userId, string request)
        {
            if (string.IsNullOrEmpty(userId))
                throw new RevoltArgumentException($"User id can't be empty for the {request} request.");
        }

        public static void MemberIdEmpty(string memberId, string request)
        {
            if (string.IsNullOrEmpty(memberId))
                throw new RevoltArgumentException($"Member id can't be empty for the {request} request.");
        }

        public static void RoleIdEmpty(string roleId, string request)
        {
            if (string.IsNullOrEmpty(roleId))
                throw new RevoltArgumentException($"Role id can't be empty for the {request} request.");
        }
    }
}
