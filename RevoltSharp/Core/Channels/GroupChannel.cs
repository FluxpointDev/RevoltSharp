using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Optional.Unsafe;

namespace RevoltSharp
{
    /// <summary>
    /// Private group channel with a list of user
    /// </summary>
    public class GroupChannel : Channel
    {
        internal GroupChannel(RevoltClient client, ChannelJson model) : base(client)
        {
            Id = model.Id;
            Type = ChannelType.Group;
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
            Permissions = new ChannelPermissions(model.Permissions);
            Icon = model.Icon != null ? new Attachment(model.Icon) : null;
            IsNsfw = model.Nsfw;
        }

        /// <summary>
        /// Default permissions for all users
        /// </summary>
        public ChannelPermissions Permissions { get; internal set; }

        /// <summary>
        /// List of user ids in the channel
        /// </summary>
        public HashSet<string> Recipents { get; internal set; }

        /// <summary>
        /// List of users in the channel
        /// </summary>
        public ConcurrentDictionary<string, User> Users { get; internal set; } = new ConcurrentDictionary<string, User>();

        /// <summary>
        /// Last message id to be posted in the channel
        /// </summary>
        public string LastMessageId { get; internal set; }

        /// <summary>
        /// Owner of the channel
        /// </summary>
        public string OwnerId { get; internal set; }

        /// <summary>
        /// Name of the channel
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Description of the channel
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Icon attachment of the channel
        /// </summary>
        /// <remarks>
        /// This may be <see langword="null" />
        /// </remarks>
        public Attachment Icon { get; internal set; }

        /// <summary>
        /// Channel has nsfw content
        /// </summary>
        public bool IsNsfw { get; internal set; }


        internal override void Update(PartialChannelJson json)
        {
            if (json.Name.HasValue)
                Name = json.Name.ValueOrDefault();

            if (json.Icon.HasValue)
                Icon = new Attachment(json.Icon.ValueOrDefault());

            if (json.Description.HasValue)
                Description = json.Description.ValueOrDefault();

            if (json.DefaultPermissions.HasValue)
                Permissions = new ChannelPermissions(json.DefaultPermissions.ValueOrDefault().Allowed);

            if (json.Nsfw.HasValue)
                IsNsfw = json.Nsfw.ValueOrDefault();

            if (json.Owner.HasValue)
                OwnerId = json.Owner.ValueOrDefault();

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
