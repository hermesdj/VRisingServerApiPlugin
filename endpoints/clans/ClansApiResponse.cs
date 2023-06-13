#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.endpoints.clans;

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
    
public readonly record struct UpdateClanNameBody(
    string Name
);

public readonly record struct UpdateClanMottoBody(
    string Motto
    );