#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.endpoints.players;

public readonly record struct PlayerListApiResponse(
    List<object> players
);
    
public readonly record struct PlayerApiResponse(
    ApiPlayerDetails? player
);