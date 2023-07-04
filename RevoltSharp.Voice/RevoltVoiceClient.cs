using System.Collections.Concurrent;

namespace RevoltSharp
{

	public class RevoltVoiceClient : IVoiceClient
	{
		public RevoltVoiceClient(RevoltClient client)
		{
			Client = client;
		}

		internal RevoltClient Client;

		public ConcurrentDictionary<string, VoiceState> Channels { get; internal set; } = new ConcurrentDictionary<string, VoiceState>();

		public async Task StartAsync()
		{
			Client.Logger.LogMessage(Client, "Connecting to Voice Server", RevoltLogSeverity.Info);

			var Req = await Client.Rest.SendRequestAsync(Rest.RequestType.Get, Client.Config.Debug.VortextUrl);
			if (Req.IsSuccessStatusCode)
				Client.Logger.LogMessage(Client, "Connected to Voice Server!", RevoltLogSeverity.Info);
			else
				Client.Logger.LogMessage(Client, "Failed to connect to Voice Server", RevoltLogSeverity.Warn);



		}

		public async Task StopAsync()
		{
			Client.Logger.LogMessage(Client, "Disconnecting from Voice Server", RevoltLogSeverity.Info);

			foreach (VoiceState s in Channels.Values)
			{
				await s.StopAsync();
				Channels.TryRemove(s.Channel.Id, out _);
			}
		}
	}
}