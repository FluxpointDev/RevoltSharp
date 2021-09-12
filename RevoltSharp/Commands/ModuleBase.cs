using System.Threading.Tasks;
using RevoltSharp.Commands.Builders;

namespace RevoltSharp.Commands
{

    /// <summary>
    ///     Provides a base class for a command module to inherit from.
    /// </summary>
    /// <typeparam name="T">A class that implements <see cref="CommandContext"/>.</typeparam>
    public abstract class ModuleBase : IModuleBase
    {
        /// <summary>
        ///     The underlying context of the command.
        /// </summary>
        /// <seealso cref="T:Discord.Commands.CommandContext" />
        /// <seealso cref="T:Discord.Commands.CommandContext" />
        public CommandContext Context { get; private set; }

        /// <summary>
        ///     Sends a message to the source channel.
        /// </summary>
        /// <param name="message">
        /// Contents of the message; optional only if <paramref name="embed" /> is specified.
        /// </param>
        /// <param name="isTTS">Specifies if Discord should read this <paramref name="message"/> aloud using text-to-speech.</param>
        /// <param name="embed">An embed to be displayed alongside the <paramref name="message"/>.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the <paramref name="message"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        protected virtual async Task<Message> ReplyAsync(string message = null)
        {
            return await Context.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }


        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <param name="filePath">The file path of the file.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich" /> <see cref="Embed" /> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="isSpoiler">Whether the message attachment should be hidden as a spoiler.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        protected virtual async Task<Message> ReplyFileAsync(string filePath, string text = null)
        {
            FileAttachment File = await Context.Client.Rest.UploadFileAsync(filePath, Rest.RevoltRestClient.UploadFileType.Attachment);
            if (File == null)
                return await Context.Channel.SendMessageAsync(text).ConfigureAwait(false);
            return await Context.Channel.SendMessageAsync(text, new string[] { File.Id }).ConfigureAwait(false);
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
}
