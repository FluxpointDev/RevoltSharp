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

        private bool _firstConnected { get; set; } = true;
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
                        if (_firstConnected)
                        {
                            Console.WriteLine("Client config WEBSOCKET_URL is an invalid format.");
                            throw new RevoltException("Client config WEBSOCKET_URL is an invalid format.");
                        }
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
            JToken payload = JsonConvert.DeserializeObject<JToken>(json);
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
                        ErrorEventJson @event = JsonConvert.DeserializeObject<ErrorEventJson>(json);
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
                            ReadyEventJson @event = payload.ToObject<ReadyEventJson>(Client.Serializer);
                            UserCache = new ConcurrentDictionary<string, User>(@event.Users.ToDictionary(x => x.Id, x => new User(Client, x)));
                            CurrentUser = SelfUser.CreateSelf(UserCache.Values.FirstOrDefault(x => x.Relationship == "User" && x.BotData != null));
                            if (CurrentUser == null)
                            {
                                Console.WriteLine("Fatal RevoltSharp error, could not load bot user.\n" +
                                    "WebSocket connection has been stopped.");
                                await Client.StopAsync();
                            }
                            // Update bot user from cache to use current user so mutual stuff and values are synced.
                            UserCache.TryUpdate(CurrentUser.Id, CurrentUser, UserCache[CurrentUser.Id]);

                            ServerCache = new ConcurrentDictionary<string, Server>(@event.Servers.ToDictionary(x => x.Id, x => new Server(Client, x)));
                            ChannelCache = new ConcurrentDictionary<string, Channel>(@event.Channels.ToDictionary(x => x.Id, x => Channel.Create(Client, x)));
                           
                            foreach(var m in @event.Members)
                            {
                                ServerCache[m.Id.Server].Members.TryAdd(m.Id.User, new ServerMember(Client, m, UserCache[m.Id.User]));
                            }
                            Console.WriteLine("Revolt WebSocket Ready!");
                            Client.InvokeReady(CurrentUser);

                            // This task will cleanup extra group channels where the bot is only a member of.
                            _ = Task.Run(async () =>
                            {
                                foreach (Channel channel in ChannelCache.Values.Where(x => x is GroupChannel))
                                {
                                    GroupChannel c = channel as GroupChannel;
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
                        MessageEventJson @event = payload.ToObject<MessageEventJson>(Client.Serializer);
                        if (@event.Author != "00000000000000000000000000" && !UserCache.ContainsKey(@event.Author))
                        {
                            User user = await Client.Rest.GetUserAsync(@event.Author);
                            UserCache.TryAdd(@event.Author, user);
                        }
                        if (!ChannelCache.TryGetValue(@event.Channel, out Channel channel))
                        {
                            channel = await Client.Rest.GetChannelAsync(@event.Channel);
                            ChannelCache.TryAdd(@event.Channel, channel);
                        }
                        if (@event.Author != "00000000000000000000000000" && channel is TextChannel TC)
                        {
                            if (!TC.Server.Members.ContainsKey(@event.Author))
                            {
                                await TC.Server.GetMemberAsync(@event.Author);
                            }
                                
                        }
                        Client.InvokeMessageRecieved(@event.ToEntity(Client));
                    }
                    break;
                case "MessageUpdate":
                    {
                        MessageUpdateEventJson @event = payload.ToObject<MessageUpdateEventJson>(Client.Serializer);
                        if (@event.Data.Author == "00000000000000000000000000")
                            return;

                        ChannelCache.TryGetValue(@event.Channel, out Channel channel);
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
                        MessageDeleteEventJson @event = payload.ToObject<MessageDeleteEventJson>(Client.Serializer);

                        ChannelCache.TryGetValue(@event.ChannelId, out Channel channel);
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
                        ChannelEventJson @event = payload.ToObject<ChannelEventJson>(Client.Serializer);
                        Channel chan = Channel.Create(Client, @event);
                        ChannelCache.TryAdd(chan.Id, chan);
                        if (!string.IsNullOrEmpty(@event.Server))
                        {
                            ServerCache.TryGetValue(@event.Server, out Server server);
                            server.ChannelIds.Add(chan.Id);
                        }
                        Client.InvokeChannelCreated(chan);
                    }
                    break;
                case "ChannelUpdate":
                    {
                        ChannelUpdateEventJson @event = payload.ToObject<ChannelUpdateEventJson>(Client.Serializer);
                        if (ChannelCache.TryGetValue(@event.Id, out Channel chan))
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

                            Channel clone = chan.Clone();
                            chan.Update(@event.Data);
                            Client.InvokeChannelUpdated(clone, chan);
                        }
                    }
                    break;
                case "ChannelDelete":
                    {
                        ChannelDeleteEventJson @event = payload.ToObject<ChannelDeleteEventJson>(Client.Serializer);
                        ChannelCache.TryRemove(@event.Id, out Channel chan);
                        if (chan is ServerChannel sc)
                        {
                            if (ServerCache.TryGetValue(sc.ServerId, out Server server))
                            {
                                server.ChannelIds.Remove(@event.Id);
                            }
                        }
                        Client.InvokeChannelDeleted(chan);
                    }
                    break;
                case "ChannelGroupJoin":
                    {
                        ChannelGroupJoinEventJson @event = payload.ToObject<ChannelGroupJoinEventJson>(Client.Serializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            Channel chan = await Client.Rest.GetChannelAsync(@event.Id);
                            GroupChannel GC = (GroupChannel)chan;
                            ChannelCache.TryAdd(@event.Id, GC);
                            foreach (var u in GC.Recipents)
                            {
                                if (UserCache.TryGetValue(u, out User User))
                                    GC.AddUser(User);
                            }
                            Client.InvokeGroupJoined(GC, CurrentUser);
                        }
                        else
                        {
                            UserCache.TryGetValue(@event.UserId, out User user);
                            if (user == null)
                            {
                                user = await Client.Rest.GetUserAsync(@event.UserId);
                                UserCache.TryAdd(user.Id, user);
                            }
                            GroupChannel GC = (GroupChannel)ChannelCache[@event.Id];
                            GC.AddUser(user);
                            Client.InvokeGroupUserJoined(GC, user);
                        }
                    }
                    break;
                case "ChannelGroupLeave":
                    {
                        ChannelGroupLeaveEventJson @event = payload.ToObject<ChannelGroupLeaveEventJson>(Client.Serializer);
                        GroupChannel GC = (GroupChannel)ChannelCache[@event.Id];

                        if (@event.UserId == CurrentUser.Id)
                        {
                            ChannelCache.TryRemove(@event.Id, out Channel chan);
                            foreach (User u in GC.Users.Values)
                            {
                                GC.RemoveUser(u, true);
                            }
                            Client.InvokeGroupLeft(GC, CurrentUser);
                        }
                        else
                        {
                            User user = await Client.Rest.GetUserAsync(@event.UserId);
                            GC.RemoveUser(user, false);
                            Client.InvokeGroupUserLeft(GC, user);
                        }
                    }
                    break;


                case "ServerUpdate":
                    {
                        ServerUpdateEventJson @event = payload.ToObject<ServerUpdateEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(@event.Id, out Server server))
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
                            Server cloned = server.Clone();
                            server.Update(@event.Data);
                            Client.InvokeServerUpdated(cloned, server);
                        }
                    }
                    break;
                case "ServerDelete":
                    {
                        ServerDeleteEventJson @event = payload.ToObject<ServerDeleteEventJson>(Client.Serializer);
                        ServerCache.TryRemove(@event.Id, out Server server);
                        foreach(string c in server.ChannelIds)
                        {
                            ChannelCache.TryRemove(c, out _);
                        }
                        foreach(ServerMember m in server.Members.Values)
                        {
                            server.RemoveMember(m.User, true);
                        }
                        Client.InvokeServerLeft(server);
                    }
                    break;
                case "ServerMemberUpdate":
                    {
                        ServerMemberUpdateEventJson @event = payload.ToObject<ServerMemberUpdateEventJson>(Client.Serializer);
                        Server server = ServerCache[@event.Id.Server];
                        ServerMember Member = await server.GetMemberAsync(@event.Id.User);
                        if (@event.Clear.HasValue)
                        {
                            if (@event.Data == null)
                                @event.Data = new PartialServerMemberJson();
                            switch (@event.Clear.ValueOrDefault())
                            {
                                case "Avatar":
                                    @event.Data.Avatar = Option.Some<AttachmentJson>(null);
                                    break;
                                case "Nickname":
                                    @event.Data.Nickname = Option.Some<string>("");
                                    break;
                            }
                        }

                        Member.Update(@event.Data);
                    }
                    break;
                case "ServerMemberJoin":
                    {
                        ServerMemberJoinEventJson @event = payload.ToObject<ServerMemberJoinEventJson>(Client.Serializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            Server server = await Client.Rest.GetServerAsync(@event.Id);
                            server.AddMember(new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.Id, User = @event.UserId } }, CurrentUser));
                            Client.InvokeServerJoined(server, CurrentUser);
                        }
                        else
                        {
                            UserCache.TryGetValue(@event.UserId, out User user);
                            if (user == null)
                                user = await Client.Rest.GetUserAsync(@event.UserId);
                            Server server = ServerCache[@event.Id];
                            ServerMember Member = new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.Id, User = @event.UserId } }, user);
                            Client.InvokeMemberJoined(server, Member);
                        }
                    }
                    break;
                case "ServerMemberLeave":
                    {
                        ServerMemberLeaveEventJson @event = payload.ToObject<ServerMemberLeaveEventJson>(Client.Serializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            ServerCache.TryRemove(@event.Id, out Server server);
                            foreach (ServerMember m in server.Members.Values)
                            {
                                server.RemoveMember(m.User, true);
                            }
                            foreach (string c in server.ChannelIds)
                            {
                                ChannelCache.TryRemove(c, out _);
                            }
                            Client.InvokeServerLeft(server);
                        }
                        else
                        {
                            Server server = ServerCache[@event.Id];
                            server.Members.TryGetValue(@event.UserId, out ServerMember Member);
                            if (Member == null)
                            {
                                User User = await Client.Rest.GetUserAsync(@event.UserId);
                                Member = new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.Id, User = @event.UserId } }, User);
                            }
                            server.RemoveMember(Member.User, false);
                            Client.InvokeMemberLeft(server, Member);
                        }
                    }
                    break;
                case "ServerRoleUpdate":
                    {
                        ServerRoleUpdateEventJson @event = payload.ToObject<ServerRoleUpdateEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(@event.Id, out Server server))
                        {
                            if (server.Roles.TryGetValue(@event.RoleId, out Role role))
                            {
                                Role cloned = role.Clone();
                                role.Update(@event.Data);
                                Client.InvokeRoleUpdated(cloned, role);
                            }
                            else
                            {
                                Role newRole = new Role(Client, @event.Data, @event.Id, @event.RoleId);
                                server.Roles.TryAdd(@event.RoleId, newRole);
                                Client.InvokeRoleCreated(newRole);
                            }
                        }
                    }
                    break;
                case "ServerRoleDelete":
                    {
                        ServerRoleDeleteEventJson @event = payload.ToObject<ServerRoleDeleteEventJson>(Client.Serializer);
                        if (ServerCache.TryGetValue(@event.Id, out Server server))
                        {
                            server.Roles.TryRemove(@event.RoleId, out Role role);
                            foreach(ServerMember m in server.Members.Values)
                            {
                                m.Roles.TryRemove(@event.RoleId, out _);
                            }
                            Client.InvokeRoleDeleted(role);
                        }
                    }
                    break;
                case "UserUpdate":
                    {
                        UserUpdateEventJson @event = payload.ToObject<UserUpdateEventJson>(Client.Serializer);
                        if (UserCache.TryGetValue(@event.Id, out User user))
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
                                SelfUser cloned = CurrentUser.Clone();
                                user.Update(@event.Data);
                                CurrentUser.Update(@event.Data);
                                Client.InvokeCurrentUserUpdated(cloned, CurrentUser);
                            }
                            else
                            {
                                User cloned = user.Clone();
                                user.Update(@event.Data);
                                Client.InvokeUserUpdated(cloned, user);
                            }
                        }
                    }
                    break;
                case "UserRelationship":
                    {
                        UserRelationshipEventJson @event = payload.ToObject<UserRelationshipEventJson>(Client.Serializer);
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
