using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class EmbedBuilder
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public FileAttachment Image { get; set; }
        public RevoltColor Color { get; set; }

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
    public class Embed
    {
        public EmbedType Type { get; }
        public string Url { get; internal set; }
        public string IconUrl { get; internal set; }
        public string Title { get; internal set; }
        public string Description { get; internal set; }
        public string Site { get; }
        public RevoltColor Color { get; internal set; }
        public Attachment Image { get; internal set; }
        public Attachment Video { get; }
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
