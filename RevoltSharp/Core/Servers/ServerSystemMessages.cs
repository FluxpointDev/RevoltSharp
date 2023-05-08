using Optionals;

namespace RevoltSharp;

public class ServerSystemMessages : Entity
{
    internal ServerSystemMessages(RevoltClient client, ServerSystemMessagesJson json) : base(client)
    {
        if (json == null)
            return;

        UserJoinedId = json.UserJoined;
        UserLeftId = json.UserLeft;
        UserKickedId = json.UserKicked;
        UserBannedId = json.UserBanned;

    }

    public Optional<string> UserJoinedId { get; set; }

    public TextChannel? UserJoinedChannel => Client.GetTextChannel(UserJoinedId);
    public Optional<string> UserLeftId { get; set; }
    public TextChannel? UserLeftChannel => Client.GetTextChannel(UserLeftId);
    public Optional<string> UserKickedId { get; set; }
    public TextChannel? UserKickedChannel => Client.GetTextChannel(UserKickedId);
    public Optional<string> UserBannedId { get; set; }
    public TextChannel? UserBannedChannel => Client.GetTextChannel(UserBannedId);
}
