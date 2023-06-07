﻿#nullable enable
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem;
using VRisingServerApiPlugin.command;

namespace VRisingServerApiPlugin.players;

public abstract class PlayersCommands : CommandHandler
{
    public static List<Command> getCommands()
    {
        var commands = new List<Command>
        {
            new(
                Pattern: "^/v-rising-server-api/connected-players$",
                Method: "GET",
                commandHandler: (_) => GetConnectedPlayers()
            ),
            new(
                Pattern: "^/v-rising-server-api/players$",
                Method: "GET",
                commandHandler: (_) => GetAllPlayers()
                ),
            new(
                Pattern: "^/v-rising-server-api/players?id=(.*)",
                Method: "GET",
                commandHandler: (context) => GetPlayerDetails(Int32.Parse(context.request.QueryString.Get("id"))))
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
        return new PlayerApiResponse(null);
    }
}