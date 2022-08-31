using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class EmojiHelper
    {

        public static async Task<Emoji> GetEmojiAsync(this RevoltRestClient rest, string emojiId)
        {
            if (string.IsNullOrEmpty(emojiId))
                throw new RevoltArgumentException("Emoji id can't be empty for this request.");

            EmojiJson Emoji =  await rest.SendRequestAsync<EmojiJson>(RequestType.Get, $"/custom/emoji/{emojiId}");
            return Emoji == null ? null : new Emoji(rest.Client, Emoji);
        }

        public static async Task<Emoji[]> GetEmojisAsync(this RevoltRestClient rest, string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            EmojiJson[] Json = await rest.SendRequestAsync<EmojiJson[]>(RequestType.Get, $"/servers/{serverId}/emojis");
            return Json.Select(x => new Emoji(rest.Client, x)).ToArray();
        }

        public static async Task<Emoji> CreateEmojiAsync(this RevoltRestClient rest, string attachmentId, string serverId, string name, bool nsfw = false)
        {
            if (string.IsNullOrEmpty(attachmentId))
                throw new RevoltArgumentException("Attachment id can't be empty for this request.");

            if (string.IsNullOrEmpty(serverId))
                throw new RevoltArgumentException("Server id can't be empty for this request.");

            if (string.IsNullOrEmpty(name))
                throw new RevoltArgumentException("Emoji name can't be empty for this request.");

            EmojiJson Emoji = await rest.SendRequestAsync<EmojiJson>(RequestType.Put, $"/custom/emoji/{attachmentId}", new CreateEmojiRequest
            {
                name = name,
                nsfw = nsfw,
                parent = new CreateEmojiParent
                {
                    id = serverId
                }
            });
            return Emoji == null ? null : new Emoji(rest.Client, Emoji);
        }

        public static async Task<HttpResponseMessage> DeleteEmojiAsync(this RevoltRestClient rest, string emojiId)
        {
            if (string.IsNullOrEmpty(emojiId))
                throw new RevoltArgumentException("Emoji id can't be empty for this request.");

            return await rest.SendRequestAsync(RequestType.Delete, $"/custom/emoji/{emojiId}");
        }
    }
}
