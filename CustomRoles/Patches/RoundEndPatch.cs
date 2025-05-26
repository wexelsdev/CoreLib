using System.Linq;
using CoreLib.CustomRoles.API.Features;
using Exiled.API.Features;
using HarmonyLib;

namespace CoreLib.CustomRoles.Patches
{
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
    internal static class RoundEndPatch
    {
        private static bool Prefix()
        {
            return !CustomRole.Registered
                .Where(role => role.RoundHolder)
                .Any(role => Player.List.Any(player => 
                    player.Role.IsAlive && role.Check(player)));
        }
    }
}