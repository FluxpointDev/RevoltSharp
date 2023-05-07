namespace RevoltSharp;

/// <summary>
/// Channel that members can speak in to other members
/// </summary>
public class VoiceChannel : ServerChannel
{
    internal VoiceChannel(RevoltClient client, ChannelJson model)
        : base(client, model)
    {
        Type = ChannelType.Voice;
    }
}
