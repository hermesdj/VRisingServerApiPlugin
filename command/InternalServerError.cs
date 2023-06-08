#nullable enable
#nullable enable
namespace VRisingServerApiPlugin.command;

public class InternalServerError
{
    public InternalServerError(string type, string title)
    {
        Type = type;
        Title = title;
    }

    public string Title { get; private set; }

    public string Type { get; private set; }
}