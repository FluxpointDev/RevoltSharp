namespace RevoltSharp;

public class SelfUser : User
{
    public string OwnerId => BotData.Owner;

    /// <summary>
    /// Get the Owner user of the bot.
    /// </summary>
    /// <remarks>
    /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
    /// </remarks>
    public User? Owner
        => Client.GetUser(OwnerId);

    public string ProfileBio { get; internal set; }

    public Attachment? Background { get; internal set; }

    internal SelfUser(RevoltClient client, UserJson model)
        : base(client, model)
    {

        ProfileBio = model.Profile?.Content;
        Background = model.Profile?.Background != null ? new Attachment(model.Profile.Background) : null;
    }


    internal new SelfUser Clone()
    {
        return (SelfUser)this.MemberwiseClone();
    }

}
