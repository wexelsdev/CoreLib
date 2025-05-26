using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.CustomRoles.API.Features;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using Respawning;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace CoreLib.CustomSquads.API.Features
{
    public abstract class CustomSquad
    {
        public static HashSet<CustomSquad> Registered { get; } = new();
    
        public abstract int Id { get; set; }
        public abstract string Name { get; }
        public abstract float Chance { get; }
        protected abstract Dictionary<string, int> SquadRoles { get; }
        protected abstract RoomType SpawnRoom { get; }
        protected virtual List<Vector3> SpawnOffsets { get; } = new() { Vector3.zero };
        protected virtual string CassieMessage { get; } = string.Empty;
    
        protected virtual void OnSpawned(Dictionary<Player, string> spawned) {}

        protected virtual void OnRoleSpawned(Player player, RoleTypeId role) { }

        protected virtual void OnRoleSpawned(Player player, CustomRole role) { }
    
        protected virtual void SubscribeEvents() { }
    
        protected virtual void UnsubscribeEvents() { }
    
        public void Spawn(HashSet<Player> toSpawn)
        {
            if (toSpawn.Count == 0)
                return;
        
            WaveManager.State = WaveQueueState.WaveSpawning;
        
            Room spawnRoom = Room.Get(SpawnRoom);
            Vector3 basePosition = spawnRoom.Position + Vector3.up;

            Dictionary<string, int> spawnCounts = new();

            Dictionary<Player, string> spawned = new();
        
            using var enumerator = toSpawn.GetEnumerator();

            foreach (KeyValuePair<string, int> role in SquadRoles)
            {
                string roleName = role.Key;
                int maxCount = role.Value;
            
                spawnCounts[roleName] = 0;

                for (int i = 0; i < maxCount; i++)
                {
                    if (!enumerator.MoveNext())
                        return;

                    Player? player = enumerator.Current;
                    Vector3 offset = SpawnOffsets[i % SpawnOffsets.Count];
                    Vector3 spawnPosition = basePosition + spawnRoom.Transform.TransformDirection(offset);

                    if (Enum.TryParse(roleName, out RoleTypeId roleId))
                    {
                        player!.Role.Set(roleId);
                        player.Teleport(spawnPosition);
                        OnRoleSpawned(player, roleId);
                    }
                    else if (CustomRole.Registered.TryGetFirst(x => x.Name == roleName, out CustomRole? customRole))
                    {
                        customRole.AddRole(player!);
                        player!.Teleport(spawnPosition);
                        OnRoleSpawned(player, customRole);
                    }
                    else
                        throw new InvalidOperationException($"CoreLib.CustomSquads.Features.CustomSquad::Spawn(HashSet<Player>) Role with name {roleName} does not exist.");

                    spawnCounts[roleName]++;
                    spawned.Add(player, roleName);
                }
            }
        
            if (!string.IsNullOrEmpty(CassieMessage)) Cassie.Message(CassieMessage, isSubtitles: true);
        
            OnSpawned(spawned);
            WaveManager.State = WaveQueueState.Idle;
        }

        public void Spawn() => Spawn(Player.List.Where(x => x.Role.Type is RoleTypeId.Spectator).ToHashSet());

        public static void Spawn(int id)
        {
            if (!Registered.TryGetFirst(x => x.Id == id, out CustomSquad squad))
                throw new InvalidOperationException($"CoreLib.CustomSquads.Features.CustomSquad::Spawn(int) CustomSquad with ID {id} is not exists");
        
            squad.Spawn();
        }

        public static void Spawn(int id, HashSet<Player> toSpawn)
        {
            if (!Registered.TryGetFirst(x => x.Id == id, out CustomSquad squad))
                throw new InvalidOperationException($"CoreLib.CustomSquads.Features.CustomSquad::Spawn(int, HashSet<Player>) CustomSquad with ID {id} is not exists");
        
            squad.Spawn(toSpawn);
        }

        public static void Spawn(string name)
        {
            if (!Registered.TryGetFirst(x => x.Name == name, out CustomSquad squad))
                throw new InvalidOperationException($"CoreLib.CustomSquads.Features.CustomSquad::Spawn(string) CustomSquad with name {name} is not exists");
        
            squad.Spawn();
        }
    
        public static void Spawn(string name, HashSet<Player> toSpawn)
        {
            if (!Registered.TryGetFirst(x => x.Name == name, out CustomSquad squad))
                throw new InvalidOperationException($"CoreLib.CustomSquads.Features.CustomSquad::Spawn(string, HashSet<Player>) CustomSquad with name {name} is not exists");
        
            squad.Spawn(toSpawn);
        }
    
        public void Register()
        {
            if (Registered.Any(x => x.Id == Id))
            {
                int maxId = Registered.Max(x => x.Id);
                Log.Error($"CustomSquad with id {Id} already exists! New Id: {maxId}");
                Id = maxId + 1;
            }
        
            Registered.Add(this);
            SubscribeEvents();
        }

        public void Unregister()
        {
            if (!Registered.Contains(this))
                throw new InvalidOperationException($"CustomItem {Name} is not registered, it cannot be unregistered");
        
            UnsubscribeEvents();
            Registered.Remove(this);
        }
    }
}