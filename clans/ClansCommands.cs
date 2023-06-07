using System.Linq;

namespace VRisingServerApiPlugin.clans;

public class ClansCommands
{
    public static ListClanResponse GetAllClans()
    {
        var clans = ServerWorld.GetAllClans()
            .Select(ClanUtils.Convert)
            .ToList();

        return new ListClanResponse(
            clans: clans
            );
    }
}