#nullable enable
using System.Collections.Generic;
using ProjectM;
using Unity.Entities;
using VRisingServerApiPlugin.clans;

namespace VRisingServerApiPlugin.players;

public static class PlayerUtils
{
    private static ApiClan? ResolveClan(ServerWorld.Player player)
    {
        var clanEntity = player.User.ClanEntity._Entity;
        ClanTeam? clan = null;

        if (ServerWorld.EntityManager.HasComponent<ClanTeam>(clanEntity))
        {
            clan = ServerWorld.EntityManager.GetComponentData<ClanTeam>(clanEntity);
        }

        return clan.HasValue ? ClanUtils.Convert(clan.Value) : null;
    }

    private static string? ResolveClanId(ServerWorld.Player player)
    {
        var clanEntity = player.User.ClanEntity._Entity;
        ClanTeam? clan = null;

        if (ServerWorld.EntityManager.HasComponent<ClanTeam>(clanEntity))
        {
            clan = ServerWorld.EntityManager.GetComponentData<ClanTeam>(clanEntity);
        }

        return clan.HasValue ? ClanUtils.Convert(clan.Value).Id : null;
    }
    
    public static ApiPlayer Convert(ServerWorld.Player player)
    {
        return new ApiPlayer(
            player.User.Index,
            player.Character.Name.ToString(),
            player.User.PlatformId.ToString(),
            ResolveClanId(player),
            (int) ServerWorld.EntityManager.GetComponentData<Equipment>(player.CharacterEntity).GetFullLevel()
        );
    }

    private static ApiPlayerStats ResolveStats(Equipment equipment)
    {
        return new ApiPlayerStats(equipment.ArmorLevel.Value, equipment.WeaponLevel.Value,
            equipment.SpellLevel.Value);
    }

    private static EmptyGear ResolveItem(PrefabGUID guid, Entity entity, EquipmentSlot slot)
    {
        var data = ServerWorld.GameDataSystem.ManagedDataRegistry.GetOrDefault<ManagedItemData?>(guid);

        if (data == null) return new EmptyGear(slot);
        
        var name = data.Name.GetGuid().ToGuid().ToString();
        var description = data.Description.Key.GetGuid().ToGuid().ToString();
        return new Gear(name, data.PrefabName, description, slot);
    }

    private static List<object> resolveGear(Equipment equipment)
    {
        return new List<object>
        {
            ResolveItem(equipment.ArmorChestSlotId, equipment.ArmorChestSlotEntity._Entity, EquipmentSlot.ARMOR_CHEST),
            ResolveItem(equipment.CloakSlotId, equipment.CloakSlotEntity._Entity, EquipmentSlot.CLOACK),
            ResolveItem(equipment.ArmorHeadgearSlotId, equipment.ArmorHeadgearSlotEntity._Entity, EquipmentSlot.ARMOR_HEADGEAR),
            ResolveItem(equipment.ArmorFootgearSlotId, equipment.ArmorFootgearSlotEntity._Entity, EquipmentSlot.ARMOR_FOOTGEAR),
            ResolveItem(equipment.ArmorLegsSlotId, equipment.ArmorLegsSlotEntity._Entity, EquipmentSlot.ARMOR_LEGS),
            ResolveItem(equipment.ArmorGlovesSlotId, equipment.ArmorGlovesSlotEntity._Entity, EquipmentSlot.ARMOR_GLOVES),
            ResolveItem(equipment.GrimoireSlotId, equipment.GrimoireSlotEntity._Entity, EquipmentSlot.GRIMOIRE),
            ResolveItem(equipment.WeaponSlotId, equipment.WeaponSlotEntity._Entity, EquipmentSlot.WEAPON)
        };
    }

    public static ApiPlayerDetails ConvertDetails(ServerWorld.Player player)
    {
        var equipment = ServerWorld.EntityManager.GetComponentData<Equipment>(player.CharacterEntity);
        
        return new ApiPlayerDetails(
            player.User.Index,
            player.Character.Name.ToString(),
            player.User.PlatformId.ToString(),
            ResolveClanId(player),
            (int) equipment.GetFullLevel(),
            ResolveStats(equipment),
            resolveGear(equipment)
            );
    }
}