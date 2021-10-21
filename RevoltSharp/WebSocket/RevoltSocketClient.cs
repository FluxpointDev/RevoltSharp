using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Optional;
using Optional.Unsafe;
using RevoltSharp.WebSocket.Events;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoltSharp.WebSocket
{
    internal class RevoltSocketClient
    {
        public RevoltSocketClient(RevoltClient client)
        {
            Client = client;
            if (string.IsNullOrEmpty(client.Config.Debug.WebsocketUrl))
                throw new RevoltException("Client config WEBSOCKET_URL can not be empty.");

            if (!Uri.IsWellFormedUriString(client.Config.Debug.WebsocketUrl, UriKind.Absolute))
                throw new RevoltException("Client config WEBSOCKET_URL is an invalid format.");
        }

        private RevoltClient Client { get; }

        private bool _firstConnected = true;
        private bool _firstError = true;

        internal ClientWebSocket WebSocket;
        internal CancellationToken CancellationToken = new CancellationToken();
        internal ConcurrentDictionary<string, Server> ServerCache = new ConcurrentDictionary<string, Server>();
        internal ConcurrentDictionary<string, Channel> ChannelCache = new ConcurrentDictionary<string, Channel>();
        internal ConcurrentDictionary<string, User> UserCache = new ConcurrentDictionary<string, User>();
        internal SelfUser CurrentUser;

        internal async Task SetupWebsocket()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                using (WebSocket = new ClientWebSocket())
                {
                    try
                    {
                        await WebSocket.ConnectAsync(new Uri($"{Client.Config.Debug.WebsocketUrl}?format=json"), CancellationToken);
                        await Send(WebSocket, JsonConvert.SerializeObject(new AuthenticateRequest { Token = Client.Token }), CancellationToken);
                        _firstError = true;
                        await Receive(WebSocket, CancellationToken);
                    }
                    catch (ArgumentException ae)
                    {
                        Console.WriteLine("Client config WEBSOCKET_URL is an invalid format.");
                        throw new RevoltException("Client config WEBSOCKET_URL is an invalid format.");
                    }
                    catch (WebSocketException we)
                    {
                        Console.WriteLine("--- WebSocket Error ---\n" + $"{we}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("--- WebSocket Exception ---\n" + $"{ex}");
                    }
                    await Task.Delay(_firstError ? 3000 : 10000, CancellationToken);
                    _firstError = false;
                }
            }
        }

        private Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken)
            => socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        private async Task Receive(ClientWebSocket socket, CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            while (!cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                await using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, cancellationToken);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        await WebSocketMessage(await reader.ReadToEndAsync());
                    }
                }
            }
        }

        internal class AuthenticateRequest
        {
            [JsonProperty("type")]
            public string Type = "Authenticate";

            [JsonProperty("token")]
            public string Token;
        }

        private class HeartbeatRequest
        {
            [JsonProperty("type")]
            public string Type = "Ping";

            [JsonProperty("data")]
            public int Data = 20000;
        }

        private async Task WebSocketMessage(string json)
        {
            var payload = JsonConvert.DeserializeObject<JToken>(json);
            if (Client.Config.Debug.LogWebSocketFull)
                Console.WriteLine("--- WebSocket Response Json ---\n" + json);
            switch (payload["type"].ToString())
            {
                case "Authenticated":
                    if (_firstConnected)
                        Console.WriteLine("Revolt WebSocket Connected!");
                    else
                        Console.WriteLine("Revolt WebSocket Reconnected!");

                    _firstConnected = false;
                    await Send(WebSocket, JsonConvert.SerializeObject(new HeartbeatRequest()), CancellationToken);

                    _ = Task.Run(async () =>
                    {
                        while (!CancellationToken.IsCancellationRequested)
                        {
                            await Task.Delay(50000, CancellationToken);
                            await Send(WebSocket, JsonConvert.SerializeObject(new HeartbeatRequest()), CancellationToken);
                        }
                    }, CancellationToken);
                    break;
                case "Pong":
                    {

                    }
                    break;
                case "Error":
                    {
                        var @event = JsonConvert.DeserializeObject<ErrorEventJson>(json);
                        Console.WriteLine("--- WebSocket Error ---\n" + json);
                        if (@event.Error == WebSocketErrorType.InvalidSession)
                        {
                            if (_firstConnected)
                                Console.WriteLine("WebSocket session is invalid, check if your bot token is correct.");
                            else
                                Console.WriteLine("WebSocket session was invalidated!");
                            await Client.StopAsync();
                        }
                        Client.InvokeWebSocketError(new WebSocketError { Messaage = @event.Message, Type = @event.Error });
                    }
                    break;
                case "Ready":
                    {
                        try
                        {
                            var @event = payload.ToObject<ReadyEventJson>(Client.Serializer);
                            UserCache = new ConcurrentDictionary<string, User>(@event.Users.ToDictionary(x => x.Id, x => new User(Client, x)));
                            CurrentUser = SelfUser.CreateSelf(UserCache.Values.FirstOrDefault(x => x.Relationship == "User" && x.BotData != null));
                            if (CurrentUser == null)
                            {
                                Console.WriteLine("Fatal RevoltSharp error, could not load bot user.\n" +
                                    "WebSocket connection has been stopped.");
                                await Client.StopAsync();
                            }

                            ServerCache = new ConcurrentDictionary<string, Server>(@event.Servers.ToDictionary(x => x.Id, x => new Server(Client, x)));
                            ChannelCache = new ConcurrentDictionary<string, Channel>(@event.Channels.ToDictionary(x => x.Id, x => Channel.Create(Client, x)));
                            Console.WriteLine("Revolt WebSocket Ready!");
                            Client.InvokeReady(CurrentUser);

                            // This task will cleanup extra group channels where the bot is only a member of.
                            _ = Task.Run(async () =>
                            {
                                foreach (var channel in ChannelCache.Values.Where(x => x is GroupChannel))
                                {
                                    var c = channel as GroupChannel;
                                    if (c.Recipents.Count == 1)
                                    {
                                        await c.DeleteChannelAsync();
                                    }
                                }
                            });
                        }
                        catch(Exception ex)
                        {
                            Console.Write(ex);
                            Console.WriteLine("Fatal RevoltSharp error, could not parse ready event.\n" +
                                "WebSocket connection has been stopped.");
                            await Client.StopAsync();
                        }
                    }
                    break;
                case "Message":
                    {
                        var @event = payload.ToObject<MessageEventJson>(Client.Serializer);
                        if (@event.Author != "00000000000000000000000000" && !UserCache.ContainsKey(@event.Author))
                        {
                            var user = await Client.Rest.GetUserAsync(@event.Author);
                            UserCache.TryAdd(@event.Author, user);
                        }
                        if (!ChannelCache.ContainsKey(@event.Channel))
                        {
                            var channel = await Client.Rest.GetChannelAsync(@event.Channel);
                            ChannelCache.TryAdd(@event.Channel, channel);
                        }
                        Client.InvokeMessageRecieved(@event.ToEntity(Client));
                    }
                    break;
                case "MessageUpdate":
                    {
                        var @event = payload.ToObject<MessageUpdateEventJson>(Client.Serializer);
                        if (@event.Data.Author == "00000000000000000000000000")
                            return;

                        ChannelCache.TryGetValue(@event.Channel, out var channel);
                        if (channel == null)
                        {
                            channel = await Client.Rest.GetChannelAsync(@event.Channel);
                            ChannelCache.TryAdd(channel.Id, channel);
                        }
                        Client.InvokeMessageUpdated(channel, @event.Id, @event.Data.Content?.ToString());
                    }
                    break;
                case "MessageDelete":
                    {
                        var @event = payload.ToObject<MessageDeleteEventJson>(Client.Serializer);

                        ChannelCache.TryGetValue(@event.ChannelId, out var channel);
                        if (channel == null)
                        {
                            channel = await Client.Rest.GetChannelAsync(@event.ChannelId);
                            ChannelCache.TryAdd(@event.ChannelId, channel);
                        }
                        Client.InvokeMessageDeleted(channel, @event.Id);
                    }
                    break;

                case "ChannelCreate":
                    {
                        var @event = payload.ToObject<ChannelEventJson>(Client.Serializer);
                        var chan = Channel.Create(Client, @event);
                        ChannelCache.TryAdd(chan.Id, chan);
                        if (!string.IsNullOrEmpty(@event.Server))
                        {
                            ServerCache.TryGetValue(@event.Server, out var server);
                            server.ChannelIds.Add(chan.Id);
                        }
                        Client.InvokeChannelCreated(chan);
                    }
                    break;
                case "ChannelUpdate":
                    {
                        var @event = payload.ToObject<ChannelUpdateEventJson>(Client.Serializer);
                        if (ChannelCache.TryGetValue(@event.Id, out var chan))
                        {
                            if (@event.Clear.HasValue)
                            {
                                if (@event.Data == null)
                                    @event.Data = new PartialChannelJson();
                                switch (@event.Clear.ValueOrDefault())
                                {
                                    case "Icon":
                                        @event.Data.Icon = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Description":
                                        @event.Data.Description = Option.Some<string>(null);
                                        break;
                                }
                            }

                            var clone = chan.Clone();
                            chan.Update(@event.Data);
                            Client.InvokeChannelUpdated(clone, chan);
                        }
                    }
                    break;
                case "ChannelDelete":
                    {
                        var @event = payload.ToObject<ChannelDeleteEventJson>(Client.Serializer);
                        ChannelCache.TryRemove(@event.Id, out var chan);
                        if (chan is ServerChannel sc)
                        {
                            if (ServerCache.TryGetValue(sc.ServerId, out var server))
                            {
                                server.ChannelIds.Remove(@event.Id);
                            }
                        }
                        Client.InvokeChannelDeleted(chan);
                    }
                    break;
                case "ChannelGroupJoin":
                    {
                        var @event = payload.ToObject<ChannelGroupJoinEventJson>(Client.Serializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            var chan = await Client.Rest.GetChannelAsync(@event.Id);
                            ChannelCache.TryAdd(@event.Id, chan);
                            Client.InvokeGroupJoined((GroupChannel)chan, CurrentUser);
                        }
                        else
                        {
                            UserCache.TryGetValue(@event.UserId, out var user);
                            if (user == null)
                            {
                                user = await Client.Rest.GetUserAsync(@event.UserId);
                                UserCache.TryAdd(user.Id, user);
                            }
                            Client.InvokeGroupUserJoined((GroupChannel)ChannelCache[@event.Id], user);
                        }
                    }
                    break;
                case "ChannelGroupLeave":
                    {
                        var @event = payload.ToObject<ChannelGroupLeaveEventJson>(Client.Serializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            ChannelCache.TryRemove(@event.Id, out var chan);
                            Client.InvokeGroupLeft((GroupChannel)chan, CurrentUser);
                        }
                        else
                        {
                            UserCache.TryGetValue(@event.UserId, out var user);
                            Client.InvokeGroupUserLeft((GroupChannel)ChannelCache[@event.Id], user);
                        }
                    }
                    break;


                case "ServerUpdate":
                    {
                        var @event = payload.ToObject<ServerUpdateEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(@event.Id, out var server))
                        {
                            if (@event.Clear.HasValue)
                            {
                                if (@event.Data == null)
                                    @event.Data = new PartialServerJson();
                                switch (@event.Clear.ValueOrDefault())
                                {
                                    case "Icon":
                                        @event.Data.Icon = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Banner":
                                        @event.Data.Banner = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Description":
                                        @event.Data.Description = Option.Some<string>(null);
                                        break;
                                }
                            }
                            var cloned = server.Clone();
                            server.Update(@event.Data);
                            Client.InvokeServerUpdated(cloned, server);
                        }
                    }
                    break;
                case "ServerDelete":
                    {
                        var @event = payload.ToObject<ServerDeleteEventJson>(Client.Serializer);
                        ServerCache.TryRemove(@event.Id, out var server);
                        foreach(var c in server.ChannelIds)
                        {
                            ChannelCache.TryRemove(c, out _);
                        }
                        Client.InvokeServerLeft(server);
                    }
                    break;
                case "ServerMemberUpdate":
                    {
                        var @event = payload.ToObject<ServerMemberUpdateEventJson>(Client.Serializer);
                        //Console.WriteLine("Server Member Update - " + json);
                    }
                    break;
                case "ServerMemberJoin":
                    {
                        var @event = payload.ToObject<ServerMemberJoinEventJson>(Client.Serializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            var server = await Client.Rest.GetServerAsync(@event.Id);
                            ServerCache.TryAdd(@event.Id, server);
                            Client.InvokeServerJoined(server, CurrentUser);
                        }
                        else
                        {
                            UserCache.TryGetValue(@event.UserId, out var user);
                            if (user == null)
                            {
                                user = await Client.Rest.GetUserAsync(@event.UserId);
                                UserCache.TryAdd(user.Id, user);
                            }
                            Client.InvokeMemberJoined(ServerCache[@event.Id], user);
                        }
                    }
                    break;
                case "ServerMemberLeave":
                    {
                        var @event = payload.ToObject<ServerMemberLeaveEventJson>(Client.Serializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            ServerCache.TryRemove(@event.Id, out var server);
                            foreach (var c in server.ChannelIds)
                            {
                                ChannelCache.TryRemove(c, out _);
                            }
                            Client.InvokeServerLeft(server);
                        }
                        else
                        {
                            UserCache.TryGetValue(@event.UserId, out var user);
                            if (user == null)
                            {
                                user = await Client.Rest.GetUserAsync(@event.UserId);
                                UserCache.TryAdd(user.Id, user);
                            }
                            Client.InvokeMemberLeft(ServerCache[@event.Id], user);
                        }
                    }
                    break;
                case "ServerRoleUpdate":
                    {
                        var @event = payload.ToObject<ServerRoleUpdateEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(@event.Id, out var server))
                        {
                            if (server.Roles.TryGetValue(@event.RoleId, out var role))
                            {
                                var cloned = role.Clone();
                                role.Update(@event.Data);
                                Client.InvokeRoleUpdated(cloned, role);
                            }
                            else
                            {
                                var newRole = new Role(Client, @event.Data, @event.Id, @event.RoleId);
                                server.Roles.TryAdd(@event.RoleId, newRole);
                                Client.InvokeRoleCreated(newRole);
                            }
                        }
                    }
                    break;
                case "ServerRoleDelete":
                    {
                        var @event = payload.ToObject<ServerRoleDeleteEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(@event.Id, out var server))
                        {
                            server.Roles.TryRemove(@event.RoleId, out var role);
                            Client.InvokeRoleDeleted(role);
                        }
                    }
                    break;
                case "UserUpdate":
                    {
                        var @event = payload.ToObject<UserUpdateEventJson>(Client.Serializer);
                        if (UserCache.TryGetValue(@event.Id, out var user))
                        {
                            if (@event.Clear.HasValue)
                            {
                                switch (@event.Clear.ValueOrDefault())
                                {
                                    case "ProfileContent":
                                        @event.Data.ProfileContent = Option.Some<string>("");
                                        break;
                                    case "StatusText":
                                        @event.Data.status = Option.Some<UserStatusJson>(null);
                                        break;
                                    case "ProfileBackground":
                                        @event.Data.ProfileBackground = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Avatar":
                                        @event.Data.avatar = Option.Some<AttachmentJson>(null);
                                        break;
                                }
                            }
                            if (@event.Id == CurrentUser.Id)
                            {
                                var cloned = CurrentUser.Clone();
                                user.Update(@event.Data);
                                CurrentUser.Update(@event.Data);
                                Client.InvokeCurrentUserUpdated(cloned, CurrentUser);
                            }
                            else
                            {
                                var cloned = user.Clone();
                                user.Update(@event.Data);
                                Client.InvokeUserUpdated(cloned, user);
                            }
                        }
                    }
                    break;
                case "UserRelationship":
                    {
                        var @event = payload.ToObject<UserRelationshipEventJson>(Client.Serializer);
                    }
                    break;
                case "ChannelStartTyping":
                case "ChannelStopTyping":
                    break;
                default:
                    {
                        if (Client.Config.Debug.LogWebSocketUnknownEvent)
                            Console.WriteLine("--- WebSocket Unknown Event ---\n" + json);
                    }
                    break;
            }

        }

        private static string FormatJsonPretty(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

    }
}
