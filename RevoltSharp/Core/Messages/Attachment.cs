namespace RevoltSharp
{
    /// <summary>
    /// Revolt file attachment for messages which could by any type including an image.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Id of the attachment file.
        /// </summary>
        public string Id { get; internal set; }
        /// <summary>
        /// The type of attachment used avatar, banner, icon, ect.
        /// </summary>
        public string Tag { get; internal set; }
        /// <summary>
        /// The original file name of the attachment.
        /// </summary>
        public string Filename { get; internal set; }
        /// <summary>
        /// The file mime type of the attachment.
        /// </summary>
        public string Type { get; internal set; }
        /// <summary>
        /// The size of the file attachment.
        /// </summary>
        public int Size { get; internal set; }
        /// <summary>
        /// The width of the image if the file is an image type.
        /// </summary>
        public int Width { get; internal set; }
        /// <summary>
        /// The height of the image if the file is an image type.
        /// </summary>
        public int Height { get; internal set; }

    }
}
