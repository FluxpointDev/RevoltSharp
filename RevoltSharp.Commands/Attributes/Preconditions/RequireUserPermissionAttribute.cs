using System;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireUserPermissionAttribute : PreconditionAttribute
{
    /// <summary>
    /// Server/group permission to check for.
    /// </summary>
    private ServerPermission? Server;

    /// <summary>
    /// Channel permission to check for.
    /// </summary>
    private ChannelPermission? Channel;

    public RequireUserPermissionAttribute(ServerPermission perm)
    {
        Server = perm;
    }

    public RequireUserPermissionAttribute(ChannelPermission perm)
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
                if (context.Client.Config.OwnerBypassPermissions)
                {
                    if (context.Client.CurrentUser.OwnerId == context.User.Id || context.Client.Config.Owners.Contains(context.User.Id))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                }

                if (context.Server.OwnerId == context.Member.Id || context.Member.Permissions.Has(Server.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());

                return Task.FromResult(PreconditionResult.FromError($"You need server permissions for **{Server.Value.ToString()}** to use this command."));
            }
            else
            {
                if (context.Channel is GroupChannel GC)
                {
                    if (context.Client.Config.OwnerBypassPermissions)
                    {
                        if (context.Client.CurrentUser.OwnerId == context.User.Id || context.Client.Config.Owners.Contains(context.User.Id))
                            return Task.FromResult(PreconditionResult.FromSuccess());
                    }

                    if (GC.OwnerId == context.User.Id || GC.Permissions.Has(Server.Value))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                }

                return Task.FromResult(PreconditionResult.FromError($"You need group permissions for **{Server.Value.ToString()}** to use this command."));
            }
        }
        else
        {
            if (context.Server != null)
            {
                if (context.Client.Config.OwnerBypassPermissions)
                {
                    if (context.Client.CurrentUser.OwnerId == context.User.Id || context.Client.Config.Owners.Contains(context.User.Id))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                }

                if (context.Server.OwnerId == context.Member.Id)
                    return Task.FromResult(PreconditionResult.FromSuccess());

                if (context.Channel is ServerChannel SC && context.Member.GetPermissions(SC).Has(Channel.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());

                return Task.FromResult(PreconditionResult.FromError($"You need channel permissions for **{Channel.Value.ToString()}** to use this command."));
            }
            else
            {
                if (context.Channel is GroupChannel GC)
                {
                    if (context.Client.Config.OwnerBypassPermissions)
                    {
                        if (context.Client.CurrentUser.OwnerId == context.User.Id || context.Client.Config.Owners.Contains(context.User.Id))
                            return Task.FromResult(PreconditionResult.FromSuccess());
                    }

                    if (GC.OwnerId == context.User.Id || GC.Permissions.Has(Channel.Value))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                }

                return Task.FromResult(PreconditionResult.FromError($"You need group permissions for **{Channel.Value.ToString()}** to use this command."));
            }
        }
    }
}