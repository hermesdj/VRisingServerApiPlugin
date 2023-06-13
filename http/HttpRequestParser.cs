#nullable enable
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Il2CppSystem.Net;
using VRisingServerApiPlugin.command;

namespace VRisingServerApiPlugin.http;

public static class HttpRequestParser
{
    public static HttpRequest ParseHttpRequest(HttpListenerRequest request, Command command)
    {
        var body = "";
        var contentType = request.Headers["Content-Type"];

        if (command.Method == "GET")
            return new HttpRequest(
                queryParams: QueryParamUtils.ParseQueryString(request.url.Query)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                urlParams: QueryParamUtils.ParseQueryString(request.url.LocalPath, command.Pattern)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                url: request.raw_url,
                body: body,
                contentType: contentType
            );

        var inputStream = new Il2CppSystem.IO.StreamReader(request.InputStream);
        body = inputStream.ReadToEnd();

        return new HttpRequest(
            queryParams: QueryParamUtils.ParseQueryString(request.url.Query)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            urlParams: QueryParamUtils.ParseQueryString(request.url.LocalPath, command.Pattern)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            url: request.raw_url,
            body: body,
            contentType: contentType
        );
    }
}