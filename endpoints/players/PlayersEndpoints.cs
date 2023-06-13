using System.Linq;
using VRisingServerApiPlugin.attributes;
using VRisingServerApiPlugin.attributes.methods;
using VRisingServerApiPlugin.attributes.parameters;
using VRisingServerApiPlugin.http;

namespace VRisingServerApiPlugin.endpoints.players;

[HttpHandler("/v-rising-server-api/players")]
public class PlayersEndpoints
{
    [HttpGet("/connected")]
    public PlayerListApiResponse GetConnectedPlayers()
    {
        var players = ServerWorld.GetAllPlayerCharacters()
            .Where(player => player.User.IsConnected)
            .Select(PlayerUtils.Convert).ToList<object>();

        return new PlayerListApiResponse(players);
    }

    [HttpGet("/")]
    public PlayerListApiResponse GetAllPlayers()
    {
        var players = ServerWorld.GetAllPlayerCharacters()
            .Select(PlayerUtils.Convert).ToList<object>();

        return new PlayerListApiResponse(players);
    }

    [HttpGet("/(?<id>[0-9]*)")]
    public PlayerApiResponse GetPlayerDetails([UrlParam("id")] int userIndex)
    {
        var player = ServerWorld.GetPlayer(userIndex);

        if (!player.HasValue)
        {
            throw new HttpException(404, $"Player with id {userIndex} not found.");
        }

        return new PlayerApiResponse(PlayerUtils.ConvertDetails(player.Value));
    }
}