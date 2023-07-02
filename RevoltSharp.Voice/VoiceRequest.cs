namespace RevoltSharp;
public class VoiceRequest
{
	internal VoiceRequest(VoiceRequestJson model)
	{
		Token = model.token;
	}

	public string Token { get; internal set; }
}
