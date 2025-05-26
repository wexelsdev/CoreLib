using System;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using HarmonyLib;

namespace CoreLib.Other.Patches
{
    [HarmonyPatch(typeof(RconCommand), nameof(RconCommand.Execute))]
    internal static class SudoCommandPatch
    {
        internal static bool Prefix(RconCommand __instance, ArraySegment<string> arguments,
            ICommandSender sender, ref string response)
        {
            response = "Команда отключена в целях безопасности";
            return false;
        }
    }
}