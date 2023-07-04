using Optionals;

namespace RevoltSharp.Rest.Requests
{
	internal class CreateWebhookRequest : IRevoltRequest
	{
		public string name { get; set; }
		public Optional<string> avatar { get; set; }
	}
}