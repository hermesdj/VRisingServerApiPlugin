namespace VRisingServerApiPlugin.attributes.methods;

public class HttpDelete : HttpAttribute
{
    public HttpDelete(string pattern) : base(pattern, "DELETE")
    {
    }

    public HttpDelete(string pattern, bool isProtected) : base(pattern, "DELETE", isProtected)
    {
    }
}