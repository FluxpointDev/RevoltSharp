namespace RevoltSharp;


public class PublicBot : CreatedEntity
{
    internal PublicBot(RevoltClient client, PublicBotJson model) : base(client, model.Id)
    {
        Username = model.Username;
        AvatarId = model.AvatarId;
        Description = model.Description;
    }

    public string Username { get; internal set; }

    public string? AvatarId { get; internal set; }

    public string? Description { get; internal set; }

    /// <summary> Returns a string that represents the current object.</summary>
    /// <returns> Bot username </returns>
    public override string ToString()
    {
        return Username;
    }
}