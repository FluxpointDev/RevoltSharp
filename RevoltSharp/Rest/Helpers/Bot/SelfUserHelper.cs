using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for current user/bot account.
/// </summary>
public static class SelfUserHelper
{
    /// <inheritdoc cref="ModifySelfAsync(RevoltRestClient, Option{string}, Option{string}, Option{UserStatusType}, Option{string}, Option{string})" />
    public static Task<SelfUser> ModifySelfAsync(this SelfUser user, Option<string> avatar = null, Option<string> statusText = null, Option<UserStatusType> statusType = null, Option<string> profileBio = null, Option<string> profileBackground = null)
       => ModifySelfAsync(user.Client.Rest, avatar, statusText, statusType, profileBio, profileBackground);

    /// <summary>
    /// Modify the current user/bot account avatar, status and profile.
    /// </summary>
    /// <returns>
    /// Modified <see cref="SelfUser"/>
    /// </returns>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<SelfUser> ModifySelfAsync(this RevoltRestClient rest, Option<string> avatar = null, Option<string> statusText = null, Option<UserStatusType> statusType = null, Option<string> profileBio = null, Option<string> profileBackground = null)
    {
        if (avatar != null)
            Conditions.AvatarIdLength(avatar.Value, nameof(ModifySelfAsync));

        if (statusText != null)
            Conditions.UserStatusTextLength(statusText.Value, nameof(ModifySelfAsync));

        if (profileBackground != null)
            Conditions.BackgroundIdLength(profileBackground.Value, nameof(ModifySelfAsync));

        UserJson Json = await rest.SendRequestAsync<UserJson>(RequestType.Patch, $"users/@me", ModifySelfRequest.Create(avatar, statusText, statusType, profileBio, profileBackground));
        return new SelfUser(rest.Client, Json);
    }
}
