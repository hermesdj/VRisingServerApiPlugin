#nullable enable
using System.Collections.Generic;
using System.Linq;
using VRisingServerApiPlugin.command;

namespace VRisingServerApiPlugin.clans;

public abstract class ClansCommands : CommandHandler
{
    public static List<Command> getCommands()
    {
        var commands = new List<Command>
        {
            new(
                Pattern: "^/v-rising-server-api/clans$",
                Method: "GET",
                CommandHandler: (_) => GetAllClans()
            )
        };

        return commands;
    }

    private static ListClanResponse GetAllClans()
    {
        var clans = ServerWorld.GetAllClans()
            .Select(ClanUtils.Convert)
            .ToList();

        return new ListClanResponse(
            clans: clans
            );
    }
}