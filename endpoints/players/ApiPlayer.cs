#nullable enable
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRisingServerApiPlugin.endpoints.players;

public class ApiUserDetails
{
    public int UserIndex { get; set; }
    public long TimeLastConnected { get; set; }
    public bool IsBot { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsConnected { get; set; }

    protected ApiUserDetails(int userIndex, long timeLastConnected, bool isBot, bool isAdmin, bool isConnected)
    {
        UserIndex = userIndex;
        TimeLastConnected = timeLastConnected;
        IsBot = isBot;
        IsAdmin = isAdmin;
        IsConnected = isConnected;
    }
}

public class ApiPlayer : ApiUserDetails
{
    public string? CharacterName { get; set; }
    public string SteamID { get; set; }
    public string? ClanId { get; set; }
    public int GearLevel { get; set; }
    public float? LastValidPositionX { get; set; }
    public float? LastValidPositionY { get; set; }

    public bool HasLocalCharacter { get; set; }

    public bool ShouldCreateCharacter { get; set; }

    public ApiPlayer(int userIndex, string? characterName, string steamID, string? clanId, int gearLevel,
        float? lastValidPositionX, float? lastValidPositionY, long timeLastConnected, bool isBot, bool isAdmin,
        bool isConnected) : base(userIndex, timeLastConnected, isBot, isAdmin, isConnected)
    {
        CharacterName = characterName;
        SteamID = steamID;
        ClanId = clanId;
        GearLevel = gearLevel;
        LastValidPositionX = lastValidPositionX;
        LastValidPositionY = lastValidPositionY;
        HasLocalCharacter = characterName != null;
        ShouldCreateCharacter = !HasLocalCharacter;
    }
}

public class ApiPlayerDetails : ApiPlayer
{
    public object Stats { get; set; }
    public List<object> Gears { get; set; }

    public ApiPlayerDetails(
        int userIndex, string? characterName, string steamID, string? clanId, int gearLevel,
        float? lastValidPositionX, float? lastValidPositionY,
        long timeLastConnected, bool isBot, bool isAdmin, bool isConnected,
        object stats,
        List<object> gears
    )
        : base(userIndex, characterName, steamID, clanId, gearLevel, lastValidPositionX, lastValidPositionY,
            timeLastConnected, isBot, isAdmin, isConnected)
    {
        Stats = stats;
        Gears = gears;
    }
}

public class ApiPlayerStats
{
    public float ArmorLevel { get; set; }
    public float WeaponLevel { get; set; }
    public float SpellLevel { get; set; }

    public ApiPlayerStats(float armorLevel, float weaponLevel, float spellLevel)
    {
        ArmorLevel = armorLevel;
        WeaponLevel = weaponLevel;
        SpellLevel = spellLevel;
    }
}

public class EmptyGear
{
    public bool IsEmpty { get; set; }

    public EquipmentSlot Slot { get; set; }

    protected EmptyGear(bool isEmpty, EquipmentSlot slot)
    {
        IsEmpty = isEmpty;
        Slot = slot;
    }

    public EmptyGear(EquipmentSlot slot)
    {
        IsEmpty = true;
        Slot = slot;
    }
};

public class Gear : EmptyGear
{
    public string Name { get; set; }
    public string Description { get; set; }

    public string PrefabName { get; set; }

    public Gear(string name, string prefabName, string description, EquipmentSlot slot) : base(false, slot)
    {
        Name = name;
        PrefabName = prefabName;
        Description = description;
    }
}

[JsonConverter(typeof(EquipmentSlotConverter))]
public enum EquipmentSlot
{
    ARMOR_CHEST,
    CLOACK,
    ARMOR_HEADGEAR,
    ARMOR_FOOTGEAR,
    ARMOR_LEGS,
    ARMOR_GLOVES,
    GRIMOIRE,
    WEAPON
}