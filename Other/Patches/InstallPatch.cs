using System;
using CommandSystem;
using HarmonyLib;

namespace CoreLib.Other.Patches
{
    [HarmonyPatch(typeof(Exiled.Events.Commands.Hub.Install), nameof(Exiled.Events.Commands.Hub.Install.Execute))]
    internal static class InstallCommandPatch
    {
        internal static bool Prefix(Exiled.Events.Commands.Hub.Install __instance, ArraySegment<string> arguments,
            ICommandSender sender, ref string response)
        {
            response = "Команда отключена в целях безопасности";
            return false;
        }
    }
}