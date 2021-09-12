namespace RevoltSharp
{
    public abstract class Channel
    {
        public string Id { get; internal set; }
        public ChannelType Type { get; internal set; }
        internal string ServerId { get; set; }
        public bool IsServer
            => !string.IsNullOrEmpty(ServerId);

        internal RevoltClient Client { get; set; }

        internal abstract void Update(PartialChannelJson json);

        internal Channel Clone()
        {
            return (Channel)this.MemberwiseClone();
        }
    }
    public enum ChannelType
    {
        Text, Voice, SavedMessages, Group, DM, Unknown
    }
}
