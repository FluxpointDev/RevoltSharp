using System;

namespace RevoltSharp
{
    public class RevoltException : Exception
    {
        public RevoltException(string message, int code = 0)
        {
            Message = message;
            Code = code;
        }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
