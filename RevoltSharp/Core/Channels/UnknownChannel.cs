namespace RevoltSharp
{
    public class UnknownChannel : Channel
    {
        internal UnknownChannel(RevoltClient client, ChannelJson model) : base(client)
        {
            Id = model.Id;
            Type = ChannelType.Unknown;
        }

        internal override void Update(PartialChannelJson json)
        {
        }
    }
    public class ServerUnknownChannel : ServerChannel
    {
        internal ServerUnknownChannel(RevoltClient client, ChannelJson model) : base(client, model)
        {
            Type = ChannelType.Unknown;
        }
    }
}
