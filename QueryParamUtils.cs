using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VRisingServerApiPlugin;

public class QueryParamUtils
{
    private static readonly Regex queryStringRegex;

    static QueryParamUtils()
    {
        queryStringRegex = new Regex(@"[\?&](?<name>[^&=]+)=(?<value>[^&=]+)");
    }
    public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string url)
    {
        Plugin.Logger?.LogInfo($"Parsing query params from {url}");
        var matches = queryStringRegex.Matches(url);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var name = match.Groups["name"].Value;
            var value = match.Groups["value"].Value;
            Plugin.Logger?.LogInfo($"Found Match {name}={value}");
            yield return new KeyValuePair<string, string>(name, value);
        }
    }
    
    public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string url, string pattern)
    {
        var regex = new Regex(pattern);
        Plugin.Logger?.LogInfo($"Parsing query params from {url}");
        var matches = regex.Matches(url);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            for (var j = 0; j < match.Groups.Count; j++)
            {
                var name = match.Groups[j].Name;
                var value = match.Groups[j].Value;
                Plugin.Logger?.LogInfo($"Found Match {name}={value}");
                yield return new KeyValuePair<string, string>(name, value);   
            }
        }
    }
}