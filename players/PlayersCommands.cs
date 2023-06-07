#nullable enable
using System.Linq;

namespace VRisingServerApiPlugin.players;

public static class PlayersCommands
{
    public static PlayerApiResponse GetConnectedPlayers()
    {
        var players = ServerWorld.GetAllPlayerCharacters()
            .Where(player => player.User.IsConnected)
            .Select(PlayerUtils.Convert).ToList();

        return new PlayerApiResponse(players);
    }
}