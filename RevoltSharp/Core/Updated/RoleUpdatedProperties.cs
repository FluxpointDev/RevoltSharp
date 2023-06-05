using Optionals;
using System;
using System.Numerics;

namespace RevoltSharp;

public class RoleUpdatedProperties : CreatedEntity
{
    internal RoleUpdatedProperties(RevoltClient client, Role role, PartialRoleJson json) : base(client, role.Id)
    {
        Name = json.Name;
        if (json.Permissions.HasValue)
            Permissions = Optional.Some(new ServerPermissions(role.Server, json.Permissions.Value.Allowed));

        Hoist = json.Hoist;
        Rank = json.Rank;
        if (json.Colour.HasValue)
            Color = Optional.Some(new RevoltColor(json.Colour.Value));
    }

    /// <summary>
    /// Id of the role.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the role was created.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

    public Optional<string> Name { get; private set; }

    public Optional<ServerPermissions> Permissions { get; private set; }

    public Optional<bool> Hoist { get; private set; }

    public Optional<BigInteger> Rank { get; private set; }

    public Optional<RevoltColor> Color { get; private set; }
}
