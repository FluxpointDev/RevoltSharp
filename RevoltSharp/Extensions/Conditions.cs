namespace RevoltSharp;

internal static class Conditions
{
    internal static void OwnerModifyCheck(ServerMember member, string request)
    {
        if (member.Server != null && member.Id == member.Server.OwnerId)
            throw new RevoltArgumentException($"You can't modify the server owner for the {request} request.");
    }

    internal static void ChannelIdEmpty(string channelId, string request)
    {
        if (string.IsNullOrWhiteSpace(channelId))
            throw new RevoltArgumentException($"Channel id can't be empty for the {request} request.");
    }

    internal static void ChannelNameEmpty(string channelname, string request)
    {
        if (string.IsNullOrWhiteSpace(channelname))
            throw new RevoltArgumentException($"Channel name can't be empty for the {request} request.");
    }

    internal static void AttachmentIdEmpty(string attachmentid, string request)
    {
        if (string.IsNullOrWhiteSpace(attachmentid))
            throw new RevoltArgumentException($"Attachment id can't be empty for the {request} request.");
    }

    internal static void InviteCodeEmpty(string inviteCode, string request)
    {
        if (string.IsNullOrWhiteSpace(inviteCode))
            throw new RevoltArgumentException($"Invite code can't be empty for the {request} request.");
    }

    internal static void MessageIdEmpty(string messageId, string request)
    {
        if (string.IsNullOrEmpty(messageId))
            throw new RevoltArgumentException($"Message id can't be empty for the {request} request.");
    }

    internal static void MessageIdEmpty(string[] messageId, string request)
    {
        if (messageId == null || messageId.Length == 0)
            throw new RevoltArgumentException($"Message id can't be empty for the {request} request.");
    }

    internal static void EmojiIdEmpty(string emojiId, string request)
    {
        if (string.IsNullOrEmpty(emojiId))
            throw new RevoltArgumentException($"Emoji id can't be empty for the {request} request.");
    }

    internal static void FileBytesEmpty(byte[] bytes, string request)
    {
        if (bytes == null || bytes.Length == 0)
            throw new RevoltArgumentException($"File data can't be empty for the {request} request.");
    }

    internal static void FileNameEmpty(string imagename, string request)
    {
        if (string.IsNullOrEmpty(imagename))
            throw new RevoltArgumentException($"File name can't be empty for the {request} request.");
    }

    internal static void EmojiNameEmpty(string emojiname, string request)
    {
        if (string.IsNullOrEmpty(emojiname))
            throw new RevoltArgumentException($"Emoji name can't be empty for the {request} request.");
    }

    internal static void ServerIdEmpty(string serverId, string request)
    {
        if (string.IsNullOrEmpty(serverId))
            throw new RevoltArgumentException($"Server id can't be empty for the {request} request.");
    }

    internal static void UserIdEmpty(string userId, string request)
    {
        if (string.IsNullOrEmpty(userId))
            throw new RevoltArgumentException($"User id can't be empty for the {request} request.");
    }

    internal static void MemberIdEmpty(string memberId, string request)
    {
        if (string.IsNullOrEmpty(memberId))
            throw new RevoltArgumentException($"Member id can't be empty for the {request} request.");
    }

    internal static void RoleIdEmpty(string roleId, string request)
    {
        if (string.IsNullOrEmpty(roleId))
            throw new RevoltArgumentException($"Role id can't be empty for the {request} request.");
    }

    internal static void RoleNameEmpty(string rolename, string request)
    {
        if (string.IsNullOrEmpty(rolename))
            throw new RevoltArgumentException($"Role name can't be empty for the {request} request.");
    }
}
