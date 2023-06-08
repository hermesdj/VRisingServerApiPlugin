#nullable enable
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using Stunlock.Localization;
using Unity.Collections;
using Entity = Unity.Entities.Entity;

namespace VRisingServerApiPlugin.players;

public static class PlayerUtils
{
    private static ClanTeam? ResolveClan(ServerWorld.Player player)
    {
        var clanEntity = player.User.ClanEntity._Entity;
        ClanTeam? clan = null;

        if (ServerWorld.EntityManager.HasComponent<ClanTeam>(clanEntity))
        {
            clan = ServerWorld.EntityManager.GetComponentData<ClanTeam>(clanEntity);
        }

        return clan;
    }
    
    public static ApiPlayer Convert(ServerWorld.Player player)
    {
        var clan = ResolveClan(player);

        return new ApiPlayer(
            UserIndex: player.User.Index,
            Name: player.Character.Name.ToString(),
            SteamID: player.User.PlatformId.ToString(),
            GearLevel: (int) ServerWorld.EntityManager.GetComponentData<Equipment>(player.CharacterEntity).GetFullLevel(),
            ClanName: clan?.Name.ToString(),
            ClanId: clan?.ClanGuid.ToString()
            );
    }

    private static ApiPlayerStats ResolveStats(Equipment equipment)
    {
        return new ApiPlayerStats(ArmorLevel: equipment.ArmorLevel.Value, WeaponLevel: equipment.WeaponLevel.Value,
            SpellLevel: equipment.SpellLevel.Value);
    }

    private static List<Gear> resolveGear(Equipment equipment)
    {
        return new List<Gear>();
    }

    public static ApiPlayerDetails ConvertDetails(ServerWorld.Player player)
    {
        var clan = ResolveClan(player);
        
        var equipment = ServerWorld.EntityManager.GetComponentData<Equipment>(player.CharacterEntity);
        
        return new ApiPlayerDetails(
            UserIndex: player.User.Index,
            Name: player.Character.Name.ToString(),
            SteamID: player.User.PlatformId.ToString(),
            GearLevel: (int) equipment.GetFullLevel(),
            ClanName: clan?.Name.ToString(),
            ClanId: clan?.ClanGuid.ToString(),
            gear: resolveGear(equipment),
            stats: ResolveStats(equipment)
            );
    }
}