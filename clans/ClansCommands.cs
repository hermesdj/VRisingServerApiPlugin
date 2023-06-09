#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ProjectM;
using Unity.Collections;
using VRisingServerApiPlugin.command;
using VRisingServerApiPlugin.http;
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
                ),
            new(
                Pattern: @"^/v-rising-server-api/clans/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})/updateName$",
                Method: "POST",
                CommandHandler: (_) => UpdateClanName(_.urlParams["id"], BodyParserUtils.Deserialize<UpdateClanNameBody>(_.body))
                ),
            new(
                Pattern: @"^/v-rising-server-api/clans/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})/updateMotto$",
                Method: "POST",
                CommandHandler: (_) => UpdateClanMotto(_.urlParams["id"], BodyParserUtils.Deserialize<UpdateClanMottoBody>(_.body))
            )
        };

        return commands;
    }

    private static GetClanResponse GetClanById(string clanId)
    {
        var clan = ClanUtils.GetClanById(clanId);
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

    private static GetClanResponse UpdateClanName(string clanId, UpdateClanNameBody? body)
    {
        Plugin.Logger?.LogInfo($"Body is {JsonSerializer.Serialize(body)}");
        if (body.HasValue)
        {
            var clan = ClanUtils.GetClanWithEntityById(clanId);

            if (clan.HasValue)
            {
                var clanTeam = ServerWorld.EntityManager.GetComponentData<ClanTeam>(clan.Value.ClanEntity);
                Plugin.Logger?.LogInfo(
                    $"Updating clan name from '{clanTeam.Name.ToString()}' to '{body.Value.Name}'");
                clanTeam.Name = new FixedString64(body.Value.Name);
                ServerWorld.EntityManager.SetComponentData(clan.Value.ClanEntity, clanTeam);
            }
        }
        
        var result = ClanUtils.GetClanById(clanId);

        return new GetClanResponse(result.HasValue ? ClanUtils.Convert(result.Value) : null);
    }

    private static GetClanResponse UpdateClanMotto(string clanId, UpdateClanMottoBody? body)
    {
        if (body.HasValue)
        {
            var clan = ClanUtils.GetClanWithEntityById(clanId);

            if (clan.HasValue)
            {
                var clanTeam = ServerWorld.EntityManager.GetComponentData<ClanTeam>(clan.Value.ClanEntity);
                Plugin.Logger?.LogInfo(
                    $"Updating clan motto from '{clanTeam.Motto.ToString()}' to '{body.Value.Motto}'");
                clanTeam.Motto = new FixedString64(body.Value.Motto);
                ServerWorld.EntityManager.SetComponentData(clan.Value.ClanEntity, clanTeam);
            }
        }
        
        var result = ClanUtils.GetClanById(clanId);

        return new GetClanResponse(result.HasValue ? ClanUtils.Convert(result.Value) : null);
    }
}