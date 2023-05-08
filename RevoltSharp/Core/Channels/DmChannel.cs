using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;

/// <summary>
/// A channel between the current user/bot account and another user.
/// </summary>
public class DMChannel : Channel
{
    internal DMChannel(RevoltClient client, ChannelJson model) : base(client, model)
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
    public bool Active { get; internal set; }

    /// <summary>
    /// The user id for this DM channel.
    /// </summary>
    public string UserId => InternalRecipents.FirstOrDefault(x => x != Client.CurrentUser?.Id);

    /// <summary>
    /// The user for this DM channel.
    /// </summary>
    public User? User => Client.GetUser(UserId);

    internal IReadOnlyList<string> InternalRecipents;

    /// <summary>
    /// The last message id sent in this DM channel.
    /// </summary>
    public string LastMessageId { get; internal set; }

    internal override void Update(PartialChannelJson json)
    {
        if (json.Active.HasValue)
            Active = json.Active.Value;
    }
}
