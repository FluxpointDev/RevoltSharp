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

    public static async Task<SelfUser> ModifySelfAsync(this RevoltRestClient rest, Option<string> avatar = null, Option<string> statusText = null, Option<UserStatusType> statusType = null, Option<string> profileBio = null, Option<string> profileBackground = null)
    {
        UserJson Json = await rest.SendRequestAsync<UserJson>(RequestType.Patch, $"users/@me", ModifySelfRequest.Create(avatar, statusText, statusType, profileBio, profileBackground));
        return new SelfUser(rest.Client, Json);
    }

    /// <inheritdoc cref="RevoltRestClient.UploadFileAsync(byte[], string, RevoltRestClient.UploadFileType)" />
    public static Task<FileAttachment> UploadFileAsync(this SelfUser user, byte[] bytes, string name, RevoltRestClient.UploadFileType type)
       => user.Client.Rest.UploadFileAsync(bytes, name, type);

    public static Task<FileAttachment> UploadFileAsync(this SelfUser user, string path, RevoltRestClient.UploadFileType type)
        => user.Client.Rest.UploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);


    public static Task<FileAttachment> UploadFileAsync(this Channel channel, byte[] bytes, string name, RevoltRestClient.UploadFileType type)
       => channel.Client.Rest.UploadFileAsync(bytes, name, type);

    public static Task<FileAttachment> UploadFileAsync(this Channel channel, string path, RevoltRestClient.UploadFileType type)
        => channel.Client.Rest.UploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);

}
