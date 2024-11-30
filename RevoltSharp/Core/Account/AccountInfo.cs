namespace RevoltSharp;

public class AccountInfo : CreatedEntity
{
    internal AccountInfo(RevoltClient client, AccountInfoJson json) : base(client, json._id)
    {
        Email = json.email;
    }

    public string Email { get; internal set; }
}
