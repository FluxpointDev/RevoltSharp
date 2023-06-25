using Optionals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest.Requests;
internal class CreateWebhookRequest : IRevoltRequest
{
	public string name { get; set; }
	public Optional<string> avatar { get; set; }
}
