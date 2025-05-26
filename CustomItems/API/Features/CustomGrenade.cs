using System;
using CoreLib.DevTools.Extensions;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Footprinting;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;

namespace CoreLib.CustomItems.API.Features
{
    public abstract class CustomGrenade : CustomItem
    {
        private ItemType _type;
        
        public override ItemType Type
        {
            get => _type;
            set
            {
                if (!value.IsThrowable() && value != ItemType.None)
                    throw new ArgumentOutOfRangeException(nameof(Type), value, "Invalid grenade type.");
                
                _type = value;
            }
        }
    
        public abstract bool ExplodeOnCollision { get; set; }
        public abstract float FuseTime { get; set; }
    
        public virtual Pickup Throw(Vector3 position, float force, float weight, float fuseTime = 3f, ItemType grenadeType = ItemType.GrenadeHE, Player? player = null)
        {
            player ??= Server.Host;

            player.Role.Is(out FpcRole fpcRole);
            Vector3 velocity = fpcRole.FirstPersonController.FpcModule.Motor.Velocity;

            Throwable throwable = (Throwable)Item.Create(grenadeType, player);

            ThrownProjectile thrownProjectile = UnityEngine.Object.Instantiate(throwable.Base.Projectile, position, throwable.Owner.CameraTransform.rotation);

            PickupSyncInfo newInfo = new()
            {
                ItemId = throwable.Type,
                Locked = !throwable.Base._repickupable,
                Serial = ItemSerialGenerator.GenerateNext(),
                WeightKg = weight,
            };
            if (thrownProjectile is TimeGrenade time)
                time.SetFieldValue("_fuseTime", fuseTime);
            thrownProjectile.NetworkInfo = newInfo;
            thrownProjectile.PreviousOwner = new Footprint(throwable.Owner.ReferenceHub);
            NetworkServer.Spawn(thrownProjectile.gameObject);
            thrownProjectile.InfoReceivedHook(default, newInfo);
            if (thrownProjectile.TryGetComponent(out Rigidbody component))
                throwable.Base.PropelBody(component, throwable.Base.FullThrowSettings.StartTorque, ThrowableNetworkHandler.GetLimitedVelocity(velocity));
            thrownProjectile.ServerActivate();

            return Pickup.Get(thrownProjectile);
        }
    
        public virtual bool Check(Projectile? grenade) => grenade is not null && Tracking.ContainsKey(grenade.Serial);

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ThrowingRequest += OnInternalThrowingRequest;
            Exiled.Events.Handlers.Player.ThrownProjectile += OnInternalThrownProjectile;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnInternalExplodingGrenade;
            Exiled.Events.Handlers.Map.ChangedIntoGrenade += OnInternalChangedIntoGrenade;
        
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ThrowingRequest -= OnInternalThrowingRequest;
            Exiled.Events.Handlers.Player.ThrownProjectile -= OnInternalThrownProjectile;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnInternalExplodingGrenade;
            Exiled.Events.Handlers.Map.ChangedIntoGrenade -= OnInternalChangedIntoGrenade;

            base.UnsubscribeEvents();
        }
        protected virtual void OnThrowingRequest(ThrowingRequestEventArgs ev) { }
        protected virtual void OnThrownProjectile(ThrownProjectileEventArgs ev) { }
        protected virtual void OnExploding(ExplodingGrenadeEventArgs ev) { }
        protected virtual void OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev) { }

        private void OnInternalThrowingRequest(ThrowingRequestEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Log.Debug($"{ev.Player.Nickname} has thrown a {Name}!");

            OnThrowingRequest(ev);
        }

        private void OnInternalThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (!Check(ev.Throwable))
                return;

            OnThrownProjectile(ev);

            if (ev.Projectile is TimeGrenadeProjectile timeGrenade)
                timeGrenade.FuseTime = FuseTime;

            if (ExplodeOnCollision)
                ev.Projectile.GameObject.AddComponent<CollisionHandler>().Init((ev.Player ?? Server.Host).GameObject, ev.Projectile.Base);
        }

        private void OnInternalExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (Check(ev.Projectile))
            {
                Log.Debug($"A {Name} is exploding!!");
                OnExploding(ev);
            }
        }
        
        private void OnInternalChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            if (ev.Projectile is TimeGrenadeProjectile timedGrenade)
                timedGrenade.FuseTime = FuseTime;

            OnChangedIntoGrenade(ev);

            if (ExplodeOnCollision)
                ev.Projectile.GameObject.AddComponent<CollisionHandler>().Init((ev.Pickup.PreviousOwner ?? Server.Host).GameObject, ev.Projectile.Base);
        }
    }
}
