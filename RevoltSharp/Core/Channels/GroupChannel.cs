using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Optional.Unsafe;

namespace RevoltSharp
{
    public class GroupChannel : Channel
    {
        public int Permissions { get; internal set; }
        public HashSet<string> Recipents { get; internal set; }

        public ConcurrentDictionary<string, User> Users { get; internal set; } = new ConcurrentDictionary<string, User>();
        public string LastMessageId { get; internal set; }
        public string OwnerId { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Attachment Icon { get; internal set; }

        internal GroupChannel(RevoltClient client, ChannelJson model)
            : base(client)
        {
            Id = model.Id;
            Recipents = model.Recipients != null ? model.Recipients.ToHashSet() : new HashSet<string>();
            if (client.WebSocket != null)
            {
                foreach (var u in Recipents)
                {
                    if (client.WebSocket.UserCache.TryGetValue(u, out User user))
                    {
                        user.MutualGroups.TryAdd(Id, this);
                        Users.TryAdd(user.Id, user);
                    }
                }
            }
            Description = model.Description;
            LastMessageId = model.LastMessageId;
            Name = model.Name;
            OwnerId = model.Owner;
            Permissions = model.Permissions;
            Icon = model.Icon != null ? new Attachment(client, model.Icon) : null;
        }

        internal override void Update(PartialChannelJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();
            if (json.Icon.HasValue)
                Icon = new Attachment(Client, json.Icon.ValueOrDefault());
            if (json.Description.HasValue)
                Description = json.Description.ValueOrDefault();
        }

        internal void AddUser(User user)
        {
            try
            {
                Recipents.Add(user.Id);
            }
            catch { }
            user.MutualGroups.TryAdd(Id, this);
            Users.TryAdd(user.Id, user);
        }

        internal void RemoveUser(User user, bool delete)
        {
            if (!delete)
            {
                Users.TryRemove(user.Id, out _);
                try
                {
                    Recipents.Remove(user.Id);
                }
                catch { }
            }
            user.MutualGroups.TryRemove(Id, out _);
            if (user.Id != user.Client.CurrentUser.Id && !user.HasMutuals())
            {
                user.Client.WebSocket.UserCache.TryRemove(user.Id, out _);
            }
        }
    }
}
