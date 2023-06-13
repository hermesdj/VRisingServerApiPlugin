#nullable enable
using System;
using VRisingServerApiPlugin.http;

namespace VRisingServerApiPlugin.command;

public class Command
{
    public string Pattern { get; }
    public string Method { get; }

    public Func<HttpRequest, object?> CommandHandler { get; }

    public Command(string pattern, string method, Func<HttpRequest, object?> commandHandler){
        Pattern = pattern;
        Method = method;
        CommandHandler = commandHandler;
    }
}