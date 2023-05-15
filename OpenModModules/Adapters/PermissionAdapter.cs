using Hydriuk.UnturnedModules.API.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules.Adapters
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class PermissionAdapter : IPermissionAdapter
    {
        private readonly IUserManager _userManager;
        private readonly IPermissionStore _permissionStore;
        private readonly IPermissionRoleStore _permissionRoleStore;

        public const int test = 1;

        public PermissionAdapter(
            IUserManager userManager,
            IPermissionChecker permissionChecker,
            IPermissionRoleStore permissionRoleStore)
        {
            _userManager = userManager;
            _permissionStore = permissionChecker.PermissionStores.First();
            _permissionRoleStore = permissionRoleStore;
        }

        public async Task<IEnumerable<string>> GetGroups(CSteamID playerId)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<IPermissionRole> roles = await _permissionRoleStore.GetRolesAsync(user);

            return roles.Select(role => role.Id);
        }

        public async Task<IEnumerable<string>> GetPrioritizedGroups(CSteamID playerId)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<IPermissionRole> roles = await _permissionRoleStore.GetRolesAsync(user);

            return roles
                .OrderBy(role => role.Priority)
                .Select(role => role.Id);
        }

        public async Task<IEnumerable<string>> GetParentGroups(string group)
        {
            IPermissionRole? role = await _permissionRoleStore.GetRoleAsync(group);

            if (role == null)
                return new List<string>();

            return role.Parents;
        }

        public async Task<IEnumerable<string>> GetPermissions(CSteamID playerId, string pattern = "")
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<string> grantedPermissions = await _permissionStore.GetGrantedPermissionsAsync(user, true);

            if (pattern == "")
                return grantedPermissions;

            IEnumerable<string> permissions = new List<string>();
            if (pattern != "")
            {
                permissions = grantedPermissions.Where(permission =>
                {
                    Match match = Regex.Match(permission, pattern);
                    return match.Success && match.Length == permission.Length;
                });
            }

            return permissions;
        }

        public async Task<IEnumerable<string>> GetPrioritizedPermission(CSteamID playerId, IEnumerable<string> permissions)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<IPermissionRole> roles = await _permissionRoleStore.GetRolesAsync(user);

            List<string> prioritizedPermissions = new List<string>();
            foreach (var role in roles.OrderBy(role => role.Priority))
            {
                IReadOnlyCollection<string> rolePermissions = await _permissionStore.GetGrantedPermissionsAsync(role);

                foreach (var permission in permissions)
                {
                    if (rolePermissions.Contains(permission))
                        prioritizedPermissions.Add(permission);
                }

                if (prioritizedPermissions.Count > 0)
                    return prioritizedPermissions;
            }

            return prioritizedPermissions;
        }

        public async Task<IEnumerable<string>> GetPrioritizedPermission(CSteamID playerId, string pattern)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<IPermissionRole> roles = await _permissionRoleStore.GetRolesAsync(user);

            List<string> prioritizedPermissions = new List<string>();
            foreach (var role in roles.OrderBy(role => role.Priority))
            {
                IReadOnlyCollection<string> permissions = await _permissionStore.GetGrantedPermissionsAsync(user);

                foreach (var permission in permissions)
                {
                    Match match = Regex.Match(permission, pattern);

                    if (match.Success && match.Length == permission.Length)
                        prioritizedPermissions.Add(permission);
                }

                if (prioritizedPermissions.Count > 0)
                    return prioritizedPermissions;
            }

            return prioritizedPermissions;
        }

        public async Task<IEnumerable<string>> GetPrioritizedPermissions(CSteamID playerId)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<IPermissionRole> roles = await _permissionRoleStore.GetRolesAsync(user);

            List<string> prioritizedPermissions = new List<string>();
            foreach (var role in roles.OrderBy(role => role.Priority))
            {
                IReadOnlyCollection<string> permissions = await _permissionStore.GetGrantedPermissionsAsync(user);

                prioritizedPermissions.AddRange(permissions);
            }

            return prioritizedPermissions;
        }

        public async Task<IEnumerable<string>> PrioritizePermissions(CSteamID playerId, IEnumerable<string> permissions)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<IPermissionRole> roles = await _permissionRoleStore.GetRolesAsync(user);

            int maxCount = permissions.Count();
            List<string> prioritizedPermissions = new List<string>();
            foreach (var role in roles.OrderBy(role => role.Priority))
            {
                IReadOnlyCollection<string> grantedPermissions = await _permissionStore.GetGrantedPermissionsAsync(user);

                var newPermissions = grantedPermissions.Intersect(permissions).Except(prioritizedPermissions);

                prioritizedPermissions.AddRange(newPermissions);

                if (prioritizedPermissions.Count == maxCount)
                    break;
            }

            return prioritizedPermissions;
        }

        public async Task<bool> HasPermission(CSteamID playerId, string permission)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return false;

            IReadOnlyCollection<string> grantedPermissions = await _permissionStore.GetGrantedPermissionsAsync(user, true);

            return grantedPermissions.Contains(permission);
        }

        public async Task<IEnumerable<string>> HasPermissions(CSteamID playerId, IEnumerable<string> permissions)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return new List<string>();

            IReadOnlyCollection<string> grantedPermissions = await _permissionStore.GetGrantedPermissionsAsync(user, true);

            return permissions.Where(permission => grantedPermissions.Contains(permission));
        }

        public async Task<bool> IsMemberOf(CSteamID playerId, string group)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, playerId.ToString(), UserSearchMode.FindById);

            if (user == null)
                return false;

            IReadOnlyCollection<IPermissionRole> roles = await _permissionRoleStore.GetRolesAsync(user);

            return roles.Any(role => role.Id == group);
        }
    }
}