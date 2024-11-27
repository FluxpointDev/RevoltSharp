namespace RevoltSharp;

public class AccountLogin
{
    internal AccountLogin(AccountLoginJson json)
    {
        if (json == null)
            return;

        switch (json.Result)
        {
            case "Success":
                ResponseType = LoginResponseType.Success;
                break;
            case "MFA":
                ResponseType = LoginResponseType.MFARequired;
                break;
            case "Disabled":
                ResponseType = LoginResponseType.Disabled;
                break;
            default:
                ResponseType = LoginResponseType.Failed;
                break;
        }

        Id = json.Id;
        UserId = json.UserId;
        Token = json.Token;
        SessionName = json.Name;
        MFATicket = json.Ticket;
        MFAMethods = json.AllowedMethods;
    }

    public LoginResponseType ResponseType { get; set; }
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public string SessionName { get; set; }
    public string MFATicket { get; set; }
    public string[] MFAMethods { get; set; }
}
