#nullable enable
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRisingServerApiPlugin.http;

public class BodyParserUtils
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.Strict,
        WriteIndented = false
    };
    
    public static T? Deserialize<T>(object? body) where T : struct
    {
        if (body == null)
        {
                Plugin.Logger?.LogWarning($"Provided body is null, cannot deserialize.");
            return null;
        };

        try
        {
            var rawText = ((JsonDocument)body).RootElement.GetRawText();
            Plugin.Logger?.LogDebug($"Deserializing to {typeof(T)} from body {rawText}");
            return JsonSerializer.Deserialize<T>(rawText, _serializerOptions);
        }
        catch (JsonException ex)
        {
            Plugin.Logger?.LogWarning($"Exception Deserializing from body : {ex.Message}");
            return null;
        }
    }
}