namespace RevoltSharp.Rest.Requests;

internal class BulkDeleteMessagesRequest : RevoltRequest
{
    public string[] ids;
}
