namespace VRisingServerApiPlugin.attributes.methods;

public class HttpPut : HttpAttribute
{
    public HttpPut(string pattern) : base(pattern, "PUT")
    {
    }

    public HttpPut(string pattern, bool isProtected) : base(pattern, "PUT", isProtected)
    {
    }
}