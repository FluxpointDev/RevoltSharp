using RevoltSharp.Rest;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class BotHelper
    {
        public static Task<SelfUser> ModifySelfAsync(this SelfUser user, Optional<string> avatar = null, Optional<string> statusText = null, Optional<UserStatusType> statusType = null, Optional<string> profileBio = null, Optional<string> profileBackground = null)
           => ModifySelfAsync(user.Client.Rest, avatar, statusText, statusType, profileBio, profileBackground);

        public static async Task<SelfUser> ModifySelfAsync(this RevoltRestClient rest, Optional<string> avatar = null, Optional<string> statusText = null, Optional<UserStatusType> statusType = null, Optional<string> profileBio = null, Optional<string> profileBackground = null)
        {
            return await rest.SendRequestAsync<SelfUser>(RequestType.Patch, $"users/@me", ModifySelfRequest.Create(avatar, statusText, statusType, profileBio, profileBackground));
        }

        public static Task<FileAttachment> UploadFileAsync(this SelfUser user, byte[] bytes, string name, RevoltRestClient.UploadFileType type)
           => user.Client.Rest.InternalUploadFileAsync(bytes, name, type);

        public static Task<FileAttachment> UploadFileAsync(this SelfUser user, string path, RevoltRestClient.UploadFileType type)
            => user.Client.Rest.InternalUploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);


        public static Task<FileAttachment> UploadFileAsync(this Channel channel, byte[] bytes, string name, RevoltRestClient.UploadFileType type)
           => channel.Client.Rest.InternalUploadFileAsync(bytes, name, type);

        public static Task<FileAttachment> UploadFileAsync(this Channel channel, string path, RevoltRestClient.UploadFileType type)
            => channel.Client.Rest.InternalUploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);

    }
}
