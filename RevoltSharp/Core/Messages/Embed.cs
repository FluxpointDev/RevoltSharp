namespace RevoltSharp
{
    /// <summary>
    /// Create a embed to use for messages
    /// </summary>
    public class EmbedBuilder
    {
        /// <summary>
        /// Embed title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Embed url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Embed icon url
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Embed description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Embed image attachment
        /// </summary>
        public FileAttachment Image { get; set; }

        /// <summary>
        /// Embed color
        /// </summary>
        public RevoltColor Color { get; set; }

        /// <summary>
        /// Build the embed to use it in messages
        /// </summary>
        /// <returns><see cref="Embed" /></returns>
        public Embed Build()
        {
            return new Embed
            {
                Title = Title,
                Url = Url,
                IconUrl = IconUrl,
                Description = Description,
                Image = Image == null ? null : new Attachment(null, null)
                {
                    Filename = Image.Id
                },
                Color = Color
            };
        }
    }

    /// <summary>
    /// Message embeds
    /// </summary>
    public class Embed
    {
        /// <summary>
        /// Type of embed
        /// </summary>
        public EmbedType Type { get; }

        /// <summary>
        /// Embed url
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// Embed icon url
        /// </summary>
        public string IconUrl { get; internal set; }

        /// <summary>
        /// Embed title
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// Embed description
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Embed site name
        /// </summary>
        public string Site { get; }

        /// <summary>
        /// Embed color
        /// </summary>
        public RevoltColor Color { get; internal set; }

        /// <summary>
        /// Embed image attachment
        /// </summary>
        public Attachment Image { get; internal set; }

        /// <summary>
        /// Embed video attachment
        /// </summary>
        public Attachment Video { get; }

        /// <summary>
        /// Embed provider
        /// </summary>
        public EmbedProvider Provider { get; }

        internal EmbedJson ToJson()
        {
            return new EmbedJson
            {
                icon_url = IconUrl,
                url = Url,
                title = Title,
                description = Description,
                media = Image == null ? null : Image.Filename,
                colour = Color?.Hex
            };
        }
    }
    public class EmbedProvider
    {
        public EmbedProviderType Type { get; }
    }
    public enum EmbedType
    {
        None, Website, Image, Text
    }
    public enum EmbedProviderType
    {
        None, YouTube, Twitch, Spotify, Soundcloud, Bandcamp
    }
}
