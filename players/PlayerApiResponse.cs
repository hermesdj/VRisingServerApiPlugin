#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.players;

public readonly record struct PlayerListApiResponse(
    List<object> players
);
    
public readonly record struct PlayerApiResponse(
    ApiPlayerDetails? player
);