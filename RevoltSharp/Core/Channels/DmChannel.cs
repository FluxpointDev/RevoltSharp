using System.Collections.Generic;

namespace RevoltSharp
{
    internal class DmChannel : Channel
    {
        public bool Active { get; }
        public IReadOnlyList<string> Recipents { get; }
        public string LastMessageId { get; }

        internal DmChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
            Active = model.Active;
            Recipents = model.Recipients != null ? model.Recipients : new string[0];
            LastMessageId = model.LastMessageId;
        }

        internal override void Update(PartialChannelJson json)
        { }
    }
}
