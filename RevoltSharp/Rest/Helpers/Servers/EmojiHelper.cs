using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;


/// <summary>
/// Revolt http/rest methods for emojis.
/// </summary>
public static class EmojiHelper
{
    /// <summary>
    /// Get an emoji.
    /// </summary>
    /// <returns>
    /// <see cref="Emoji" /> or <see langword="null" /> if no emoji found.
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Emoji?> GetEmojiAsync(this RevoltRestClient rest, string emojiId)
    {
        Conditions.EmojiIdLength(emojiId, nameof(GetEmojiAsync));

        if (rest.Client.WebSocket != null && rest.Client.WebSocket.EmojiCache.TryGetValue(emojiId, out Emoji emoji))
            return emoji;

        EmojiJson? Emoji = await rest.GetAsync<EmojiJson>($"/custom/emoji/{emojiId}");
        if (Emoji == null)
            return null;
        return new Emoji(rest.Client, Emoji);
    }

    /// <inheritdoc cref="GetEmojisAsync(RevoltRestClient, string)" />
    public static Task<IReadOnlyCollection<Emoji>> GetEmojisAsync(this Server server)
        => GetEmojisAsync(server.Client.Rest, server.Id);

    /// <summary>
    /// Get all emojis from a server
    /// </summary>
    /// <returns>
    /// List of server <see cref="Emoji" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<Emoji>> GetEmojisAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdLength(serverId, nameof(GetEmojisAsync));

        EmojiJson[]? Json = await rest.GetAsync<EmojiJson[]>($"/servers/{serverId}/emojis");
        if (Json == null)
            return System.Array.Empty<Emoji>();
        return Json.Select(x => new Emoji(rest.Client, x)).ToImmutableArray();
    }

    /// <inheritdoc cref="CreateEmojiAsync(RevoltRestClient, string, string, string, bool)" />
    public static Task<Emoji> CreateEmojiAsync(this Server server, string attachmentIdOrFile, string emojiName, bool nsfw = false)
        => CreateEmojiAsync(server.Client.Rest, server.Id, attachmentIdOrFile, emojiName, nsfw);

    public static Task<Emoji> CreateEmojiAsync(this Server server, byte[] bytes, string fileName, string emojiName, bool nsfw = false)
        => CreateEmojiAsync(server.Client.Rest, server.Id, bytes, fileName, emojiName, nsfw);

    public static async Task<Emoji> CreateEmojiAsync(this RevoltRestClient rest, string serverId, byte[] bytes, string fileName, string emojiName, bool nsfw = false)
    {
        FileAttachment File = await rest.UploadFileAsync(bytes, fileName, UploadFileType.Emoji);
        return await CreateEmojiAsync(rest, serverId, File.Id, emojiName, nsfw);
    }

    /// <summary>
    /// Create a server <see cref="Emoji" />
    /// </summary>
    /// <remarks>
    /// You need <see cref="ServerPermission.ManageCustomisation" /> and has a max count of 100 per-server.
    /// </remarks>
    /// <param name="rest"></param>
    /// <param name="attachmentIdOrFile">Uploaded file attachment from rest UploadFileAsync</param>
    /// <param name="serverId">Server id</param>
    /// <param name="name">Name of emoji</param>
    /// <param name="nsfw">Is the emoji nsfw</param>
    /// <returns><see cref="Emoji" /></returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Emoji> CreateEmojiAsync(this RevoltRestClient rest, string serverId, string attachmentIdOrFile, string emojiName, bool nsfw = false)
    {
        Conditions.NotAllowedForBots(rest, nameof(CreateEmojiAsync));
        Conditions.ServerIdLength(serverId, nameof(CreateEmojiAsync));
        Conditions.EmojiNameLength(emojiName, nameof(CreateEmojiAsync));
        Conditions.AttachmentIdLength(attachmentIdOrFile, nameof(CreateEmojiAsync), true);

        if (!string.IsNullOrEmpty(attachmentIdOrFile) && (attachmentIdOrFile.Contains('/') || attachmentIdOrFile.Contains('\\')))
        {
            FileAttachment File = await rest.UploadFileAsync(attachmentIdOrFile, UploadFileType.Emoji);
            attachmentIdOrFile = File.Id;
        }

        Conditions.AttachmentIdLength(attachmentIdOrFile, nameof(CreateEmojiAsync));

        EmojiJson Emoji = await rest.PutAsync<EmojiJson>($"/custom/emoji/{attachmentIdOrFile}", new CreateEmojiRequest
        {
            name = emojiName,
            nsfw = nsfw,
            parent = new CreateEmojiParent
            {
                id = serverId
            }
        });
        return new Emoji(rest.Client, Emoji);
    }

    /// <inheritdoc cref="DeleteEmojiAsync(RevoltRestClient, string)" />
    public static Task DeleteAsync(this Emoji emoji)
       => DeleteEmojiAsync(emoji.Client.Rest, emoji.Id);

    /// <inheritdoc cref="DeleteEmojiAsync(RevoltRestClient, string)" />
    public static Task DeleteEmojiAsync(this Server server, Emoji emoji)
       => DeleteEmojiAsync(server.Client.Rest, emoji.Id);


    /// <summary>
    /// Delete an <see cref="Emoji" /> from a server
    /// </summary>
    /// <remarks>
    /// You need <see cref="ServerPermission.ManageCustomisation" />
    /// </remarks>
    /// <param name="rest"></param>
    /// <param name="emojiId">Emoji id</param>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task DeleteEmojiAsync(this RevoltRestClient rest, string emojiId)
    {
        Conditions.EmojiIdLength(emojiId, nameof(DeleteEmojiAsync));
        Conditions.NotAllowedForBots(rest, nameof(DeleteEmojiAsync));

        await rest.DeleteAsync($"/custom/emoji/{emojiId}");
    }
}