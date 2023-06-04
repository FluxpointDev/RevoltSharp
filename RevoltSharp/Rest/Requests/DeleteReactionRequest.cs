namespace RevoltSharp.Rest.Requests;

internal class DeleteReactionRequest : IRevoltRequest
{
    public string user_id { get; internal set; }
    public bool remove_all { get; internal set; }
}
