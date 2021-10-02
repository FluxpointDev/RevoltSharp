namespace RevoltSharp
{
    internal class SavedMessagesChannel : Channel
    {
        public string User { get; }

        internal SavedMessagesChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
            User = model.User;
        }

        internal override void Update(PartialChannelJson json)
        { }
    }
}
