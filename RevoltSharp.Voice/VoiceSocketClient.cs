using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoltSharp;


	public class VoiceSocketClient
	{
		public VoiceSocketClient(RevoltClient client, string channelId, string token)
		{
			RevoltClient = client;
			Token = token;
			ChannelId = channelId;
		}
		internal string Token;
		internal string ChannelId;
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
					Console.WriteLine("RUN VOICE");
					try
					{
						Uri uri = new Uri($"{RevoltClient.Config.Debug.VortextWebsocketUrl}");

						if (!string.IsNullOrEmpty(RevoltClient.Config.CfClearance))
						{
							WebSocket.Options.Cookies = new System.Net.CookieContainer();
							WebSocket.Options.Cookies.SetCookies(uri, $"cf_clearance={RevoltClient.Config.CfClearance}");
						}
						WebSocket.Options.SetRequestHeader("User-Agent", RevoltClient.Config.UserAgent);

						await WebSocket.ConnectAsync(uri, CancellationToken);
						Console.WriteLine("SEND AUTH");
						await Send(WebSocket, JsonConvert.SerializeObject(new AuthenticateRequest(ChannelId, Token)), CancellationToken);
						await Send(WebSocket, JsonConvert.SerializeObject(new RoomInfoRequest()), CancellationToken);
						await Send(WebSocket, JsonConvert.SerializeObject(new InitilizeTransportRequest()), CancellationToken);
						//_firstError = true;

						Console.WriteLine("RUN REC");
						await Receive(WebSocket, CancellationToken);
					}
					catch (ArgumentException ae)
					{
						Console.WriteLine("VOICE FAILED");
						Console.WriteLine(ae);
						//if (_firstConnected)
						//	Client.InvokeLogAndThrowException("Client config WebsocketUrl is an invalid format.");
					}
					catch (WebSocketException we)
					{
						Console.WriteLine("VOICE FAILED");
						Console.WriteLine(we);
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
						Console.WriteLine("VOICE FAILED");
						Console.WriteLine("--- WebSocket Exception ---\n" + $"{ex}");
						//if (_firstConnected)
						//	Client.InvokeLogAndThrowException("Failed to connect to Revolt.");
					}
					Console.WriteLine("VOICE FAILED");
					StopWebSocket = true;
					//await Task.Delay(_firstError ? 3000 : 10000, CancellationToken);
					//_firstError = false;
				}

			}
		}

		private Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken)
			=> socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

		private async Task Receive(ClientWebSocket socket, CancellationToken cancellationToken)
		{
			ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
			while (!cancellationToken.IsCancellationRequested)
			{
				WebSocketReceiveResult result;
				await using (MemoryStream ms = new MemoryStream())
				{
					do
					{
						result = await socket.ReceiveAsync(buffer, cancellationToken);
						ms.Write(buffer.Array, buffer.Offset, result.Count);
					} while (!result.EndOfMessage);

					if (result.MessageType == WebSocketMessageType.Close)
						break;

					ms.Seek(0, SeekOrigin.Begin);
					using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
					{
						_ = WebSocketMessage(await reader.ReadToEndAsync());
					}
				}
			}
		}

		private async Task WebSocketMessage(string json)
		{
			JToken payload = JsonConvert.DeserializeObject<JToken>(json);
			Console.WriteLine("--- Voice Socket Response Json ---\n" + FormatJsonPretty(json));
		}

		private static string FormatJsonPretty(string json)
		{
			dynamic parsedJson = JsonConvert.DeserializeObject(json);
			return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
		}
	}