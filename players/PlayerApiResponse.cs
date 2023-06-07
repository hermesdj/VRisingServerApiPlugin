#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.players;

public readonly record struct PlayerApiResponse(
    List<ApiPlayer> players
    );