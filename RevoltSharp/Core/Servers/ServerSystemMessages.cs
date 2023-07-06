using Optionals;

namespace RevoltSharp;


public class ServerSystemMessages : Entity
{
    internal ServerSystemMessages(RevoltClient client, ServerSystemMessagesJson json) : base(client)
    {
        if (json == null)
            return;

        UserJoinedChannelId = json.UserJoined;
        UserLeftChannelId = json.UserLeft;
        UserKickedChannelId = json.UserKicked;
        UserBannedChannelId = json.UserBanned;

    }

    public Optional<string> UserJoinedChannelId { get; set; }

    public TextChannel? UserJoinedChannel => Client.GetTextChannel(UserJoinedChannelId);
    public Optional<string> UserLeftChannelId { get; set; }
    public TextChannel? UserLeftChannel => Client.GetTextChannel(UserLeftChannelId);
    public Optional<string> UserKickedChannelId { get; set; }
    public TextChannel? UserKickedChannel => Client.GetTextChannel(UserKickedChannelId);
    public Optional<string> UserBannedChannelId { get; set; }
    public TextChannel? UserBannedChannel => Client.GetTextChannel(UserBannedChannelId);
}