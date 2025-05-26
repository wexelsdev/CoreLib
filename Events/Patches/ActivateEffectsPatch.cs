using CoreLib.Events.EventArgs.Consumable;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Usables;
using Mirror;

namespace CoreLib.Events.Patches
{
    [HarmonyPatch(typeof(Consumable), "ActivateEffects")]
    public class ActivateEffectsPatch
    {
        private static bool Prefix(Consumable __instance)
        {
            if (!NetworkServer.active)
                return false;

            if (__instance != null)
            {
                ActivatingEffectsEventArgs ev = new ActivatingEffectsEventArgs(Player.Get(__instance.Owner), __instance);
                Handlers.Consumable.OnActivatingEffects(ev);
                return ev.IsAllowed;
            }

            return false;
        }
    }
}