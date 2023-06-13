using System;

namespace VRisingServerApiPlugin.http;

public class HttpException : Exception
{
    public int Status { get; }

    public HttpException()
    {
        Status = 500;
    }

    public HttpException(int status, string message) : base(message)
    {
        Status = status;
    }

    public HttpException(int status, string message, Exception inner) : base(message, inner)
    {
        Status = status;
    }
}