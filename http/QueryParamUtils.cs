#nullable enable
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VRisingServerApiPlugin.http;

public class QueryParamUtils
{
    private static readonly Regex queryStringRegex;

    static QueryParamUtils()
    {
        queryStringRegex = new Regex(@"[\?&](?<name>[^&=]+)=(?<value>[^&=]+)");
    }
    public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string url)
    {
        ApiPlugin.Logger?.LogDebug($"Parsing query params from {url}");
        var matches = queryStringRegex.Matches(url);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var name = match.Groups["name"].Value;
            var value = match.Groups["value"].Value;
            yield return new KeyValuePair<string, string>(name, value);
        }
    }
    
    public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string url, string pattern)
    {
        var regex = new Regex(pattern);
        ApiPlugin.Logger?.LogDebug($"Parsing query params from {url} with Pattern {pattern}");
        var matches = regex.Matches(url);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            if (match.Groups.Count <= 1) continue;
            for (var j = 1; j < match.Groups.Count; j++)
            {
                var group = match.Groups[j];
                var name = group.Name;
                var value = group.Value;
                yield return new KeyValuePair<string, string>(name, value);   
            }
        }
    }
}