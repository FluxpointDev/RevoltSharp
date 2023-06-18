using System;

namespace RevoltSharp;

public class ServerBan : CreatedEntity
{
    internal ServerBan(RevoltClient client, ServerBanUserJson json, ServerBanInfoJson jsonInfo) : base(client, json.Id)
    {
        Username = json.Username;
        Reason = jsonInfo.Reason;
        Avatar = Attachment.Create(client, json.Avatar);
    }

    /// <summary>
    /// Id of the ban.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the ban occured.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

    /// <summary>
    /// Username of the banned user.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Reason for ban creation.
    /// </summary>
    public string? Reason { get; internal set; }

    public Attachment? Avatar { get; }
}
