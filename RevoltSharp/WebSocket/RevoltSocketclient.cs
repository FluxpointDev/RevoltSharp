using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Optional;
using Optional.Unsafe;
using RevoltSharp.Rest;
using RevoltSharp.WebSocket.Events;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoltSharp.WebSocket
{
    internal class RevoltSocketclient
    {
        public RevoltSocketclient(RevoltClient client)
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
                        if (Event.error == "InvalidSession")
                        {
                            await Client.StopAsync();
                            if (FirstConnected)
                                Console.WriteLine("WebSocket session is invalid, check if your bot token is correct.");
                            else
                                Console.WriteLine("WebSocket session is invalid!");
                        }
                    }
                    break;
                case "Ready":
                    {
                        try
                        {
                            ReadyEventJson Event = Payload.ToObject<ReadyEventJson>(Client.Serializer);
                            Usercache = new ConcurrentDictionary<string, User>(Event.users.ToDictionary(x => x.id, x => x.ToEntity()));
                            CurrentUser = SelfUser.CreateSelf(Usercache.Values.FirstOrDefault(x => x.Relationship == "User" && x.BotData != null));
                            if (CurrentUser == null)
                            {
                                Console.WriteLine("Fatal RevoltSharp error, could not load bot user.\n" +
                                    "WebSocket connection has been stopped.");
                                await Client.StopAsync();
                            }
                            ServerCache = new ConcurrentDictionary<string, Server>(Event.servers.ToDictionary(x => x.id, x => x.ToEntity(Client)));
                            ChannelCache = new ConcurrentDictionary<string, Channel>(Event.channels.ToDictionary(x => x.id, x => x.ToEntity(Client)));
                            
                            Client.InvokeReady(CurrentUser);

                            // This task will cleanup extra group channels where the bot is only a member of.
                            _ = Task.Run(async () =>
                            {
                                foreach (Channel c in ChannelCache.Values.Where(x => x.Type == ChannelType.Group))
                                {
                                    GroupChannel GC = (GroupChannel)c;
                                    if (GC.Recipents.Length == 1)
                                    {
                                        await GC.DeleteChannelAsync();
                                    }
                                }
                            });
                        }
                        catch
                        {
                            Console.WriteLine("Fatal RevoltSharp error, could not parse ready event.\n" +
                                "WebSocket connection has been stopped.");
                            await Client.StopAsync();
                        }
                    }
                    break;
                case "Message":
                    {
                        MessageEventJson Event = Payload.ToObject<MessageEventJson>(Client.Serializer);
                        if (!Usercache.ContainsKey(Event.author))
                        {
                            User User = await Client.Rest.GetUserAsync(Event.author);
                            Usercache.TryAdd(Event.author, User);
                        }
                        if (!ChannelCache.ContainsKey(Event.channel))
                        {
                            Channel Channel = await Client.Rest.GetChannelAsync(Event.channel);
                            ChannelCache.TryAdd(Event.channel, Channel);
                        }
                        Client.InvokeMessageRecieved(Event.ToEntity(Client));
                        
                    }
                    break;
                case "MessageUpdate":
                    {
                        MessageUpdateEventJson Event = Payload.ToObject<MessageUpdateEventJson>(Client.Serializer);
                        if (!Usercache.ContainsKey(Event.data.author))
                        {
                            User User = await Client.Rest.GetUserAsync(Event.data.author);
                            Usercache.TryAdd(Event.data.author, User);
                        }
                        if (!ChannelCache.ContainsKey(Event.data.channel))
                        {
                            Channel Channel = await Client.Rest.GetChannelAsync(Event.data.channel);
                            ChannelCache.TryAdd(Event.data.channel, Channel);
                        }
                        Client.InvokeMessageUpdated(Message.Create(Client,Event.data));
                    }
                    break;
                case "MessageDelete":
                    {
                        MessageDeleteEventJson Event = Payload.ToObject<MessageDeleteEventJson>(Client.Serializer);
                        if (!ChannelCache.ContainsKey(Event.channel_id))
                        {
                            Channel Channel = await Client.Rest.GetChannelAsync(Event.channel_id);
                            if (Channel == null)
                                return;
                            ChannelCache.TryAdd(Event.channel_id, Channel);
                            if (Channel.IsServer && !ServerCache.ContainsKey(Channel.ServerId))
                            {
                                Server server = await Client.Rest.GetServerAsync(Channel.ServerId);
                                if (server == null)
                                    return;
                                ServerCache.TryAdd(Channel.ServerId, server);
                            }
                        }
                        Client.InvokeMessageDeleted(ChannelCache[Event.channel_id], Event.id);
                    }
                    break;


                case "ChannelCreate":
                    {
                        ChannelEventJson Event = Payload.ToObject<ChannelEventJson>(Client.Serializer);
                        Channel Chan = Event.ToEntity(Client);
                        ChannelCache.TryAdd(Event.id, Chan);
                        if (Chan.IsServer)
                        {
                            ServerCache.TryGetValue(Event.server, out Server server);
                            if (server == null)
                            {
                                server = await Client.Rest.GetServerAsync(Event.server);
                                if (server == null)
                                    return;
                                ServerCache.TryAdd(Event.server, server);
                            }
                            server.ChannelIds.Add(Event.id);
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
                                        Event.data.icon = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Description":
                                        Event.data.description = Option.Some<string>(null);
                                        break;
                                }
                            }

                            Channel Clone = Chan.Clone();
                            Chan.Update(Event.data);
                            Client.InvokeChannelUpdated(Clone, Chan);
                        }
                        else
                        {
                            Channel GetChan = await Client.Rest.GetChannelAsync(Event.id);
                            if (GetChan == null)
                                return;
                            ChannelCache.TryAdd(Event.id, GetChan);
                            Client.InvokeChannelUpdated(GetChan, GetChan);
                        }
                    }
                    break;
                case "ChannelDelete":
                    {
                        ChannelDeleteEventJson Event = Payload.ToObject<ChannelDeleteEventJson>(Client.Serializer);
                        Console.WriteLine("Removed channel");
                        ChannelCache.TryRemove(Event.id, out Channel Chan);
                        if (Chan == null)
                            return;

                        if (Chan.IsServer)
                        {
                            if (ServerCache.TryRemove(Chan.ServerId, out Server Server))
                            {
                                Server.ChannelIds.Remove(Event.id);
                            }
                            else
                            {
                                Server server = await Client.Rest.GetServerAsync(Chan.ServerId);
                                if (server == null)
                                    return;
                                ServerCache.TryAdd(Chan.ServerId, server);
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
                        }
                        Client.InvokeGroupJoined((GroupChannel)ChannelCache[Event.id], Event.user_id);
                    }
                    break;
                case "ChannelGroupLeave":
                    {
                        ChannelGroupLeaveEventJson Event = Payload.ToObject<ChannelGroupLeaveEventJson>(Client.Serializer);
                        if (Event.user_id == CurrentUser.Id)
                        {
                            Console.WriteLine("LEFT GROUP");
                            ChannelCache.TryRemove(Event.id, out Channel Chan);
                        }
                        Client.InvokeGroupLeft((GroupChannel)ChannelCache[Event.id], Event.user_id);
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
                                        Event.data.icon = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Banner":
                                        Event.data.banner = Option.Some<AttachmentJson>(null);
                                        break;
                                    case "Description":
                                        Event.data.description = Option.Some<string>(null);
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
                            Client.InvokeServerJoined(Server);
                        }
                        else
                        {
                            Client.InvokeMemberJoined(ServerCache[Event.id], Event.user_id);
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
                            Client.InvokeMemberLeft(ServerCache[Event.id], Event.user_id);
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
                                Role Role = Event.data.ToEntity(Client, Event.id, Event.role_id);
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
                            User.Update(Event.data);
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
