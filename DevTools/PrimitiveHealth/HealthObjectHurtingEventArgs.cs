using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using UnityEngine;

namespace CoreLib.DevTools.PrimitiveHealth
{
    public class HealthObjectHurtingEventArgs : IDeniableEvent
    {
        public HealthObjectHurtingEventArgs(HealthObject healthObject, Player attacker, float damage, Vector3 hitPosition, DamageType damageType)
        {
            Attacker = attacker;
            Damage = damage;
            HitPosition = hitPosition;
            DamageType = damageType;
            HealthObject = healthObject;
        }

        public Player Attacker { get; }
    
        public float Damage { get; set; }
    
        public Vector3 HitPosition { get; }
    
        public DamageType DamageType { get; }
    
        public HealthObject HealthObject { get; }
    
        public bool IsAllowed { get; set; } = true;
    }
}