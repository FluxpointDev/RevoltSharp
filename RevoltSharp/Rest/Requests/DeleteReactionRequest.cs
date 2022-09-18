namespace RevoltSharp.Rest.Requests
{
    internal class DeleteReactionRequest : RevoltRequest
    {
        public string user_id;
        public bool remove_all;
    }
}
