using Optionals;

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
        public string Image { get; set; }

        /// <summary>
        /// Embed color
        /// </summary>
        public RevoltColor? Color { get; set; }

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
                Image = Image,
                Color = Color
            };
        }
    }

    public class MessageEmbed
    {
        internal MessageEmbed(EmbedJson model)
        {
            Type = model.type;
            switch (Type)
            {
                case EmbedType.Image:
                    Image = new EmbedMedia(null) { Url = model.url, Height = model.height, Width = model.width };
                    break;
                case EmbedType.Video:
                    Video = new EmbedMedia(null) { Url = model.url, Height = model.height, Width = model.width };
                    break;
            }
            Url = model.url;
            IconUrl = model.icon_url;
            Title = model.title;
            Description = model.description;
            Site = model.site_name;
            if (model.colour.HasValue)
                Color = new RevoltColor(model.colour.Value);
            Image = model.image == null ? null : new EmbedMedia(model.image);
            Media = model.media == null ? null : new EmbedMedia(model.media as EmbedMediaJson);
            Video = model.video == null ? null : new EmbedMedia(model.video);
            Provider = model.special == null ? EmbedProviderType.None : model.special.Type;
        }

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
        public RevoltColor? Color { get; internal set; }

        /// <summary>
        /// Embed image attachment
        /// </summary>
        public EmbedMedia? Image { get; internal set; }

        public EmbedMedia? Media { get; internal set; }

        /// <summary>
        /// Embed video attachment
        /// </summary>
        public EmbedMedia? Video { get; internal set; }

        /// <summary>
        /// Embed provider
        /// </summary>
        public EmbedProviderType Provider { get; }
    }

    /// <summary>
    /// Message embeds
    /// </summary>
    public class Embed
    {
        
        public Embed()
        {

        }

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
        /// Embed image url
        /// </summary>
        public string Image { get; internal set; }

        /// <summary>
        /// Embed color
        /// </summary>
        public RevoltColor Color { get; internal set; } = new RevoltColor("");


        internal EmbedJson ToJson()
        {
            EmbedJson Json = new EmbedJson();
            if (!string.IsNullOrEmpty(IconUrl))
                Json.icon_url = IconUrl;

            if (!string.IsNullOrEmpty(Url))
                Json.url = Url;

            if (!string.IsNullOrEmpty(Title))
                Json.title = Title;

            if (!string.IsNullOrEmpty(Description))
                Json.description = Description;

            if (!string.IsNullOrEmpty(Image))
                Json.media = Image;

            if (Color != null && !Color.IsEmpty)
                Json.colour = Optional.Some(Color.Hex);

            return Json;
        }
    }
    public class EmbedMedia
    {
        internal EmbedMedia(EmbedMediaJson model)
        {
            if (model == null)
                return;
            Url = model.url;
            Width = model.width;
            Height = model.height;
        }
        public string Url;
        public int Width;
        public int Height;
    }
    public enum EmbedType
    {
        None, Website, Image, Video, Text
    }
    public enum EmbedProviderType
    {
        None, GIF, Lightspeed, YouTube, Twitch, Spotify, Soundcloud, Bandcamp
    }
}
