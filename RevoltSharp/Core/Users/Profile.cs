namespace RevoltSharp
{
    public class Profile : Entity
    {
        public string Bio { get; internal set; }
        public Attachment Background { get; internal set; }

        internal Profile(RevoltClient client, ProfileJson model)
            : base(client)
        {
            Bio = model.Content;
            Background = model.Background != null ? new Attachment(model.Background) : null;
        }
    }
}
