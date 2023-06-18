using System;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireUserPermissionAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override string? ErrorMessage { get; set; }

    /// <summary>
    /// Permission to check for.
    /// </summary>
    private ServerPermission Perm;

    public RequireUserPermissionAttribute(ServerPermission perm)
    {
        Perm = perm;
    }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Server == null)
            return Task.FromResult(PreconditionResult.FromError("You need to run this command in a Revolt server."));

        if (!context.Member.Permissions.Has(Perm))
            return Task.FromResult(PreconditionResult.FromError($"You need server permissions for **{Perm.ToString()}** to use this command."));

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
