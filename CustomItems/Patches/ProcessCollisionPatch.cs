using CoreLib.CustomItems.API.Features;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using UnityEngine;

namespace CoreLib.CustomItems.Patches
{
    [HarmonyPatch(typeof(CollisionDetectionPickup), nameof(CollisionDetectionPickup.ProcessCollision))]
    internal static class ProcessCollisionPatch
    {
        internal static bool Prefix(CollisionDetectionPickup __instance, Collision collision)
        {
            if (CustomItem.Tracking.TryGetValue(__instance.Info.Serial, out CustomItem item) && !item.CanBrakeGlass)
            {
                return false;
            }

            return true;
        }
    }
}