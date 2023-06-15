#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRisingServerApiPlugin.attributes;
using VRisingServerApiPlugin.attributes.methods;
using VRisingServerApiPlugin.attributes.parameters;
using VRisingServerApiPlugin.http;

namespace VRisingServerApiPlugin.command;

public static class CommandRegistry
{
    private static readonly List<Command> Commands = new();

    public static List<Command> GetCommands() => Commands;

    public static void RegisterAll() => RegisterAll(Assembly.GetCallingAssembly());

    public static void RegisterAll(Assembly assembly)
    {
        var types = ListAllHttpHandlerTypes(assembly);

        foreach (var type in types)
        {
            Register(type);
        }
    }

    private static void Register(Type type)
    {
        if (type.GetCustomAttribute(typeof(HttpHandlerAttribute), true) is not HttpHandlerAttribute attr)
        {
            throw new Exception($"Type {type.Name} is not an Http Handler !");
        }

        var methods = type.GetMethods()
            .Where(methodInfo => methodInfo.GetCustomAttributes(typeof(HttpAttribute), true).Length > 0);

        var container = Activator.CreateInstance(type);

        foreach (var method in methods)
        {
            RegisterMethod(container, method, attr);
        }
    }

    private static void RegisterMethod(object? container, MethodBase method,
        HttpHandlerAttribute httpHandlerAttribute)
    {
        if (method.GetCustomAttribute(typeof(HttpAttribute), true) is not HttpAttribute httpAttribute) return;

        var paramDictionary = ParseParameters(method);
        var pattern =
            $"^{httpHandlerAttribute.BasePath}{ParamUtils.ConvertUrlPattern(httpAttribute.Pattern, paramDictionary)}$";

        var isProtected = httpAttribute.Protected || httpHandlerAttribute.AllRouteProtected;

        var command = new Command(
            pattern: pattern,
            method: httpAttribute.Method,
            commandHandler: GenerateHandler(container, method, paramDictionary),
            isProtected: isProtected
        );

        Commands.Add(command);
    }

    private static Dictionary<string, HttpParameter> ParseParameters(MethodBase methodBase)
    {
        var result = new Dictionary<string, HttpParameter>();

        foreach (var parameter in methodBase.GetParameters())
        {
            var paramAttr = parameter.GetCustomAttribute(typeof(HttpParamAttribute), true);
            var httpParameter = new HttpParameter
            {
                ParameterType = parameter.ParameterType,
                Attribute = paramAttr as HttpAttribute
            };

            switch (paramAttr)
            {
                case null:
                    throw new HttpException(500, "Param attribute cannot be null !");
                case RequestBody:
                    httpParameter.Name = "RequestBody";
                    httpParameter.Parse = (request) =>
                        BodyParserUtils.Deserialize(request.contentType, parameter.ParameterType, request.body);
                    break;
                case UrlParam param:
                    httpParameter.Name = param.Name;
                    httpParameter.Parse = (request) =>
                        ParamUtils.ParseUrlOrQueryArg(request.urlParams, param.Name, parameter);
                    break;
                case QueryParam param:
                    httpParameter.Name = param.Name;
                    httpParameter.Parse = (request) =>
                        ParamUtils.ParseUrlOrQueryArg(request.queryParams, param.Name, parameter);
                    break;
                default:
                    throw new HttpException(500,
                        "No default case should happen while checking the parameter attrs");
            }

            result.Add(httpParameter.Name, httpParameter);
        }

        return result;
    }

    private static Func<HttpRequest, object?> GenerateHandler(object? container, MethodBase method,
        Dictionary<string, HttpParameter> httpParameters)
    {
        return request =>
        {
            var args = method.GetParameters()
                .Where(info => info.GetCustomAttribute(typeof(HttpParamAttribute), true) != null)
                .Select(parameterInfo => parameterInfo.GetCustomAttribute(typeof(HttpParamAttribute), true))
                .Select(attribute =>
                {
                    return attribute switch
                    {
                        RequestBody => "RequestBody",
                        UrlParam param => param.Name,
                        QueryParam param => param.Name,
                        _ => throw new HttpException(500, "Invalid parameters definition !")
                    };
                })
                .Where(httpParameters.ContainsKey)
                .Select(name => httpParameters[name])
                .Select(parameter => parameter.Parse(request))
                .ToList();

            if (args.Count != httpParameters.Count)
            {
                ApiPlugin.Logger?.LogInfo($"parameters are {httpParameters} and parsed args are {args}");
                throw new HttpException(400, "Invalid parameters !");
            }

            try
            {
                return method.Invoke(container, args.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException == null) throw;

                ApiPlugin.Logger?.LogError(
                    $"Inner Exception {ex.InnerException?.Message}, stack: {ex.InnerException?.StackTrace}");
                throw ex.InnerException!;
            }
        };
    }

    private static IEnumerable<Type> ListAllHttpHandlerTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => type.IsDefined(typeof(HttpHandlerAttribute)));
    }

    public record struct HttpParameter(
        string Name,
        HttpAttribute? Attribute,
        Type ParameterType,
        Func<HttpRequest, object?> Parse
    );
}