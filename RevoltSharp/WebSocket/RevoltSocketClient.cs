using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Optionals;
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
        internal bool StopWebSocket = false;

        internal ClientWebSocket WebSocket;
        internal CancellationToken CancellationToken = new CancellationToken();
        internal ConcurrentDictionary<string, Server> ServerCache = new ConcurrentDictionary<string, Server>();
        internal ConcurrentDictionary<string, Channel> ChannelCache = new ConcurrentDictionary<string, Channel>();
        internal ConcurrentDictionary<string, User> UserCache = new ConcurrentDictionary<string, User>();
        internal ConcurrentDictionary<string, Emoji> EmojiCache = new ConcurrentDictionary<string, Emoji>();
        internal SelfUser CurrentUser;

        internal async Task SetupWebsocket()
        {
            StopWebSocket = false;
            while (!CancellationToken.IsCancellationRequested && !StopWebSocket)
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
                            Console.WriteLine("[RevoltSharp] Client config WEBSOCKET_URL is an invalid format.");
                            throw new RevoltException("Client config WEBSOCKET_URL is an invalid format.");
                        }
                    }
                    catch (WebSocketException we)
                    {
                        Console.WriteLine("--- WebSocket Error ---\n" + $"{we}");
                        if (_firstConnected)
                        {
                            if (we.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                                throw new RevoltException("Client token may be invalid.");
                            else
                                throw new RevoltException("Failed to connect to Revolt.");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("--- WebSocket Exception ---\n" + $"{ex}");
                        if (_firstConnected)
                            throw new RevoltException("Failed to connect to Revolt.");
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
                        WebSocketMessage(await reader.ReadToEndAsync());
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
            
            try
            {
                if (Client.Config.Debug.LogWebSocketFull && payload["type"].ToString() != "Ready")
                    Console.WriteLine("--- WebSocket Response Json ---\n" + FormatJsonPretty(json));


                switch (payload["type"].ToString())
                {
                    case "Authenticated":
                        if (_firstConnected)
                            Console.WriteLine("[RevoltSharp] WebSocket Connected!");
                        else
                            Console.WriteLine("[RevoltSharp] WebSocket Reconnected!");
                        
                        Client.InvokeConnected();

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
                            if (Client.Config.Debug.LogWebSocketError)
                                Console.WriteLine("--- WebSocket Error ---\n" + json);
                            if (@event.Error == SocketErrorType.InvalidSession)
                            {
                                if (_firstConnected)
                                    Console.WriteLine("[RevoltSharp] WebSocket session is invalid, check if your bot token is correct.");
                                else
                                    Console.WriteLine("[RevoltSharp] WebSocket session was invalidated!");
                                
                                await Client.StopAsync();
                            }
                            
                            Client.InvokeWebSocketError(new SocketError { Messaage = @event.Message, Type = @event.Error });
                        }
                        break;
                    case "Ready":
                        {
                            try
                            {
                                ReadyEventJson @event = payload.ToObject<ReadyEventJson>(Client.Serializer);
                                if (Client.Config.Debug.LogWebSocketReady)
                                    Console.WriteLine("--- WebSocket Ready ---\n" + FormatJsonPretty(json));

                                UserCache = new ConcurrentDictionary<string, User>(@event.Users.ToDictionary(x => x.Id, x => new User(Client, x)));
                                
                                if (!Client.UserBot)
                                    CurrentUser = new SelfUser(Client, @event.Users.FirstOrDefault(x => x.Relationship == "User" && x.Bot != null));
                                else
                                    CurrentUser = new SelfUser(Client, @event.Users.FirstOrDefault(x => x.Id == CurrentUser.Id));

                                if (CurrentUser == null)
                                {
                                    Console.WriteLine("[RevoltSharp] Fatal error, could not load bot user.\n" +
                                        "WebSocket connection has been stopped.");
                                    await Client.StopAsync();
                                }

                                // Update bot user from cache to use current user so mutual stuff and values are synced.
                                UserCache.TryUpdate(CurrentUser.Id, CurrentUser, UserCache[CurrentUser.Id]);

                                ServerCache = new ConcurrentDictionary<string, Server>(@event.Servers.ToDictionary(x => x.Id, x => new Server(Client, x)));
                                ChannelCache = new ConcurrentDictionary<string, Channel>(@event.Channels.ToDictionary(x => x.Id, x => Channel.Create(Client, x)));


                                foreach (var m in @event.Members)
                                {
                                    if (ServerCache.TryGetValue(m.Id.Server, out Server s))
                                        s.InternalMembers.TryAdd(m.Id.User, new ServerMember(Client, m, null, UserCache[m.Id.User]));
                                }
                                foreach (var m in @event.Emojis)
                                {
                                    Emoji Emote = new Emoji(Client, m);
                                    EmojiCache.TryAdd(m.Id, Emote);
                                    if (ServerCache.TryGetValue(m.Parent.ServerId, out Server s))
                                        s.InternalEmojis.TryAdd(m.Id, Emote);
                                }
                                Console.WriteLine("[RevoltSharp] WebSocket Ready!");


                                Client.InvokeReady(CurrentUser);

                                // This task will cleanup extra group channels where the bot is only a member of.
                                _ = Task.Run(async () =>
                                {
                                    foreach (Channel channel in ChannelCache.Values.Where(x => x is GroupChannel))
                                    {
                                        GroupChannel c = channel as GroupChannel;
                                        if (c.Recipents.Count == 1)
                                        {
                                            await c.LeaveAsync();
                                        }
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                Console.WriteLine("[RevoltSharp] Fatal error, could not parse ready event.\n" +
                                    "WebSocket connection has been stopped.");
                                Client.InvokeWebSocketError(new SocketError() { Messaage = "Fatal error, could not parse ready event.\nWebSocket connection has been stopped.", Type = SocketErrorType.Unknown });
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

                            switch (channel.Type)
                            {
                                case ChannelType.Group:
                                    (channel as GroupChannel).LastMessageId = @event.Id;
                                    break;
                                case ChannelType.Text:
                                    (channel as TextChannel).LastMessageId = @event.Id;
                                    break;
                            }

                            if (@event.Author != "00000000000000000000000000" && channel is TextChannel TC)
                            {
                                if (!TC.Server.InternalMembers.ContainsKey(@event.Author))
                                {
                                    ServerMember Member = await TC.Server.GetMemberAsync(@event.Author);
                                    TC.Server.InternalMembers.TryAdd(@event.Author, Member);
                                }  
                            }
                            Message MSG = @event.ToEntity(Client);
                            Client.InvokeMessageRecieved(MSG);
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

                                    if (@event.Clear.Value.Contains("Icon"))
                                        @event.Data.Icon = Optional.Some<AttachmentJson>(null);

                                    if (@event.Clear.Value.Contains("Description"))
                                        @event.Data.Description = Optional.Some<string>(null);

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
                                Console.WriteLine("[RevoltSharp] Joined Group: " + GC.Name);
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
                                Console.WriteLine("[RevoltSharp] Left Group: " + GC.Name);
                                ChannelCache.TryRemove(@event.Id, out Channel chan);
                                foreach (User u in GC.CachedUsers.Values)
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

                    case "ServerCreate":
                        {
                            ServerJoinEventJson @event = payload.ToObject<ServerJoinEventJson>(Client.Serializer);
                            ServerCache.TryAdd(@event.Server.Id, @event.Server);
                            foreach (ServerChannel c in @event.Channels)
                            {
                                ChannelCache.TryAdd(c.Id, c);
                            }
                            Client.InvokeServerJoined(@event.Server, CurrentUser);
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

                                    if (@event.Clear.Value.Contains("Icon"))
                                        @event.Data.Icon = Optional.Some<AttachmentJson>(null);

                                    if (@event.Clear.Value.Contains("Banner"))
                                        @event.Data.Banner = Optional.Some<AttachmentJson>(null);

                                    if (@event.Clear.Value.Contains("Description"))
                                        @event.Data.Description = Optional.Some<string>(null);
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
                            foreach (string c in server.ChannelIds)
                            {
                                ChannelCache.TryRemove(c, out _);
                            }
                            foreach (ServerMember m in server.InternalMembers.Values)
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

                                foreach(string s in @event.Clear.Value)
                                {
                                    switch (s)
                                    {
                                        case "Avatar":
                                            @event.Data.Avatar = Optional.Some<AttachmentJson>(null);
                                            break;
                                        case "Nickname":
                                            @event.Data.Nickname = Optional.Some<string>("");
                                            break;
                                        case "Timeout":
                                            @event.Data.ClearTimeout = true;
                                            break;
                                        case "Roles":
                                            @event.Data.Roles = Optional.Some<string[]>(new string[0]);
                                            break;
                                    }
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
                                Console.WriteLine("[RevoltSharp] Joined Server: " + server.Name);
                                server.AddMember(new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.Id, User = @event.UserId } }, null, CurrentUser));
                                Client.InvokeServerJoined(server, CurrentUser);
                            }
                            else
                            {
                                UserCache.TryGetValue(@event.UserId, out User user);
                                if (user == null)
                                    user = await Client.Rest.GetUserAsync(@event.UserId);
                                Server server = ServerCache[@event.Id];
                                ServerMember Member = new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.Id, User = @event.UserId } }, null, user);
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
                                Console.WriteLine("[RevoltSharp] Left Server: " + server.Name);
                                foreach (ServerMember m in server.InternalMembers.Values)
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
                                server.InternalMembers.TryGetValue(@event.UserId, out ServerMember Member);
                                if (Member == null)
                                {
                                    User User = await Client.Rest.GetUserAsync(@event.UserId);
                                    Member = new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.Id, User = @event.UserId } }, null, User);
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
                                if (server.InternalRoles.TryGetValue(@event.RoleId, out Role role))
                                {
                                    Role cloned = role.Clone();
                                    role.Update(@event.Data);
                                    Client.InvokeRoleUpdated(cloned, role);
                                }
                                else
                                {
                                    Role newRole = new Role(Client, @event.Data, @event.Id, @event.RoleId);
                                    server.InternalRoles.TryAdd(@event.RoleId, newRole);
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
                                server.InternalRoles.TryRemove(@event.RoleId, out Role role);
                                foreach (ServerMember m in server.InternalMembers.Values)
                                {
                                    m.InternalRoles.TryRemove(@event.RoleId, out _);
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
                                    if (@event.Clear.Value.Contains("ProfileContent"))
                                        @event.Data.ProfileContent = Optional.Some<string>("");

                                    if (@event.Clear.Value.Contains("StatusText"))
                                        @event.Data.status = Optional.Some<UserStatusJson>(null);

                                    if (@event.Clear.Value.Contains("ProfileBackground"))
                                        @event.Data.ProfileBackground = Optional.Some<AttachmentJson>(null);

                                    if (@event.Clear.Value.Contains("Avatar"))
                                        @event.Data.avatar = Optional.Some<AttachmentJson>(null);
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
                    case "EmojiCreate":
                        {
                            ServerEmojiCreatedEventJson @event = payload.ToObject<ServerEmojiCreatedEventJson>(Client.Serializer);
                            Emoji Emoji = new Emoji(Client, @event);
                            EmojiCache.TryAdd(Emoji.Id, Emoji);
                            ServerCache.TryGetValue(Emoji.ServerId, out Server Server);
                            Server.InternalEmojis.TryAdd(Emoji.Id, Emoji);
                            Client.InvokeEmojiCreated(Server, Emoji);
                        }
                        break;
                    case "EmojiDelete":
                        {
                            ServerEmojiDeleteEventJson @event = payload.ToObject<ServerEmojiDeleteEventJson>(Client.Serializer);
                            EmojiCache.TryRemove(@event.Id, out Emoji Emoji);
                            ServerCache.TryGetValue(Emoji.ServerId, out Server Server);
                            Server.InternalEmojis.TryRemove(Emoji.Id, out Emoji Emoji2);
                            Client.InvokeEmojiDeleted(Server, Emoji);
                        }
                        break;
                    case "MessageReact":
                        {
                            ReactionAddedEventJson @event = payload.ToObject<ReactionAddedEventJson>(Client.Serializer);
                            EmojiCache.TryGetValue(@event.EmojiId, out Emoji emoji);
                            if (emoji == null)
                            {
                                if (!Char.IsDigit(@event.EmojiId[0]))
                                    emoji = new Emoji(Client, @event.EmojiId);
                                else
                                {
                                    Emoji Emote = await Client.Rest.GetEmojiAsync(@event.EmojiId);
                                    if (Emote == null)
                                        return;
                                    emoji = Emote;
                                }
                            }
                            ChannelCache.TryGetValue(@event.ChannelId, out Channel channel);
                            ServerChannel SC = channel as ServerChannel;
                            ServerMember SM = SC.Server.GetCachedMember(@event.UserId);
                            Client.InvokeReactionAdded(emoji, SC, SM);
                        }
                        break;
                    case "MessageUnreact":
                        {
                            ReactionRemovedEventJson @event = payload.ToObject<ReactionRemovedEventJson>(Client.Serializer);
                            EmojiCache.TryGetValue(@event.EmojiId, out Emoji emoji);
                            if (emoji == null)
                            {
                                if (!Char.IsDigit(@event.EmojiId[0]))
                                    emoji = new Emoji(Client, @event.EmojiId);
                                else
                                {
                                    Emoji Emote = await Client.Rest.GetEmojiAsync(@event.EmojiId);
                                    if (Emote == null)
                                        return;
                                    emoji = Emote;
                                }
                            }
                                
                            ChannelCache.TryGetValue(@event.ChannelId, out Channel channel);
                            ServerChannel SC = channel as ServerChannel;
                            ServerMember SM = SC.Server.GetCachedMember(@event.UserId);
                            Client.InvokeReactionRemoved(emoji, SC, SM);
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
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static string FormatJsonPretty(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

    }
}
