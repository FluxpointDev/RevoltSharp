using RevoltSharp.Rest;

namespace RevoltSharp.Requests;
internal class AdminStrikeCreateRequest : IRevoltRequest
{
    public string user_id;
    public string reason;
}
