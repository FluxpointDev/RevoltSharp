using System;
using Optionals;

namespace RevoltSharp;


/// <summary>
/// User status mode and presence text.
/// </summary>
public class UserStatus
{
    internal UserStatus(UserJson? json)
    {
        if (json == null)
            return;
        Update(Optional.Some(json.Online), json.Status == null ? Optional.None<UserStatusJson>() : Optional.Some(json.Status));
    }

    internal void Update(PartialUserJson json)
    {
        Update(json.Online, json.Status);
    }

    internal void Update(Optional<bool> online, Optional<UserStatusJson> status)
    {
        if (!status.HasValue)
        {
            Type = UserStatusType.Offline;
            Text = null;
            return;
        }

        Text = status.Value.Text;
        if (Enum.TryParse(status.Value.Presence, ignoreCase: true, out UserStatusType ST))
            Type = ST;
        else
            if (online.HasValue)
            Type = online.Value ? UserStatusType.Online : UserStatusType.Offline;
    }

    /// <summary>
    /// Custom text status for the user.
    /// </summary>
    public string? Text { get; internal set; }

    /// <summary>
    /// Status mode for the user.
    /// </summary>
    public UserStatusType Type { get; internal set; }
}

/// <summary>
/// Status mode for the user.
/// </summary>
public enum UserStatusType
{
    /// <summary>
    /// User is not online on Revolt.
    /// </summary>
    Offline,

    /// <summary>
    /// User is online and using Revolt.
    /// </summary>
    Online,

    /// <summary>
    /// User is away from their computer.
    /// </summary>
    Idle,

    /// <summary>
    /// User is focused on a task but is available.
    /// </summary>
    Focus,

    /// <summary>
    /// Do not FK WITH THIS USER.
    /// </summary>
    Busy,

    /// <summary>
    /// Who you gonna call? Ghost busters!
    /// </summary>
    Invisible
}