using System;

namespace VRisingServerApiPlugin.attributes.methods;

[AttributeUsage(AttributeTargets.Method)]
public class HttpAttribute : Attribute
{
    public string Pattern { get; set; }

    public string Method { get; set; }

    public bool Protected { get; set; }

    public HttpAttribute(string pattern, string method, bool isProtected = false)
    {
        Pattern = pattern;
        Method = method;
        Protected = isProtected;
    }
}