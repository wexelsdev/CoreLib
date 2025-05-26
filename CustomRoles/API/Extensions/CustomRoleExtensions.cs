using CoreLib.CustomRoles.API.Features;
using Exiled.API.Features;

namespace CoreLib.CustomRoles.API.Extensions
{
    public static class CustomRoleExtensions
    {
        public static bool IsCustomRole(this Player player) =>
            CustomRole.Tracking.ContainsKey(player);

        public static CustomRole? GetCustomRole(this Player player) =>
            CustomRole.Tracking.ContainsKey(player) ? CustomRole.Tracking[player] : null;

        public static bool TryGetCustomRole(this Player player, out CustomRole? role)
        {
            role = player.GetCustomRole();
            return role != null;
        }
    }
}