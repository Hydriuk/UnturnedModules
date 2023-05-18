#if OPENMOD
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IPermissionAdapter
    {
        Task<IEnumerable<string>> GetPermissions(CSteamID playerId, string pattern = "");

        Task<bool> HasPermission(CSteamID playerId, string permission);

        Task<IEnumerable<string>> HasPermissions(CSteamID playerId, IEnumerable<string> permissions);

        Task<IEnumerable<string>> GetGroups(CSteamID playerId);

        Task<IEnumerable<string>> GetPrioritizedGroups(CSteamID playerId);

        Task<IEnumerable<string>> GetParentGroups(string group);

        Task<bool> IsMemberOf(CSteamID playerId, string group);

        Task<IEnumerable<string>> GetPrioritizedPermission(CSteamID playerId, IEnumerable<string> permissions);

        Task<IEnumerable<string>> GetPrioritizedPermission(CSteamID playerId, string pattern);

        Task<IEnumerable<string>> GetPrioritizedPermissions(CSteamID playerId);

        Task<IEnumerable<string>> PrioritizePermissions(CSteamID playerId, IEnumerable<string> permissions);
    }
}