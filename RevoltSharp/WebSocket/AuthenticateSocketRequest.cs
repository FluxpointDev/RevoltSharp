using Newtonsoft.Json;

namespace RevoltSharp.WebSocket;
internal class AuthenticateSocketRequest
{
    internal AuthenticateSocketRequest(string token)
    {
        Token = token;
    }

    [JsonProperty("type")]
    public string Type = "Authenticate";

    [JsonProperty("token")]
    public string Token;
}
