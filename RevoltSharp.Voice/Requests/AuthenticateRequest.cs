using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{

	internal class AuthenticateRequest
	{
		internal AuthenticateRequest(string channelId, string token)
		{
			data = new AuthDataRequest
			{
				roomId = channelId,
				token = token
			};
		}
		public int id = 3;
		public string type = "Authenticate";
		public AuthDataRequest data;
	}
	internal class AuthDataRequest
	{
		public string roomId;
		public string token;
	}
	internal class RoomInfoRequest
	{
		public int id = 13;
		public string type = "RoomInfo";
	}
}