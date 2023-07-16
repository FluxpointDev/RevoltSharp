using Optionals;

namespace RevoltSharp;

/// <summary>
/// Properties that have been updated for the user.
/// </summary>
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

    /// <summary>
    /// Status text that has been updated.
    /// </summary>
    public Optional<string?> StatusText { get; internal set; }

    /// <summary>
    /// Avatar that has been updated or <see langword="null" /> if removed.
    /// </summary>
    public Optional<Attachment?> Avatar { get; internal set; }

    /// <summary>
    /// User online status has been updated.
    /// </summary>
    public Optional<bool> Online { get; internal set; }

    /// <summary>
    /// User instance privileged has been updated.
    /// </summary>
    public Optional<bool> Privileged { get; internal set; }

    /// <summary>
    /// Username that has been updated.
    /// </summary>
    public Optional<string> Username { get; internal set; }

    /// <summary>
    /// Discriminator that has been updated.
    /// </summary>
    public Optional<string> Discriminator { get; internal set; }

    /// <summary>
    /// Display name that has been updated.
    /// </summary>
    public Optional<string?> DisplayName { get; internal set; }

    /// <summary>
    /// Badges that have been updated.
    /// </summary>
    public Optional<ulong> Badges { get; internal set; }

    /// <summary>
    /// User flags that have been updated.
    /// </summary>
    public Optional<ulong> Flags { get; internal set; }
}