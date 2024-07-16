using RevoltSharp.Commands.Builders;
using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;



/// <summary>
///     Provides a base class for a command module to inherit from with a <see cref="CommandContext"/>.
///  </summary>
public abstract class ModuleBase : IModuleBase
{
    /// <summary>
    ///     The underlying context of the command.
    /// </summary>
    public CommandContext? Context { get; private set; }

    /// <summary>
    ///     Sends a message to the source channel.
    /// </summary>
    /// <param name="message">
    /// Contents of the message; optional only if <paramref name="embeds" /> is specified.
    /// </param>
    /// <param name="embeds">An embed to be displayed alongside the <paramref name="message"/>.</param>
    /// <param name="attachments"></param>
    /// <param name="masquerade"></param>
    /// <param name="interactions"></param>
    /// <param name="replies"></param>
    protected virtual async Task<UserMessage> ReplyAsync(string message, Embed[] embeds = null, string[] attachments = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null, MessageFlag flags = MessageFlag.None)
    {
        return await Context.Channel.SendMessageAsync(message, embeds, attachments, masquerade, interactions, replies, flags).ConfigureAwait(false);
    }


    /// <summary>
    ///     Sends a file to this message channel with an optional caption.
    /// </summary>
    /// <param name="filePath">The file path of the file.</param>
    /// <param name="text">The message to be sent.</param>
    /// <param name="embeds">The <see cref="Embed" /> to be sent.</param>
    /// <param name="masquerade"></param>
    /// <param name="interactions"></param>
    /// <param name="replies"></param>
    protected virtual async Task<UserMessage> ReplyFileAsync(string filePath, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null, MessageFlag flags = MessageFlag.None)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new RevoltArgumentException("File path cannot be empty when uploading files.");
        FileAttachment File = await Context.Client.Rest.UploadFileAsync(filePath, UploadFileType.Attachment);
        return await Context.Channel.SendMessageAsync(text, embeds, new string[] { File.Id }, masquerade, interactions, replies, flags).ConfigureAwait(false);
    }


    /// <summary>
    ///     The method to execute before executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual void BeforeExecute(CommandInfo command)
    {
    }
    /// <summary>
    ///     The method to execute after executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual void AfterExecute(CommandInfo command)
    {
    }

    /// <summary>
    ///     The method to execute when building the module.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/> used to create the module.</param>
    /// <param name="builder">The builder used to build the module.</param>
    protected virtual void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
    {
    }

    //IModuleBase
    void IModuleBase.SetContext(CommandContext context)
    {
        Context = context;
    }
    void IModuleBase.BeforeExecute(CommandInfo command) => BeforeExecute(command);
    void IModuleBase.AfterExecute(CommandInfo command) => AfterExecute(command);
    void IModuleBase.OnModuleBuilding(CommandService commandService, ModuleBuilder builder) => OnModuleBuilding(commandService, builder);
}