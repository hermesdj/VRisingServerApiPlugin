using System.Collections.Generic;

namespace VRisingServerApiPlugin.command;

public interface CommandHandler
{
    internal static List<Command> getCommands() => new List<Command>();
}