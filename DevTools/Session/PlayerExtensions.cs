using System.Collections.Generic;
using System.Linq;
using CoreLib.DevTools.Session.Models;
using Exiled.API.Features;

namespace CoreLib.DevTools.Session
{
    public static class PlayerExtensions
    {
        private static Dictionary<Player, Models.Session> Sessions { get; } = new();
    
        public static Models.Session WriteEmptySession(this Player player)
        {
            Models.Session empty = Models.Session.CreateEmpty(player);

            if (Sessions.TryGetValue(player, out Models.Session session))
            {
                foreach (SessionVariable variable in session.Variables)
                {
                    player.SessionVariables.Remove(variable.Key);
                }
            }

            Sessions[player] = empty;
            return empty;
        }

        public static Models.Session WriteSession(this Player player, Models.Session newSession)
        {
            if (Sessions.TryGetValue(player, out Models.Session session))
            {
                foreach (SessionVariable variable in session.Variables)
                {
                    player.SessionVariables.Remove(variable.Key);
                }
            }

            Sessions[player] = newSession;
            return newSession;
        }

        public static void WriteVariables(this Player player, List<SessionVariable> variables)
        {
            if (Sessions.TryGetValue(player, out Models.Session session))
            {
                foreach (SessionVariable variable in session.Variables)
                {
                    player.SessionVariables.Remove(variable.Key);
                }

                session.Variables = variables;
                return;
            }

            Sessions[player] = new Models.Session(player, variables);
        }

        public static SessionVariable WriteVariable(this Player player, string key, object value)
        {
            if (!Sessions.TryGetValue(player, out Models.Session session))
                session = player.WriteEmptySession();

            if (session.Variables.Any(x => x.Key == key))
            {
                SessionVariable variable = session.Variables.First(x => x.Key == key);
            
                variable.Value = value;
            
                return variable;
            }

            SessionVariable newVariable = SessionVariable.Create(key, value);
            session.Variables.Add(newVariable);
            return newVariable;
        }

        public static void RemoveVariable(this Player player, SessionVariable variable)
        {
            if (!Sessions.TryGetValue(player, out Models.Session session))
                return;

            if (session.Variables.All(x => x != variable))
                return;

            foreach (KeyValuePair<string, object> var in player.SessionVariables.Where(x => x.Key == variable.Key).ToList())
            {
                player.SessionVariables.Remove(var.Key);
            }
        
            session.Variables.Remove(variable);
        }

        public static Models.Session? GetSession(this Player player) => !Sessions.TryGetValue(player, out Models.Session session) ? null : session;

        public static bool TryGetSession(this Player player, out Models.Session? session)
        {
            session = player.GetSession();
            return session is not null;
        }
    }
}