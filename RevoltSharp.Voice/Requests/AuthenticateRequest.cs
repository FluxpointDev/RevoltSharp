namespace RevoltSharp;


internal class AuthenticateRequest
{
    internal AuthenticateRequest(string channelId, string token)
    {
        data = new AuthDataRequest
        {
            roomId = channelId,
            token = token
        };
    }
    public int id = 3;
    public string type = "Authenticate";
    public AuthDataRequest data;
}
internal class AuthDataRequest
{
    public string roomId = null!;
    public string token = null!;
}
internal class RoomInfoRequest
{
    public int id = 13;
    public string type = "RoomInfo";
}