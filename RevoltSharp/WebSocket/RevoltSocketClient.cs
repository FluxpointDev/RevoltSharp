using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Optionals;
using RevoltSharp.WebSocket.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoltSharp.WebSocket;


internal class RevoltSocketClient
{
    internal RevoltSocketClient(RevoltClient client)
    {
        Client = client;
        if (string.IsNullOrEmpty(client.Config.Debug.WebsocketUrl))
            throw new RevoltException("Client config WebsocketUrl can not be empty.");

        if (!Uri.IsWellFormedUriString(client.Config.Debug.WebsocketUrl, UriKind.Absolute))
            throw new RevoltException("Client config WebsocketUrl is an invalid format.");
    }

    internal RevoltClient Client { get; }

    private bool _firstConnected { get; set; } = true;
    private bool _firstError = true;
    internal bool StopWebSocket = false;

    internal ClientWebSocket? WebSocket;
    internal CancellationToken CancellationToken = new CancellationToken();
    internal ConcurrentDictionary<string, Server> ServerCache = new ConcurrentDictionary<string, Server>();
    internal ConcurrentDictionary<string, Channel> ChannelCache = new ConcurrentDictionary<string, Channel>();
    internal ConcurrentDictionary<string, User> UserCache = new ConcurrentDictionary<string, User>();
    internal ConcurrentDictionary<string, Emoji> EmojiCache = new ConcurrentDictionary<string, Emoji>();
    internal SelfUser? CurrentUser => Client.CurrentUser;

    internal async Task SetupWebsocket()
    {
        StopWebSocket = false;
        
        while (!CancellationToken.IsCancellationRequested && !StopWebSocket)
        {
            using (WebSocket = new ClientWebSocket())
            {
                if (Client.Config.WebSocketProxy != null)
                    WebSocket.Options.Proxy = Client.Config.WebSocketProxy;

                try
                {
                    Uri uri = new Uri($"{Client.Config.Debug.WebsocketUrl}?format=json&version=1");

                    if (!string.IsNullOrEmpty(Client.Config.CfClearance))
                    {
                        WebSocket.Options.Cookies = new System.Net.CookieContainer();
                        WebSocket.Options.Cookies.SetCookies(uri, $"cf_clearance={Client.Config.CfClearance}");
                    }
                    WebSocket.Options.SetRequestHeader("User-Agent", Client.Config.UserAgent);

                    await WebSocket.ConnectAsync(uri, CancellationToken);
                    await Send(WebSocket, JsonConvert.SerializeObject(new AuthenticateRequest(Client.Token)), CancellationToken);
                    _firstError = true;
                    await Receive(WebSocket, CancellationToken);
                }
                catch (ArgumentException)
                {
                    if (_firstConnected)
                        Client.InvokeLogAndThrowException("Client config WebsocketUrl is an invalid format.");
                }
                catch (WebSocketException we)
                {
                    if (_firstConnected)
                    {
                        if (we.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                            Client.InvokeLogAndThrowException("Failed to connect to Revolt, the instance may be down or having issues.");

                        Client.InvokeLogAndThrowException("Failed to connect to Revolt.");
                    }
                    else
                    {
                        if (we.WebSocketErrorCode != WebSocketError.ConnectionClosedPrematurely)
                            Client.InvokeLog($"WebSocket Internal Error {we.ErrorCode} {we.WebSocketErrorCode}", RevoltLogSeverity.Error);
                    }

                }
                catch (Exception ex)
                {
                    Client.InvokeLog($"WebSocket Error {ex.Message}", RevoltLogSeverity.Error);
                    if (_firstConnected)
                        Client.InvokeLogAndThrowException("Failed to connect to Revolt.");
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
                    _ = WebSocketMessage(await reader.ReadToEndAsync());
                }
            }
        }
    }

    internal class AuthenticateRequest
    {
        internal AuthenticateRequest(string token)
        {
            Token = token;
        }

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
            if (Client.Config.Debug.LogWebSocketFull)
            {
                switch (payload["type"].ToString())
                {
                    case "Ready":
                    case "UserUpdate":
                    case "ChannelStartTyping":
                    case "ChannelStopTyping":
                        break;
                    default:
                        Client.Logger.LogJson("WebSocket Response Json", json);
                        break;
                }

            }


            switch (payload["type"].ToString())
            {
                case "Authenticated":
                    if (_firstConnected)
                    {
                        Client.InvokeConnected();
                        Client.InvokeLog("WebSocket Connected!", RevoltLogSeverity.Debug);
                    }
                    else
                        Client.InvokeLog("WebSocket Reconnected!", RevoltLogSeverity.Debug);

                    _firstConnected = false;
                    await Send(WebSocket, JsonConvert.SerializeObject(new HeartbeatRequest()), CancellationToken);

                    //_ = Task.Run(async () =>
                    //{
                    //    while (!CancellationToken.IsCancellationRequested)
                    //    {
                    //        await Task.Delay(50000, CancellationToken);
                    //        await Send(WebSocket, JsonConvert.SerializeObject(new HeartbeatRequest()), CancellationToken);
                    //    }
                    //}, CancellationToken);
                    break;
                case "MessageAppend":
                    {

                    }
                    break;
                case "Ping":
                    {
                        HeartbeatRequest @event = JsonConvert.DeserializeObject<HeartbeatRequest>(json);
                        await Send(WebSocket, JsonConvert.SerializeObject(new HeartbeatRequest() { Data = @event.Data }), CancellationToken);
                    }
                    break;
                case "Pong":
                    {

                    }
                    break;
                case "Bulk":
                    {
                        BulkEventsJson @event = JsonConvert.DeserializeObject<BulkEventsJson>(json);
                        foreach (dynamic e in @event.Events)
                        {
                            await WebSocketMessage(JsonConvert.SerializeObject(e));
                        }
                    }
                    break;
                case "Error":
                    {
                        ErrorEventJson @event = JsonConvert.DeserializeObject<ErrorEventJson>(json);

                        if (Client.Config.Debug.LogWebSocketError)
                            Client.Logger.LogJson("WebSocket Error", json);

                        if (@event.Error == RevoltErrorType.InvalidSession)
                        {
                            if (_firstConnected)
                                Client.InvokeLog("WebSocket session is invalid, check if your bot token is correct.", RevoltLogSeverity.Error);
                            else
                                Client.InvokeLog("WebSocket session was invalidated!", RevoltLogSeverity.Error);

                            await Client.StopAsync();
                        }

                        Client.InvokeWebSocketError(Client, new SocketError { Message = @event.Message, Type = @event.Error });
                    }
                    break;
                case "Ready":
                    {
                        try
                        {
                            ReadyEventJson @event = payload.ToObject<ReadyEventJson>(RevoltClient.Deserializer);
                            if (Client.Config.Debug.LogWebSocketReady)
                                Client.Logger.LogJson("WebSocket Ready", json);

                            ClearAllCache();

                            UserCache = new ConcurrentDictionary<string, User>(@event.Users.ToDictionary(x => x.Id, x => new User(Client, x)));

                            SelfUser SocketSelfUser = null;
                            if (!Client.UserBot)
                                SocketSelfUser = new SelfUser(Client, @event.Users.FirstOrDefault(x => x.Relationship == "User" && x.Bot != null));
                            else
                                SocketSelfUser = new SelfUser(Client, @event.Users.FirstOrDefault(x => x.Id == CurrentUser.Id));

                            if (SocketSelfUser == null)
                            {
                                Client.InvokeLog("Fatal error, could not load bot user.\n" +
                                    "WebSocket connection has been stopped.", RevoltLogSeverity.Error);
                                await Client.StopAsync();
                            }

                            Client.CurrentUser = SocketSelfUser;

                            // Update bot user from cache to use current user so mutual stuff and values are synced.
                            UserCache.TryUpdate(CurrentUser.Id, CurrentUser, UserCache[CurrentUser.Id]);

                            ServerCache = new ConcurrentDictionary<string, Server>(@event.Servers.ToDictionary(x => x.Id, x => new Server(Client, x)));
                            ChannelCache = new ConcurrentDictionary<string, Channel>(@event.Channels.ToDictionary(x => x.Id, x => Channel.Create(Client, x)));

                            foreach (ServerMemberJson m in @event.Members)
                            {
                                if (ServerCache.TryGetValue(m.Id.Server, out Server s))
                                {
                                    s.AddMember(new ServerMember(Client, m, null, UserCache[m.Id.User]));
                                }
                            }

                            Client.SavedMessagesChannel = (SavedMessagesChannel)ChannelCache.Values.FirstOrDefault(x => x.Type == ChannelType.SavedMessages);

                            foreach (Channel? c in ChannelCache.Values.Where(x => x.Type == ChannelType.DM))
                            {
                                DMChannel DM = (DMChannel)c;
                                if (UserCache.TryGetValue(DM.UserId, out User user))
                                    user.InternalMutualDMs.TryAdd(c.Id, DM);
                            }

                            foreach (EmojiJson m in @event.Emojis)
                            {
                                Emoji Emote = new Emoji(Client, m);
                                EmojiCache.TryAdd(m.Id, Emote);
                                if (ServerCache.TryGetValue(m.Parent.ServerId, out Server s))
                                    s.InternalEmojis.TryAdd(m.Id, Emote);
                            }
                            Client.InvokeLog("WebSocket Ready!", RevoltLogSeverity.Debug);

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
                            Client.InvokeWebSocketError(Client, new SocketError() { Message = "Fatal error, could not parse ready event.\n" +
                                "WebSocket connection has been stopped.", Type = RevoltErrorType.Unknown });
                            await Client.StopAsync();
                        }
                    }
                    break;
                case "Message":
                    {
                        MessageEventJson @event = payload.ToObject<MessageEventJson>(RevoltClient.Deserializer);

                        User? User = null;
                        if (@event.AuthorId != "00000000000000000000000000" && @event.Webhook == null && !UserCache.ContainsKey(@event.AuthorId))
                        {
                            User = new User(Client, @event.User);
                            UserCache.TryAdd(@event.AuthorId, User);
                        }

                        if (!ChannelCache.TryGetValue(@event.ChannelId, out Channel channel))
                            channel = await Client.Rest.GetChannelAsync(@event.ChannelId);

                        if (channel == null)
                            return;

                        switch (channel.Type)
                        {
                            case ChannelType.Group:
                                (channel as GroupChannel).LastMessageId = @event.MessageId;
                                break;
                            case ChannelType.Text:
                                {
                                    (channel as TextChannel).LastMessageId = @event.MessageId;
                                    if (@event.AuthorId != "00000000000000000000000000" && @event.Webhook == null && channel is TextChannel TC)
                                    {
                                        if (!TC.Server.InternalMembers.ContainsKey(@event.AuthorId))
                                        {
                                            TC.Server.AddMember(new ServerMember(Client, @event.Member, @event.User, User));
                                        }
                                    }
                                }
                                break;
                            case ChannelType.DM:
                                (channel as DMChannel).LastMessageId = @event.MessageId;
                                break;
                            case ChannelType.SavedMessages:
                                (channel as SavedMessagesChannel).LastMessageId = @event.MessageId;
                                break;
                        }

                        Message MSG = Message.Create(Client, @event);
                        Client.InvokeMessageRecieved(MSG);
                    }
                    break;
                case "MessageUpdate":
                    {
                        MessageUpdateEventJson @event = payload.ToObject<MessageUpdateEventJson>(RevoltClient.Deserializer);

                        if (!ChannelCache.ContainsKey(@event.ChannelId))
                        {
                            Channel channel = await Client.Rest.GetChannelAsync(@event.ChannelId);
                            if (channel == null)
                                return;
                        }

                        Downloadable<string, Message> message = new Downloadable<string, Message>(@event.MessageId, () => Client.Rest.GetMessageAsync(@event.ChannelId, @event.MessageId));
                        Client.InvokeMessageUpdated(message, new MessageUpdatedProperties(Client, @event));
                    }
                    break;
                case "MessageDelete":
                    {
                        MessageDeleteEventJson @event = payload.ToObject<MessageDeleteEventJson>(RevoltClient.Deserializer);

                        if (!ChannelCache.TryGetValue(@event.ChannelId, out Channel channel))
                        {
                            channel = await Client.Rest.GetChannelAsync(@event.ChannelId);
                            if (channel == null)
                                return;
                        }

                        Client.InvokeMessageDeleted(channel, @event.MessageId);
                    }
                    break;
                case "BulkMessageDelete":
                    {
                        MessageDeleteEventJson @event = payload.ToObject<MessageDeleteEventJson>(RevoltClient.Deserializer);

                        if (!ChannelCache.TryGetValue(@event.ChannelId, out Channel channel))
                        {
                            channel = await Client.Rest.GetChannelAsync(@event.ChannelId);
                            if (channel == null)
                                return;
                        }

                        Client.InvokeMessagesBulkDeleted(channel, @event.MessageIds);
                    }
                    break;

                case "ChannelCreate":
                    {
                        ChannelEventJson @event = payload.ToObject<ChannelEventJson>(RevoltClient.Deserializer);
                        Channel chan = Channel.Create(Client, @event);
                        ChannelCache.TryAdd(chan.Id, chan);

                        switch (chan.Type)
                        {
                            case ChannelType.Text:
                            case ChannelType.Voice:
                                {
                                    ServerCache.TryGetValue(@event.ServerId, out Server server);
                                    if (server == null)
                                    {
                                        server = await Client.Rest.GetServerAsync(@event.ServerId);
                                        if (server == null)
                                            return;
                                    }

                                    server.ChannelIds.Add(chan.Id);
                                    if (chan.Type == ChannelType.Text)
                                        Client.InvokeChannelCreated((TextChannel)chan);
                                    else
                                        Client.InvokeChannelCreated((VoiceChannel)chan);
                                }
                                break;
                            case ChannelType.Group:
                                {
                                    GroupChannel GC = (GroupChannel)chan;
                                    foreach (string u in GC.Recipents)
                                    {
                                        if (UserCache.TryGetValue(u, out User User))
                                            GC.AddUser(User);
                                    }

                                    Client.InvokeGroupJoined(GC, CurrentUser);
                                }
                                break;
                            case ChannelType.DM:
                                {
                                    Client.InvokeDMChannelOpened((DMChannel)chan);
                                }
                                break;
                            case ChannelType.SavedMessages:
                                {
                                    Client.SavedMessagesChannel = (SavedMessagesChannel)chan;
                                }
                                break;
                        }
                    }
                    break;
                case "ChannelUpdate":
                    {
                        ChannelUpdateEventJson @event = payload.ToObject<ChannelUpdateEventJson>(RevoltClient.Deserializer);
                        if (!ChannelCache.TryGetValue(@event.ChannelId, out Channel chan))
                        {
                            await Client.Rest.GetChannelAsync(@event.ChannelId);
                            return;
                        }

                        if (@event.Clear.HasValue)
                        {
                            @event.Data ??= new PartialChannelJson();

                            if (@event.Clear.Value.Contains("Icon"))
                                @event.Data.Icon = Optional.Some<AttachmentJson?>(null);

                            if (@event.Clear.Value.Contains("Description"))
                                @event.Data.Description = Optional.Some<string>(null);

                        }

                        Channel clone = chan.Clone();

                        chan.Update(@event.Data);
                        Client.InvokeChannelUpdated(clone, chan, new ChannelUpdatedProperties(chan, @event.Data));

                    }
                    break;
                case "ChannelDelete":
                    {
                        ChannelDeleteEventJson @event = payload.ToObject<ChannelDeleteEventJson>(RevoltClient.Deserializer);
                        if (!ChannelCache.TryRemove(@event.ChannelId, out Channel chan))
                            return;

                        if (chan is ServerChannel sc)
                        {
                            if (ServerCache.TryGetValue(sc.ServerId, out Server server))
                                server.ChannelIds.Remove(@event.ChannelId);
                        }
                        Client.InvokeChannelDeleted(chan);
                    }
                    break;
                case "ChannelGroupJoin":
                    {
                        ChannelGroupJoinEventJson @event = payload.ToObject<ChannelGroupJoinEventJson>(RevoltClient.Deserializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            // Might be obsolete
                            Channel chan = await Client.Rest.GetChannelAsync(@event.ChannelId);
                            GroupChannel GC = (GroupChannel)chan;
                            ChannelCache.TryAdd(@event.ChannelId, GC);
                            foreach (string u in GC.Recipents)
                            {
                                if (UserCache.TryGetValue(u, out User User))
                                    GC.AddUser(User);
                            }
                            Client.InvokeGroupJoined(GC, CurrentUser);
                        }
                        else
                        {
                            User user = await Client.Rest.GetUserAsync(@event.UserId);
                            if (user == null)
                                return;

                            GroupChannel GC = (GroupChannel)ChannelCache[@event.ChannelId];
                            GC.AddUser(user);
                            Client.InvokeGroupUserJoined(GC, user);
                        }
                    }
                    break;
                case "ChannelGroupLeave":
                    {
                        ChannelGroupLeaveEventJson @event = payload.ToObject<ChannelGroupLeaveEventJson>(RevoltClient.Deserializer);
                        if (!ChannelCache.TryGetValue(@event.ChannelId, out Channel Channel))
                            return;
                        GroupChannel GC = Channel as GroupChannel;

                        if (@event.UserId == CurrentUser.Id)
                        {
                            Client.InvokeLog("Left Group: " + GC.Name, RevoltLogSeverity.Debug);
                            ChannelCache.TryRemove(@event.ChannelId, out Channel chan);
                            _ = Task.Run(() =>
                            {
                                foreach (User u in GC.CachedUsers.Values)
                                {
                                    GC.RemoveUser(u, true);
                                }
                            });

                            Client.InvokeGroupLeft(GC, CurrentUser);
                        }
                        else
                        {
                            User User = await Client.Rest.GetUserAsync(@event.UserId);
                            if (User == null)
                            {
                                GC.RemoveUser(Client, @event.UserId);
                                return;
                            }

                            GC.RemoveUser(User, false);
                            Client.InvokeGroupUserLeft(GC, User);
                        }
                    }
                    break;

                case "ServerCreate":
                    {
                        ServerJoinEventJson @event = payload.ToObject<ServerJoinEventJson>(RevoltClient.Deserializer);
                        Server server = new Server(Client, @event.ServerJson);
                        ServerCache.TryAdd(@event.ServerJson.Id, server);
                        if (UserCache.TryGetValue(server.OwnerId, out User Owner))
                            Owner.InternalMutualServers.TryAdd(server.Id, server);

                        foreach (ChannelJson c in @event.ChannelsJson)
                        {
                            if (Channel.Create(Client, c) is not TextChannel Chan)
                                continue;

                            ChannelCache.TryAdd(c.Id, Chan);
                        }
                        Client.InvokeServerJoined(server, CurrentUser);
                    }
                    break;
                case "ServerUpdate":
                    {
                        ServerUpdateEventJson @event = payload.ToObject<ServerUpdateEventJson>(RevoltClient.Deserializer);
                        if (!ServerCache.TryGetValue(@event.ServerId, out Server server))
                            return;

                        if (@event.Clear.HasValue)
                        {
                            @event.Data ??= new PartialServerJson();

                            if (@event.Clear.Value.Contains("Icon"))
                                @event.Data.Icon = Optional.Some<AttachmentJson>(null);

                            if (@event.Clear.Value.Contains("Banner"))
                                @event.Data.Banner = Optional.Some<AttachmentJson>(null);

                            if (@event.Clear.Value.Contains("Description"))
                                @event.Data.Description = Optional.Some<string>(null);
                        }
                        Server cloned = server.Clone();
                        server.Update(@event.Data);
                        Client.InvokeServerUpdated(cloned, server, new ServerUpdatedProperties(server, @event.Data));

                    }
                    break;
                case "ServerDelete":
                    {
                        ServerDeleteEventJson @event = payload.ToObject<ServerDeleteEventJson>(RevoltClient.Deserializer);
                        if (!ServerCache.TryRemove(@event.ServerId, out Server server))
                            return;

                        _ = Task.Run(() =>
                        {
                            foreach (string c in server.ChannelIds)
                            {
                                ChannelCache.TryRemove(c, out _);
                            }
                            foreach (ServerMember m in server.InternalMembers.Values)
                            {
                                server.RemoveMember(m.User);
                            }
                            foreach (Emoji e in server.InternalEmojis.Values)
                            {
                                EmojiCache.TryRemove(e.Id, out _);
                            }
                        });

                        Client.InvokeServerLeft(server);
                    }
                    break;
                case "ServerMemberUpdate":
                    {
                        ServerMemberUpdateEventJson @event = payload.ToObject<ServerMemberUpdateEventJson>(RevoltClient.Deserializer);
                        if (!ServerCache.TryGetValue(@event.Id.Server, out Server Server))
                            return;

                        if (!Server.InternalMembers.TryGetValue(@event.Id.User, out ServerMember Member))
                        {
                            Member = await Server.GetMemberAsync(@event.Id.User);
                            if (Member == null)
                                return;
                        }

                        if (@event.Clear.HasValue)
                        {
                            @event.Data ??= new PartialServerMemberJson();

                            foreach (string s in @event.Clear.Value)
                            {
                                switch (s)
                                {
                                    case "Avatar":
                                        @event.Data.Avatar = Optional.Some<AttachmentJson>(null);
                                        break;
                                    case "Nickname":
                                        @event.Data.Nickname = Optional.Some("");
                                        break;
                                    case "Timeout":
                                        @event.Data.ClearTimeout = true;
                                        break;
                                    case "Roles":
                                        @event.Data.Roles = Optional.Some(Array.Empty<string>());
                                        break;
                                }
                            }
                        }

                        Member.Update(@event.Data);
                    }
                    break;
                case "ServerMemberJoin":
                    {
                        ServerMemberJoinEventJson @event = payload.ToObject<ServerMemberJoinEventJson>(RevoltClient.Deserializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            Server server = await Client.Rest.GetServerAsync(@event.ServerId);
                            if (server == null)
                                return;

                            Client.InvokeLog("Joined Server: " + server.Name, RevoltLogSeverity.Debug);

                            ServerMember Member = new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.ServerId, User = @event.UserId } }, null, CurrentUser);
                            server.AddMember(Member);
                            Client.InvokeServerJoined(server, CurrentUser);
                        }
                        else
                        {
                            if (!ServerCache.TryGetValue(@event.ServerId, out Server Server))
                            {
                                Server = await Client.Rest.GetServerAsync(@event.ServerId);
                                if (Server == null)
                                    return;
                            }

                            User user = await Client.Rest.GetUserAsync(@event.UserId);
                            if (user == null)
                                return;

                            ServerMember Member = new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.ServerId, User = @event.UserId } }, null, user);
                            Server.AddMember(Member);
                            Client.InvokeMemberJoined(Server, Member);
                        }
                    }
                    break;
                case "ServerMemberLeave":
                    {
                        ServerMemberLeaveEventJson @event = payload.ToObject<ServerMemberLeaveEventJson>(RevoltClient.Deserializer);
                        if (@event.UserId == CurrentUser.Id)
                        {
                            if (!ServerCache.TryRemove(@event.ServerId, out Server server))
                            {
                                CurrentUser.InternalMutualServers.TryRemove(@event.ServerId, out server);
                                return;
                            }

                            Client.InvokeLog("Left Server: " + server.Name, RevoltLogSeverity.Debug);
                            _ = Task.Run(() =>
                            {
                                foreach (string c in server.ChannelIds)
                                {
                                    ChannelCache.TryRemove(c, out _);
                                }

                                foreach (ServerMember m in server.InternalMembers.Values)
                                {
                                    server.RemoveMember(m.User);
                                }
                                
                                foreach (Emoji e in server.InternalEmojis.Values)
                                {
                                    EmojiCache.TryRemove(e.Id, out _);
                                }
                            });

                            Client.InvokeServerLeft(server);
                        }
                        else
                        {
                            UserCache.TryGetValue(@event.UserId, out User user);
                            if (!ServerCache.TryGetValue(@event.ServerId, out Server Server))
                            {
                                user.InternalMutualServers.TryRemove(@event.ServerId, out Server);
                                return;
                            }

                            user = await Client.Rest.GetUserAsync(@event.UserId);


                            Server.InternalMembers.TryGetValue(@event.UserId, out ServerMember Member);
                            Member ??= new ServerMember(Client, new ServerMemberJson { Id = new ServerMemberIdsJson { Server = @event.ServerId, User = @event.UserId } }, null, user);

                            if (user == null)
                            {
                                Server.RemoveMember(Client, @event.UserId);
                                return;
                            }

                            Server.RemoveMember(Member.User);
                            Client.InvokeMemberLeft(Server, Member);
                        }
                    }
                    break;
                case "ServerRoleUpdate":
                    {
                        ServerRoleUpdateEventJson @event = payload.ToObject<ServerRoleUpdateEventJson>(RevoltClient.Deserializer);
                        if (!ServerCache.TryGetValue(@event.ServerId, out Server server))
                            return;

                        @event.Data ??= new PartialRoleJson();

                        if (server.InternalRoles.TryGetValue(@event.RoleId, out Role role))
                        {
                            Role cloned = role.Clone();
                            role.Update(@event.Data);
                            if (@event.Data.Permissions.HasValue)
                            {
                                _ = Task.Run(() =>
                                {
                                    foreach (ServerMember i in server.InternalMembers.Values)
                                    {
                                        i.Permissions = new ServerPermissions(server, i);
                                    }
                                });
                            }
                            Client.InvokeRoleUpdated(cloned, role, new RoleUpdatedProperties(Client, role, @event.Data));
                        }
                        else
                        {
                            Role newRole = new Role(Client, @event.Data, @event.ServerId, @event.RoleId);
                            server.InternalRoles.TryAdd(@event.RoleId, newRole);
                            Client.InvokeRoleCreated(newRole);
                        }

                    }
                    break;
                case "ServerRoleDelete":
                    {
                        ServerRoleEventsJson @event = payload.ToObject<ServerRoleEventsJson>(RevoltClient.Deserializer);
                        if (!ServerCache.TryGetValue(@event.Id, out Server server))
                            return;

                        if (!server.InternalRoles.TryRemove(@event.RoleId, out Role role))
                            return;

                        _ = Task.Run(() =>
                        {
                            foreach (Channel c in Client.WebSocket.ChannelCache.Values)
                            {
                                if (c is ServerChannel SC)
                                {
                                    SC.InternalRolePermissions.Remove(@event.RoleId);
                                }
                            }

                            foreach (ServerMember m in server.InternalMembers.Values)
                            {
                                m.InternalRoles.TryRemove(@event.RoleId, out _);
                                m.Permissions = new ServerPermissions(server, m);
                            }
                        });
                        Client.InvokeRoleDeleted(role);
                    }
                    break;
                case "UserUpdate":
                    {
                        UserUpdateEventJson @event = payload.ToObject<UserUpdateEventJson>(RevoltClient.Deserializer);
                        if (!UserCache.TryGetValue(@event.Id, out User user))
                            return;
                        if (@event.Clear.HasValue)
                        {
                            string[] clr = @event.Clear.Value;
                            if (@event.Data.Profile.HasValue)
                            {
                                if (clr.Contains("ProfileContent"))
                                    @event.Data.Profile.Value.Content = Optional.Some<string?>(null);

                                if (clr.Contains("ProfileBackground"))
                                    @event.Data.Profile.Value.Background = Optional.Some<AttachmentJson?>(null);
                            }

                            if (clr.Contains("StatusText"))
                                @event.Data.Status = Optional.Some(new UserStatusJson());

                            if (clr.Contains("Avatar"))
                                @event.Data.Avatar = Optional.Some<AttachmentJson>(null);

                            if (clr.Contains("DisplayName"))
                                @event.Data.DisplayName = Optional.Some<string>(null);
                        }
                        if (@event.Id == CurrentUser.Id)
                        {
                            SelfUser cloned = CurrentUser.Clone();
                            user.Update(@event.Data);
                            CurrentUser.Update(@event.Data);
                            Client.InvokeCurrentUserUpdated(cloned, CurrentUser, new SelfUserUpdatedProperties(Client, @event.Data));
                        }
                        else
                        {
                            User cloned = user.Clone();
                            user.Update(@event.Data);
                            Client.InvokeUserUpdated(cloned, user, new UserUpdatedProperties(Client, @event.Data));
                        }
                    }
                    break;
                case "UserRelationship":
                    {
                        UserRelationshipEventJson @event = payload.ToObject<UserRelationshipEventJson>(RevoltClient.Deserializer);
                        Client.Logger.LogJson("Relationship", @event);

                        if (UserCache.TryGetValue(@event.User.Id, out User user))
                        {
                            if (!string.IsNullOrEmpty(@event.Status) && Enum.TryParse(@event.Status, ignoreCase: true, out UserRelationship UR))
                                user.Relationship = UR;
                            else
                                user.Relationship = UserRelationship.None;
                        }

                        Downloadable<string, User> DownloadUser = new Downloadable<string, User>(@event.User.Id, async () =>
                        {
                            if (UserCache.TryGetValue(@event.User.Id, out User User))
                                return User;

                            return await Client.Rest.GetUserAsync(@event.User.Id);
                        });

                        Client.InvokeUserRelationshipUpdated(user, user.Relationship);
                    }
                    break;
                case "EmojiCreate":
                    {
                        ServerEmojiCreatedEventJson @event = payload.ToObject<ServerEmojiCreatedEventJson>(RevoltClient.Deserializer);
                        Emoji AddedEmoji = new Emoji(Client, @event);
                        EmojiCache.TryAdd(AddedEmoji.Id, AddedEmoji);

                        if (!ServerCache.TryGetValue(AddedEmoji.ServerId, out Server Server))
                            return;

                        Server.InternalEmojis.TryAdd(AddedEmoji.Id, AddedEmoji);
                        Client.InvokeEmojiCreated(Server, AddedEmoji);
                    }
                    break;
                case "EmojiDelete":
                    {
                        ServerEmojiDeleteEventJson @event = payload.ToObject<ServerEmojiDeleteEventJson>(RevoltClient.Deserializer);
                        if (!EmojiCache.TryRemove(@event.EmojiId, out Emoji Emoji))
                            return;

                        if (!ServerCache.TryGetValue(Emoji.ServerId, out Server Server))
                            return;

                        Server.InternalEmojis.TryRemove(Emoji.Id, out Emoji ServerEmoji);

                        Client.InvokeEmojiDeleted(Server, Emoji);
                    }
                    break;
                case "MessageReact":
                    {
                        ReactionAddedEventJson @event = payload.ToObject<ReactionAddedEventJson>(RevoltClient.Deserializer);

                        ChannelCache.TryGetValue(@event.ChannelId, out Channel channel);
                        if (channel == null)
                            return;

                        EmojiCache.TryGetValue(@event.EmojiId, out Emoji emoji);
                        if (emoji == null)
                        {
                            if (!Char.IsDigit(@event.EmojiId[0]))
                                emoji = new Emoji(Client, @event.EmojiId);
                            else
                            {
                                Emoji Emote = await Client.Rest.GetEmojiAsync(@event.EmojiId);
                                if (Emote == null)
                                    emoji = new Emoji(Client, @event.EmojiId);
                                else
                                    emoji = Emote;
                            }
                        }



                        Downloadable<string, User> DownloadUser = new Downloadable<string, User>(@event.UserId, async () =>
                        {
                            if (UserCache.TryGetValue(@event.UserId, out User User))
                                return User;

                            return await Client.Rest.GetUserAsync(@event.UserId);
                        });
                        Downloadable<string, Message> message = new Downloadable<string, Message>(@event.MessageId, async () =>
                        {
                            return await Client.Rest.GetMessageAsync(@event.ChannelId, @event.MessageId);

                        });
                        Client.InvokeReactionAdded(emoji, channel, DownloadUser, message);
                    }
                    break;
                case "MessageUnreact":
                    {
                        ReactionRemovedEventJson @event = payload.ToObject<ReactionRemovedEventJson>(RevoltClient.Deserializer);

                        ChannelCache.TryGetValue(@event.ChannelId, out Channel channel);
                        if (channel == null)
                            return;

                        EmojiCache.TryGetValue(@event.EmojiId, out Emoji emoji);
                        if (emoji == null)
                        {
                            if (!Char.IsDigit(@event.EmojiId[0]))
                                emoji = new Emoji(Client, @event.EmojiId);
                            else
                            {
                                Emoji Emote = await Client.Rest.GetEmojiAsync(@event.EmojiId);
                                if (Emote == null)
                                    emoji = new Emoji(Client, @event.EmojiId);
                                else
                                    emoji = Emote;
                            }
                        }



                        Downloadable<string, User> DownloadUser = new Downloadable<string, User>(@event.UserId, async () =>
                        {
                            if (UserCache.TryGetValue(@event.UserId, out User User))
                                return User;

                            return await Client.Rest.GetUserAsync(@event.UserId);
                        });
                        Downloadable<string, Message> message = new Downloadable<string, Message>(@event.MessageId, async () =>
                        {
                            return await Client.Rest.GetMessageAsync(@event.ChannelId, @event.MessageId);

                        });
                        Client.InvokeReactionRemoved(emoji, channel, DownloadUser, message);
                    }
                    break;
                case "MessageRemoveReaction":
                    {
                        ReactionRemovedEventJson @event = payload.ToObject<ReactionRemovedEventJson>(RevoltClient.Deserializer);

                        ChannelCache.TryGetValue(@event.ChannelId, out Channel channel);
                        if (channel == null)
                            return;

                        EmojiCache.TryGetValue(@event.EmojiId, out Emoji emoji);
                        if (emoji == null)
                        {
                            if (!Char.IsDigit(@event.EmojiId[0]))
                                emoji = new Emoji(Client, @event.EmojiId);
                            else
                            {
                                Emoji Emote = await Client.Rest.GetEmojiAsync(@event.EmojiId);
                                if (Emote == null)
                                    emoji = new Emoji(Client, @event.EmojiId);
                                else
                                    emoji = Emote;
                            }
                        }

                        Downloadable<string, Message> message = new Downloadable<string, Message>(@event.MessageId, async () =>
                        {
                            return await Client.Rest.GetMessageAsync(@event.ChannelId, @event.MessageId);

                        });
                        Client.InvokeReactionBulkRemoved(emoji, channel, message);
                    }
                    break;
                case "UserPlatformWipe":
                    {
                        UserPlatformWipedEventJson @event = payload.ToObject<UserPlatformWipedEventJson>(RevoltClient.Deserializer);
                        if (!UserCache.Remove(@event.UserId, out User User))
                        {
                            User = await Client.Rest.GetUserAsync(@event.UserId);
                            UserCache.TryRemove(@event.UserId, out _);
                        }

                        _ = Task.Run(() =>
                        {
                            foreach (Channel c in ChannelCache.Values)
                            {
                                switch (c.Type)
                                {
                                    case ChannelType.DM:
                                        DMChannel DM = (DMChannel)c;
                                        if (DM.UserId == User.Id)
                                        {
                                            User.InternalMutualDMs.TryRemove(c.Id, out DMChannel _);
                                            ChannelCache.Remove(c.Id, out Channel C);
                                            if (DM.Active)
                                                DM.CloseAsync();
                                        }
                                        break;
                                    case ChannelType.Group:
                                        GroupChannel GC = (GroupChannel)c;
                                        User.InternalMutualGroups.TryRemove(c.Id, out GroupChannel _);
                                        GC.CachedUsers.TryRemove(User.Id, out User GU);
                                        break;
                                }
                            }
                            foreach (Server s in ServerCache.Values)
                            {
                                User.InternalMutualServers.TryRemove(s.Id, out Server _);
                                s.InternalMembers.Remove(User.Id, out ServerMember SM);
                            }
                        });

                        Client.InvokeUserPlatformRemoved(@event.UserId, User, new UserFlags(@event.Flags));
                    }
                    break;
                case "WebhookCreate":
                    {
                        WebhookCreateEventJson @event = payload.ToObject<WebhookCreateEventJson>(RevoltClient.Deserializer);
                        Client.InvokeWebhookCreated(new Webhook(Client, @event));
                    }
                    break;
                case "Logout":
                    {
                        Client.Logger.LogMessage("Client has been logged out by the server, this may be an expired session or the bot token has been reset.", RevoltLogSeverity.Warn);

                        Client.InvokeLogout();
                        
                        await Client.StopAsync();
                    }
                    break;
                case "ChannelStartTyping":
                case "ChannelStopTyping":
                case "ChannelAck":
                    break;
                default:
                    {
                        if (Client.Config.Debug.LogWebSocketUnknownEvent)
                            Client.Logger.LogJson("WebSocket Unknown Event", json);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Client.InvokeLog($"WebSocket Event {payload["type"].ToString()} Error ", RevoltLogSeverity.Error);
            Console.WriteLine(ex);
        }
    }

    internal void ClearAllCache()
    {
        ChannelCache.Clear();
        EmojiCache.Clear();
        ServerCache.Clear();
        UserCache.Clear();
    }

}