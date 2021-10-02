namespace RevoltSharp
{
    public class Profile : Entity
    {
        public string Bio { get; internal set; }
        public Attachment Background { get; internal set; }

        public Profile(RevoltClient client, ProfileJson model)
            : base(client)
        {
            Bio = model.Content;
            Background = new Attachment(client, model.Background);
        }
    }
}
