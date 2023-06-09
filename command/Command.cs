#nullable enable
using System;
using VRisingServerApiPlugin.http;

namespace VRisingServerApiPlugin.command;

public class Command
{
    public string Pattern { get; set; }
    public string Method { get; set; }

    public Func<HttpRequest, object> CommandHandler { get; set; }

    public Command(string Pattern, string Method, Func<HttpRequest, object> CommandHandler){
        this.Pattern = Pattern;
        this.Method = Method;
        this.CommandHandler = CommandHandler;
    }
}