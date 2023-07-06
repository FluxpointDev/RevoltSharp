using System;

namespace RevoltSharp;


/// <summary>
/// Server or default emoji
/// </summary>
public class Emoji : CreatedEntity
{
    internal Emoji(RevoltClient client, EmojiJson model) : base(client, model.Id)
    {
        Name = model.Name!;
        CreatorId = model.CreatorId;
        ServerId = model.Parent.ServerId;
        IsAnimated = model.Animated;
        IsNsfw = model.Nsfw;
    }

    internal Emoji(RevoltClient client, string emoji) : base(client, emoji)
    {
        Name = emoji;
    }

    public Emoji(string emoji) : base(null, emoji.StartsWith(':') ? emoji.Substring(1, emoji.Length - 2) : emoji)
    {
        Name = Id;
    }

    /// <summary>
    /// Id of the emoji.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the emoji was created.
    /// </summary>
    public new DateTimeOffset? CreatedAt => base.CreatedAt;

    public string Name { get; internal set; }

    public bool IsServerEmoji
        => !string.IsNullOrEmpty(ServerId);

    public string? ServerId { get; internal set; }

    /// <summary>
    /// Server that the Emoji is from.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Server? Server => Client.GetServer(ServerId);

    public string? CreatorId { get; internal set; }

    public User? Creator => Client.GetUser(CreatorId);

    public bool IsAnimated { get; internal set; }

    public bool IsNsfw { get; internal set; }

    public string ImageUrl
        => IsServerEmoji ? Client.Config.Debug.UploadUrl + "/emojis/" + Id : string.Empty;

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Emoji name or unicode </returns>
    public override string ToString()
    {
        return Name;
    }

}