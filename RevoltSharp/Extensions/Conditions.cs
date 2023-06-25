using RevoltSharp.Rest;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace RevoltSharp;

internal static class Conditions
{
    internal static bool OptionHasValue(Option<string> option)
    {
        if (option != null && !string.IsNullOrEmpty(option.Value))
            return true;
        return false;
    }

	#region Channel
	internal static void ChannelIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Channel id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Channel id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void ChannelNameLength(string name, string request)
	{
		if (name.Length < 1)
			throw new RevoltArgumentException($"Channel name can't be empty for the {request} request.");

		if (name.Length > Const.All_MaxNameLength)
			throw new RevoltArgumentException($"Channel name length can't be more than {Const.All_MaxNameLength} characters for the {request} request.");
	}

	internal static void ChannelDescriptionLength(string desc, string request)
	{
		if (desc.Length > Const.All_MaxDescriptionLength)
			throw new RevoltArgumentException($"Channel description length can't be more than {Const.All_MaxDescriptionLength} characters for the {request} request.");
	}

	internal static void OwnerIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Owner id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Owner id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}
	internal static void OwnerModifyCheck(ServerMember member, string request)
		=> OwnerModifyCheck(member.Server, member.Id, request);

	internal static void OwnerModifyCheck(Server server, string memberId, string request)
	{
		if (server.OwnerId == memberId)
			throw new RevoltRestException($"You can't modify the server owner for the {request} request.", 400, RevoltErrorType.InvalidOperation);
	}
	#endregion

	#region Member
	internal static void MemberIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Member id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Member id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}
	#endregion

	#region Role
	internal static void RoleIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Role id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Role id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void RoleNameLength(string name, string request)
	{
		if (name.Length < 1)
			throw new RevoltArgumentException($"Role name can't be empty for the {request} request.");

		if (name.Length > Const.All_MaxNameLength)
			throw new RevoltArgumentException($"Role name length can't be more than {Const.All_MaxNameLength} characters for the {request} request.");
	}

	internal static void RoleListEmpty(string[] roles, string request)
	{
		if (roles == null || !roles.Any())
			throw new RevoltArgumentException($"Role id list can't be empty for the {request} request.");
	}

	internal static void RoleList(string[] roles, string request)
	{
		if (roles == null || !roles.Any())
			throw new RevoltArgumentException($"Role id list can't be empty for the {request} request.");
	}
	#endregion

	#region User
	internal static void UserIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"User id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"User id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}
	#endregion

	#region Icon
	internal static void IconIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Icon id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Icon id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}
	#endregion

	#region Message

	internal static void MessageIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Message id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Message id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void AttachmentIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Attachment id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Attachment id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void MessageIdsCount(string[] ids, string request)
	{
		if (ids.Length > Const.Message_MaxDeleteListCount)
			throw new RevoltArgumentException($"Message ids list can't be more than {Const.Message_MaxDeleteListCount} for the {request} request.");
	}

	internal static void EmbedsNotAllowedForUsers(RevoltRestClient rest, Embed[] embeds, string request)
	{
		if (rest.Client.UserBot && embeds != null && embeds.Any())
			throw new RevoltArgumentException($"User accounts can't send messages with embeds for the {request} request.");
	}

	internal static void MessagePropertiesEmpty(string content, string[] attachments, Embed[] embeds, string request)
	{
		if (string.IsNullOrEmpty(content) && (attachments == null || attachments.Length == 0) && (embeds == null || embeds.Length == 0))
			throw new RevoltArgumentException($"Message content, attachments and embed can't be empty for the {request} request.");
	}

	internal static void FileBytesEmpty(byte[] bytes, string request)
	{
		if (bytes == null || bytes.Length == 0)
			throw new RevoltArgumentException($"File data can't be empty for the {request} request.");
	}

	internal static void FileNameEmpty(string name, string request)
	{
		if (string.IsNullOrEmpty(name))
			throw new RevoltArgumentException($"File name can't be empty for the {request} request.");
	}

	internal static void MessageContentLength(string content, string request)
	{
		if (!string.IsNullOrEmpty(content) && content.Length > Const.Message_MaxContentLength)
			throw new RevoltArgumentException($"Message content is more than {Const.Message_MaxContentLength} characters for the {request} request.");
	}

	internal static void MessageSearchCount(int count, string request)
	{
		if (count < 1)
			throw new RevoltArgumentException($"Message search count can't be less than 1 for the {request} request.");

		if (count > Const.Message_MaxSearchListCount)
			throw new RevoltArgumentException($"Message search count can't be more than {Const.Message_MaxSearchListCount} for the {request} request.");
	}

	internal static void MessageNearbyIdLength(string id, string request)
	{
		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Message nearby id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void MessageAfterIdLength(string id, string request)
	{
		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Message after id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void MessageBeforeIdLength(string id, string request)
	{
		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Message before id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void ImageSizeLength(int? size, string request)
	{
		if (size != null && size <= 0)
			throw new RevoltArgumentException($"Image size cannot be zero for the {request} request.");
	}

	internal static void ReplyListCount(MessageReply[] list, string request)
	{
		if (list.Length > 5)
			throw new RevoltArgumentException($"Replies list can't be more than 5 for the {request} request.");
	}

	#endregion

	#region Server
	internal static void ServerIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Server id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Server id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}
	internal static void InviteCodeEmpty(string inviteCode, string request)
	{
		if (string.IsNullOrWhiteSpace(inviteCode))
			throw new RevoltArgumentException($"Invite code can't be empty for the {request} request.");
	}
	#endregion

	#region Emoji

	internal static void EmojiIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Emoji id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Emoji id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void EmojiNameLength(string name, string request)
	{
		if (name.Length < 1)
			throw new RevoltArgumentException($"Emoji name can't be empty for the {request} request.");

		if (name.Length > Const.All_MaxNameLength)
			throw new RevoltArgumentException($"Emoji name length can't be more than {Const.All_MaxNameLength} characters for the {request} request.");
	}

	#endregion

	#region Bot

	internal static void AvatarIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Avatar id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Avatar id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
	}

	internal static void BackgroundIdLength(string id, string request)
	{
		if (id.Length < 1)
			throw new RevoltArgumentException($"Background id can't be empty for the {request} request.");

		if (id.Length > Const.All_MaxIdLength)
			throw new RevoltArgumentException($"Background id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
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

	internal static void NotSelf(RevoltRestClient rest, string userId, string request)
	{
		if (userId == rest.Client.CurrentUser.Id)
			throw new RevoltArgumentException($"Cannot perform the {request} request against the current user/bot account.");
	}

	#endregion
}
