using CoreLib.DevTools.Other.Enums;
using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using UnityEngine;

namespace CoreLib.DevTools.Components
{
    public class GrenadeCollision : MonoBehaviour
    {
        private bool _initialized;

        private GameObject _owner { get; set; } = null!;

        private EffectGrenade _grenade { get; set; } = null!;

        private float Delay { get; set; }

        public void Init(GameObject owner, ThrownProjectile grenade, float delay = 0f)
        {
            _owner = owner;
            _grenade = (EffectGrenade) grenade;
            Delay = delay;
            _initialized = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_initialized)
                return;

            if (_owner == null)
                Log.Error("Owner is null!");
        
            if (_grenade == null)
                Log.Error("Grenade is null!");
        
            if (other.collider == null)
                Log.Error("Collider is null!");

            const Mask ignoreMask = Mask.DoorFrames;

            if (((1 << other.gameObject.layer) & (int)ignoreMask) != 0)
                return;

            Timing.CallDelayed(Delay, () => _grenade!.TargetTime = 0.10000000149011612f);
        }
    }
}