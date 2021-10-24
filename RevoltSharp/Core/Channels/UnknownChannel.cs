namespace RevoltSharp
{
    public class UnknownChannel : Channel
    {
        public UnknownChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
            Type = ChannelType.Unknown;
        }

        internal override void Update(PartialChannelJson json)
        { }
    }
    public class ServerUnknownChannel : ServerChannel
    {
        public ServerUnknownChannel(RevoltClient client, ChannelJson model)
            : base(client, model)
        {
           
        }

        internal override void Update(PartialChannelJson json)
        { }
    }
}
