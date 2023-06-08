#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.http;

public readonly record struct HttpRequest(
    object? body,
    string url,
    Dictionary<string, string> urlParams,
    Dictionary<string, string> queryParams
);