#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.players;

public readonly record struct ApiPlayer(
    int UserIndex,
    string Name, 
    string SteamID,
    string? ClanName,
    string? ClanId,
    int GearLevel
);

public readonly record struct ApiPlayerDetails(
    int UserIndex,
    string Name,
    string SteamID,
    string? ClanName,
    string? ClanId,
    int GearLevel,
    List<Gear> gear,
    ApiPlayerStats stats
);

public readonly record struct ApiPlayerStats(
    float ArmorLevel,
    float WeaponLevel,
    float SpellLevel
);

public readonly record struct Gear(
    string Name
);