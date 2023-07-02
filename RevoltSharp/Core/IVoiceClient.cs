using System.Threading.Tasks;

namespace RevoltSharp;
public interface IVoiceClient
{
	Task StartAsync();

	Task StopAsync();
}
