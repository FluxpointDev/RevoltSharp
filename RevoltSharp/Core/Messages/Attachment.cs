namespace RevoltSharp
{
    /// <summary>
    /// Revolt file attachment for messages which could by any type including an image.
    /// </summary>
    public class Attachment : Entity
    {
        /// <summary>
        /// Id of the attachment file.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The type of attachment used avatar, banner, icon, ect.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// The original file name of the attachment.
        /// </summary>
        public string Filename { get; }

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
        public int Width { get; }

        /// <summary>
        /// The height of the image if the file is an image type.
        /// </summary>
        public int Height { get; }

        public Attachment(RevoltClient client, AttachmentJson model)
            : base(client)
        {
            Id = model.Id;
            Tag = model.Tag;
            Filename = model.Filename;
            Type = model.Metadata.Type;
            Size = model.Size;
            Width = model.Metadata.Width;
            Height = model.Metadata.Height;
        }
    }
}
