#nullable enable
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRisingServerApiPlugin.players;

public class ApiUserDetails
{
    private long TimeLastConnected { get; set; }
    private bool IsBot { get; set; }
    private bool IsAdmin { get; set; }
    private bool IsConnected { get; set; }
}

public class ApiPlayer
{
    public int UserIndex { get; set; }
    public string CharacterName { get; set; }
    public string SteamID { get; set; }
    public string? ClanId { get; set; }
    public int GearLevel { get; set; }

    public ApiPlayer(int userIndex, string characterName, string steamID, string? clanId, int gearLevel)
    {
        UserIndex = userIndex;
        CharacterName = characterName;
        SteamID = steamID;
        ClanId = clanId;
        GearLevel = gearLevel;
    }
}

public class ApiPlayerDetails : ApiPlayer
{
    public object Stats { get; set; }
    public List<object> Gears { get; set; }

    public ApiPlayerDetails(int userIndex, string characterName, string steamID, string? clanId, int gearLevel, object stats, List<object> gears) : base(userIndex, characterName, steamID, clanId, gearLevel)
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
    public bool IsEmpty {get; set;}
    
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