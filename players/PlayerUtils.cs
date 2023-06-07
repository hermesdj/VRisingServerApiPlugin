#nullable enable
using ProjectM;

namespace VRisingServerApiPlugin.players;

public static class PlayerUtils
{
    public static ApiPlayer Convert(ServerWorld.Player player)
    {
        var clanEntity = player.User.ClanEntity._Entity;
        ClanTeam? clan = null;

        if (ServerWorld.EntityManager.HasComponent<ClanTeam>(clanEntity))
        {
            clan = ServerWorld.EntityManager.GetComponentData<ClanTeam>(clanEntity);
        }

        return new ApiPlayer(
            UserIndex: player.User.Index,
            Name: player.Character.Name.ToString(),
            SteamID: player.User.PlatformId.ToString(),
            GearLevel: (int) ServerWorld.EntityManager.GetComponentData<Equipment>(player.CharacterEntity).GetFullLevel(),
            ClanName: clan?.Name.ToString(),
            ClanId: clan?.ClanGuid.ToString()
            );
    }
}