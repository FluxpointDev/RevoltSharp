using RevoltSharp.Core.Servers;

namespace RevoltSharp;

public class ServerBan : Entity
{
    internal ServerBan(RevoltClient client, ServerBanUserJson json, ServerBanInfoJson jsonInfo) : base(client)
    {
        if (json == null)
            return;

        Id = json.Id;
        Username = json.Username;
        Reason = jsonInfo.Reason;
        Avatar = Attachment.Create(client, json.Avatar);

    }

    public string Id { get; internal set; }
    public string Username { get; }
    public string Reason { get; internal set; }
    public Attachment? Avatar { get; }
}
