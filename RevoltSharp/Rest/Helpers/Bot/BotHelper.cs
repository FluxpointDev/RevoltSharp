using RevoltSharp.Rest;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;


/// <summary>
/// Revolt http/rest methods for current user/bot account.
/// </summary>
public static class BotHelper
{
    /// <inheritdoc cref="RevoltRestClient.UploadFileAsync(byte[], string, UploadFileType)" />
    public static Task<FileAttachment> UploadFileAsync(this Channel channel, byte[] bytes, string name, UploadFileType type)
       => channel.Client.Rest.UploadFileAsync(bytes, name, type);

    /// <inheritdoc cref="RevoltRestClient.UploadFileAsync(byte[], string, UploadFileType)" />
    public static Task<FileAttachment> UploadFileAsync(this Channel channel, string path, UploadFileType type)
        => channel.Client.Rest.UploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);

    /// <inheritdoc cref="GetOrCreateSavedMessageChannelAsync(RevoltRestClient)" />
    public static Task<SavedMessagesChannel?> GetOrCreateSavedMessageChannelAsync(this SelfUser user)
        => GetOrCreateSavedMessageChannelAsync(user.Client.Rest);

    /// <summary>
    /// Get or create the current user/bot's saved messages channel that is private.
    /// </summary>
    /// <returns>
    /// <see cref="SavedMessagesChannel" /> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<SavedMessagesChannel?> GetOrCreateSavedMessageChannelAsync(this RevoltRestClient rest)
    {
        if (rest.Client.SavedMessagesChannel != null)
            return rest.Client.SavedMessagesChannel;

        ChannelJson SC = await rest.GetAsync<ChannelJson>("/users/" + rest.Client.CurrentUser.Id + "/dm");
        if (SC == null)
            return null;
        return Channel.Create(rest.Client, SC) as SavedMessagesChannel;
    }

    public static async Task<PublicBot?> GetPublicBotAsync(this RevoltRestClient rest, string id)
    {
        Conditions.UserIdLength(id, nameof(GetPublicBotAsync));

        PublicBotJson? Bot = await rest.GetAsync<PublicBotJson>($"/bots/{id}/invite");
        if (Bot == null)
            return null;

        return new PublicBot(rest.Client, Bot);
    }
}
