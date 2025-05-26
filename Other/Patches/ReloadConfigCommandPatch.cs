using System;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Features;
using HarmonyLib;

namespace CoreLib.Other.Patches
{
    [HarmonyPatch(typeof(ReloadConfigCommand), nameof(ReloadConfigCommand.Execute))]
    internal static class ReloadConfigCommandPatch
    {
        internal static bool Prefix(ReloadConfigCommand __instance, ArraySegment<string> arguments,
            ICommandSender sender, ref string response)
        {
            if (Player.Get(sender) == null)
            {
                response = "Данную команду нельзя выполнять в игре";
                return false;
            }
        
            return true;
        }
    }
}