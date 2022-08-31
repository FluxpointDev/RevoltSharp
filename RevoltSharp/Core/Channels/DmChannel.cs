using System.Collections.Generic;

namespace RevoltSharp
{
    internal class DmChannel : Channel
    {
        internal DmChannel(RevoltClient client, ChannelJson model) : base(client)
        {
            Id = model.Id;
            Type = ChannelType.Dm;
            Active = model.Active;
            Recipents = model.Recipients != null ? model.Recipients : new string[0];
            LastMessageId = model.LastMessageId;
        }

        public bool Active { get; }
        public IReadOnlyList<string> Recipents { get; }
        public string LastMessageId { get; }

        

        internal override void Update(PartialChannelJson json)
        {
        }
    }
}
