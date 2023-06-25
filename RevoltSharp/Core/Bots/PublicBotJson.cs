using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp;

internal class PublicBotJson
{
	[JsonProperty("_id")]
	public string Id { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; }

	[JsonProperty("avatar")]
	public string AvatarId { get; set; }

	[JsonProperty("description")]
	public string Description { get; set; }
}
