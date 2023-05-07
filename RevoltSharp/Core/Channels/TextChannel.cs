namespace RevoltSharp
{
    /// <summary>
    /// Server text channel that members can talk in
    /// </summary>
    public class TextChannel : ServerChannel
    {
        internal TextChannel(RevoltClient client, ChannelJson model)
            : base(client, model)
        {
            Type = ChannelType.Text;
            LastMessageId = model.LastMessageId;
            IsNsfw = model.Nsfw;
        }

        /// <summary>
        /// Last message id to be posted in the channel
        /// </summary>
        public string LastMessageId { get; internal set; }

        /// <summary>
        /// Channel has nsfw content
        /// </summary>
        public bool IsNsfw { get; internal set; }

        internal override void Update(PartialChannelJson json)
        {
            if (json.IsNsfw.HasValue)
                IsNsfw = json.IsNsfw.Value;
            base.Update(json);
        }
    }
}
