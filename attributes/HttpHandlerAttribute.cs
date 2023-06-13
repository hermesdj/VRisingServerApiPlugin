using System;

namespace VRisingServerApiPlugin.attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HttpHandlerAttribute : Attribute
{
    public string BasePath { get; set; }

    public bool AllRouteProtected { get; set; }

    public HttpHandlerAttribute(string basePath = "/", bool allRouteProtected = false)
    {
        BasePath = basePath;
        AllRouteProtected = allRouteProtected;
    }
}