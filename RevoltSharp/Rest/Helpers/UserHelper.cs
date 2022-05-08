using RevoltSharp.Commands;
using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class UserHelper
    {
        public static Task<User> GetUserAsync(this Server server, string userId)
            => GetUserAsync(server.Client.Rest, userId);

        public static Task<User> GetUserAsync(this CommandContext context, string userId)
            => GetUserAsync(context.Client.Rest, userId);

        public static async Task<User> GetUserAsync(this RevoltRestClient rest, string userId)
        {
            if (rest.Client.WebSocket != null && rest.Client.WebSocket.UserCache.TryGetValue(userId, out User User))
                return User;

            UserJson Data = await rest.SendRequestAsync<UserJson>(RequestType.Get, $"users/{userId}");
            if (Data == null)
                return null;
            User user = new User(rest.Client, Data);
            if (rest.Client.WebSocket != null)
                rest.Client.WebSocket.UserCache.TryAdd(userId, user);
            return user;
        }

        public static Task<Profile> GetProfileAsync(this CommandContext context, string userId)
            => GetProfileAsync(context.Client.Rest, userId);

        public static Task<Profile> GetProfileAsync(this User user)
            => GetProfileAsync(user.Client.Rest, user.Id);

        public static async Task<Profile> GetProfileAsync(this RevoltRestClient rest, string userId)
        {
            ProfileJson Data = await rest.SendRequestAsync<ProfileJson>(RequestType.Get, $"users/{userId}/profile");
            return new Profile(rest.Client, Data);
        }

    }
}
