using System;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireGroupAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Channel.Type != ChannelType.Group)
            return Task.FromResult(PreconditionResult.FromError("You need to run this command in a group channel."));

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}