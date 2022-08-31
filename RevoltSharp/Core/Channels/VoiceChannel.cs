using Optional.Unsafe;

namespace RevoltSharp
{
    public class VoiceChannel : ServerChannel
    {
        internal VoiceChannel(RevoltClient client, ChannelJson model)
            : base(client, model)
        {
            Type = ChannelType.Voice;
        }
    }
}
