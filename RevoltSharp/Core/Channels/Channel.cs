namespace RevoltSharp
{
    /// <summary>
    /// Revolt channel that can be casted to types <see cref="GroupChannel"/>, <see cref="TextChannel"/>, <see cref="VoiceChannel"/> and <see cref="UnknownChannel"/>
    /// </summary>
    public abstract class Channel : Entity
    {
        public string Id { get; internal set; }
        public ChannelType Type { get; internal set; }

        protected Channel(RevoltClient client)
            : base(client)
        { }

        internal abstract void Update(PartialChannelJson json);

        internal Channel Clone()
        {
            return (Channel) this.MemberwiseClone();
        }

        public static Channel Create(RevoltClient client, ChannelJson model)
        {
            switch (model.ChannelType)
            {
                case ChannelType.SavedMessages:
                    return new SavedMessagesChannel(client, model);
                case ChannelType.Dm:
                    return new DmChannel(client, model);
                case ChannelType.Group:
                    return new GroupChannel(client, model);
                case ChannelType.Text:
                    return new TextChannel(client, model);
                case ChannelType.Voice:
                    return new VoiceChannel(client, model);
                default:
                {
                    if (!string.IsNullOrEmpty(model.Server))
                        return new ServerUnknownChannel(client, model);

                    return new UnknownChannel(client, model);
                }
            }
        }
    }
}
