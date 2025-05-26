using System.Collections.Generic;
using Exiled.API.Features;

namespace CoreLib.DevTools.Session.Models
{
    public class Session
    {
        internal Session(Player owner, List<SessionVariable> variables)
        {
            Owner = owner;
            Variables = variables;
        }
    
        public Player Owner { get; }

        public List<SessionVariable> Variables { get; internal set; }

        public static Session CreateEmpty(Player owner) => new Session(owner, new List<SessionVariable>());
    }
}