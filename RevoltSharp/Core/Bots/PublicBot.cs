namespace RevoltSharp;

/// <summary>
/// Info about a public bot on Revolt.
/// </summary>
public class PublicBot : CreatedEntity
{
    internal PublicBot(RevoltClient client, PublicBotJson model) : base(client, model.Id)
    {
        Username = model.Username!;
        AvatarId = model.AvatarId;
        Description = model.Description;
    }

    /// <summary>
    /// The username of the bot.
    /// </summary>
    public string Username { get; internal set; }

    /// <summary>
    /// The avatar id of the bot.
    /// </summary>
    public string? AvatarId { get; internal set; }

    /// <summary>
    /// Gets the bot's avatar.
    /// </summary>
    /// <param name="which">Which avatar to return.</param>
    /// <returns>URL of the image</returns>
    public string? GetAvatarUrl(AvatarSources which = AvatarSources.Any)
    {
        if (!string.IsNullOrEmpty(AvatarId) && (which | AvatarSources.User) != 0)
        {
            return $"{Client.Config.Debug.UploadUrl}avatars/{Id}/{AvatarId}";
        }

        if ((which | AvatarSources.Default) != 0)
        {
            return $"{Client.Config.ApiUrl}users/{Id}/default_avatar";
        }

        return null;
    }

    /// <summary>
    /// The description of the bot.
    /// </summary>
    public string? Description { get; internal set; }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Bot username </returns>
    public override string ToString()
    {
        return Username;
    }
}