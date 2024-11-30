using RevoltSharp.Rest;

namespace RevoltSharp;
internal class ModifyAccountSessionRequest : IRevoltRequest
{
    public string friendly_name { get; set; }
}
