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
            if (string.IsNullOrEmpty(client.Config.Debug.WEBSOCKET_URL))
                throw new RevoltException("Client config WEBSOCKET_URL can not be empty.");

            if (!Uri.IsWellFormedUriString(client.Config.Debug.WEBSOCKET_URL, UriKind.Absolute))
                throw new RevoltException("Client config WEBSOCKET_URL is an invalid format.");
        }

        public RevoltClient Client { get; private set; }
        internal ClientWebSocket WebSocket;
        internal bool FirstConnected = true;
        internal CancellationToken CancellationToken = new CancellationToken();
        internal ConcurrentDictionary<string, Server> ServerCache = new ConcurrentDictionary<string, Server>();
        internal ConcurrentDictionary<string, Channel> ChannelCache = new ConcurrentDictionary<string, Channel>();
        internal ConcurrentDictionary<string, User> Usercache = new ConcurrentDictionary<string, User>();
        internal SelfUser CurrentUser;
        internal bool FirstError = true;
        internal async Task SetupWebsocket()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                using (WebSocket = new ClientWebSocket())
                {
                    try
                    {
                        await WebSocket.ConnectAsync(new Uri(Client.Config.Debug.WEBSOCKET_URL + "?format=json"), CancellationToken);
                        await Send(WebSocket, Newtonsoft.Json.JsonConvert.SerializeObject(new AuthenticateRequest { token = Client.Token }), CancellationToken);
                        FirstError = true;
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
                    await Task.Delay(FirstError ? 3000 : 10000);
                    FirstError = false;
                }
            }
        }

        private async Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken) =>
        await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        private async Task Receive(ClientWebSocket socket, CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            while (!cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
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
                        WebSocketMessage(await reader.ReadToEndAsync());
                    }
                }
            };
        }

        internal Task Heartbeat;

        internal class AuthenticateRequest
        {
            public string type = "Authenticate";
            public string token;
        }
        internal class HeartbeatRequest
        {
            public string type = "Ping";
            public int data = 20000;
        }

        private async Task WebSocketMessage(string json)
        {
            JToken Payload = Newtonsoft.Json.JsonConvert.DeserializeObject<JToken>(json);
            if (Client.Config.Debug.LogWebSocketFull)
                Console.WriteLine("--- WebSocket Response Json ---\n" + json);
            switch (Payload["type"].ToString())
            {
                case "Authenticated":
                    if (FirstConnected)
                        Console.WriteLine("Revolt WebSocket Connected!");
                    else
                        Console.WriteLine("Revolt WebSocket Reconnected!");

                    FirstConnected = false;
                    Send(WebSocket, Newtonsoft.Json.JsonConvert.SerializeObject(new HeartbeatRequest()), CancellationToken);

                    Heartbeat = Task.Run(async () =>
                    {
                        while (!CancellationToken.IsCancellationRequested)
                        {
                            await Task.Delay(50000);
                            await Send(WebSocket, Newtonsoft.Json.JsonConvert.SerializeObject(new HeartbeatRequest()), CancellationToken);
                        }
                    });
                    break;
                case "Pong":
                    {

                    }
                    break;
                case "Error":
                    {
                        ErrorEventJson Event = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorEventJson>(json);
                        Console.WriteLine("--- WebSocket Error ---\n" + json);
                        if (Event.Error == WebSocketErrorType.InvalidSession)
                        {
                            if (FirstConnected)
                                Console.WriteLine("WebSocket session is invalid, check if your bot token is correct.");
                            else
                                Console.WriteLine("WebSocket session was invalidated!");
                            await Client.StopAsync();
                        }
                        Client.InvokeWebSocketError(new WebSocketError { Messaage = Event.Message, Type = Event.Error });
                    }
                    break;
                case "Ready":
                    {
                        try
                        {
                            ReadyEventJson Event = Payload.ToObject<ReadyEventJson>(Client.Serializer);
                            Usercache = new ConcurrentDictionary<string, User>(Event.users.ToDictionary(x => x.Id, x => new User(Client, x)));
                            CurrentUser = SelfUser.CreateSelf(Usercache.Values.FirstOrDefault(x => x.Relationship == "User" && x.BotData != null));
                            if (CurrentUser == null)
                            {
                                Console.WriteLine("Fatal RevoltSharp error, could not load bot user.\n" +
                                    "WebSocket connection has been stopped.");
                                await Client.StopAsync();
                            }

                            ServerCache = new ConcurrentDictionary<string, Server>(Event.servers.ToDictionary(x => x.Id, x => new Server(Client, x)));
                            ChannelCache = new ConcurrentDictionary<string, Channel>(Event.channels.ToDictionary(x => x.Id, x => Channel.Create(Client, x)));
                            Console.WriteLine("Revolt WebSocket Ready!");
                            Client.InvokeReady(CurrentUser);

                            // This task will cleanup extra group channels where the bot is only a member of.
                            _ = Task.Run(async () =>
                            {
                                foreach (GroupChannel c in ChannelCache.Values.Where(x => x is GroupChannel))
                                {
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
                        MessageEventJson Event = Payload.ToObject<MessageEventJson>(Client.Serializer);
                        if (Event.Author != "00000000000000000000000000" && !Usercache.ContainsKey(Event.Author))
                        {
                            User User = await Client.Rest.GetUserAsync(Event.Author);
                            Usercache.TryAdd(Event.Author, User);
                        }
                        if (!ChannelCache.ContainsKey(Event.Channel))
                        {
                            Channel Channel = await Client.Rest.GetChannelAsync(Event.Channel);
                            ChannelCache.TryAdd(Event.Channel, Channel);
                        }
                        Client.InvokeMessageRecieved(Event.ToEntity(Client));
                    }
                    break;
                case "MessageUpdate":
                    {
                        MessageUpdateEventJson Event = Payload.ToObject<MessageUpdateEventJson>(Client.Serializer);
                        if (Event.data.Author == "00000000000000000000000000")
                            return;

                        ChannelCache.TryGetValue(Event.channel, out Channel Channel);
                        if (Channel == null)
                        {
                            Channel = await Client.Rest.GetChannelAsync(Event.channel);
                            ChannelCache.TryAdd(Channel.Id, Channel);
                        }
                        Client.InvokeMessageUpdated(Channel, Event.id, Event.data.Content?.ToString());
                    }
                    break;
                case "MessageDelete":
                    {
                        MessageDeleteEventJson Event = Payload.ToObject<MessageDeleteEventJson>(Client.Serializer);
                        
                        ChannelCache.TryGetValue(Event.channel_id, out Channel Channel);
                        if (Channel == null)
                        {
                            Channel = await Client.Rest.GetChannelAsync(Event.channel_id);
                            ChannelCache.TryAdd(Event.channel_id, Channel);
                        }
                        Client.InvokeMessageDeleted(Channel, Event.id);
                    }
                    break;

                case "ChannelCreate":
                    {
                        ChannelEventJson Event = Payload.ToObject<ChannelEventJson>(Client.Serializer);
                        Channel Chan = Channel.Create(Client, Event);
                        ChannelCache.TryAdd(Chan.Id, Chan);
                        if (!string.IsNullOrEmpty(Event.Server))
                        {
                            ServerCache.TryGetValue(Event.Server, out Server server);
                            server.ChannelIds.Add(Chan.Id);
                        }
                        Client.InvokeChannelCreated(Chan);
                    }
                    break;
                case "ChannelUpdate":
                    {
                        ChannelUpdateEventJson Event = Payload.ToObject<ChannelUpdateEventJson>(Client.Serializer);
                        if (ChannelCache.TryGetValue(Event.id, out Channel Chan))
                        {
                            if (Event.clear.HasValue)
                            {
                                if (Event.data == null)
                                    Event.data = new PartialChannelJson();
                                switch (Event.clear.ValueOrDefault())
                                {
                                    case "Icon":
                                        Event.data.Icon = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Description":
                                        Event.data.Description = Option.Some<string>(null);
                                        break;
                                }
                            }

                            Channel Clone = Chan.Clone();
                            Chan.Update(Event.data);
                            Client.InvokeChannelUpdated(Clone, Chan);
                        }
                    }
                    break;
                case "ChannelDelete":
                    {
                        ChannelDeleteEventJson Event = Payload.ToObject<ChannelDeleteEventJson>(Client.Serializer);
                        ChannelCache.TryRemove(Event.id, out Channel Chan);
                        if (Chan is ServerChannel SC)
                        {
                            if (ServerCache.TryGetValue(SC.ServerId, out Server Server))
                            {
                                Server.ChannelIds.Remove(Event.id);
                            }
                        }
                        Client.InvokeChannelDeleted(Chan);
                    }
                    break;
                case "ChannelGroupJoin":
                    {
                        ChannelGroupJoinEventJson Event = Payload.ToObject<ChannelGroupJoinEventJson>(Client.Serializer);
                        if (Event.user_id == CurrentUser.Id)
                        {
                            Channel Chan = await Client.Rest.GetChannelAsync(Event.id);
                            ChannelCache.TryAdd(Event.id, Chan);
                            Client.InvokeGroupJoined((GroupChannel)Chan, CurrentUser);
                        }
                        else
                        {
                            Usercache.TryGetValue(Event.user_id, out User User);
                            if (User == null)
                            {
                                User = await Client.Rest.GetUserAsync(Event.user_id);
                                Usercache.TryAdd(User.Id, User);
                            }
                            Client.InvokeGroupUserJoined((GroupChannel)ChannelCache[Event.id], User);
                        }
                    }
                    break;
                case "ChannelGroupLeave":
                    {
                        ChannelGroupLeaveEventJson Event = Payload.ToObject<ChannelGroupLeaveEventJson>(Client.Serializer);
                        if (Event.user_id == CurrentUser.Id)
                        {
                            ChannelCache.TryRemove(Event.id, out Channel Chan);
                            Client.InvokeGroupLeft((GroupChannel)Chan, CurrentUser);
                        }
                        else
                        {
                            Usercache.TryGetValue(Event.user_id, out User User);
                            Client.InvokeGroupUserLeft((GroupChannel)ChannelCache[Event.id], User);
                        }
                    }
                    break;


                case "ServerUpdate":
                    {
                        ServerUpdateEventJson Event = Payload.ToObject<ServerUpdateEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(Event.id, out Server Server))
                        {
                            if (Event.clear.HasValue)
                            {
                                if (Event.data == null)
                                    Event.data = new PartialServerJson();
                                switch (Event.clear.ValueOrDefault())
                                {
                                    case "Icon":
                                        Event.data.Icon = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Banner":
                                        Event.data.Banner = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Description":
                                        Event.data.Description = Option.Some<string>(null);
                                        break;
                                }
                            }
                            Server Cloned = Server.Clone();
                            Server.Update(Event.data);
                            Client.InvokeServerUpdated(Cloned, Server);
                        }
                    }
                    break;
                case "ServerDelete":
                    {
                        ServerDeleteEventJson Event = Payload.ToObject<ServerDeleteEventJson>(Client.Serializer);
                        ServerCache.TryRemove(Event.id, out Server Server);
                        foreach(var c in Server.ChannelIds)
                        {
                            ChannelCache.TryRemove(c, out Channel Chan);
                        }
                        Client.InvokeServerLeft(Server);
                    }
                    break;
                case "ServerMemberUpdate":
                    {
                        ServerMemberUpdateEventJson Event = Payload.ToObject<ServerMemberUpdateEventJson>(Client.Serializer);
                        //Console.WriteLine("Server Member Update - " + json);
                    }
                    break;
                case "ServerMemberJoin":
                    {
                        ServerMemberJoinEventJson Event = Payload.ToObject<ServerMemberJoinEventJson>(Client.Serializer);
                        if (Event.user_id == CurrentUser.Id)
                        {
                            Server Server = await Client.Rest.GetServerAsync(Event.id);
                            ServerCache.TryAdd(Event.id, Server);
                            Client.InvokeServerJoined(Server, CurrentUser);
                        }
                        else
                        {
                            Usercache.TryGetValue(Event.user_id, out User User);
                            if (User == null)
                            {
                                User = await Client.Rest.GetUserAsync(Event.user_id);
                                Usercache.TryAdd(User.Id, User);
                            }
                            Client.InvokeMemberJoined(ServerCache[Event.id], User);
                        }
                    }
                    break;
                case "ServerMemberLeave":
                    {
                        ServerMemberLeaveEventJson Event = Payload.ToObject<ServerMemberLeaveEventJson>(Client.Serializer);
                        if (Event.user_id == CurrentUser.Id)
                        {
                            ServerCache.TryRemove(Event.id, out Server Server);
                            foreach (var c in Server.ChannelIds)
                            {
                                ChannelCache.TryRemove(c, out Channel Chan);
                            }
                            Client.InvokeServerLeft(Server);
                        }
                        else
                        {
                            Usercache.TryGetValue(Event.user_id, out User User);
                            if (User == null)
                            {
                                User = await Client.Rest.GetUserAsync(Event.user_id);
                                Usercache.TryAdd(User.Id, User);
                            }
                            Client.InvokeMemberLeft(ServerCache[Event.id], User);
                        }
                    }
                    break;
                case "ServerRoleUpdate":
                    {
                        ServerRoleUpdateEventJson Event = Payload.ToObject<ServerRoleUpdateEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(Event.id, out Server server))
                        {
                            if (server.Roles.TryGetValue(Event.role_id, out Role role))
                            {
                                Role Cloned = role.Clone();
                                role.Update(Event.data);
                                Client.InvokeRoleUpdated(Cloned, role);
                            }
                            else
                            {
                                Role Role = new Role(Client, Event.data, Event.id, Event.role_id);
                                server.Roles.TryAdd(Event.role_id, Role);
                                Client.InvokeRoleCreated(Role);
                            }
                        }
                    }
                    break;
                case "ServerRoleDelete":
                    {
                        ServerRoleDeleteEventJson Event = Payload.ToObject<ServerRoleDeleteEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(Event.id, out Server server))
                        {
                            server.Roles.TryRemove(Event.role_id, out Role role);
                            Client.InvokeRoleDeleted(role);
                        }
                    }
                    break;
                case "UserUpdate":
                    {
                        UserUpdateEventJson Event = Payload.ToObject<UserUpdateEventJson>(Client.Serializer);
                        if (Usercache.TryGetValue(Event.id, out User User))
                        {
                            if (Event.clear.HasValue)
                            {
                                switch (Event.clear.ValueOrDefault())
                                {
                                    case "ProfileContent":
                                        Event.data.ProfileContent = Option.Some<string>("");
                                        break;
                                    case "StatusText":
                                        Event.data.status = Option.Some<UserStatusJson>(null);
                                        break;
                                    case "ProfileBackground":
                                        Event.data.ProfileBackground = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Avatar":
                                        Event.data.avatar = Option.Some<AttachmentJson>(null);
                                        break;
                                }
                            }
                            if (Event.id == CurrentUser.Id)
                            {
                                SelfUser Cloned = CurrentUser.Clone();
                                User.Update(Event.data);
                                CurrentUser.Update(Event.data);
                                Client.InvokeCurrentUserUpdated(Cloned, CurrentUser);
                            }
                            else
                            {
                                User Cloned = User.Clone();
                                User.Update(Event.data);
                                Client.InvokeUserUpdated(Cloned, User);
                            }
                        }
                    }
                    break;
                case "UserRelationship":
                    {
                        UserRelationshipEventJson Event = Payload.ToObject<UserRelationshipEventJson>(Client.Serializer);
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
