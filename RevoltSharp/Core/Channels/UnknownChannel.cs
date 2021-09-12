namespace RevoltSharp
{
    public class UnknownChannel : Channel
    {
        internal override void Update(PartialChannelJson json)
        {
            
        }
    }
    public class ServerUnknownChannel : UnknownChannel
    {
        public string ServerId { get { return base.ServerId; } internal set { base.ServerId = value; } }
    }
}
