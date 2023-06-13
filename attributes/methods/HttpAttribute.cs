using System;

namespace VRisingServerApiPlugin.attributes.methods;

[AttributeUsage(AttributeTargets.Method)]
public class HttpAttribute : Attribute
{
    public string Pattern { get; set; }

    public string Method { get; set; }
    
    public HttpAttribute(string pattern, string method)
    {
        Pattern = pattern;
        Method = method;
    }
}