namespace RevoltSharp;

public static class VoiceHelper
{
	public static async Task<VoiceRequest> JoinChannelAsync(this VoiceChannel channel)
	{
		VoiceRequestJson json = await channel.Client.Rest.PostAsync<VoiceRequestJson>($"/channels/{channel.Id}/join_call");
		return new VoiceRequest(json);
	}
}
