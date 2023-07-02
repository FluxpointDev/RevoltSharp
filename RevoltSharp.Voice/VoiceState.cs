using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp;

public class VoiceState
{
	internal VoiceState(VoiceChannel channel, VoiceSocketClient con)
	{
		Connection = con;
		Channel = channel;
	}

	internal VoiceSocketClient Connection;
	public VoiceChannel Channel { get; internal set; }

	public async Task ConnectAsync()
	{

	}

	public async Task StopAsync()
	{

	}

}
