using Optionals;

namespace RevoltSharp;

public class SelfUserUpdatedProperties : UserUpdatedProperties
{
    internal SelfUserUpdatedProperties(RevoltClient client, PartialUserJson json) : base(client, json)
    {

    }

    public Optional<string> ProfileContent;

    public Optional<Attachment> ProfileBackground;
}
