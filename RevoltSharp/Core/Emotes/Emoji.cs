
using System;

namespace RevoltSharp;

/// <summary>
/// Server or default emoji
/// </summary>
public class Emoji : Entity
{
    internal Emoji(RevoltClient client, EmojiJson model) : base(client)
    {
        Id = model.Id;
        Name = model.Name;
        CreatorId = model.CreatorId;
        ServerId = model.Parent.ServerId;
        IsAnimated = model.Animated;
        IsNsfw = model.Nsfw;
    }

    internal Emoji(RevoltClient client, string emoji) : base(client)
    {
        Id = emoji;
        Name = emoji;
    }

    public Emoji(string emoji) : base(null)
    {
        if (emoji.StartsWith(':'))
            emoji = emoji.Substring(1, emoji.Length - 2);
        Id = emoji;
        Name = emoji;
    }

    public string Id { get; internal set; }

    public string Name { get; internal set; }

    public bool IsServerEmoji
        => !string.IsNullOrEmpty(ServerId);

    public string ServerId { get; internal set; }

    /// <summary>
    /// Server that the Emoji is from.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Server? Server => Client.GetServer(ServerId);

    public string CreatorId { get; internal set; }

    public User? Creator { get; internal set }

    public bool IsAnimated { get; internal set; }

    public bool IsNsfw { get; internal set; }

    public string ImageUrl
        => Client.Config.Debug.UploadUrl + "/emojis/" + Id;

}
