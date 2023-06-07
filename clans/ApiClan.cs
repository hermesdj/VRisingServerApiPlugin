using System.Collections.Generic;

namespace VRisingServerApiPlugin.clans;

public readonly record struct ApiClan(
    string ClanId,
    string ClanName,
    List<int> PlayerIds
    );