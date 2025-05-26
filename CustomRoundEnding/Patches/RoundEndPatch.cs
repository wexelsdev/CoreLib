using System.Collections.Generic;
using System.Reflection.Emit;
using CoreLib.CustomRoundEnding.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using HarmonyLib;
using RoundRestarting;

namespace CoreLib.CustomRoundEnding.Patches
{
    //[HarmonyPatch(typeof(RoundSummary), "InitiateRoundEnd", MethodType.Enumerator)]
    internal static class RoundEndPatch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            
            int index = newInstructions.FindIndex(x =>
                x.opcode == OpCodes.Call && ReferenceEquals(x.operand, AccessTools.Method(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))));

            if (index == -1)
            {
                Log.Error("CoreLib: Could not find RoundRestart::InitiateRoundRestart call in RoundSummary::InitiateRoundEnd.");
                
                foreach (var instr in newInstructions)
                    yield return instr;
                
                ListPool<CodeInstruction>.Pool.Return(newInstructions);
                yield break;
            }
            
            newInstructions.RemoveAt(index);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RoundEndPatch), nameof(RestartRound)))
            });
            
            foreach (var instr in newInstructions)
                yield return instr;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        internal static void RestartRound()
        {
            API.Hints.Clear();
            API.InProgress = false;
            
            switch (CorePlugin.Instance!.Config.RestartType)
            {
                case RestartType.Default:
                    Round.Restart(false);
                    return;
                case RestartType.Silent:
                    Round.Restart(false, true);
                    return;
                case RestartType.Server:
                    Round.Restart(false, restartAction: ServerStatic.NextRoundAction.Restart);
                    return;
                default:
                    Round.Restart(false);
                    return;
            }
        }
    }
}