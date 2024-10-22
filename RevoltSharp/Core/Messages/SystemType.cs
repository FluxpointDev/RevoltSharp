namespace RevoltSharp;

public enum SystemType
{
    /// <summary>
    /// Unknown system message type.
    /// </summary>
    Unknown,

    /// <summary>
    /// System message with text.
    /// </summary>
    Text,

    /// <summary>
    /// User has been added to the group.
    /// </summary>
    GroupUserAdded,

    /// <summary>
    /// User has been removed from the group.
    /// </summary>
    GroupUserRemoved,

    /// <summary>
    /// User has joined the server.
    /// </summary>
    ServerUserJoined,

    /// <summary>
    /// User has left the server.
    /// </summary>
    ServerUserLeft,

    /// <summary>
    /// User has been kicked from the server.
    /// </summary>
    ServerUserKicked,

    /// <summary>
    /// User has been banned from the server.
    /// </summary>
    ServerUserBanned,

    /// <summary>
    /// Group channel name has been changed.
    /// </summary>
    GroupNameChanged,

    /// <summary>
    /// Group channel description has been changed.
    /// </summary>
    GroupDescriptionChanged,

    /// <summary>
    /// Group channel icon has been changed.
    /// </summary>
    GroupIconChanged,

    /// <summary>
    /// Group channel owner has been changed.
    /// </summary>
    GroupOwnerChanged
}
