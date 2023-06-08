#nullable enable
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem;
using VRisingServerApiPlugin.command;
using VRisingServerApiPlugin.http;

namespace VRisingServerApiPlugin.players;

public abstract class PlayersCommands : CommandHandler
{
    public static List<Command> getCommands()
    {
        var commands = new List<Command>
        {
            new(
                Pattern: @"^/v-rising-server-api/players/connected$",
                Method: "GET",
                CommandHandler: (_) => GetConnectedPlayers()
            ),
            new(
                Pattern: @"^/v-rising-server-api/players$",
                Method: "GET",
                CommandHandler: (_) => GetAllPlayers()
                ),
            new(
                Pattern: @"^/v-rising-server-api/players/(?<id>[0-9]*)$",
                Method: "GET",
                CommandHandler: (request) => GetPlayerDetails(Int32.Parse(request.urlParams["id"]))
                )
        };

        return commands;
    }

    private static PlayerListApiResponse GetConnectedPlayers()
    {
        var players = ServerWorld.GetAllPlayerCharacters()
            .Where(player => player.User.IsConnected)
            .Select(PlayerUtils.Convert).ToList();

        return new PlayerListApiResponse(players);
    }

    private static PlayerListApiResponse GetAllPlayers()
    {
        var players = ServerWorld.GetAllPlayerCharacters()
            .Select(PlayerUtils.Convert).ToList();

        return new PlayerListApiResponse(players);
    }

    private static PlayerApiResponse GetPlayerDetails(int userIndex)
    {
        var player = ServerWorld.GetPlayer(userIndex);
        return player.HasValue ? new PlayerApiResponse(PlayerUtils.ConvertDetails(player.Value)) : new PlayerApiResponse();
    }
}