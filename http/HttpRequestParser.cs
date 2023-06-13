#nullable enable
using System.Linq;
using Il2CppSystem.Net;
using Il2CppSystem.Security.Principal;
using VRisingServerApiPlugin.command;

namespace VRisingServerApiPlugin.http;

public static class HttpRequestParser
{
    public static HttpRequest ParseHttpRequest(HttpListenerContext context, Command command)
    {
        var request = context.request;

        var authenticatedUser = ParseAuthenticatedUser(context);

        if (authenticatedUser.HasValue)
        {
            ApiPlugin.Logger?.LogInfo(
                $"Authenticated user with name {authenticatedUser?.Username} and IsAuthorized {authenticatedUser?.IsAuthorized}");
        }

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
                contentType: contentType,
                user: authenticatedUser
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
            contentType: contentType,
            user: authenticatedUser
        );
    }

    private static AuthenticatedUser? ParseAuthenticatedUser(HttpListenerContext context)
    {
        context.ParseAuthentication(AuthenticationSchemes.Basic);

        if (context.user == null) return null;

        var principal = context.user.TryCast<GenericPrincipal>();
        var identity = principal?.m_identity.TryCast<HttpListenerBasicIdentity>();

        if (identity == null) return null;

        var username = identity.Name;
        var password = identity.password;

        var isAuthorized = ApiPlugin.Instance.CheckAuthenticationOfUser(username, password);

        return new AuthenticatedUser(Username: identity.Name, Password: identity.password,
            IsAuthorized: isAuthorized);
    }

    public readonly record struct AuthenticatedUser(
        string Username,
        string Password,
        bool IsAuthorized
    );
}