namespace RevoltSharp
{
    public class UnknownChannel : Channel
    {
        public UnknownChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
        }

        internal override void Update(PartialChannelJson json)
        { }
    }
    public class ServerUnknownChannel : Channel
    {


        public ServerUnknownChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
        }

        internal override void Update(PartialChannelJson json)
        { }
    }
}
