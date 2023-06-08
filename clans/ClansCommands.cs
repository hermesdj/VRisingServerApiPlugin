#nullable enable
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using VRisingServerApiPlugin.command;
using Guid = Il2CppSystem.Guid;

namespace VRisingServerApiPlugin.clans;

public abstract class ClansCommands : CommandHandler
{
    public static List<Command> getCommands()
    {
        var commands = new List<Command>
        {
            new(
                Pattern: @"^/v-rising-server-api/clans$",
                Method: "GET",
                CommandHandler: (_) => GetAllClans()
            ),
            new(
                Pattern: @"^/v-rising-server-api/clans/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})$",
                Method: "GET",
                CommandHandler: (_) => GetClanById(_.urlParams["id"])
                ),
            new(
                Pattern: @"^/v-rising-server-api/clans/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})/players$",
                Method: "GET",
                CommandHandler: (_) => GetAllClanPlayers(_.urlParams["id"])
                )
        };

        return commands;
    }

    private static GetClanResponse GetClanById(string clanId)
    {
        ClanTeam? clan = ServerWorld.GetAllClans()
            .FirstOrDefault(clanTeam => clanTeam.ClanGuid.Equals(Guid.Parse(clanId)));
        
        return new GetClanResponse(clan.HasValue ? ClanUtils.Convert(clan.Value) : null);
    }

    private static ListClanResponse GetAllClans()
    {
        var clans = ServerWorld.GetAllClans()
            .Select(ClanUtils.ConvertWithPlayers)
            .ToList<object>();

        return new ListClanResponse(
            clans: clans
            );
    }

    private static ListClanPlayersResponse GetAllClanPlayers(string clanId)
    {
        ClanTeam? clan = ServerWorld.GetAllClans()
            .FirstOrDefault(clanTeam => clanTeam.ClanGuid.Equals(Guid.Parse(clanId)));

        return new ListClanPlayersResponse(players: clan.HasValue ? ClanUtils.GetClanPlayers(clan.Value) : null, id: clan?.ClanGuid.ToString());
    }
}