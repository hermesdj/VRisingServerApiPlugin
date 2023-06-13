#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRisingServerApiPlugin.attributes;
using VRisingServerApiPlugin.attributes.methods;
using VRisingServerApiPlugin.attributes.parameters;
using VRisingServerApiPlugin.http;
using Guid = Il2CppSystem.Guid;

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

    private static void RegisterMethod(object? container, MethodInfo method,
        HttpHandlerAttribute httpHandlerAttribute)
    {
        if (method.GetCustomAttribute(typeof(HttpAttribute), true) is not HttpAttribute httpAttribute) return;

        var pattern = $"^{httpHandlerAttribute.BasePath}{httpAttribute.Pattern}$";

        var isProtected = httpAttribute.Protected || httpHandlerAttribute.AllRouteProtected;

        var command = new Command(
            pattern: pattern,
            method: httpAttribute.Method,
            commandHandler: GenerateHandler(container, method),
            isProtected: isProtected
        );

        Commands.Add(command);
    }

    private static Func<HttpRequest, object?> GenerateHandler(object? container, MethodBase method)
    {
        return request =>
        {
            var args = new List<object?>();
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var paramAttr = parameter.GetCustomAttribute(typeof(HttpParamAttribute), true);

                switch (paramAttr)
                {
                    case null:
                        throw new HttpException(500, "Param attribute cannot be null !");
                    case RequestBody:
                        args.Add(
                            BodyParserUtils.Deserialize(request.contentType, parameter.ParameterType, request.body));
                        break;
                    case UrlParam param:
                        args.Add(ParseUrlOrQueryArg(request.urlParams, param.Name, parameter));
                        break;
                    case QueryParam param:
                        args.Add(ParseUrlOrQueryArg(request.queryParams, param.Name, parameter));
                        break;
                    default:
                        throw new HttpException(500,
                            "No default case should happen while checking the parameter attrs");
                }
            }

            if (args.Count != parameters.Length)
            {
                ApiPlugin.Logger?.LogInfo($"parameters are {parameters} and parsed args are {args}");
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

    private static object? ParseUrlOrQueryArg(Dictionary<string, string> dictionary, string name,
        ParameterInfo parameter)
    {
        var result = parameter.DefaultValue;

        if (dictionary.TryGetValue(name, out var value))
        {
            result = parameter.ParameterType == typeof(Guid)
                ? Guid.Parse(value)
                : Convert.ChangeType(value, parameter.ParameterType);
        }

        return result;
    }

    private static IEnumerable<Type> ListAllHttpHandlerTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(type => type.IsDefined(typeof(HttpHandlerAttribute)));
    }
}