using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class Conditions
    {
        public static void ChannelIdEmpty(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                throw new RevoltArgumentException("Channel id can't be empty for this request.");
        }

        public static void MessageIdEmpty(string messageId)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new RevoltArgumentException("Message id can't be empty for this request.");
        }

        public static void MessageIdEmpty(string[] messageId)
        {
            if (messageId == null || messageId.Length == 0)
                throw new RevoltArgumentException("Message id can't be empty for this request.");
        }

        public static void EmojiIdEmpty(string emojiId)
        {
            if (string.IsNullOrEmpty(emojiId))
                throw new RevoltArgumentException("Emoji id can't be empty for this request.");
        }

        public static void ServerIdEmpty(string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");
        }

        public static void UserIdEmpty(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new RevoltArgumentException("User id can't be empty for this request.");
        }

        public static void RoleIdEmpty(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                throw new RevoltArgumentException("Role id can't be empty for this request.");
        }
    }
}
