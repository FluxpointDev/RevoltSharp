using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp;

public class PublicBot : CreatedEntity
{
	internal PublicBot(RevoltClient client, PublicBotJson model) : base(client, model.Id)
	{
		Username = model.Username;
		AvatarId = model.AvatarId;
		Description = model.Description;
	}

	public string Username { get; internal set; }

	public string AvatarId { get; internal set; }

	public string Description { get; internal set; }
}
