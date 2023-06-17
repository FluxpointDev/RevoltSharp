using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireContextAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Server == null)
            return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? "You need to run this command in a Revolt server."));

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
