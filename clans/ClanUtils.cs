#nullable enable
using System.Collections.Generic;
using System.Linq;
using ProjectM;

namespace VRisingServerApiPlugin.clans;

public static class ClanUtils
{
    public static ApiClan Convert(ClanTeam clanTeam)
    {
        return new ApiClan(
            clanTeam.ClanGuid.ToString(),
            clanTeam.Name.ToString(),
            clanTeam.Motto.ToString(),
            clanTeam.TeamValue
            );
    }

    public static List<int> GetClanPlayers(ClanTeam clanTeam)
    {
        return ServerWorld.GetAllPlayerCharacters()
            .Where(player => ServerWorld.EntityManager.HasComponent<ClanTeam>(player.User.ClanEntity._Entity))
            .Where(player =>
                ServerWorld.EntityManager.GetComponentData<ClanTeam>(player.User.ClanEntity._Entity).ClanGuid ==
                clanTeam.ClanGuid)
            .Select(player => player.User.Index)
            .ToList();
    }
    
    public static ApiClanWithPlayers ConvertWithPlayers(ClanTeam clanTeam)
    {
        return new ApiClanWithPlayers(
            clanTeam.ClanGuid.ToString(),
            clanTeam.Name.ToString(),
            clanTeam.Motto.ToString(),
            clanTeam.TeamValue,
            playerIds: GetClanPlayers(clanTeam)
        );
    }
}