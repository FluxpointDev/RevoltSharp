namespace RevoltSharp.Rest.Requests;

internal class DeleteReactionRequest : IRevoltRequest
{
    public string user_id;
    public bool remove_all;
}
