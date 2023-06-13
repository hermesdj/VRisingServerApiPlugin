namespace VRisingServerApiPlugin.attributes.methods;

public class HttpPost : HttpAttribute
{
    public HttpPost(string pattern) : base(pattern, "POST")
    {
    }

    public HttpPost(string pattern, bool isProtected) : base(pattern, "POST", isProtected)
    {
    }
}