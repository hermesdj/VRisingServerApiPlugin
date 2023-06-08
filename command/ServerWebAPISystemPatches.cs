#nullable enable
using System;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppSystem.IO;
using Il2CppSystem.Net;
using Il2CppSystem.Text.RegularExpressions;
using ProjectM;
using ProjectM.Network;
using VRisingServerApiPlugin.clans;
using VRisingServerApiPlugin.http;
using VRisingServerApiPlugin.players;
using VRisingServerApiPlugin.query;

namespace VRisingServerApiPlugin.command;

[HarmonyPatch(typeof(ServerWebAPISystem))]
public class ServerWebAPISystemPatches
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.Strict,
        WriteIndented = false
    };

    [HarmonyPostfix]
    [HarmonyPatch("OnCreate")]
    public static void OnCreate(ServerWebAPISystem __instance)
    {
        if (!SettingsManager.ServerHostSettings.API.Enabled)
        {
            Plugin.Logger?.LogInfo($"HTTP API is not enabled !");
            return;
        }
        
        PlayersCommands.getCommands()
            .ForEach(Command => __instance._HttpReceiveService.AddRoute(new HttpServiceReceiveThread.Route(
                new Regex(Command.Pattern),
                Command.Method,
                BuildAdapter(Command)
            )));
        
        ClansCommands.getCommands()
            .ForEach(Command => __instance._HttpReceiveService.AddRoute(new HttpServiceReceiveThread.Route(
                new Regex(Command.Pattern),
                Command.Method,
                BuildAdapter(Command)
            )));
    }

    private static HttpServiceReceiveThread.RequestHandler BuildAdapter(
        Command command)
    {
        return DelegateSupport.ConvertDelegate<HttpServiceReceiveThread.RequestHandler>(
            new Action<HttpListenerContext>(context =>
            {
                var request = HttpRequestParser.ParseHttpRequest(context.request, command);
                Plugin.Logger?.LogInfo($"Http Request parsed is {JsonSerializer.Serialize(request, _serializerOptions)}");
                var commandResponse = QueryDispatcher.Instance.Dispatch(() => command.CommandHandler(request));
                while (commandResponse.Status == Status.PENDING)
                {
                    Thread.Sleep(25);
                }

                object? responseData;

                if (commandResponse.Status is Status.FAILURE or Status.PENDING)
                {
                    Plugin.Logger?.LogInfo($"Request with url '{context.Request.Url.ToString()}' failed with message : {commandResponse.Exception?.Message}");
                    context.Response.StatusCode = 500;
                    responseData = new InternalServerError("about:blank", "Internal Server Error");
                }
                else
                {
                    responseData = commandResponse.Data;
                }

                context.Response.ContentType = MediaTypeNames.Application.Json;

                var responseWriter = new StreamWriter(context.Response.OutputStream);
                
                responseWriter.Write(JsonSerializer.Serialize(responseData,  _serializerOptions));
                responseWriter.Flush();
            })
        )!;
    }
}