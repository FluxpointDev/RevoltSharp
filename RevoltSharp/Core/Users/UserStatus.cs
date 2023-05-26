using System;

namespace RevoltSharp;

/// <summary>
/// User status mode and presence text.
/// </summary>
public class UserStatus
{
    internal UserStatus(UserJson json)
    {
        if (json == null)
            return;

        if (json.Online)
            Type = UserStatusType.Online;
        else
            Type = UserStatusType.Offline;
        if (json.Status != null)
        {
            Text = json.Status.Text;
            if (json.Status != null && Enum.TryParse(json.Status.Presence, ignoreCase: true, out UserStatusType ST))
                Type = ST;
            else
                Type = UserStatusType.Offline;
        }
        else
            Type = UserStatusType.Offline;
    }

    internal void Update(PartialUserJson json)
    {
        if (json.online.HasValue)
        {
            if (json.online.Value)
                Type = UserStatusType.Online;
            else
                Type = UserStatusType.Offline;
        }

        if (json.status.HasValue && json.status.Value != null)
        {
            Text = json.status.Value.Text;
            if (json.status.Value != null && Enum.TryParse(json.status.Value.Presence, ignoreCase: true, out UserStatusType ST))
                Type = ST;
            else
                Type = UserStatusType.Offline;
        }
    }

    /// <summary>
    /// Custom text status for the user.
    /// </summary>
    public string Text { get; internal set; }

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