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
        public string ServerId { get; internal set; }

        public ServerUnknownChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
            ServerId = model.Server;
            Server = client.GetServer(ServerId);
        }

        internal override void Update(PartialChannelJson json)
        { }
    }
}
