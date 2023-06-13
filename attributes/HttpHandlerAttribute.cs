using System;

namespace VRisingServerApiPlugin.attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HttpHandlerAttribute : Attribute
{
    public string BasePath { get; set; }

    public HttpHandlerAttribute(string basePath = "/")
    {
        BasePath = basePath;
    }
}