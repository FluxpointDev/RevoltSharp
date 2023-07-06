using Optionals;

namespace RevoltSharp;


public class SelfUserUpdatedProperties : UserUpdatedProperties
{
    internal SelfUserUpdatedProperties(RevoltClient client, PartialUserJson json) : base(client, json)
    {
        ProfileContent = json.Profile.Value.Content;
        if (json.Profile.Value.Background.HasValue)
            ProfileBackground = Optional.Some(Attachment.Create(client, json.Profile.Value.Background.Value));

    }

    public Optional<string?> ProfileContent;

    public Optional<Attachment?> ProfileBackground;
}
