using System;

namespace RevoltSharp;

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
    public RevoltRestException(string message, int code, RevoltErrorType type) : base(message, code)
    {
        Type = type;
    }

    public RevoltErrorType Type { get; private set; } = RevoltErrorType.Unknown;

    public string Permission;
}

public class RevoltArgumentException : RevoltException
{
    public RevoltArgumentException(string message) : base(message, 400)
    {

    }
}
