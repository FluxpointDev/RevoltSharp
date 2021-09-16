namespace RevoltSharp
{
    public class Profile
    {
        public string bio { get; internal set; }
        public Attachment background { get; internal set; }

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
