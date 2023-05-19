using Hydriuk.UnturnedModules.Adapters;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Assets;
using Rocket.Unturned.Player;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hydriuk.RocketModModules.Adapters
{
    public class PermissionAdapter : IPermissionAdapter
    {
        private readonly RocketPermissions _rocketPermissions;

        public PermissionAdapter()
        {
            Asset<RocketPermissions> permissionsAsset = new XMLFileAsset<RocketPermissions>(Rocket.Core.Environment.PermissionFile);
            _rocketPermissions = permissionsAsset.Instance;

            _rocketPermissions.Groups = _rocketPermissions.Groups
                .OrderBy(group => group.Priority)
                .ToList();
        }

        public async Task<IEnumerable<string>> GetGroups(CSteamID playerId) => await GetPrioritizedGroups(playerId);

        public Task<IEnumerable<string>> GetPrioritizedGroups(CSteamID playerId)
        {
            return Task.FromResult(_rocketPermissions.Groups
                .Where(group => _rocketPermissions.DefaultGroup != group.Id && group.Members.Contains(playerId.ToString()))
                .Select(group => group.Id));
        }

        public Task<IEnumerable<string>> GetParentGroups(string group)
        {
            RocketPermissionsGroup rocketGroup = R.Permissions.GetGroup(group);

            return Task.FromResult<IEnumerable<string>>(new List<string>() { rocketGroup.ParentGroup });
        }

        public Task<IEnumerable<string>> GetPermissions(CSteamID playerId, string pattern = "")
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerId);

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

        public Task<IEnumerable<string>> GetPrioritizedPermission(CSteamID playerId, IEnumerable<string> permissions)
        {
            List<string> prioritizedPermissions = new List<string>();
            foreach (var group in _rocketPermissions.Groups)
            {
                if (_rocketPermissions.DefaultGroup != group.Id && !group.Members.Contains(playerId.ToString()))
                    continue;

                var rolePermissions = group.Permissions.Select(permission => permission.Name);

                foreach (var permission in permissions)
                {
                    if (rolePermissions.Contains(permission))
                        prioritizedPermissions.Add(permission);
                }

                if (prioritizedPermissions.Count > 0)
                    return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
            }

            return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
        }

        public Task<IEnumerable<string>> GetPrioritizedPermission(CSteamID playerId, string pattern)
        {
            List<string> prioritizedPermissions = new List<string>();
            foreach (var group in _rocketPermissions.Groups)
            {
                if (_rocketPermissions.DefaultGroup != group.Id && !group.Members.Contains(playerId.ToString()))
                    continue;

                var permissions = group.Permissions.Select(permission => permission.Name);

                foreach (var permission in permissions)
                {
                    Match match = Regex.Match(permission, pattern);

                    if (match.Success && match.Length == permission.Length)
                        prioritizedPermissions.Add(permission);
                }

                if (prioritizedPermissions.Count > 0)
                    return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
            }

            return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
        }

        public Task<IEnumerable<string>> GetPrioritizedPermissions(CSteamID playerId)
        {
            List<string> prioritizedPermissions = new List<string>();
            foreach (var group in _rocketPermissions.Groups)
            {
                if (_rocketPermissions.DefaultGroup != group.Id && !group.Members.Contains(playerId.ToString()))
                    continue;

                IEnumerable<string> permissions = group.Permissions.Select(permission => permission.Name); ;

                prioritizedPermissions.AddRange(permissions);
            }

            return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
        }

        public Task<IEnumerable<string>> PrioritizePermissions(CSteamID playerId, IEnumerable<string> permissions)
        {
            int maxCount = permissions.Count();
            List<string> prioritizedPermissions = new List<string>();
            foreach (var group in _rocketPermissions.Groups)
            {
                if (_rocketPermissions.DefaultGroup != group.Id && !group.Members.Contains(playerId.ToString()))
                    continue;

                IEnumerable<string> grantedPermissions = group.Permissions.Select(permission => permission.Name); ;

                grantedPermissions = grantedPermissions.Intersect(permissions).Except(prioritizedPermissions);

                prioritizedPermissions.AddRange(grantedPermissions);

                if (prioritizedPermissions.Count == maxCount)
                    break;
            }

            return Task.FromResult<IEnumerable<string>>(prioritizedPermissions);
        }

        public Task<bool> HasPermission(CSteamID playerId, string permission)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerId);

            bool hasPermission = uPlayer.HasPermission(permission);

            return Task.FromResult(hasPermission);
        }

        public Task<IEnumerable<string>> HasPermissions(CSteamID playerId, IEnumerable<string> permissions)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerId);

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