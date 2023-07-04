namespace RevoltSharp
{

    public class SocketError
    {
        public string? Message { get; internal set; }
        public RevoltErrorType Type { get; internal set; }
    }
}