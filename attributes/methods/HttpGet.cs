namespace VRisingServerApiPlugin.attributes.methods;

public class HttpGet : HttpAttribute
{
    public HttpGet(string pattern) : base(pattern, "GET")
    {
    }

    public HttpGet(string pattern, bool isProtected) : base(pattern, "GET", isProtected)
    {
    }
}