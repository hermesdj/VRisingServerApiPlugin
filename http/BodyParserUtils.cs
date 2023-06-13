#nullable enable
using System;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRisingServerApiPlugin.http;

public static class BodyParserUtils
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
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
        }

        try
        {
            var rawText = ((JsonDocument)body).RootElement.GetRawText();
            Plugin.Logger?.LogDebug($"Deserializing to {typeof(T)} from body {rawText}");
            return JsonSerializer.Deserialize<T>(rawText, SerializerOptions);
        }
        catch (JsonException ex)
        {
            Plugin.Logger?.LogWarning($"Exception Deserializing from body : {ex.Message}");
            return null;
        }
    }

    public static object? Deserialize(string contentType, Type objectType, string body)
    {
        if (contentType == MediaTypeNames.Application.Json)
        {
            try
            {
                return JsonSerializer.Deserialize(body, objectType, SerializerOptions);
            }
            catch (JsonException ex)
            {
                Plugin.Logger?.LogWarning($"Exception Deserializing from body : {ex.Message}");
                return null;
            }
        }
        else
        {
            throw new Exception($"Unsupported content type {contentType}");
        }
    }
}