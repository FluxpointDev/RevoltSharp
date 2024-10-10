using System;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// Server/group permission to check for.
    /// </summary>
    private ServerPermission? Server;

    /// <summary>
    /// Channel permission to check for.
    /// </summary>
    private ChannelPermission? Channel;

    public RequireBotPermissionAttribute(ServerPermission perm)
    {
        Server = perm;
    }

    public RequireBotPermissionAttribute(ChannelPermission perm)
    {
        Channel = perm;
    }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Server == null && context.Channel.Type != ChannelType.Group)
            return Task.FromResult(PreconditionResult.FromError("You need to run this command in a Revolt server or group."));

        if (Server.HasValue)
        {
            if (context.Server != null)
            {
                if (context.Server.OwnerId == context.Client.CurrentUser.Id || context.Member.Permissions.Has(Server.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());

                return Task.FromResult(PreconditionResult.FromError($"Bot needs server permissions for **{Server.Value.ToString()}** to use this command."));
            }
            else
            {
                if (context.Channel is GroupChannel GC)
                {
                    if (GC.OwnerId == context.Client.CurrentUser.Id || GC.Permissions.Has(Server.Value))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                }

                return Task.FromResult(PreconditionResult.FromError($"Bot needs group permissions for **{Server.Value.ToString()}** to use this command."));
            }
        }
        else
        {
            if (context.Server != null)
            {
                if (context.Server.OwnerId == context.Client.CurrentUser.Id)
                    return Task.FromResult(PreconditionResult.FromSuccess());

                if (context.Channel is ServerChannel SC && context.Server.CurrentUser.GetPermissions(SC).Has(Channel.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());

                return Task.FromResult(PreconditionResult.FromError($"Bot needs channel permissions for **{Channel.Value.ToString()}** to use this command."));
            }
            else
            {
                if (context.Channel is GroupChannel GC)
                {
                    if (GC.OwnerId == context.Client.CurrentUser.Id || GC.Permissions.Has(Channel.Value))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                }

                return Task.FromResult(PreconditionResult.FromError($"Bot needs group permissions for **{Channel.Value.ToString()}** to use this command."));
            }
        }
    }
}