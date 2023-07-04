using System;

namespace RevoltSharp
{

    /// <summary>
    /// Uploaded file attachment that can be used in other requests such as CreateEmojiAsync
    /// </summary>
    public class FileAttachment : CreatedEntity
    {
        public FileAttachment(RevoltClient client, string id) : base(client, id)
        {

        }

        /// <summary>
        /// Id of the attachment.
        /// </summary>
        public new string Id => base.Id;

        /// <summary>
        /// Date of when the attachment was created.
        /// </summary>
        public new DateTimeOffset CreatedAt => base.CreatedAt;
    }
}