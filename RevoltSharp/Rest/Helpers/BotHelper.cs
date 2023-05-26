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
    public static Task<FileAttachment> UploadFileAsync(this SelfUser user, byte[] bytes, string name, UploadFileType type)
       => user.Client.Rest.UploadFileAsync(bytes, name, type);

    /// <inheritdoc cref="RevoltRestClient.UploadFileAsync(byte[], string, UploadFileType)" />
    public static Task<FileAttachment> UploadFileAsync(this SelfUser user, string path, UploadFileType type)
        => user.Client.Rest.UploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);

    /// <inheritdoc cref="RevoltRestClient.UploadFileAsync(byte[], string, UploadFileType)" />
    public static Task<FileAttachment> UploadFileAsync(this Channel channel, byte[] bytes, string name, UploadFileType type)
       => channel.Client.Rest.UploadFileAsync(bytes, name, type);

    /// <inheritdoc cref="RevoltRestClient.UploadFileAsync(byte[], string, UploadFileType)" />
    public static Task<FileAttachment> UploadFileAsync(this Channel channel, string path, UploadFileType type)
        => channel.Client.Rest.UploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);

    /// <summary>
    /// Get or create the current user/bot's saved messages channel that is private.
    /// </summary>
    /// <returns><see cref="SavedMessagesChannel" /> or <see langword="null" /></returns>
    public static Task<SavedMessagesChannel?> GetOrCreateSavedMessageChannelAsync(this RevoltRestClient rest)
    #pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        => rest.SendRequestAsync<SavedMessagesChannel>(RequestType.Get, "/users/" + rest.Client.CurrentUser.Id + "/dm");
    #pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

}
