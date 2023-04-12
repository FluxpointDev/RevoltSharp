using System;

namespace RevoltSharp
{
    public class RevoltException : Exception
    {
        public RevoltException(string message, int code = 0) : base(message)
        {
            Code = code;
        }
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
