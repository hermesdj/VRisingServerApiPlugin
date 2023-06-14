namespace VRisingServerApiPlugin.attributes.methods;

public class HttpPatch : HttpAttribute
{
    public HttpPatch(string pattern) : base(pattern, "PATCH")
    {
    }

    public HttpPatch(string pattern, bool isProtected) : base(pattern, "PATCH", isProtected)
    {
    }
}