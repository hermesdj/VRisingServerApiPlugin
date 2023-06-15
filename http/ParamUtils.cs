#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using VRisingServerApiPlugin.command;
using Guid = Il2CppSystem.Guid;

namespace VRisingServerApiPlugin.http;

public static class ParamUtils
{
    private static readonly Regex QueryStringRegex = new Regex(@"[\?&](?<name>[^&=]+)=(?<value>[^&=]+)");

    private static readonly IDictionary<Type, string> TypePatternDictionary = new Dictionary<Type, string>()
    {
        {
            typeof(Guid),
            @"(?<$1>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})"
        },
        {
            typeof(System.Guid),
            @"(?<$1>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})"
        },
        { typeof(int), @"(?<$1>[0-9]*)" },
        { typeof(long), @"(?<$1>[0-9]*)" },
        { typeof(string), @"(?<$1>\w*)" },
        { typeof(bool), @"(?<$1>true|false)" }
    };

    public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string url)
    {
        ApiPlugin.Logger?.LogDebug($"Parsing query params from {url}");
        var matches = QueryStringRegex.Matches(url);
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

    public static object? ParseUrlOrQueryArg(IReadOnlyDictionary<string, string> dictionary, string name,
        ParameterInfo parameter)
    {
        if (!dictionary.TryGetValue(name, out var value)) return parameter.DefaultValue;

        object? result;

        if (parameter.ParameterType == typeof(Guid))
        {
            result = Guid.Parse(value);
        }
        else if (parameter.ParameterType == typeof(System.Guid))
        {
            result = System.Guid.Parse(value);
        }
        else
        {
            result = Convert.ChangeType(value, parameter.ParameterType);
        }

        return result;
    }

    public static string ConvertUrlPattern(string urlPattern,
        Dictionary<string, CommandRegistry.HttpParameter> parameters)
    {
        var urlParamPattern = new Regex(@"{(\w+)}");
        var matches = urlParamPattern.Matches(urlPattern);
        var result = urlPattern;

        foreach (Match match in matches)
        {
            if (!match.Success) continue;

            if (match.Groups.Count == 0) continue;

            foreach (Group group in match.Groups)
            {
                if (!group.Success) continue;

                var paramName = group.Value;

                if (!parameters.TryGetValue(paramName, out var paramType)) continue;

                if (!TypePatternDictionary.TryGetValue(paramType.ParameterType, out var pattern)) continue;

                result = Regex.Replace(result, "{(" + paramName + ")}", pattern);
            }
        }

        return result;
    }
}