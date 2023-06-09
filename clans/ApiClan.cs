#nullable enable
using System.Collections.Generic;

namespace VRisingServerApiPlugin.clans;

public class ApiClan
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    public string Description { get; set; }

    public int Team { get; set; }

    public ApiClan(string clanId, string clanName, string description, int team)
    {
        Id = clanId;
        Name = clanName;
        Description = description;
        Team = team;
    }
}

public class ApiClanWithPlayers : ApiClan {
    public List<int> playerIds { get; set; }

    public ApiClanWithPlayers(string clanId, string clanName, string description, int team, List<int> playerIds) : base(clanId, clanName, description, team)
    {
        this.playerIds = playerIds;
    }
}