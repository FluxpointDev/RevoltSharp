namespace RevoltSharp;

public static class VoiceHelper
{
	public static async Task<VoiceState> JoinChannelAsync(this VoiceChannel channel)
	{
		VoiceRequestJson json = await channel.Client.Rest.PostAsync<VoiceRequestJson>($"/channels/{channel.Id}/join_call");
		VoiceState State = new VoiceState(channel, new VoiceSocketClient(channel.Client, json.token));
		await State.ConnectAsync();


		return State;
	}
}
