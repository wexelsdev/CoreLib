using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CoreLib.DevTools.Extensions;
using CoreLib.DevTools.HtmlHelper;
using CoreLib.UI;
using Exiled.API.Features;
using GameCore;
using MEC;
using UnityEngine;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;


namespace CoreLib.CustomRoundEnding
{
    public static class API
    {
        public static bool InProgress { get; internal set; }
    
        private static int RestartTime { get; } = ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 30);

        internal static Dictionary<Player, Hint> Hints { get; } = new();
    
        private static readonly Dictionary<RoundSummary.LeadingTeam, string> Translations = new()
        {
            {RoundSummary.LeadingTeam.Draw , "<color=#FFFFFF>НИЧЬЯ</color>"},
            {RoundSummary.LeadingTeam.ChaosInsurgency , "<color=#FFFFFF>ПОБЕДА</color> <color=#00FF00>ПХ</color>"},
            {RoundSummary.LeadingTeam.FacilityForces , "<color=#FFFFFF>ПОБЕДА</color> <color=#0000FF>Фонда</color>"},
            {RoundSummary.LeadingTeam.Anomalies , "<color=#FFFFFF>ПОБЕДА</color> <color=#FF0000>SCP</color>"},
            {(RoundSummary.LeadingTeam)3, "<color=#FFFFFF>ПОБЕДА</color> <color=#FFD1DC>Фламинго</color>"}
        };

        public static void Win(RoundSummary.LeadingTeam wonTeam)
        {
            if (InProgress)
                return;
        
            RoundSummary.singleton.SetFieldValue("_roundEnded", true);
        
            Timing.RunCoroutine(UpdateTimeCoroutine(Translations[wonTeam]));
            InProgress = true;
        }

        public static void Win(string team)
        {
            if (InProgress)
                return;
        
            RoundSummary.singleton.SetFieldValue("_roundEnded", true);
        
            Timing.RunCoroutine(UpdateTimeCoroutine($"<color=#FFFFFF>ПОБЕДА</color> {team}"));
            InProgress = true;
        }

        private static IEnumerator<float> UpdateTimeCoroutine(string translation)
        {
            for (int i = RestartTime; i >= 0; i--)
            {
                foreach (Player player in Player.List.Where(x => !x.IsHost && !Hints.ContainsKey(x)))
                {
                    InitHints(player, translation);
                }
            
                foreach (Player player in Player.List.Where(x => !x.IsHost && Hints.ContainsKey(x)))
                {
                    Hint hint = Hints[player];

                    hint.Text = Regex.Replace(hint.Text, @"Следующий раунд начнётся через: <color=#FF0000>\d+</color> секунд\.", $"Следующий раунд начнётся через: <color=#FF0000>{i}</color> секунд."
                        .Bold()
                        .Size());
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    
        private static void InitHints(Player player, string translation)
        {
            if (Hints.ContainsKey(player))
                throw new InvalidOperationException($"Hints for player {player.Nickname} is already inited!");
    
            int scientistsEscaped = Round.EscapedScientists;
            int dClassesEscaped = Round.EscapedDClasses;
            bool warheadDetonated = Warhead.IsDetonated;
            int killedByScp = Round.KillsByScp;

            string roundEndString = "Раунд завершён!"
                .Bold()
                .Size();
        
            string translationString = translation
                .Bold()
                .Size(115);
        
            string alphaString = 
                "Боеголовка \"Альфа\" ".Bold() + (warheadDetonated ? "была ".Bold() : "не была ".Bold()) + 
                "детонирована в течение раунда."
                    .Bold()
                    .Size();
        
            string dClassString = 
                $"<color=#FF0000>{dClassesEscaped}</color> Сотрудников Класса D сбежало"
                    .Bold()
                    .Size();
        
            string scientistsString = 
                $"<color=#FF0000>{scientistsEscaped}</color> Научных Сотрудников сбежало"
                    .Bold()
                    .Size();
        
            string roundTime =
                $"Выполнение миссии заняло: <color=#FF0000>{Math.Round(Round.ElapsedTime.TotalMinutes)}</color> минут и <color=#FF0000>{Round.ElapsedTime.Seconds}</color> секунд."
                    .Bold()
                    .Size();

            string restartTime = 
                $"Следующий раунд начнётся через: <color=#FF0000>{RestartTime}</color> секунд."
                    .Bold()
                    .Size();

            string scpKills = 
                $"В общей сложности SCP было убито: <color=#FF0000>{killedByScp}</color> человек."
                    .Bold()
                    .Size();

            Hints[player] = player.ShowHint($"{roundEndString}\n{translationString}\n\n\n\n\n\n{alphaString}\n\n\n{dClassString}\n\n{scientistsString}\n\n\n{roundTime}\n\n{restartTime}\n\n{scpKills}", new Vector2(0, 280), RestartTime);
        }
    }
}