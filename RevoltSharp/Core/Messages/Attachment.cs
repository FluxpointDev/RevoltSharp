using System;

namespace RevoltSharp;

/// <summary>
/// Revolt file attachment for messages which could by any type including an image.
/// </summary>
public class Attachment : CreatedEntity
{
    private Attachment(RevoltClient client, AttachmentJson model) : base(client, model.Id)
    {
        Tag = model.Tag;
        Filename = model.Filename;
        Size = model.Size;
        Type = model.Metadata.Type;
        Width = model.Metadata.Width;
        Height = model.Metadata.Height;
        Deleted = model.Deleted ?? false;
        Reported = model.Reported ?? false;
    }

    /// <summary>
    /// Id of the attachment.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the attachment was created.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

    /// <summary>
    /// The type of attachment used avatar, banner, icon, ect.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// The original file name of the attachment.
    /// </summary>
    public string Filename { get; internal set; }

    /// <summary>
    /// The file mime type of the attachment.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// The size of the file attachment.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// The width of the image if the file is an image type.
    /// </summary>
    public int? Width { get; }

    /// <summary>
    /// The height of the image if the file is an image type.
    /// </summary>
    public int? Height { get; }

    /// <summary>
    /// File has been deleted.
    /// </summary>
    public bool Deleted { get; }

    /// <summary>
    /// File has been reported by a user.
    /// </summary>
    public bool Reported { get; }

    /// <summary>
    /// The URL of the attachment.
    /// </summary>
    public string GetUrl(int? size = null) => $"{Client.Config.Debug.UploadUrl}{Tag}/{Id}/{Filename}{(size != null ? $"?size={size}" : null)}";

    internal static Attachment? Create(RevoltClient client, AttachmentJson model)
    {
        if (model != null)
            return new Attachment(client, model);
        return null;
    }

    internal AttachmentJson ToJson()
    {
        return new AttachmentJson
        {
            Id = Id,
            Tag = Tag,
            Filename = Filename,
            Metadata = new AttachmentMetaJson
            {
                Type = "Image",
                Height = Height,
                Width = Width
            },
            ContentType = Type,
            Size = Size
        };
    }
}