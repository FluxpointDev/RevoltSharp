using System;

namespace RevoltSharp;

public class ServerBanInfo : CreatedEntity
{
    internal ServerBanInfo(RevoltClient client, ServerBanInfoJson jsonInfo) : base(client, jsonInfo.Id.UserId)
    {
        UserId = jsonInfo.Id.UserId;
        Reason = jsonInfo.Reason;
    }

    public string UserId { get; internal set; }
    public string Reason { get; internal set; }
}
public class ServerBanUser : CreatedEntity
{
    internal ServerBanUser(RevoltClient client, ServerBanUserJson json, ServerBanInfoJson jsonInfo) : base(client, json.Id)
    {
        Username = json.Username;
        Discriminator = json.Discriminator;
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

    private string Discriminator { get; set; }

    /// <summary>
    /// Reason for ban creation.
    /// </summary>
    public string? Reason { get; internal set; }

    public Attachment? Avatar { get; }
}
