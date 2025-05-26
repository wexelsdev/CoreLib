using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreLib.DevTools.Enums;
using CoreLib.DevTools.Extensions;
using CoreLib.DevTools.SpawnTools;
using CoreLib.UI;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp0492;
using Exiled.Events.EventArgs.Scp106;
using Exiled.Events.Handlers;
using Exiled.Loader;
using InventorySystem.Configs;
using MEC;
using PlayerRoles;
using UnityEngine;
using CustomAbility = CoreLib.CustomAbilities.API.Features.CustomAbility;
using Player = Exiled.API.Features.Player;

namespace CoreLib.CustomRoles.API.Features
{
    public abstract class CustomRole
    {
        public static List<CustomRole> Registered { get; } = new();

        public static Dictionary<Player, CustomRole> Tracking { get; } = new();
        
        public abstract int Id { get; set; }
        public virtual int MaxHealth { get; set; } = 100;
        public abstract string Name { get; set; }
        public virtual string Description { get; set; } = "";
        public virtual string CustomInfo { get; set; } = "";
        public virtual CustomInfoColor CustomInfoColor { get; set; } = CustomInfoColor.None;
        public abstract RoleTypeId Role { get; set; }
        public virtual RoleTypeId ReplacingRole { get; set; } = RoleTypeId.None;
        public virtual List<CustomAbility>? Abilities { get; set; } = new();
        public virtual List<ItemType> Inventory { get; set; } = new();
        public virtual Dictionary<AmmoType, ushort> Ammo { get; set; } = new();
        public abstract bool KeepPositionOnSpawn { get; set; }
        public abstract bool KeepInventoryOnSpawn { get; set; }
        public virtual bool RemovalKillsPlayer { get; set; } = true;
        public virtual bool ScpIgnore { get; set; } = false;
        public virtual bool RoundIgnore { get; set; } = false;
        public virtual bool RoundHolder { get; set; } = false;
        public abstract float SpawnChance { get; set; }
        public virtual int SpawnLimit { get; set; } = 0;
        public virtual int MinPlayers { get; set; } = 0;
        public virtual bool IgnoreSpawnSystem { get; set; } = false;
        public virtual Exiled.API.Features.Broadcast Broadcast { get; set; } = new();
        public virtual Vector3 Scale { get; set; } = Vector3.one;
        public virtual Dictionary<RoleTypeId, float> CustomRoleFfMultiplier { get; set; } = new();
        public virtual string ConsoleMessage { get; set; } = "";
        public virtual string? SchematicName { get; set; } = "";
        
        public virtual List<RoomSpawnPoint> RoomSpawnpoints { get; set; } = new();
        private bool _onlySpawned;
    
        public void AddRole(Player player, bool overrideLimit = false, bool overrideMinPlayers = false)
        {
            Log.Debug($"{Name}: Adding role to {player.Nickname}.");

            if (SpawnLimit > 0 && Player.List.Count(Check) >= SpawnLimit && !overrideLimit)
            {
                Log.Debug($"{Name}: Role spawn limit reached. Not adding {player.Nickname} to role list.");
                return;
            }
        
            if (MinPlayers > 0 && Player.List.Count(x => !Check(x)) < MinPlayers && !overrideMinPlayers)
            {
                Log.Debug($"{Name}: Not adding {player.Nickname} to role list. Role requires {MinPlayers} players.");
                return;
            }
            
            if (Tracking.ContainsKey(player))
                return;
            
            Log.Debug("Adding player to role list.");
            Tracking.Add(player, this);
        
            _onlySpawned = true;
        
            if (Role != RoleTypeId.None)
            {
                switch (KeepPositionOnSpawn)
                {
                    case true when KeepInventoryOnSpawn:
                        player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                        break;
                    case true:
                        player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.AssignInventory);
                        break;
                    default:
                    {
                        if (KeepInventoryOnSpawn && player.IsAlive)
                            player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
                        else
                            player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.All);
                        break;
                    }
                }
            }
        
            Timing.CallDelayed(0.25f, () =>
            {
                if (!KeepInventoryOnSpawn)
                {
                    Log.Debug($"{Name}: Clearing {player.Nickname}'s inventory.");
                    player.ClearInventory();
                }

                foreach (ItemType item in Inventory)
                {
                    Log.Debug($"{Name}: Adding {item} to inventory.");
                    player.AddItem(item);
                }

                if (Ammo.Count > 0)
                {
                    Log.Debug($"{Name}: Adding Ammo to {player.Nickname} inventory.");
                    foreach (AmmoType type in EnumUtils<AmmoType>.Values)
                    {
                        if (type != AmmoType.None)
                            player.SetAmmo(type, Ammo.ContainsKey(type) ? Ammo[type] == ushort.MaxValue ? InventoryLimits.GetAmmoLimit(type.GetItemType(), player.ReferenceHub) : Ammo[type] : (ushort)0);
                    }
                }

                if (!string.IsNullOrEmpty(SchematicName))
                {
                    Log.Debug($"{Name}: Spawning schematic.");
                    
                    player.GameObject.AddComponent<SchematicController>().Init(SchematicName!);
                }
            });

            Log.Debug($"{Name}: Setting health values.");
            player.Health = MaxHealth;
            player.MaxHealth = MaxHealth;
            player.Scale = Scale;

            Vector3 position = GetSpawnPosition();
            if (position != Vector3.zero)
            {
                player.Position = position;
            }

            if (ScpIgnore)
            {
                Log.Debug($"{Name}: Ignoring scps.");
            
                Scp173Role.TurnedPlayers.Add(player);
                Scp096Role.TurnedPlayers.Add(player);
                Scp079Role.TurnedPlayers.Add(player);
                Scp049Role.TurnedPlayers.Add(player);
            }

            if (RoundIgnore)
            {
                Log.Debug($"{Name}: Ignoring round.");
            
                Round.IgnoredPlayers.Add(player.ReferenceHub);
            }

            Log.Debug($"{Name}: Setting player info");

            player.CustomInfo = CustomInfoColor == CustomInfoColor.None ? $"{player.CustomName}\n{CustomInfo}" : $"<color={CustomInfoColor.GetHexColor()}>{player.CustomName}\n{CustomInfo}</color>";
            player.InfoArea &= ~(PlayerInfoArea.Role | PlayerInfoArea.Nickname);

            if (Abilities is not null)
            {
                foreach (CustomAbility ability in Abilities)
                    CustomAbility.Remove(ability);
                
                foreach (CustomAbility ability in Abilities)
                    CustomAbility.Give(player, ability);
            }

            ShowMessage(player);
            ShowBroadcast(player);
            RoleAdded(player);
        
            player.UniqueRole = Name;
            player.TryAddCustomRoleFriendlyFire(Name, CustomRoleFfMultiplier);

            if (!string.IsNullOrEmpty(ConsoleMessage))
            {
                StringBuilder builder = StringBuilderPool.Pool.Get();

                builder.AppendLine(Name);
                builder.AppendLine(Description);
                builder.AppendLine();
                builder.AppendLine(ConsoleMessage);

                player.SendConsoleMessage(StringBuilderPool.Pool.ToStringReturn(builder), "white");
            }
        
            foreach (var item in Tracking)
            {
                Log.Debug($"{item.Key.Nickname} | {item.Key.CustomInfo}");
            }
        
            Timing.CallDelayed(0.1f, () => _onlySpawned = false);
        }
        public virtual void RemoveRole(Player player)
        {
            if (!Tracking.ContainsKey(player))
                return;
        
            Log.Debug($"{Name}: Removing role from {player.Nickname}");

            Tracking.Remove(player);
        
            player.CustomInfo = string.Empty;
            player.InfoArea |= PlayerInfoArea.Role | PlayerInfoArea.Nickname;
            player.Scale = Vector3.one;
        
            if (Abilities is not null)
            {
                foreach (CustomAbility ability in Abilities)
                {
                    CustomAbility.Remove(ability);
                }
            }
        
            if (ScpIgnore)
            {
                Log.Debug($"{Name}: Removing scps.");
                Scp173Role.TurnedPlayers.Remove(player);
                Scp096Role.TurnedPlayers.Remove(player);
                Scp079Role.TurnedPlayers.Remove(player);
                Scp049Role.TurnedPlayers.Remove(player);
            }
        
            if (RoundIgnore)
            {
                Log.Debug($"{Name}: Removing round.");
            
                Round.IgnoredPlayers.Remove(player.ReferenceHub);
            }
        
            RoleRemoved(player);
        
            player.UniqueRole = string.Empty;
            player.TryRemoveCustomeRoleFriendlyFire(Name);

            if (RemovalKillsPlayer)
                player.Role.Set(RoleTypeId.Spectator);
        }
    
        protected virtual void ShowMessage(Player player) => player.ShowCoreHint(string.Format(CorePlugin.Instance!.Config.GotRoleHint.Content, Name, Description), CorePlugin.Instance.Config.GotRoleHint.Duration);
        protected virtual void ShowBroadcast(Player player) => player.Broadcast(Broadcast);
    
        protected virtual void RoleAdded(Player player) { }
        protected virtual void RoleRemoved(Player player) { }

        protected virtual void OnHurting(HurtingEventArgs ev) { }
        protected virtual void OnDying(DyingEventArgs ev) { }

        protected virtual void SubscribeEvents()
        {
            Log.Debug($"{Name}: Loading events for role {Name}.");
            Exiled.Events.Handlers.Player.ChangingNickname += OnInternalChangingNickname;
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Spawned += OnInternalSpawned;
            Exiled.Events.Handlers.Player.SpawningRagdoll += OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.Destroying += OnDestroying;
            Exiled.Events.Handlers.Player.Hurting += OnInternalHurting;
            Exiled.Events.Handlers.Player.Dying += OnInternalDying;
            Scp106.Attacking += OnInternalScp106Attacking;
            Scp0492.ConsumingCorpse += OnInternalScp0492ConsumingCorpse;
            Scp0492.TriggeringBloodlust += OnInternalScp0492TriggeringBloodlust;
            Exiled.Events.Handlers.Player.MakingNoise += OnInternalMakingNoise;
        }

        protected virtual void UnsubscribeEvents()
        {
            Log.Debug($"{Name}: Unloading events for role {Name}.");
            Exiled.Events.Handlers.Player.ChangingNickname -= OnInternalChangingNickname;
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Spawned -= OnInternalSpawned;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= OnSpawningRagdoll;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;
            Exiled.Events.Handlers.Player.Dying -= OnInternalDying;
            Scp106.Attacking -= OnInternalScp106Attacking;
            Scp0492.ConsumingCorpse -= OnInternalScp0492ConsumingCorpse;
            Scp0492.TriggeringBloodlust -= OnInternalScp0492TriggeringBloodlust;
            Exiled.Events.Handlers.Player.MakingNoise += OnInternalMakingNoise;
        }
    
        private void OnInternalMakingNoise(MakingNoiseEventArgs ev)
        { 
            if (ScpIgnore == false) return;
        
            ev.IsAllowed = !Check(ev.Player);
        }
        private void OnInternalScp0492TriggeringBloodlust(TriggeringBloodlustEventArgs ev)
        { 
            if (ScpIgnore == false) return;
        
            ev.IsAllowed = !Check(ev.Player);
        }
        private void OnInternalScp0492ConsumingCorpse(ConsumingCorpseEventArgs ev)
        { 
            if (ScpIgnore == false) return;
        
            ev.IsAllowed = !Check(ev.Player);
        }
        private void OnInternalScp106Attacking(AttackingEventArgs ev)
        { 
            if (ScpIgnore == false) return;
        
            ev.IsAllowed = !Check(ev.Player);
        }
        private void OnInternalDying(DyingEventArgs ev)
        {
            if (!Check(ev.Player)) return;
        
            if (ev.Attacker != null && ev.Attacker.IsScp)
                ev.IsAllowed = !ScpIgnore;
      
            OnDying(ev);
        }
        private void OnInternalHurting(HurtingEventArgs ev)
        {
            if (!Check(ev.Player)) return;
        
            if (ev.Attacker != null && ev.Attacker.IsScp)
                ev.IsAllowed = !ScpIgnore;
        
            OnHurting(ev);
        }
    
        private void OnInternalChangingNickname(ChangingNicknameEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            ev.Player.CustomInfo = $"{ev.NewName}\n{CustomInfo}";
        }

        private void OnInternalSpawned(SpawnedEventArgs ev)
        {
            if (Round.InProgress 
                && !IgnoreSpawnSystem 
                && SpawnChance > 0 
                && ev.Player.Role.IsAlive 
                && !Tracking.ContainsKey(ev.Player) 
                && !ev.Player.IsScp 
                && Loader.Random.NextDouble() * 100 <= SpawnChance
                && (ReplacingRole == RoleTypeId.None || ev.Player.Role.Type == ReplacingRole))
                AddRole(ev.Player);
        }

        private void OnInternalChangingRole(ChangingRoleEventArgs ev)
        {
            if(ev.Reason == SpawnReason.Destroyed)
                return;

            Log.Debug($"ChangingRole {ev.Player == null}");
        
            if (Check(ev.Player!) && !_onlySpawned)
            {
                Log.Debug("Check");
                if (ev.Player != null) RemoveRole(ev.Player);
            }
        }

        private void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (Check(ev.Player))
                ev.Role = Role;
        }

        private void OnDestroying(DestroyingEventArgs ev) => RemoveRole(ev.Player);

        public bool Check(Player? player)
        {
            if (player == null)
                return false;
            
            if (!Tracking.TryGetValue(player, out var value))
                return false;

            if (value == null)
                return false;
            
            return value == this;
        }

        public void Register()
        {
            if (Registered.Contains(this))
                return;
            
            if (Registered.Any(x => x.Id == Id))
            {
                int maxId = Registered.Max(x => x.Id);
                Log.Error($"CustomRole with id {Id} already exists! New Id: {maxId}");
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
            Registered.Remove(this);
        }
        
        private Vector3 GetSpawnPosition()
        {
            if (!RoomSpawnpoints.Any())
                return Vector3.zero;
            
            foreach (RoomSpawnPoint roomSpawnPoint in RoomSpawnpoints)
            {
                if (Loader.Random.NextDouble() * 100.0 <= roomSpawnPoint.Chance)
                    return roomSpawnPoint.Info.Position;
            }
            
            return Vector3.zero;
        }
    }
}