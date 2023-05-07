using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    /// <summary>
    /// A channel between the current bot/user and another user.
    /// </summary>
    public class DMChannel : Channel
    {
        internal DMChannel(RevoltClient client, ChannelJson model) : base(client)
        {
            Id = model.Id;
            Type = ChannelType.DM;
            Active = model.Active;
            InternalRecipents = model.Recipients != null ? model.Recipients : new string[0];
            LastMessageId = model.LastMessageId;
        }

        /// <summary>
        /// If the channel is still open for both users.
        /// </summary>
        public bool Active { get; }

        public string UserId => InternalRecipents.FirstOrDefault(x => x != Client.CurrentUser?.Id);

        internal IReadOnlyList<string> InternalRecipents;
        public string LastMessageId { get; }

        

        internal override void Update(PartialChannelJson json)
        {
        }
    }
}
