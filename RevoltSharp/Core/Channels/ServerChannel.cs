﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;


/// <summary>
/// Base channel for all servers that can be casted to <see cref="TextChannel" /> <see cref="VoiceChannel" /> or <see cref="UnknownServerChannel" />
/// </summary>
public class ServerChannel : Channel
{
    internal ServerChannel(RevoltClient client, ChannelJson model) : base(client, model)
    {
        ServerId = model.ServerId!;
        DefaultPermissions = new ChannelPermissions(Server, model.DefaultPermissions);
        InternalRolePermissions = model.RolePermissions != null ? model.RolePermissions.ToDictionary(x => x.Key, x => new ChannelPermissions(Server, x.Value)) : new Dictionary<string, ChannelPermissions>();
        Name = model.Name!;
        Description = model.Description;
        Icon = Attachment.Create(client, model.Icon);
    }

    /// <summary>
    /// If of the parent server
    /// </summary>
    public string ServerId { get; internal set; }

    /// <summary>
    /// Parent server of the channel
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    [JsonIgnore]
    public Server? Server
        => Client.GetServer(ServerId);

    /// <summary>
    /// Default permissions for all members in the channel
    /// </summary>
    public ChannelPermissions DefaultPermissions { get; internal set; }

    /// <summary>
    /// Role permission for the channel that wil override default permissions
    /// </summary>

    internal Dictionary<string, ChannelPermissions> InternalRolePermissions { get; set; }

    /// <summary>
    /// Role permission for the channel that wil override default permissions
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<ChannelPermissions> RolePermissions
        => InternalRolePermissions.Values;

    /// <summary>
    /// Name of the channel
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// Description of the channel
    /// </summary>
    public string? Description { get; internal set; }

    /// <summary>
    /// Icon attachment of the channel
    /// </summary>
    /// <remarks>
    /// This may be <see langword="null" />
    /// </remarks>
    public Attachment? Icon { get; internal set; }

    /// <summary>
    /// Check if a member has a permission for the channel
    /// </summary>
    /// <param name="member"></param>
    /// <param name="permission"></param>
    /// <returns><see langword="true" /> if member has permission</returns>
    public bool HasPermission(ServerMember member, ChannelPermission permission)
    {
        bool HasDefault = DefaultPermissions.Has(permission);
        if (HasDefault)
            return true;
        foreach (KeyValuePair<string, ChannelPermissions> c in InternalRolePermissions)
        {
            if (member.InternalRoles.ContainsKey(c.Key))
            {
                bool HasRole = c.Value.Has(permission);
                if (HasRole)
                    return true;
            }
        }
        return false;
    }

    internal override void Update(PartialChannelJson json)
    {
        if (json.Name.HasValue)
            Name = json.Name.Value;

        if (json.Icon.HasValue)
            Icon = Attachment.Create(Client, json.Icon.Value);

        if (json.DefaultPermissions.HasValue)
            DefaultPermissions = new ChannelPermissions(Server, json.DefaultPermissions.Value);

        if (json.Description.HasValue)
            Description = json.Description.Value;

        if (json.RolePermissions.HasValue)
        {
            foreach (KeyValuePair<string, PermissionsJson> i in json.RolePermissions.Value)
            {
                if (InternalRolePermissions.TryGetValue(i.Key, out ChannelPermissions CP))
                {
                    CP.RawAllowed = i.Value.Allowed;
                    CP.RawDenied = i.Value.Denied;
                }
                else
                    InternalRolePermissions.Add(i.Key, new ChannelPermissions(Server, i.Value));
            }
        }
    }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Server channel name </returns>
    public override string ToString()
    {
        return Name;
    }
}