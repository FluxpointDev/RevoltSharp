namespace RevoltSharp
{
    /// <summary>
    /// Server emoji
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

        public string Id { get; internal set; }

        public string Name { get; internal set; }

        public string ServerId { get; internal set; }

        public string CreatorId { get; internal set; }

        public bool IsAnimated { get; internal set; }

        public bool IsNsfw { get; internal set; }

        public string ImageUrl
            => Client.Config.Debug.UploadUrl + "/emojis/" + Id;

    }
}
