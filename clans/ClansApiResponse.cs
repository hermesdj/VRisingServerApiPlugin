#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.clans;

public readonly record struct GetClanResponse(
    object? clan
);

public readonly record struct ListClanResponse(
    List<object> clans
);

public readonly record struct ListClanPlayersResponse(
    List<int>? players,
    string? id
    );