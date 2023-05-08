namespace RevoltSharp;

/// <summary>
/// Revolt channel that can be casted to types <see cref="GroupChannel"/>, <see cref="TextChannel"/>, <see cref="VoiceChannel"/> <see cref="ServerChannel" /> <see cref="UnknownServerChannel" /> or <see cref="UnknownChannel"/>
/// </summary>
public abstract class Channel : CreatedEntity
{
    /// <summary>
    /// Id of the channel.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// The channel type to cast to.
    /// </summary>
    public ChannelType Type { get; internal set; }

    internal Channel(RevoltClient client, ChannelJson model)
        : base(client, model?.Id)
    { }

    internal abstract void Update(PartialChannelJson json);

    internal Channel Clone()
    {
        return (Channel) this.MemberwiseClone();
    }

    internal static Channel Create(RevoltClient client, ChannelJson model)
    {
        switch (model.ChannelType)
        {
            case ChannelType.SavedMessages:
                return new SavedMessagesChannel(client, model);
            case ChannelType.DM:
                return new DMChannel(client, model);
            case ChannelType.Group:
                return new GroupChannel(client, model);
            case ChannelType.Text:
                return new TextChannel(client, model);
            case ChannelType.Voice:
                return new VoiceChannel(client, model);
            default:
            {
                    if (!string.IsNullOrEmpty(model.Server))
                    {
                        return new UnknownServerChannel(client, model);
                    }

                    return new UnknownChannel(client, model);
            }
        }
    }
}
