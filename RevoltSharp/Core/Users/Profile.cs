namespace RevoltSharp;

public class Profile : Entity
{
    internal Profile(RevoltClient client, ProfileJson model) : base(client)
    {
        Bio = model.Content;
        Background = Attachment.Create(client, model.Background);
    }

    public string? Bio { get; internal set; }
    public Attachment? Background { get; internal set; }
}
