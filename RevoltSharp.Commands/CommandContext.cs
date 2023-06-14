namespace RevoltSharp.Commands;

/// <summary> The context of a command which may contain the client, user, guild, channel, and message. </summary>
public class CommandContext
{
    /// <inheritdoc/>
    public RevoltClient Client { get; }
    /// <inheritdoc/>
    public Server Server { get; }
    /// <inheritdoc/>
    public Channel Channel { get; }
    /// <inheritdoc/>
    public User User { get; }
    /// <inheritdoc/>
    public ServerMember Member { get; }
    /// <inheritdoc/>
    public UserMessage Message { get; }

    public CommandInfo Command { get; set; }

    public string Prefix { get; set; }

    /// <summary> Indicates whether the channel that the command is executed in is a private channel. </summary>
    public bool IsPrivate => Channel is DMChannel;

    /// <summary>
    ///     Initializes a new <see cref="CommandContext" /> class with the provided client and message.
    /// </summary>
    /// <param name="client">The underlying client.</param>
    /// <param name="msg">The underlying message.</param>
    public CommandContext(RevoltClient client, UserMessage msg)
    {
        Client = client;
        Channel = msg.Channel;
        User = msg.Author;
        Message = msg;
        if (Channel is TextChannel channel)
        {
            Server = channel.Server;
            if (Server.InternalMembers.TryGetValue(msg.AuthorId, out ServerMember MB))
                Member = MB;
        }
    }

}
