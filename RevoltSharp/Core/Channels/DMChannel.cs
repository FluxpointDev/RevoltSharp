namespace RevoltSharp
{
    internal class DMChannel : Channel
    {
        public bool Active { get; internal set; }
        public string[] Recipents { get; internal set; }
        public string LastMessageId { get; internal set; }

        internal override void Update(PartialChannelJson json)
        {
            
        }
    }
}
