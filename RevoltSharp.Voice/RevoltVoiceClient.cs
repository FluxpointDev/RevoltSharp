namespace RevoltSharp;

public class RevoltVoiceClient : IVoiceClient
{
	public RevoltVoiceClient(RevoltClient client)
	{

		Client = client;
	}

	private RevoltClient Client;

	public async Task StartAsync()
	{
		Client.Logger.LogMessage(Client, "Connecting to Voice Server", RevoltLogSeverity.Info);
	}

	public async Task StopAsync()
	{
		Client.Logger.LogMessage(Client, "Disconnecting from Voice Server", RevoltLogSeverity.Info);
	}
}
