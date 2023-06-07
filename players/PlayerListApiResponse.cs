#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.players;

public readonly record struct PlayerListApiResponse(
    List<ApiPlayer> players
);
    
public readonly record struct PlayerApiResponse(
    ApiPlayer? player
);