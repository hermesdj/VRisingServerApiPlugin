using System.Linq;
using ProjectM;

namespace VRisingServerApiPlugin.clans;

public static class ClanUtils
{
    public static ApiClan Convert(ClanTeam clanTeam)
    {
        var playerIds = ServerWorld.GetAllPlayerCharacters()
            .Where(player => ServerWorld.EntityManager.HasComponent<ClanTeam>(player.User.ClanEntity._Entity))
            .Where(player =>
                ServerWorld.EntityManager.GetComponentData<ClanTeam>(player.User.ClanEntity._Entity).ClanGuid ==
                clanTeam.ClanGuid)
            .Select(player => player.User.Index)
            .ToList();

        return new ApiClan(
            ClanName: clanTeam.Name.ToString(),
            ClanId: clanTeam.ClanGuid.ToString(),
            PlayerIds: playerIds
        );
    }
}