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

    public class RevoltRestException : RevoltException
    {
        public RevoltRestException(string message, int code) : base(message, code)
        {

        }
    }

    public class RevoltArgumentException : RevoltException
    {
        public RevoltArgumentException(string message) : base(message, 400)
        {

        }
    }
}
