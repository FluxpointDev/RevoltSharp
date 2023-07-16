using System;

namespace RevoltSharp;

public class InitilizeTransportRequest
{
    public int id = 29;
    public string type = "InitializeTransports";
    public IntTransportDataRequest data = new IntTransportDataRequest();
}
public class IntTransportDataRequest
{
    public string mode = "SplitWebRTC";
    public IntTransportCapsRequest rtpCapabilities = new IntTransportCapsRequest();

}
public class IntTransportCapsRequest
{
    public IntTransportCodecsRequest[] codecs = new IntTransportCodecsRequest[]
    {
        new IntTransportCodecsRequest()
    };
    public IntTransportHeaderRequest[] headerExtensions = new IntTransportHeaderRequest[]
    {
        new IntTransportHeaderRequest
        {
            kind = "audio",
            uri = "urn:ietf:params:rtp-hdrext:sdes:mid",
            prefferedId = 1,
            direction = "sendrecv"
        },
        new IntTransportHeaderRequest
        {
            kind = "video",
            uri = "urn:ietf:params:rtp-hdrext:sdes:mid",
            prefferedId = 1,
            direction = "sendrecv"
        },
        new IntTransportHeaderRequest
        {
            kind = "video",
            uri = "http://www.webrtc.org/experiments/rtp-hdrext/abs-send-time",
            prefferedId = 4,
            direction = "sendrecv"
        },
        new IntTransportHeaderRequest
        {
            kind = "video",
            uri = "http://www.ietf.org/id/draft-holmer-rmcat-transport-wide-cc-extensions-01",
            prefferedId = 5,
            direction = "sendrecv"
        },
        new IntTransportHeaderRequest
        {
            kind = "audio",
            uri = "urn:ietf:params:rtp-hdrext:ssrc-audio-level",
            prefferedId = 10,
            direction = "sendrecv"
        },
        new IntTransportHeaderRequest
        {
            kind = "video",
            uri = "urn:ietf:params:rtp-hdrext:toffset",
            prefferedId = 12,
            direction = "sendrecv"
        }
    };
}
public class IntTransportCodecsRequest
{
    public int channels = 2;
    public int clockRate = 48000;
    public string kind = "audio";
    public string mimeType = "audio/opus";
    public int preferredPayloadType = 100;
    public IntTransportCodecsParamsRequest parameters = new IntTransportCodecsParamsRequest();
    public IntTransportCodescFeedbackRequest[] rtcpFeedback = Array.Empty<IntTransportCodescFeedbackRequest>();
}
public class IntTransportCodescFeedbackRequest
{
    public string parameter = "";
    public string type = "transport-cc";
}
public class IntTransportCodecsParamsRequest
{
    public int maxplaybackrate = 48000;
    public int stereo = 1;
    public int useinbandfec = 1;
}
public class IntTransportHeaderRequest
{
    public string kind = null!;
    public string uri = null!;
    public int prefferedId;
    public bool prefferedEncrypt = false;
    public string direction = null!;
}