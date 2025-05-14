using System;

namespace RevoltSharp;


public class ServerBanInfo : CreatedEntity
{
    internal ServerBanInfo(RevoltClient client, ServerBanInfoJson jsonInfo) : base(client, jsonInfo.Id.UserId)
    {
        UserId = jsonInfo.Id.UserId;
        Reason = jsonInfo.Reason;
    }

    public string UserId { get; internal set; }
    public string? Reason { get; internal set; }
}
public class ServerBan : CreatedEntity
{
    internal ServerBan(RevoltClient client, ServerBanUserJson json, ServerBanInfoJson jsonInfo) : base(client, json.Id)
    {
        Username = json.Username;
        Discriminator = json.Discriminator;
        Reason = jsonInfo.Reason;
        Avatar = Attachment.Create(client, json.Avatar);
    }

    /// <summary>
    /// Id of the ban.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the ban occured.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

    /// <summary>
    /// Username of the banned user.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Unique identity number of the banned user.
    /// </summary>
    private string Discriminator { get; set; }

    /// <summary>
    /// Reason for ban creation.
    /// </summary>
    public string? Reason { get; internal set; }

    public Attachment? Avatar { get; }

    /// <summary>
    /// Get the username and discriminator of the user.
    /// </summary>
    public string Tag
        => $"{Username}#{Discriminator}";

    /// <summary>
    /// Gets the user's avatar.
    /// </summary>
    /// <param name="which">Which avatar to return.</param>
    /// <param name="size"></param>
    /// <returns>URL of the image</returns>
    public string? GetAvatarUrl(AvatarSources which = AvatarSources.Any)
    {
        if (Avatar != null && (which | AvatarSources.User) != 0)
            return Avatar.GetUrl();

        if ((which | AvatarSources.Default) != 0)
        {
            return $"{Client.Config.ApiUrl}users/{Id}/default_avatar";
        }

        return null;
    }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Name#0001 ban </returns>
    public override string ToString()
    {
        return Username + "#" + Discriminator + " ban";
    }
}