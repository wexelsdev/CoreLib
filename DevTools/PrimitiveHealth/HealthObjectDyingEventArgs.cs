using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;

namespace CoreLib.DevTools.PrimitiveHealth
{
    public class HealthObjectDyingEventArgs : IDeniableEvent
    {
        public HealthObjectDyingEventArgs(HealthObject healthObject, Player? attacker, DamageType damageType)
        {
            Attacker = attacker;
            DamageType = damageType;
            HealthObject = healthObject;
        }

        public Player? Attacker { get; }
    
        public DamageType DamageType { get; }
    
        public HealthObject HealthObject { get; }
    
        public bool IsAllowed { get; set; } = true;
    }
}