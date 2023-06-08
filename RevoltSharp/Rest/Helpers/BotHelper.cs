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
    /// <inheritdoc cref="ModifySelfAsync(RevoltRestClient, Option{string}, Option{string}, Option{UserStatusType}, Option{string}, Option{string})" />
    public static Task<SelfUser> ModifySelfAsync(this SelfUser user, Option<string> avatar = null, Option<string> statusText = null, Option<UserStatusType> statusType = null, Option<string> profileBio = null, Option<string> profileBackground = null)
       => ModifySelfAsync(user.Client.Rest, avatar, statusText, statusType, profileBio, profileBackground);

    /// <summary>
    /// Modify the current user/bot account avatar, status and profile.
    /// </summary>
    /// <returns>Modified <see cref="SelfUser"/></returns>
    public static async Task<SelfUser> ModifySelfAsync(this RevoltRestClient rest, Option<string> avatar = null, Option<string> statusText = null, Option<UserStatusType> statusType = null, Option<string> profileBio = null, Option<string> profileBackground = null)
    {
        UserJson Json = await rest.SendRequestAsync<UserJson>(RequestType.Patch, $"users/@me", ModifySelfRequest.Create(avatar, statusText, statusType, profileBio, profileBackground));
        return new SelfUser(rest.Client, Json);
    }

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
    public static async Task<SavedMessagesChannel?> GetOrCreateSavedMessageChannelAsync(this RevoltRestClient rest)
    {
        ChannelJson SC = await rest.SendRequestAsync<ChannelJson>(RequestType.Get, "/users/" + rest.Client.CurrentUser.Id + "/dm");
        if (SC == null)
            return null;
        return Channel.Create(rest.Client, SC) as SavedMessagesChannel;
    }

}
