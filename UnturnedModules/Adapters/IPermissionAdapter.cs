using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IPermissionAdapter
    {
        Task<IEnumerable<string>> GetPermissions(CSteamID playerId, string pattern = "");

        Task<bool> HasPermission(CSteamID playerId, string permission);

        Task<IEnumerable<string>> HasPermissions(CSteamID playerId, IEnumerable<string> permissions);

        Task<IEnumerable<string>> GetGroups(CSteamID playerId);

        Task<IEnumerable<string>> GetPrioritizedGroups(CSteamID playerId);

        Task<IEnumerable<string>> GetParentGroups(string group);

        Task<bool> IsMemberOf(CSteamID playerId, string group);

        Task<IEnumerable<string>> GetPrioritizedPermissions(CSteamID playerId, string pattern);

        Task<IEnumerable<string>> GetPrioritizedPermissions(CSteamID playerId);
    }
}