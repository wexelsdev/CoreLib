using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.DevTools.Extensions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Lockers;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Keycards;
using MEC;
using UnityEngine;
using KeycardPickup = Exiled.API.Features.Pickups.KeycardPickup;
using RoomSpawnPoint = CoreLib.DevTools.SpawnTools.RoomSpawnPoint;

namespace CoreLib.CustomItems.API.Features
{
    public abstract class CustomKeycard : CustomItem
    {
        private ItemType _type;
        
        public override ItemType Type
        {
            get => _type;
            set
            {
                if (!value.IsKeycard())
                    throw new ArgumentOutOfRangeException(nameof(Type), value, "Invalid keycard type.");

                _type = value;
            }
        }
        
        public abstract string KeycardName { get; set; }
        public abstract string KeycardLabel { get; set; }
        protected virtual Color32 KeycardLabelColor { get; set; } = UnityEngine.Color.white;
        protected virtual Color32 KeycardPermissionsColor { get; set; } = UnityEngine.Color.white;
        protected virtual KeycardPermissions Permissions { get; set; } = KeycardPermissions.None;
        protected virtual Color32 TintColor { get; set; } = UnityEngine.Color.white;
        protected virtual int RankTaskForce { get; set; } = 0;
        
        public override void Give(Player player, Item item, bool displayMessage = true)
        {
            item = CreateKeycard();
            
            Log.Debug($"{Name}.{nameof(Give)}: Item Serial: {item.Serial} Ammo: {(item is Firearm firearm ? firearm.MagazineAmmo : -1)}");

            player.AddItem(item);

            Log.Debug($"{nameof(Give)}: Adding {item.Serial} to tracker.");
                
            if (!Tracking.ContainsKey(item.Serial))
                Tracking.Add(item.Serial, this);
                
            Timing.CallDelayed(0.05f, () => OnAcquired(player, item, displayMessage));
        }

        public override HashSet<Pickup> Spawn(Player? previousOwner = null)
        {
            if (SpawnPoints == null || SpawnPoints.Count < 1)
                return [];

            int toCreate = SpawnPoints.Count;
    
            List<Item> keycards = [];

            HashSet<Pickup> pickups = [];
            
            for (int i = 0; i < toCreate; i++)
            {
                keycards.Add(CreateKeycard());
            }

            int iteration = 0;
            
            foreach (RoomSpawnPoint spawnPoint in SpawnPoints)
            {
                Item keycard = keycards[iteration];

                Pickup pickup = keycard.CreatePickup(spawnPoint.Info.Position, Quaternion.Euler(spawnPoint.Info.Rotation));

                pickup.Rigidbody.isKinematic = spawnPoint.Info.IsKinematic;

                pickup.Scale = Scale;
                
                if (!Tracking.ContainsKey(pickup.Serial))
                    Tracking.Add(pickup.Serial, this);
                
                pickups.Add(pickup);
                
                iteration++;
            }

            return pickups;
        }

        protected virtual void OnInteractingDoor(Player player, Door door) { }
        protected virtual void OnInteractingLocker(Player player, Chamber chamber) { }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.InteractingDoor += OnInternalInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInternalInteractingLocker;
            Exiled.Events.Handlers.Item.KeycardInteracting += OnInternalKeycardInteracting;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.InteractingDoor -= OnInternalInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInternalInteractingLocker;
            Exiled.Events.Handlers.Item.KeycardInteracting -= OnInternalKeycardInteracting;
        }

        private void OnInternalKeycardInteracting(KeycardInteractingEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            OnInteractingDoor(ev.Player, ev.Door);
        }

        private void OnInternalInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnInteractingDoor(ev.Player, ev.Door);
        }

        private void OnInternalInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnInteractingLocker(ev.Player, ev.InteractingChamber); 
        }
        
        public Item CreateKeycard()
        {
            Item item = Item.Create(Type);
            Keycard keycard = item.As<Keycard>();

            if (!keycard.Base.Customizable)
            {
                Log.Error($"Keycard type {Type} is not customizable.");
                return null;
            }

            DetailBase[] details = keycard.Base.Details;

            foreach (var detail in details)
            {
                Type detailType = detail.GetType();

                try
                {
                    if (detail is NametagDetail && !string.IsNullOrEmpty(KeycardName))
                    {
                        detailType.SetStaticFieldValue("_customNametag", KeycardName);
                        
                        Log.Debug($"Set NametagDetail to {KeycardName}");
                    }

                    if (detail is CustomItemNameDetail && !string.IsNullOrEmpty(Name))
                    {
                        details.SetFieldValue("Name", Name);
                        
                        Log.Debug($"Set CustomItemNameDetail to {Name}");
                    }

                    if (detail is CustomLabelDetail)
                    {
                        if (!string.IsNullOrEmpty(KeycardLabel))
                        {
                            detailType.SetStaticFieldValue("_customText", KeycardLabel);
                            
                            Log.Debug($"Set CustomLabelDetail text to {KeycardLabel}");
                        }
                        
                        detailType.SetStaticFieldValue("_customColor", KeycardLabelColor);
                        
                        Log.Debug($"Set CustomLabelDetail color to {KeycardLabelColor}");
                    }

                    if (detail is CustomPermsDetail)
                    {
                        detailType.SetStaticFieldValue("_customLevels", new KeycardLevels((DoorPermissionFlags)Permissions));
                        detailType.SetStaticFieldValue("_customColor", KeycardPermissionsColor);
                        
                        Log.Debug($"Set CustomPermsDetail permissions to {Permissions}");
                    }

                    if (detail is CustomTintDetail)
                    {
                        detailType.SetStaticFieldValue("_customColor", TintColor);
                        
                        Log.Debug($"Set CustomTintDetail color to {TintColor}");
                    }

                    if (detail is CustomRankDetail)
                    {
                        detailType.SetStaticFieldValue("_index", RankTaskForce);
                        
                        Log.Debug($"Set CustomRankDetail index to {RankTaskForce}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Ошибка при обработке детали {detailType.Name}: {ex}");
                }
            }

            return item;
        }
    }
}