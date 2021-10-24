using Optional.Unsafe;

namespace RevoltSharp
{
    public class TextChannel : ServerChannel
    {
        public string LastMessageId { get; internal set; }

        public bool IsNsfw { get; internal set; }

        public TextChannel(RevoltClient client, ChannelJson model)
            : base(client, model)
        {
            Type = ChannelType.Text;
            LastMessageId = model.LastMessageId;
            IsNsfw = model.Nsfw;
        }
    }
}
