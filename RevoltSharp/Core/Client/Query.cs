namespace RevoltSharp;
public class Query
{
    internal Query(QueryJson json)
    {
        RevoltVersion = json.RevoltVersion;
        AppUrl = json.AppUrl;
        WebsocketUrl = json.WebsocketUrl;
        CaptchaEnabled = json.ServerFeatures.captcha.enabled;
        EmailEnabled = json.ServerFeatures.email;
        InviteOnly = json.ServerFeatures.invite_only;
        ImageServerUrl = json.ServerFeatures.ImageServer.url;
        if (!ImageServerUrl.EndsWith("/"))
            ImageServerUrl += "/";

        JanuaryServerUrl = json.ServerFeatures.JanuaryServer.url;
        if (!JanuaryServerUrl.EndsWith("/"))
            JanuaryServerUrl += "/";

        VoiceApiUrl = json.ServerFeatures.VoiceServer.url;
        if (!VoiceApiUrl.EndsWith("/"))
            VoiceApiUrl += "/";

        VoiceWebsocketUrl = json.ServerFeatures.VoiceServer.ws;
    }

    public string RevoltVersion { get; internal set; }

    public string AppUrl { get; internal set; }

    public string WebsocketUrl { get; internal set; }

    public bool CaptchaEnabled { get; internal set; }

    public bool EmailEnabled { get; internal set; }

    public bool InviteOnly { get; internal set; }

    public string ImageServerUrl { get; internal set; }

    public string JanuaryServerUrl { get; internal set; }

    public string VoiceApiUrl { get; internal set; }

    public string VoiceWebsocketUrl { get; internal set; }
}
