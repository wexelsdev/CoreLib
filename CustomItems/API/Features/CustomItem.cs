using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.UI;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp914;
using MEC;
using PlayerRoles;
using UnityEngine;
using RoomSpawnPoint = CoreLib.DevTools.SpawnTools.RoomSpawnPoint;
using Server = Exiled.Events.Handlers.Server;

namespace CoreLib.CustomItems.API.Features
{
    public abstract class CustomItem
    {
        public static List<CustomItem> Registered { get; } = new();
        
        public static Dictionary<ushort, CustomItem> Tracking { get; } = new();
        
        public abstract int Id { get; protected set; }
        public abstract ItemType Type { get; set; }
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public virtual string Color { get; set; } = "";
        protected virtual float Weight { get; set; } = 15.0f;
        protected virtual Vector3 Scale { get; set; } = Vector3.one;
        public virtual HashSet<RoomSpawnPoint>? SpawnPoints { get; set; }
        protected virtual bool ShouldMessageOnGban { get; } = false;
        protected virtual bool IsUsing { get; } = true;
        public virtual bool CanBrakeGlass { get; } = true;
        
        protected virtual void ShowDroppedMessage(Player player)
        {
            if (CorePlugin.Instance!.Config.DroppedHint.Show)
                player.ShowCoreHint($"<b><color=red>-</color>[<color=#{Color}>{Name}</color>]<color=red>-</color></b>", CorePlugin.Instance.Config.DroppedHint.Duration);
        }
    
        protected virtual void ShowPickedUpMessage(Player player)
        {
            if (CorePlugin.Instance!.Config.PickedUpHint.Show)
                player.ShowCoreHint($"<b><color=green>+</color>[<color=#{Color}>{Name}</color>]<color=green>+</color></b>", CorePlugin.Instance.Config.PickedUpHint.Duration);
        }

        protected virtual void ShowSelectedMessage(Player player)
        {
            if (CorePlugin.Instance!.Config.SelectedHint.Show)
                player.ShowCoreHint(string.Format(CorePlugin.Instance.Config.SelectedHint.Content, $"<b>=[<color=#{Color}>{Name}</color>]=</b>", Description), CorePlugin.Instance.Config.SelectedHint.Duration);
        }

        public virtual HashSet<Pickup> Spawn(Player? previousOwner = null)
        {
            if (SpawnPoints == null || SpawnPoints.Count < 1)
                return [];

            HashSet<Pickup> returnPickups = new();
            
            foreach (RoomSpawnPoint roomSpawnPoint in SpawnPoints)
            {
                Pickup pickup = Pickup.Create(Type);
                pickup.Rigidbody.isKinematic = roomSpawnPoint.Info.IsKinematic;
                pickup.Spawn(roomSpawnPoint.Info.Position, Quaternion.Euler(roomSpawnPoint.Info.Rotation), previousOwner ?? Exiled.API.Features.Server.Host);

                pickup.Scale = Scale;
                
                if (!Tracking.ContainsKey(pickup.Serial))
                    Tracking.Add(pickup.Serial, this);
                
                returnPickups.Add(pickup);
            }

            return returnPickups;
        }

        public virtual void Give(Player player, Item item, bool displayMessage = true)
        {
            try
            {
                Log.Debug($"{Name}.{nameof(Give)}: Item Serial: {item.Serial} Ammo: {(item is Firearm firearm ? firearm.MagazineAmmo : -1)}");

                player.AddItem(item);

                Log.Debug($"{nameof(Give)}: Adding {item.Serial} to tracker.");
                
                if (!Tracking.ContainsKey(item.Serial))
                    Tracking.Add(item.Serial, this);
                
                Timing.CallDelayed(0.05f, () => OnAcquired(player, item, displayMessage));
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(Give)}: {e}");
            }
        }
        public virtual void Give(Player player, Pickup pickup, bool displayMessage = true) => Give(player, player.AddItem(pickup), displayMessage);
        public virtual void Give(Player player, bool displayMessage = true) => Give(player, Item.Create(Type), displayMessage);
    
        protected virtual void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying += OnInternalOwnerDying;
            Exiled.Events.Handlers.Player.DroppingItem += OnInternalDroppingItem;
            Exiled.Events.Handlers.Player.DroppedItem += OnInternalDroppedItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnInternalDroppingAmmo;
            Exiled.Events.Handlers.Player.ChangingItem += OnInternalChanging;
            Exiled.Events.Handlers.Player.Escaping += OnInternalOwnerEscaping;
            Exiled.Events.Handlers.Player.PickingUpItem += OnInternalPickingUp;
            Exiled.Events.Handlers.Player.ItemAdded += OnInternalItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved += OnInternalItemRemoved;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += OnInternalUpgradingPickup;
            Server.WaitingForPlayers += OnWaitingForPlayers; 
            Exiled.Events.Handlers.Player.Handcuffing += OnInternalOwnerHandcuffing;
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalOwnerChangingRole;
            Exiled.Events.Handlers.Player.UsingItem += OnInternalUsingItem;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem += OnInternalUpgradingInventoryItem;
        }

        protected virtual void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying -= OnInternalOwnerDying;
            Exiled.Events.Handlers.Player.DroppingItem -= OnInternalDroppingItem;
            Exiled.Events.Handlers.Player.DroppedItem -= OnInternalDroppedItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnInternalDroppingAmmo;
            Exiled.Events.Handlers.Player.ChangingItem -= OnInternalChanging;
            Exiled.Events.Handlers.Player.Escaping -= OnInternalOwnerEscaping;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnInternalPickingUp;
            Exiled.Events.Handlers.Player.ItemAdded -= OnInternalItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved -= OnInternalItemRemoved;
            Exiled.Events.Handlers.Scp914.UpgradingPickup -= OnInternalUpgradingPickup;
            Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Handcuffing -= OnInternalOwnerHandcuffing;
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalOwnerChangingRole;
            Exiled.Events.Handlers.Player.UsingItem -= OnInternalUsingItem;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem -= OnInternalUpgradingInventoryItem;
        }

        protected virtual void OnOwnerChangingRole(ChangingRoleEventArgs ev) { }
        protected virtual void OnOwnerDying(DyingEventArgs ev) { }
        protected virtual void OnOwnerEscaping(EscapingEventArgs ev) { }
        protected virtual void OnOwnerHandcuffing(HandcuffingEventArgs ev) { }
        protected virtual void OnDroppingItem(DroppingItemEventArgs ev) { }
        protected virtual void OnDroppedItem(DroppedItemEventArgs ev) { }
        protected virtual void OnDroppingAmmo(DroppingAmmoEventArgs ev) { }
        protected virtual void OnPickingUp(PickingUpItemEventArgs ev) { }
        protected virtual void OnChanging(ChangingItemEventArgs ev) => ShowSelectedMessage(ev.Player);
        protected virtual void OnUpgrading(UpgradingPickupEventArgs ev) { }
        protected virtual void OnUpgrading(UpgradingInventoryItemEventArgs ev) { }
        protected virtual void OnUsing(UsingItemEventArgs ev) { }
    
        protected virtual void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (displayMessage)
                ShowPickedUpMessage(player);
        }
    
        private void OnRemoved(ItemRemovedEventArgs ev) => ShowDroppedMessage(ev.Player);
        
        protected virtual void OnWaitingForPlayers()
        {
            Tracking.Clear();
        }

        private void OnInternalOwnerChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason is SpawnReason.Destroyed)
                return;

            foreach (Item? _ in ev.Player.Items.ToList().Where(Check))
            {
                OnOwnerChangingRole(ev);
            }
        }

        private void OnInternalOwnerDying(DyingEventArgs ev)
        {
            foreach (Item? item in ev.Player.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerDying(ev);

                if (!ev.IsAllowed)
                    continue;

                MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
            }

            MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
        }

        private void OnInternalOwnerEscaping(EscapingEventArgs ev)
        {
            foreach (Item? item in ev.Player.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerEscaping(ev);

                if (!ev.IsAllowed)
                    continue;

                MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
            }

            MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync));
        }

        private void OnInternalOwnerHandcuffing(HandcuffingEventArgs ev)
        {
            foreach (Item? item in ev.Target.Items.ToList())
            {
                if (!Check(item))
                    continue;

                OnOwnerHandcuffing(ev);
            }
        }

        private void OnInternalDroppingItem(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
        
            Rigidbody? rigidbody = ev.Item.Base.GetComponent<Rigidbody>();
            
            if (rigidbody is not null && rigidbody.isKinematic)
            {
                rigidbody.isKinematic = false;
            }

            OnDroppingItem(ev);
        }
    
        private void OnInternalDroppedItem(DroppedItemEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            if (ev.Pickup.Rigidbody.isKinematic)
                ev.Pickup.Rigidbody.isKinematic = false;

            ev.Pickup.Scale = Scale;
            
            OnDroppedItem(ev);
        }

        private void OnInternalDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            if (Type != ev.ItemType)
                return;

            OnDroppingAmmo(ev);
        }

        private void OnInternalPickingUp(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) || ev.Player.Items.Count >= 8 || !ev.IsAllowed)
                return;

            OnPickingUp(ev);
        }

        private void OnInternalItemAdded(ItemAddedEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            OnAcquired(ev.Player, ev.Item, true);
        }
    
        private void OnInternalItemRemoved(ItemRemovedEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;
        
            OnRemoved(ev);
        }

        private void OnInternalChanging(ChangingItemEventArgs ev)
        {
            if (!Check(ev.Item))
            { 
                MirrorExtensions.ResyncSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName));
                return;
            }

            if (ShouldMessageOnGban)
            {
                foreach (Player player in Player.Get(RoleTypeId.Spectator)) 
                    Timing.CallDelayed(0.5f, () => player.SendFakeSyncVar(ev.Player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{ev.Player.Nickname} (CustomItem: {Name})"));
            }

            OnChanging(ev);
        }

        private void OnInternalUpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.IsAllowed = false;

            OnUpgrading(ev);
        }

        private void OnInternalUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            ev.IsAllowed = IsUsing;
        
            OnUsing(new UsingItemEventArgs(ev.Player.ReferenceHub, ev.Usable.Base, ev.Cooldown));
        }
    
        private void OnInternalUpgradingPickup(UpgradingPickupEventArgs ev)
        {
            if (!Check(ev.Pickup)) 
                return;

            ev.IsAllowed = false;

            Timing.CallDelayed(3.5f, () =>
            {
                ev.Pickup.Position = ev.OutputPosition;
                OnUpgrading(ev);
            });
        }

        public bool Check(ushort serial)
        {
            if (!Tracking.TryGetValue(serial, out CustomItem item))
                return false;

            return item == this;
        }

        public bool Check(Item? item) => item != null && Check(item.Serial);

        public bool Check(Pickup? pickup) => pickup != null && Check(pickup.Serial);
    
        public void Register()
        {
            if (Registered.Contains(this))
                return;
            
            if (Registered.Any(x => x.Id == Id))
            {
                int maxId = Registered.Max(x => x.Id);
                Log.Error($"CustomItem with id {Id} already exists! New Id: {maxId}");
                Id = maxId + 1;
            }

            Registered.Add(this);
            SubscribeEvents();
        }

        public void Unregister()
        {
            if (!Registered.Contains(this))
                return;
            
            UnsubscribeEvents();

            foreach (KeyValuePair<ushort, CustomItem> trackingItem in Tracking.Where(x => x.Value == this))
            {
                Pickup.Get(trackingItem.Key)?.Destroy();
                Item.Get(trackingItem.Key)?.Destroy();
            }
            
            Registered.Remove(this);
        }
    }
}