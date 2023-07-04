namespace RevoltSharp
{

    /// <summary>
    /// Channel is an unknown type that can't be fully used 
    /// </summary>
    public class UnknownChannel : Channel
    {
        internal UnknownChannel(RevoltClient client, ChannelJson model) : base(client, model)
        {
            Type = ChannelType.Unknown;
        }

        internal override void Update(PartialChannelJson json)
        {
        }

        /// <summary> Returns a string that represents the current object.</summary>
        /// <returns> Unknown Channel </returns>
        public override string ToString()
        {
            return "Unknown Channel";
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