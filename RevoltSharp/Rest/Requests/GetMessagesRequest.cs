namespace RevoltSharp.Rest.Requests
{
    public class GetMessagesRequest : RevoltRequest
    {
        public int limit = 1;
        public string before;
        public string after;
        public string sort = "Latest";
        public string nearby;
        public bool include_users;
    }
}
