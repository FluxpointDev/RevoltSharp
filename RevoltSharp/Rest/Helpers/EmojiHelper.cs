using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class EmojiHelper
{
    /// <summary>
    /// Get an emoji
    /// </summary>
    /// <param name="rest"></param>
    /// <param name="emojiId">Emoji id</param>
    /// <returns><see cref="Emoji" /> or <see langword="null" /></returns>
    /// <exception cref="RevoltArgumentException"></exception>
    public static async Task<Emoji> GetEmojiAsync(this RevoltRestClient rest, string emojiId)
    {
        Conditions.EmojiIdEmpty(emojiId, "GetEmojiAsync");

        EmojiJson Emoji =  await rest.SendRequestAsync<EmojiJson>(RequestType.Get, $"/custom/emoji/{emojiId}");
        return Emoji == null ? null : new Emoji(rest.Client, Emoji);
    }

    /// <inheritdoc cref="GetEmojisAsync(RevoltRestClient, string)" />
    public static Task<Emoji[]> GetEmojisAsync(this Server server)
        => GetEmojisAsync(server.Client.Rest, server.Id);

    /// <summary>
    /// Get all emojis from a server
    /// </summary>
    /// <param name="rest"></param>
    /// <param name="serverId">Server id</param>
    /// <returns>List of server <see cref="Emoji" /></returns>
    /// <exception cref="RevoltArgumentException"></exception>
    public static async Task<Emoji[]> GetEmojisAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "GetEmojisAsync");

        EmojiJson[] Json = await rest.SendRequestAsync<EmojiJson[]>(RequestType.Get, $"/servers/{serverId}/emojis");
        return Json.Select(x => new Emoji(rest.Client, x)).ToArray();
    }

    public static Task<Emoji> CreateEmojiAsync(this Server server, string attachmentId, string name, bool nsfw = false)
        => CreateEmojiAsync(server.Client.Rest, server.Id, attachmentId, name, nsfw);


    /// <summary>
    /// Create a server <see cref="Emoji" />
    /// </summary>
    /// <remarks>
    /// You need <see cref="ServerPermission.ManageCustomisation" />
    /// </remarks>
    /// <param name="rest"></param>
    /// <param name="attachmentId">Uploaded file attachment from rest UploadFileAsync</param>
    /// <param name="serverId">Server id</param>
    /// <param name="name">Name of emoji</param>
    /// <param name="nsfw">Is the emoji nsfw</param>
    /// <returns><see cref="Emoji" /></returns>
    /// <exception cref="RevoltArgumentException"></exception>
    public static async Task<Emoji> CreateEmojiAsync(this RevoltRestClient rest, string serverId, string attachmentId, string name, bool nsfw = false)
    {
        Conditions.AttachmentIdEmpty(attachmentId, "CreateEmojiAsync");
        Conditions.ServerIdEmpty(serverId, "CreateEmojiAsync");
        Conditions.EmojiNameEmpty(name, "CreateEmojiAsync");

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


    public static Task<HttpResponseMessage> DeleteAsync(this Emoji emoji)
       => DeleteEmojiAsync(emoji.Client.Rest, emoji.Id);


    public static Task<HttpResponseMessage> DeleteEmojiAsync(this Server server, Emoji emoji)
       => DeleteEmojiAsync(server.Client.Rest, emoji.Id);


    /// <summary>
    /// Delete an <see cref="Emoji" /> from a server
    /// </summary>
    /// <remarks>
    /// You need <see cref="ServerPermission.ManageCustomisation" />
    /// </remarks>
    /// <param name="rest"></param>
    /// <param name="emojiId">Emoji id</param>
    /// <returns><see cref="HttpResponseMessage" /></returns>
    /// <exception cref="RevoltArgumentException"></exception>
    public static async Task<HttpResponseMessage> DeleteEmojiAsync(this RevoltRestClient rest, string emojiId)
    {
        Conditions.EmojiIdEmpty(emojiId, "DeleteEmojiAsync");

        return await rest.SendRequestAsync(RequestType.Delete, $"/custom/emoji/{emojiId}");
    }
}
