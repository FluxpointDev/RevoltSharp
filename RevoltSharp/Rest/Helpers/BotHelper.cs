using System.Threading.Tasks;

namespace RevoltSharp.Rest
{
    public static class BotHelper
    {
        public static async Task ModifySelfAsync(this RevoltRestClient rest, Optional<string> avatar = null, Optional<string> statusText = null, Optional<UserStatusType> statusType = null, Optional<string> profileBio = null, Optional<string> profileBackground = null)
        {
            await rest.SendRequestAsync(RequestType.Patch, $"users/@me", ModifySelfRequest.Create(avatar, statusText, statusType, profileBio, profileBackground));
        }
    }
}
