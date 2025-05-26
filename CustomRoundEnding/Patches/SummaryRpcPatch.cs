using HarmonyLib;

namespace CoreLib.CustomRoundEnding.Patches
{
    //[HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.RpcShowRoundSummary))]
    internal static class SummaryRpcPatch
    {
        internal static bool Prefix(RoundSummary.SumInfo_ClassList listStart,
            RoundSummary.SumInfo_ClassList listFinish,
            RoundSummary.LeadingTeam leadingTeam,
            int eDS,
            int eSc,
            int scpKills,
            int roundCd,
            int seconds)
        {
            if (!CorePlugin.Instance!.Config.CustomRoundEnding) 
                return true;
        
            API.Win(leadingTeam);
            
            return false;
        }
    }
}