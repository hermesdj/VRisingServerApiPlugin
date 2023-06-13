namespace VRisingServerApiPlugin.attributes.parameters;

public class UrlParam : HttpParamAttribute
{
    public string Name { get; set; }

    public UrlParam(string name)
    {
        Name = name;
    }
}