using System;
using Il2CppSystem.Net;

namespace VRisingServerApiPlugin.command;

public readonly record struct Command(
    string Pattern,
    string Method,
    Func<HttpListenerContext, object> commandHandler
);