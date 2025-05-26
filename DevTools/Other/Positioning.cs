using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreLib.DevTools.Other
{
	public static class Positioning
	{
		internal static Room GetRoomOrThrow(RoomType roomType)
		{
			Room room = Room.Get(roomType);
			if (room == null)
			{
				string message = $"GetRoomOrThrow({roomType}) - Room is null";
				Log.Error(message);
				throw new NullReferenceException(message);
			}
			return room;
		}
		
		public static Vector3 WorldPosition(Vector3 localPosition, RoomType roomType)
		{
			Room room = GetRoomOrThrow(roomType);
			return WorldPosition(localPosition, room);
		}
		
		public static Vector3 WorldPosition(Vector3 localPosition, Room room)
		{
			if (room == null)
			{
				throw new ArgumentNullException(nameof(room), "WorldPosition - room is null");
			}
			return room.WorldPosition(localPosition);
		}
		
		public static Pickup SpawnPickup(ItemType itemType, Vector3 localPosition, Vector3 localRotation, Room room, bool useGravity = true)
		{
			return SpawnPickup(Item.Create(itemType), localPosition, localRotation, room, useGravity);
		}
		
		public static Pickup SpawnPickup(Item item, Vector3 localPosition, Vector3 localRotation, Room room, bool useGravity = true)
		{
			Vector3 globalPosition = WorldPosition(localPosition, room);
			Quaternion globalRotation = room.Rotation * Quaternion.Euler(localRotation);
			return SpawnPickup(item, globalPosition, globalRotation, useGravity);
		}
		
		public static Pickup SpawnPickup(ItemType itemType, Vector3 globalPosition, Quaternion globalRotation, bool useGravity = true)
		{
			return SpawnPickup(Item.Create(itemType), globalPosition, globalRotation, useGravity);
		}
		
		public static Pickup SpawnPickup(Item item, Vector3 globalPosition, Quaternion globalRotation, bool useGravity = true)
		{
			Pickup pickup = item.CreatePickup(globalPosition, globalRotation);
			pickup.Rigidbody.isKinematic = !useGravity;
			if (pickup is FirearmPickup firearmPickup)
			{
				firearmPickup.Ammo = firearmPickup.MaxAmmo;
			}
			return pickup;
		}
		
		public static Vector3 RandomReachable(Vector3 from, int iterations = 3, float distancePerIteration = 10f)
		{
			if (iterations < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(iterations), "GetReachablePosition - iterations must be greater than 0");
			}
			for (int i = 0; i < iterations; i++)
			{
				Ray ray = new (from, Random.onUnitSphere);
				if (Physics.Raycast(ray, out var hit, distancePerIteration, (LayerMask)134496257))
				{
					from = hit.point + hit.normal * 0.01f;
				}
				else
				{
					from = ray.GetPoint(distancePerIteration);
				}
			}
			return from;
		}
		
		public const bool FreezePickups = false;
	}
}