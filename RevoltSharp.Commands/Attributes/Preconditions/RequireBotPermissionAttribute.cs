using System;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override string? ErrorMessage { get; set; }

    /// <summary>
    /// Permission to check for.
    /// </summary>
    private ServerPermission Perm;

    public RequireBotPermissionAttribute(ServerPermission perm)
    {
        Perm = perm;
    }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Server == null)
            return Task.FromResult(PreconditionResult.FromError("You need to run this command in a Revolt server."));

        if (!context.Server.CurrentUser.Permissions.Has(Perm))
            return Task.FromResult(PreconditionResult.FromError($"Current user/bot account needs server permission for **{Perm.ToString()}** to use this command."));

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
