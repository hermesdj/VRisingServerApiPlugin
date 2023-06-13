namespace VRisingServerApiPlugin.attributes.parameters;

public class QueryParam : HttpParamAttribute
{
    public string Name { get; set; }

    public QueryParam(string name)
    {
        Name = name;
    }
}