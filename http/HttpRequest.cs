#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.http;

public readonly record struct HttpRequest(
    string body,
    string url,
    string contentType,
    Dictionary<string, string> urlParams,
    Dictionary<string, string> queryParams,
    HttpRequestParser.AuthenticatedUser? user = null
);