using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hydriuk.RocketModModules.Adapters
{
    public class PermissionAdapter : IPermissionAdapter
    {
        public async Task<IEnumerable<string>> GetGroups(CSteamID playerId) => await GetPrioritizedGroups(playerId);

        public Task<IEnumerable<string>> GetPrioritizedGroups(CSteamID playerId)
        {
            IRocketPlayer uPlayer = new RocketPlayer(playerId.ToString());

            return Task.FromResult(R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(group => group.Priority)
                .Select(group => group.Id)
            );
        }

        public Task<IEnumerable<string>> GetParentGroups(string group)
        {
            RocketPermissionsGroup rocketGroup = R.Permissions.GetGroup(group);

            return Task.FromResult<IEnumerable<string>>(new List<string>() { rocketGroup.ParentGroup });
        }

        public Task<IEnumerable<string>> GetPermissions(CSteamID playerId, string pattern = "")
        {
            IRocketPlayer uPlayer = new RocketPlayer(playerId.ToString());

            List<string> permissions = uPlayer
                .GetPermissions()
                .Select(permission => permission.Name)
                .ToList();

            if (pattern != "")
                permissions = permissions.FindAll(permission =>
                {
                    Match match = Regex.Match(permission, pattern);
                    return match.Success && match.Length == permission.Length;
                });

            return Task.FromResult<IEnumerable<string>>(permissions);
        }

        public Task<IEnumerable<string>> GetPrioritizedPermissions(CSteamID playerId, string pattern)
        {
            IRocketPlayer uPlayer = new RocketPlayer(playerId.ToString());

            HashSet<string> prioritizedPermissions = new HashSet<string>();

            foreach (var group in R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(group => group.Priority))
            {
                var permissions = group.Permissions.Select(permission => permission.Name);

                foreach (var permission in permissions)
                {
                    Match match = Regex.Match(permission, pattern);

                    if (match.Success && match.Length == permission.Length)
                        prioritizedPermissions.Add(permission);
                }
            }

            return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
        }

        public Task<IEnumerable<string>> GetPrioritizedPermissions(CSteamID playerId)
        {
            IRocketPlayer uPlayer = new RocketPlayer(playerId.ToString());

            HashSet<string> prioritizedPermissions = new HashSet<string>();

            foreach (var group in R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(group => group.Priority))
            {
                IEnumerable<string> permissions = group.Permissions.Select(permission => permission.Name); ;

                foreach (var permission in permissions)
                {
                    prioritizedPermissions.Add(permission);
                }
            }

            return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
        }

        public Task<bool> HasPermission(CSteamID playerId, string permission)
        {
            IRocketPlayer uPlayer = new RocketPlayer(playerId.ToString());

            bool hasPermission = uPlayer.HasPermission(permission);

            return Task.FromResult(hasPermission);
        }

        public Task<IEnumerable<string>> HasPermissions(CSteamID playerId, IEnumerable<string> permissions)
        {
            IRocketPlayer uPlayer = new RocketPlayer(playerId.ToString());

            IEnumerable<string> grantedPermissions = permissions
                .Where(permission => uPlayer.HasPermission(permission));

            return Task.FromResult(grantedPermissions);
        }

        public Task<bool> IsMemberOf(CSteamID playerId, string group)
        {
            RocketPermissionsGroup rocketGroup = R.Permissions.GetGroup(group);

            bool isMember = rocketGroup.Members.Contains(playerId.ToString());

            return Task.FromResult(isMember);
        }
    }
}