using CoreLib.DevTools.Extensions;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using MEC;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace CoreLib.DevTools.SpawnTools
{
    public class Spawn
    {
        public static Pickup PickupObject(ItemType baseItem, Vector3 pos, Quaternion rot, Vector3 scale, Room? room, float weight = 1, bool freeze = false, Player? previousOwner = null)
        {
            Pickup pickup = Pickup.CreateAndSpawn(baseItem, Vector3.zero);

            if (freeze && previousOwner == null)
            {
                pickup.GameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        
            if (previousOwner is not null)
                pickup.PreviousOwner = previousOwner;

            if (room is not null)
            {
                pickup.GameObject.transform.SetParent(room.transform);
        
                pickup.GameObject.transform.localPosition = pos;
            }
            else
            {
                pickup.GameObject.transform.position = pos;
            }
        
            pickup.GameObject.transform.localRotation = rot;

            pickup.Scale = scale;
            pickup.Weight = weight;

            pickup.GameObject.SetActive(true);

            return pickup;
        }
    
        // не я ебал это писать
        // может в будушем сделаю но не щас
        /*public static void ProjectileObject(Player player, ProjectileType type, float fuseTime, Vector3 position)
        {
            try
            {
                switch (type)
                {
                    case ProjectileType.Flashbang:
                        FlashGrenade flash = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                        flash.FuseTime = fuseTime;
                        flash.SpawnActive(player.Position);
                        break;
                    case ProjectileType.FragGrenade:
                        ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(type.GetItemType(), player);
                        grenade.FuseTime = fuseTime;
                        ExplosionGrenade FragGrenade = (ExplosionGrenade)UnityEngine.Object.Instantiate(grenade.Base.Projectile, position, Quaternion.identity);
                        FragGrenade._maxRadius = grenade.MaxRadius;
                        FragGrenade._scpDamageMultiplier = grenade.ScpDamageMultiplier;
                        FragGrenade._burnedDuration = grenade.BurnDuration;
                        FragGrenade._deafenedDuration = grenade.DeafenDuration;
                        FragGrenade._concussedDuration = grenade.ConcussDuration;
                        FragGrenade._fuseTime = grenade.FuseTime;
                        FragGrenade.PreviousOwner = player.Footprint;
                        FragGrenade.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        FragGrenade.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        NetworkServer.Spawn(FragGrenade.gameObject);
                        FragGrenade.ServerActivate();
                        break;
                    case ProjectileType.Scp018:
                        Scp018 scp018 = (Scp018)Item.Create(type.GetItemType(), player);
                        scp018.FuseTime = fuseTime;
                        scp018.SpawnActive(player.Position);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }*/
        
        //Эта хуйня после эксайлда нового не робит :(, мне лень переделывать
        /*public static GameObject Object(ObjectType obj, Vector3 position, Quaternion rotation, Vector3 scale, RoomType baseRoom)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(obj.GetObjectByMode(), position, Quaternion.identity, baseRoom.FindRoom().transform);
            gameObject.transform.rotation = rotation;
            gameObject.transform.localScale = scale;

            switch (obj)
            {
                case ObjectType.LczDoor:
                case ObjectType.HczDoor:
                case ObjectType.EzDoor:
                {
                    gameObject.AddComponent<DoorObject>().Init(new DoorSerializable());
                    break;
                }

                case ObjectType.WorkStation:
                {
                    gameObject.AddComponent<WorkstationObject>().Init(new WorkstationSerializable());
                    break;
                }

                case ObjectType.ItemSpawnPoint:
                {
                    gameObject.transform.position += Vector3.up * 0.1f;
                    gameObject.AddComponent<ItemSpawnPointObject>().Init(new ItemSpawnPointSerializable());
                    break;
                }

                case ObjectType.PlayerSpawnPoint:
                {
                    gameObject.transform.position += Vector3.up * 0.25f;
                    gameObject.AddComponent<PlayerSpawnPointObject>().Init(new PlayerSpawnPointSerializable());
                    break;
                }

                case ObjectType.RagdollSpawnPoint:
                {
                    gameObject.transform.position += Vector3.up * 1.5f;
                    gameObject.AddComponent<RagdollSpawnPointObject>().Init(new RagdollSpawnPointSerializable());
                    break;
                }

                case ObjectType.DummySpawnPoint:
                {
                    break;
                }

                case ObjectType.SportShootingTarget:
                case ObjectType.DboyShootingTarget:
                case ObjectType.BinaryShootingTarget:
                {
                    gameObject.AddComponent<ShootingTargetObject>().Init(new ShootingTargetSerializable());
                    break;
                }

                case ObjectType.Primitive:
                {
                    gameObject.transform.position += Vector3.up * 0.5f;
                    gameObject.AddComponent<PrimitiveObject>().Init(new PrimitiveSerializable());
                    break;
                }

                case ObjectType.LightSource:
                {
                    gameObject.transform.position += Vector3.up * 0.5f;
                    gameObject.AddComponent<LightSourceObject>().Init(new LightSourceSerializable());
                    break;
                }

                case ObjectType.RoomLight:
                {
                    gameObject.transform.position += Vector3.up * 0.25f;
                    gameObject.AddComponent<RoomLightObject>().Init(new RoomLightSerializable());
                    break;
                }

                case ObjectType.Teleporter:
                {
                    gameObject.transform.position += Vector3.up;
                    gameObject.AddComponent<TeleportObject>().Init(new SerializableTeleport(), true);
                    break;
                }

                case ObjectType.PedestalLocker:
                case ObjectType.LargeGunLocker:
                case ObjectType.RifleRackLocker:
                case ObjectType.MiscLocker:
                case ObjectType.MedkitLocker:
                case ObjectType.AdrenalineLocker:
                {
                    gameObject.AddComponent<LockerObject>().Init(new LockerSerializable(), true);
                    break;
                }
            }

            if (gameObject.TryGetComponent(out MapEditorObject mapObject))
            {
                MapEditorReborn.API.API.SpawnedObjects.Add(mapObject);
                Timing.CallDelayed(0.1f, () => mapObject.UpdateIndicator());
            }
            return gameObject;
        }*/
    }
}