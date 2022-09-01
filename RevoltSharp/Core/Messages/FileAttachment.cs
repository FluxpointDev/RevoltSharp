namespace RevoltSharp
{
    /// <summary>
    /// Uploaded file attachment that can be used in other requests such as CreateEmojiAsync
    /// </summary>
    public class FileAttachment
    {
        public FileAttachment(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Attachment id
        /// </summary>
        public string Id { get; internal set; }
    }
}
