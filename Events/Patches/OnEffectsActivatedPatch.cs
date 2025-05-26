using CoreLib.Events.EventArgs.Consumable;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Usables;
using Mirror;

namespace CoreLib.Events.Patches
{
    [HarmonyPatch(typeof(Consumable), "ActivateEffects")]
    public class EffectsActivatedPatch
    {
        public static void Postfix(Consumable __instance)
        {
            if (!NetworkServer.active)
                return;
            if (__instance == null)
                return;
        
            Handlers.Consumable.OnEffectsActivated(new EffectsActivatedEventArgs(Player.Get(__instance.Owner), __instance));
        }
    }
}