namespace RevoltSharp.Rest.Requests;

internal class BulkDeleteMessagesRequest : IRevoltRequest
{
    public string[] ids;
}
