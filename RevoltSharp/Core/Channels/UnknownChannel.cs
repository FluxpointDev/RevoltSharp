namespace RevoltSharp
{
    /// <summary>
    /// Channel is an unknown type that can't be fully used 
    /// </summary>
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

    /// <summary>
    /// Channel is an unknown type that can't be fully used
    /// </summary>
    public class UnknownServerChannel : ServerChannel
    {
        internal UnknownServerChannel(RevoltClient client, ChannelJson model) : base(client, model)
        {
            Type = ChannelType.Unknown;
        }
    }
}
