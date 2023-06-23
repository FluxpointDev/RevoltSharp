using Optionals;

namespace RevoltSharp;

public class UserUpdatedProperties
{
    internal UserUpdatedProperties(RevoltClient client, PartialUserJson json)
    {
        if (json.Status.HasValue && json.Status.Value.Text != null)
            StatusText = Optional.Some(json.Status.Value.Text!);

        Avatar = json.Avatar.ToModel(client);
        Online = json.Online;
        Privileged = json.Privileged;
        Username = json.Username;
        Discriminator = json.Discriminator;
        DisplayName = json.DisplayName;
        Badges = json.Badges;
        Flags = json.Flags;
    }

    public Optional<string> StatusText { get; internal set; }

    public Optional<Attachment?> Avatar { get; internal set; }

    public Optional<bool> Online { get; internal set; }

    public Optional<bool> Privileged { get; internal set; }

    public Optional<string> Username { get; internal set; }

    public Optional<string> Discriminator { get; internal set; }

    public Optional<string> DisplayName { get; internal set; }

    public Optional<ulong> Badges { get; internal set; }

    public Optional<ulong> Flags { get; internal set; }
}
