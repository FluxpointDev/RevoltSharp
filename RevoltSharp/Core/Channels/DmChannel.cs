﻿using System.Linq;

namespace RevoltSharp;


/// <summary>
/// A channel between the current user/bot account and another user.
/// </summary>
public class DMChannel : Channel
{
    internal DMChannel(RevoltClient client, ChannelJson model) : base(client, model)
    {
        Type = ChannelType.DM;
        Active = model.Active;
        UserId = model.Recipients.First(x => x != client.CurrentUser.Id);
        LastMessageId = model.LastMessageId;
    }

    /// <summary>
    /// If the channel is still open for both users.
    /// </summary>
    public bool Active { get; internal set; }

    /// <summary>
    /// The user id for this DM channel.
    /// </summary>
    public string UserId { get; internal set; }

    /// <summary>
    /// The user for this DM channel.
    /// </summary>
    public User? User => Client.GetUser(UserId);

    /// <summary>
    /// The last message id sent in this DM channel.
    /// </summary>
    public string? LastMessageId { get; internal set; }

    internal override void Update(PartialChannelJson json)
    {
        if (json.Active.HasValue)
            Active = json.Active.Value;
    }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> User#0001 or Unknown User </returns>
    public override string ToString()
    {
        return User != null ? User.Tag : "Unknown User";
    }
}