namespace VRisingServerApiPlugin.attributes.methods;

public class HttpGet : HttpAttribute
{
    public HttpGet(string pattern) : base(pattern, "GET")
    {
    }
}