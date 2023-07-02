using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp;

public class VoiceSocketClient
{
	public VoiceSocketClient(RevoltClient client, string token)
	{
		Token = token;
	}
	internal string Token;
	internal bool StopWebSocket = false;

	internal RevoltClient RevoltClient;
	internal ClientWebSocket? WebSocket;
	internal CancellationToken CancellationToken = new CancellationToken();
	internal async Task InternalConnectAsync()
	{
		StopWebSocket = false;
		while (!CancellationToken.IsCancellationRequested && !StopWebSocket)
		{
			using (WebSocket = new ClientWebSocket())
			{
				try
				{
					Uri uri = new Uri($"{RevoltClient.Config.Debug.WebsocketUrl}?format=json&version=1");

					if (!string.IsNullOrEmpty(RevoltClient.Config.CfClearance))
					{
						WebSocket.Options.Cookies = new System.Net.CookieContainer();
						WebSocket.Options.Cookies.SetCookies(uri, $"cf_clearance={RevoltClient.Config.CfClearance}");
					}
					WebSocket.Options.SetRequestHeader("User-Agent", RevoltClient.Config.UserAgent);

					await WebSocket.ConnectAsync(uri, CancellationToken);
					//await Send(WebSocket, JsonConvert.SerializeObject(new AuthenticateRequest(Client.Token)), CancellationToken);
					//_firstError = true;
					//await Receive(WebSocket, CancellationToken);
				}
				catch (ArgumentException)
				{
					//if (_firstConnected)
					//	Client.InvokeLogAndThrowException("Client config WebsocketUrl is an invalid format.");
				}
				catch (WebSocketException we)
				{
					//if (_firstConnected)
					//{
					//	if (we.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
					//		Client.InvokeLogAndThrowException("Failed to connect to Revolt, the instance may be down or having issues.");

					//	Client.InvokeLogAndThrowException("Failed to connect to Revolt.");
					//}
					//else
					//{
					//	Console.WriteLine($"--- WebSocket Internal Error {we.ErrorCode} {we.WebSocketErrorCode} ---\n" + $"{we}");
					//}

				}
				catch (Exception ex)
				{
					Console.WriteLine("--- WebSocket Exception ---\n" + $"{ex}");
					//if (_firstConnected)
					//	Client.InvokeLogAndThrowException("Failed to connect to Revolt.");
				}
				//await Task.Delay(_firstError ? 3000 : 10000, CancellationToken);
				//_firstError = false;
			}
		}
	}

}
