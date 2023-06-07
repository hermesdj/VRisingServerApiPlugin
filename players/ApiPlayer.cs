#nullable enable
namespace VRisingServerApiPlugin.players;

public readonly record struct ApiPlayer(
    int UserIndex,
    string Name, 
    string SteamID,
    string? ClanName,
    string? ClanId,
    int GearLevel
);