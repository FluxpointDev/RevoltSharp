using Optional.Unsafe;
using System;

namespace RevoltSharp
{
    public class TextChannel : ServerChannel
    {
        internal TextChannel(RevoltClient client, ChannelJson model)
            : base(client, model)
        {
            Type = ChannelType.Text;
            LastMessageId = model.LastMessageId;
            IsNsfw = model.Nsfw;
        }

        public string LastMessageId { get; internal set; }

        public bool IsNsfw { get; internal set; }

        internal override void Update(PartialChannelJson json)
        {
            if (json.Nsfw.HasValue)
                IsNsfw = json.Nsfw.ValueOrDefault();
            base.Update(json);
        }
    }
}
