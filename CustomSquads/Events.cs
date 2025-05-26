using System.Linq;
using CoreLib.CustomSquads.API.Features;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Server;

namespace CoreLib.CustomSquads
{
    internal static class Events
    {
        internal static void Subscription()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
        }
    
        internal static void UnSubscription()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
        }

        private static void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            CustomSquad? squad = CustomSquad.Registered.GetRandomValue();

            if (squad == null)
                return;

            if (UnityEngine.Random.value > squad.Chance)
                return;
        
            squad.Spawn(ev.Players.ToHashSet());
            ev.IsAllowed = false;
        }
    }
}