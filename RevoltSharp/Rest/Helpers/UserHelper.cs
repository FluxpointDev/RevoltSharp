using RevoltSharp.Commands;
using System;
using System.Threading.Tasks;

namespace RevoltSharp.Rest
{
    public static class UserHelper
    {
        public static Task<User> GetUserAsync(this Server server, string userId)
            => GetUserAsync(server.Client.Rest, userId);

        public static Task<User> GetUserAsync(this CommandContext context, string userId)
            => GetUserAsync(context.Client.Rest, userId);

        public static async Task<User> GetUserAsync(this RevoltRestClient rest, string userId)
        {
            if (rest.Client.WebSocket != null && rest.Client.WebSocket.Usercache.TryGetValue(userId, out User User))
                return User;

            UserJson Data = await rest.SendRequestAsync<UserJson>(RequestType.Get, $"users/{userId}", null);
            return User.Create(rest.Client, Data);
        }

        public static Task<Profile> GetProfileAsync(this User user)
            => GetProfileAsync(user.Client.Rest, user.Id);

        public static async Task<Profile> GetProfileAsync(this RevoltRestClient rest, string userId)
        {
            ProfileJson Data = await rest.SendRequestAsync<ProfileJson>(RequestType.Get, $"users/{userId}/profile", null);
            return Profile.Create(Data);
        }

        internal static async Task<DMChannel> GetDMChannel(this RevoltRestClient rest, string userId)
        {
            ChannelJson Data = await rest.SendRequestAsync<ChannelJson>(RequestType.Get, $"users/{userId}/dm", null);
            return (DMChannel)Data.ToEntity(rest.Client);
        }
    }
}
