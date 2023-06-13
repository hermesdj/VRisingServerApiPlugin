using Il2CppSystem;
using System.Linq;
using ProjectM;
using Unity.Collections;
using VRisingServerApiPlugin.attributes;
using VRisingServerApiPlugin.attributes.methods;
using VRisingServerApiPlugin.attributes.parameters;
using VRisingServerApiPlugin.http;

namespace VRisingServerApiPlugin.endpoints.clans;

[HttpHandler("/v-rising-server-api/clans")]
public class ClansEndpoints
{
    [HttpGet(@"/")]
    public ListClanResponse GetAllClans()
    {
        var clans = ServerWorld.GetAllClans()
            .Select(ClanUtils.ConvertWithPlayers)
            .ToList<object>();

        return new ListClanResponse(
            clans: clans
        );
    }

    [HttpGet(@"/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})")]
    public GetClanResponse GetClanById([UrlParam("id")] Guid clanId)
    {
        var clan = ClanUtils.GetClanById(clanId);
        return new GetClanResponse(clan.HasValue ? ClanUtils.Convert(clan.Value) : null);
    }

    [HttpGet(@"/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})/players")]
    public ListClanPlayersResponse GetClanPlayers([UrlParam("id")] Guid clanId)
    {
        ClanTeam? clan = ServerWorld.GetAllClans()
            .FirstOrDefault(clanTeam => clanTeam.ClanGuid.Equals(clanId));

        return new ListClanPlayersResponse(players: ClanUtils.GetClanPlayers(clan.Value),
            id: clan?.ClanGuid.ToString());
    }

    [HttpPost(
        @"/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})/updateName")]
    public GetClanResponse UpdateClanName([UrlParam("id")] Guid clanId, [RequestBody] UpdateClanNameBody? body)
    {
        if (body.HasValue)
        {
            var clan = ClanUtils.GetClanWithEntityById(clanId);

            if (clan.HasValue)
            {
                if (!ServerWorld.EntityManager.HasComponent<ClanTeam>(clan.Value.ClanEntity))
                {
                    throw new HttpException(404, $"Clan with id {clanId.ToString()} not found");
                }

                var clanTeam = clan.Value.ClanTeam;
                Plugin.Logger?.LogInfo(
                    $"Updating clan name from '{clanTeam.Name.ToString()}' to '{body.Value.Name}'");
                clanTeam.Name = new FixedString64(body.Value.Name);
                ServerWorld.EntityManager.SetComponentData(clan.Value.ClanEntity, clanTeam);
            }
        }

        var result = ClanUtils.GetClanById(clanId);

        return new GetClanResponse(result.HasValue ? ClanUtils.Convert(result.Value) : null);
    }

    [HttpPost(
        @"/(?<id>[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12})/updateMotto")]
    public GetClanResponse UpdateClanMotto([UrlParam("id")] Guid clanId, [RequestBody] UpdateClanMottoBody? body)
    {
        Plugin.Logger?.LogInfo($"Clan GUID is {clanId.ToString()}");
        if (body.HasValue)
        {
            var clan = ClanUtils.GetClanWithEntityById(clanId);

            if (clan.HasValue)
            {
                if (!ServerWorld.EntityManager.HasComponent<ClanTeam>(clan.Value.ClanEntity))
                {
                    throw new HttpException(404, $"Clan with id {clanId.ToString()} not found");
                }

                var clanTeam = clan.Value.ClanTeam;
                Plugin.Logger?.LogInfo(
                    $"Updating clan motto from '{clanTeam.Motto.ToString()}' to '{body.Value.Motto}'");
                clanTeam.Motto = new FixedString64(body.Value.Motto);
                ServerWorld.EntityManager.SetComponentData(clan.Value.ClanEntity, clanTeam);
            }
        }

        var result = ClanUtils.GetClanById(clanId);

        return new GetClanResponse(result.HasValue ? ClanUtils.Convert(result.Value) : null);
    }
}