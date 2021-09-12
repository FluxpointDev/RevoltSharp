namespace RevoltSharp
{
    public class Profile
    {
        public string bio;
        public Attachment background;

        internal static Profile Create(ProfileJson json)
        {
            return new Profile
            {
                bio = json.content,
                background = json.background != null ? json.background.ToEntity() : null
            };
        }
    }
}
