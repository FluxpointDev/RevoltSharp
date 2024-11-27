namespace RevoltSharp;

public class AccountSession : CreatedEntity
{
    public AccountSession(AccountSessionJson session, RevoltClient client) : base(client, session.Id)
    {
        Id = session.Id;
        Name = session.Name;
    }

    public string Id { get; internal set; }

    public string Name { get; internal set; }
}
