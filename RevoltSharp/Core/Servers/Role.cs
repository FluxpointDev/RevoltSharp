using System.Numerics;

namespace RevoltSharp;

/// <summary>
/// Server role
/// </summary>
public class Role : CreatedEntity
{
    public string Id { get; internal set; }

    public string ServerId { get; internal set; }

    /// <summary>
    /// Server that the Role is from.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public Server? Server => Client.GetServer(ServerId);

    public string Name { get; internal set; }

    public ServerPermissions Permissions { get; internal set; }

    public bool IsHoisted { get; internal set; }

    public BigInteger Rank { get; internal set; }

    public RevoltColor Color { get; internal set; }

    internal Role(RevoltClient client, RoleJson model, string serverId, string roleId)
        : base(client, roleId)
    {

        Id = roleId;
        
        Color = new RevoltColor(model.Colour);
        IsHoisted = model.Hoist;
        Name = model.Name;
        Permissions = model.Permissions == null ? new ServerPermissions(0) : new ServerPermissions(model.Permissions.Allowed);
        Rank = model.Rank;
        ServerId = serverId;
    }

    internal Role(RevoltClient client, PartialRoleJson model, string serverId, string roleId)
        : base(client, roleId)
    {
        Id = roleId;
        Name = model.Name.Value;
        if (model.Permissions.HasValue)
            Permissions = new ServerPermissions(model.Permissions.Value.Allowed);
        else
            Permissions = new ServerPermissions(0);

        if (model.Colour.HasValue)
            Color = new RevoltColor(model.Colour.Value);
        else
            Color = new RevoltColor("");

        ServerId = serverId;
    }

    internal void Update(PartialRoleJson json)
    {
        if (json.Name.HasValue)
            Name = json.Name.Value;

        if (json.Permissions.HasValue)
            Permissions = new ServerPermissions(json.Permissions.Value.Allowed);

        if (json.Hoist.HasValue)
            IsHoisted = json.Hoist.Value;

        if (json.Rank.HasValue)
            Rank = json.Rank.Value;

        if (json.Colour.HasValue)
            Color = new RevoltColor(json.Colour.Value);
    }

    internal Role Clone()
    {
        return (Role) this.MemberwiseClone();
    }
}
