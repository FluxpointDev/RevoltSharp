using RevoltSharp.Rest;
using System.Linq;

namespace RevoltSharp;

internal static class Conditions
{
    internal static void EmbedsNotAllowedForUsers(RevoltRestClient rest, Embed[] embeds, string request)
    {
        if (rest.Client.UserBot && embeds != null && embeds.Any())
            throw new RevoltArgumentException($"User accounts can't send messages with embeds for the {request} request.");
    }

    internal static void OwnerModifyCheck(ServerMember member, string request)
        => OwnerModifyCheck(member.Server, member.Id, request);

    internal static void OwnerModifyCheck(Server server, string memberId, string request)
    {
        if (server.OwnerId == memberId)
            throw new RevoltRestException($"You can't modify the server owner for the {request} request.", 400, RevoltErrorType.InvalidOperation);
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

    internal static void MessagePropertiesEmpty(string content, string[] attachments, Embed[] embeds, string request)
    {
        if (string.IsNullOrEmpty(content) && (attachments == null || attachments.Length == 0) && (embeds == null || embeds.Length == 0))
            throw new RevoltArgumentException($"Message content, attachments and embed can't be empty for the {request} request.");
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

    internal static void MessageContentLength(string content, string request)
    {
        if (content != null && content.Length > 2000)
            throw new RevoltArgumentException($"Message content is more than 2000 characters for the {request} request.");
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

    internal static void RoleListEmpty(string[] roles, string request)
    {
        if (roles == null || !roles.Any())
            throw new RevoltArgumentException($"Role id list can't be empty for the {request} request.");
    }

    internal static void NotAllowedForBots(RevoltRestClient rest, string request)
    {
        if (!rest.Client.UserBot)
            throw new RevoltRestException($"The {request} is not allowed to be used for bots.", 400, RevoltErrorType.NotAllowedForBots);
    }

    internal static void NotAllowedForUsers(RevoltRestClient rest, string request)
    {
        if (rest.Client.UserBot)
            throw new RevoltRestException($"The {request} is not allowed to be used for user accounts.", 400, RevoltErrorType.NotAllowedForBots);
    }

    internal static void RoleNameEmpty(string rolename, string request)
    {
        if (string.IsNullOrEmpty(rolename))
            throw new RevoltArgumentException($"Role name can't be empty for the {request} request.");
    }

    internal static void NotSelf(string userId, string selfId, string request)
    {
        if (userId == selfId)
            throw new RevoltArgumentException($"Cannot perform the {request} request against yourself.");
    }

    internal static void ImageSizeLength(int? size, string request)
    {
        if (size != null && size <= 0)
            throw new RevoltArgumentException($"Image size cannot be zero for the {request} request.");
    }
}
