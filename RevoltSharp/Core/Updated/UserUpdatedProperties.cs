using Optionals;

namespace RevoltSharp;

public class UserUpdatedProperties
{
    internal UserUpdatedProperties(RevoltClient client, PartialUserJson json)
    {
        if (json.status.HasValue)
            StatusText = Optional.Some(json.status.Value.Text);


        Avatar = json.avatar.ToModel(client);
    }

    public Optional<string> StatusText { get; internal set; }

    public Optional<Attachment?> Avatar { get; internal set; }

    public Optional<bool> Online { get; internal set; }

    public Optional<bool> Privileged { get; internal set; }

    public Optional<string> Username { get; internal set; }

    public Optional<ulong> Badges { get; internal set; }

    public Optional<ulong> Flags { get; internal set; }
}
