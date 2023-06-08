#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.clans;

public readonly record struct ListClanResponse(
    List<ApiClan> clans
);