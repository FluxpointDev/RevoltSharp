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

    /// <summary>
    /// Parse the emoji format or id to use for reactions.
    /// </summary>
    /// <param name="emoji"></param>
    /// <param name="parseDefaultEmojis"></param>
    public Emoji(string emoji, bool parseDefaultEmojis = true) : base(null, emoji.StartsWith(':') ? emoji.Substring(1, emoji.Length - 2) : emoji)
    {
        Name = Id;
        if (parseDefaultEmojis)
        {
            EmojiList.NameToUnicode.TryGetValue(emoji, out var Name);
        }
    }

    /// <summary>
    /// Id of the emoji.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the emoji was created.
    /// </summary>
    public new DateTimeOffset? CreatedAt => base.CreatedAt;

    /// <summary>
    /// The name of the emoji.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// Is this emoji from a server.
    /// </summary>
    public bool IsServerEmoji
        => !string.IsNullOrEmpty(ServerId);

    /// <summary>
    /// The server id of where the emoji is from.
    /// </summary>
    public string? ServerId { get; internal set; }

    /// <summary>
    /// Server that the Emoji is from.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Server? Server => Client.GetServer(ServerId);

    /// <summary>
    /// The user id that created this emoji.
    /// </summary>
    public string? CreatorId { get; internal set; }

    /// <summary>
    /// Is the emoji animated.
    /// </summary>
    public bool IsAnimated { get; internal set; }

    /// <summary>
    /// Is the emoji not safe for work (+18).
    /// </summary>
    public bool IsNsfw { get; internal set; }

    /// <summary>
    /// The image url of the emoji or empty if unicode.
    /// </summary>
    public string? ImageUrl
        => IsServerEmoji ? Client.Config.Debug.UploadUrl + "/emojis/" + Id : string.Empty;

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Emoji name or unicode </returns>
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// Parse a default emoji by name or fail
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentException"></exception>
    public static string? ParseName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("Name argument for ParseName is empty.");

        if (EmojiList.NameToUnicode.TryGetValue(name, out string emoji))
            return emoji;

        throw new ArgumentException("Could not parse default emoji type from name " + name);
    }

    /// <summary>
    /// Try parse a default emoji by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="emoji"></param>
    /// <returns><see cref="bool"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryParseName(string name, out string? emoji)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("Name argument for ParseName is empty.");

        return EmojiList.NameToUnicode.TryGetValue(name, out emoji);
    }
}