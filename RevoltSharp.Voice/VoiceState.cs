using System.Threading.Tasks;

namespace RevoltSharp;


public class VoiceState
{
    internal VoiceState(VoiceChannel channel, VoiceSocketClient con)
    {
        Connection = con;
        Channel = channel;
    }

    internal VoiceSocketClient Connection;
    public VoiceChannel Channel { get; internal set; }

    public async Task ConnectAsync()
    {
        await Connection.InternalConnectAsync();
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task StopAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        Connection.StopWebSocket = true;
    }
}
