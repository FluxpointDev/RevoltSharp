namespace RevoltSharp;


public class PublicBot : CreatedEntity
{
    internal PublicBot(RevoltClient client, PublicBotJson model) : base(client, model.Id)
    {
        Username = model.Username!;
        AvatarId = model.AvatarId;
        Description = model.Description;
    }

    public string Username { get; internal set; }

    public string? AvatarId { get; internal set; }

    /// <summary>
    /// Gets the bot's avatar.
    /// </summary>
    /// <param name="which">Which avatar to return.</param>
    /// <param name="size"></param>
    /// <returns>URL of the image</returns>
    public string? GetAvatarUrl(AvatarSources which = AvatarSources.Any, int? size = null)
    {
        if (!string.IsNullOrEmpty(AvatarId) && (which | AvatarSources.User) != 0)
        {
            Conditions.ImageSizeLength(size, nameof(GetAvatarUrl));
            return $"{Client.Config.Debug.UploadUrl}avatars/{Id}/{AvatarId}{(size != null ? $"?size={size}" : null)}";
        }

        if ((which | AvatarSources.Default) != 0)
        {
            Conditions.ImageSizeLength(size, nameof(GetAvatarUrl));
            return $"{Client.Config.ApiUrl}users/{Id}/default_avatar{(size != null ? $"?size={size}" : null)}";
        }

        return null;
    }

    public string? Description { get; internal set; }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Bot username </returns>
    public override string ToString()
    {
        return Username;
    }
}