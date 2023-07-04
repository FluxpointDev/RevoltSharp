using System;
using System.Threading.Tasks;

namespace RevoltSharp.Commands
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireContextAttribute : PreconditionAttribute
    {

        public RequireContextAttribute(ContextType contextTypes)
        {
            Contexts = contextTypes;
        }

        /// <inheritdoc />
        public override string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets the context required to execute the command.
        /// </summary>
        public ContextType Contexts { get; }

        /// <inheritdoc />
        public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
        {
            bool isValid = false;

            if ((Contexts & ContextType.Server) != 0)
                isValid = context.Channel is ServerChannel;
            if ((Contexts & ContextType.DM) != 0)
                isValid = isValid || context.Channel is DMChannel;
            if ((Contexts & ContextType.Group) != 0)
                isValid = isValid || context.Channel is GroupChannel;

            if (isValid)
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError($"Invalid channel context for command, require contexts are {Contexts}"));
        }
    }

    [Flags]
    public enum ContextType
    {
        DM = 0x01,
        Group = 0x02,
        Server = 0x03
    }
}