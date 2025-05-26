using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;

namespace CoreLib.CustomItems.API.Features
{
    public abstract class CustomWeapon : CustomItem
    {
        private ItemType _type;
        
        public override ItemType Type
        {
            get => _type;
            set
            {
                if (!value.IsWeapon(false) && value != ItemType.None)
                    throw new ArgumentOutOfRangeException($"{nameof(Type)}", value, "Invalid weapon type.");

                _type = value;
            }
        }

        public abstract float Damage { get; set; }
        public virtual byte ClipSize { get; set; }
        public virtual bool FriendlyFire { get; set; }

        protected virtual AttachmentName[] Attachments { get; set; } = Array.Empty<AttachmentName>();

        protected virtual AttachmentName[] BlacklistAttachments { get; set; } =
            {
                AttachmentName.StandardMagJHP,
                AttachmentName.StandardMagFMJ,
                AttachmentName.LowcapMagJHP,
                AttachmentName.LowcapMagFMJ,
                AttachmentName.ExtendedMagJHP,
                AttachmentName.ExtendedMagFMJ,
                AttachmentName.DrumMagJHP,
                AttachmentName.DrumMagFMJ,
                AttachmentName.StandardMagAP,
                AttachmentName.DrumMagAP,
                AttachmentName.ExtendedMagAP,
                AttachmentName.CylinderMag5,
                AttachmentName.CylinderMag6,
                AttachmentName.CylinderMag7,
                AttachmentName.LowcapMagAP,
                AttachmentName.ShotgunDoubleShot,
                AttachmentName.ShotgunSingleShot
            };

        public override HashSet<Pickup> Spawn(Player? previousOwner = null)
        {
            HashSet<Pickup> pickups = base.Spawn(previousOwner);
            
            foreach (Pickup pickup in pickups.Where(x => x is FirearmPickup))
            {
                FirearmPickup? firearm = pickup as FirearmPickup;

                if (ClipSize > 0)
                    firearm!.Ammo = ClipSize;
            }

            return pickups;
        }

        public override void Give(Player player, Item item, bool displayMessage = true)
        {
            base.Give(player, item, displayMessage);
            
            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);

                if (ClipSize > 0)
                    firearm.MagazineAmmo = ClipSize;
            }
        }

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnInternalReloading;
            Exiled.Events.Handlers.Player.ReloadedWeapon += OnInternalReloaded;
            Exiled.Events.Handlers.Player.Shooting += OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot += OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting += OnInternalHurting;
            Exiled.Events.Handlers.Item.ChangingAmmo += OnInternalChangingAmmo;
            Exiled.Events.Handlers.Item.ChangingAttachments += OnInternalChangingAttachments;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnInternalReloading;
            Exiled.Events.Handlers.Player.ReloadedWeapon -= OnInternalReloaded;
            Exiled.Events.Handlers.Player.Shooting -= OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot -= OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;
            Exiled.Events.Handlers.Item.ChangingAmmo -= OnInternalChangingAmmo;
            Exiled.Events.Handlers.Item.ChangingAttachments -= OnInternalChangingAttachments;

            base.UnsubscribeEvents();
        }

        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            if (ev.Item is Firearm firearm && firearm.TotalAmmo > ClipSize)
            {
                firearm.MagazineAmmo = ClipSize;
                firearm.BarrelAmmo = 0;
            }

            base.OnChanging(ev);
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (item is Firearm firearm && firearm.TotalAmmo > ClipSize)
            {
                if (!Attachments.IsEmpty())
                {
                    foreach (AttachmentName name in Attachments)
                        firearm.AddAttachment(name);
                }
                
                firearm.MagazineAmmo = ClipSize;
                firearm.BarrelAmmo = 0;
            }

            base.OnAcquired(player, item, displayMessage);
        }

        protected virtual void OnReloading(ReloadingWeaponEventArgs ev) { }

        protected virtual void OnReloaded(ReloadedWeaponEventArgs ev) { }

        protected virtual void OnShooting(ShootingEventArgs ev) { }

        protected virtual void OnShot(ShotEventArgs ev) { }

        protected virtual void OnChangingAmmo(ChangingAmmoEventArgs ev) { }

        protected virtual void OnChangingAttachments(ChangingAttachmentsEventArgs ev) { }

        protected virtual void OnHurting(HurtingEventArgs ev) { }

        private void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ClipSize > 0 && ev.Firearm.MagazineAmmo >= ClipSize)
            {
                ev.IsAllowed = false;
                return;
            }

            OnReloading(ev);
        }

        private void OnInternalReloaded(ReloadedWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ClipSize > 0)
            {
                //int ammoChambered =
                //    ((AutomaticActionModule)ev.Firearm.Base.Modules.FirstOrDefault(x => x is AutomaticActionModule))
                //    ?.SyncAmmoChambered ?? 0;

                int ammodrop = -(ClipSize - ev.Firearm.MagazineAmmo) - 1;

                if (ClipSize <= 1)
                {
                    ev.Firearm.MagazineAmmo = 0;
                }
                else
                {
                    ev.Firearm.MagazineAmmo = ClipSize - 1;
                }

                ev.Firearm.BarrelAmmo = 1;

                ev.Player.AddAmmo(ev.Firearm.AmmoType, (ushort)Mathf.Clamp(ammodrop, ushort.MinValue, ushort.MaxValue));
            }

            OnReloaded(ev);
        }

        private void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnShooting(ev);
        }

        private void OnInternalShot(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnShot(ev);
        }

        private void OnInternalHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker is null)
            {
                return;
            }

            if (ev.Player is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: target null");
                return;
            }

            if (!Check(ev.Attacker.CurrentItem))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: !Check()");
                return;
            }

            if (ev.Attacker == ev.Player)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: attacker == target");
                return;
            }

            if (ev.DamageHandler is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler null");
                return;
            }

            if (!ev.DamageHandler.CustomBase.BaseIs(out FirearmDamageHandler firearmDamageHandler))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler not firearm");
                return;
            }

            if (!Check(firearmDamageHandler.Item))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: type != type");
                return;
            }

            if (!FriendlyFire && (ev.Attacker.Role.Team == ev.Player.Role.Team))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: FF is disabled for this weapon!");
                return;
            }
        
            if (ev.IsAllowed && Damage > 0f)
                ev.Amount = Damage;
        
            OnHurting(ev);
        }

        private void OnInternalChangingAmmo(ChangingAmmoEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.NewAmmo > ClipSize)
                ev.NewAmmo = ClipSize;

            OnChangingAmmo(ev);
        }

        private void OnInternalChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
        
            ev.IsAllowed = !ev.NewAttachmentIdentifiers.Any(x => BlacklistAttachments.Contains(x.Name));

            OnChangingAttachments(ev);
        }
    }
}