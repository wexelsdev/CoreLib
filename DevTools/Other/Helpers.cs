using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CommandSystem;
using CoreLib.DevTools.Other.Enums;
using Decals;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Interfaces;
using InventorySystem.Items.Firearms.Modules;
using CoreLib.DevTools.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreLib.DevTools.Other
{
	public static class Helpers
	{
		private static Firearm? _decalWeapon = Item.Create(ItemType.GunCOM18) as Firearm;
		private static Firearm DecalWeapon
		{
			get
			{
				Firearm? result;
				if ((result = _decalWeapon) == null)
				{
					result = _decalWeapon = (Firearm)Item.Create(ItemType.GunCOM18);
				}
				return result;
			}
		}
		public static IEnumerable<T> GetEnumValues<T>() where T : Enum
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
	
		public static EffectGrenadeProjectile SpawnGrenade(ItemType type, Vector3 position, Player? owner = null, float fuseTime = 3f, float scpDamageMultiplier = 3f, Vector3? overrideScale = null)
		{
			return SpawnGrenade(Item.Create(type), position, owner, fuseTime, scpDamageMultiplier, overrideScale);
		}
		
		public static EffectGrenadeProjectile SpawnGrenade(Item item, Vector3 position, Player? owner, float fuseTime = 3f, float scpDamageMultiplier = 3f, Vector3? overrideScale = null)
		{
			ItemType type = item.Type;
			if (type == ItemType.GrenadeHE)
			{
				ExplosiveGrenade explosiveGrenade = (ExplosiveGrenade)item;
				explosiveGrenade.FuseTime = fuseTime;
				explosiveGrenade.ScpDamageMultiplier = scpDamageMultiplier;
				explosiveGrenade.Scale = (overrideScale ?? Vector3.one);
				ExplosionGrenadeProjectile explosionGrenadeProjectile = explosiveGrenade.SpawnActive(position, owner);
				return explosionGrenadeProjectile;
			}
			if (type != ItemType.GrenadeFlash)
			{
				ArgumentException ex = new ArgumentException(
					$"Helpers.SpawnGrenade(): Type must be either {ItemType.GrenadeHE} or {ItemType.GrenadeFlash}, not {item.Type}");
				Log.Error(ex);
				throw ex;
			}
			FlashGrenade flashGrenade = (FlashGrenade)item;
			flashGrenade.FuseTime = fuseTime;
			flashGrenade.Scale = (overrideScale ?? Vector3.one);
			FlashbangProjectile flashbangProjectile = flashGrenade.SpawnActive(position, owner);
			return flashbangProjectile;
		}
		
		public static T? GetClosest<T>(this IEnumerable<T> list, Vector3 toPosition, float maxDistance = 3.4028235E+38f) where T : IPosition
		{
			T? closest = list.MinBy(obj => SqrDistance(toPosition, obj.Position));
			if (closest == null || DistanceMoreThan(toPosition, closest.Position, maxDistance))
			{
				return default;
			}
			return closest;
		}
		
		public static Quaternion GetSpread(float degrees)
		{
			return Quaternion.Euler(Random.insideUnitSphere * degrees);
		}
		
		public static float GetInaccuracy(this Firearm firearm)
		{
			float inaccuracy = 0f;
			ModuleBase[] modules = firearm.Base.Modules;
			foreach (var t in modules)
			{
				if (t is IInaccuracyProviderModule inaccuracyProviderModule)
				{
					inaccuracy += inaccuracyProviderModule.Inaccuracy;
				}
			}
			return inaccuracy;
		}
		
		public static Vector3 GetShotDirection(this Firearm firearm)
		{
			if (firearm.Owner == null)
			{
				return Vector3.zero;
			}
			Vector3 direction = firearm.Owner.CameraTransform.forward;
			return GetSpread(firearm.GetInaccuracy()) * direction;
		}
		
		public static bool Chance(this double probability)
		{
			return Random.value < probability;
		}
		
		public static bool Chance(this float probability)
		{
			return Random.value < probability;
		}
		
		public static bool IsPlayer(this ICommandSender sender, out Player player)
		{
			player = Player.Get(sender);
			return player != null;
		}
		
		public static MethodBase GetCallingMethod(int depth = 1)
		{
			return new StackTrace().GetFrame(1 + depth).GetMethod();
		}
		
		public static Type? GetCallingType(int depth = 1)
		{
			return new StackTrace().GetFrame(1 + depth).GetMethod().DeclaringType;
		}
		
		public static float SqrDistance(Vector3 a, Vector3 b)
		{
			return (a - b).sqrMagnitude;
		}
		
		public static bool DistanceMoreThan(Vector3 a, Vector3 b, float distance)
		{
			return SqrDistance(a, b) > distance * distance;
		}
		
		public static bool DistanceLessThan(Vector3 a, Vector3 b, float distance)
		{
			return SqrDistance(a, b) < distance * distance;
		}
		
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			if (!gameObject.TryGetComponent<T>(out var component))
			{
				return gameObject.AddComponent<T>();
			}
			return component;
		}
		
		public static Vector3 MaxAvailableRaycastPosition(this Player player, float maxDistance, Mask maskMask = Mask.HitregObstacles)
		{
			Ray ray = new(player.CameraTransform.position, player.CameraTransform.forward);
			if (!Physics.Raycast(ray, out var hit, maxDistance, (int)maskMask))
			{
				return ray.GetPoint(maxDistance);
			}
			return hit.point;
		}
		
		public static float MaxAvailableRaycastDistance(this Player player, float maxDistance, Mask maskMask = Mask.HitregObstacles)
		{
			if (!Physics.Raycast(new Ray(player.CameraTransform.position, player.CameraTransform.forward), out var hit, maxDistance, (int)maskMask))
			{
				return maxDistance;
			}
			return hit.distance;
		}
		
		public static void SetParentSync(this Transform transform, Transform parent)
		{
			Vector3 originalPosition = transform.position;
			Quaternion originalRotation = transform.rotation;
			transform.SetParent(parent, false);
			transform.position = originalPosition;
			transform.rotation = originalRotation;
		}
		
		public static void SpawnDecal(Ray ray, RaycastHit hit, DecalPoolType type)
		{
			if (DecalWeapon.Base.TryGetModule(out ImpactEffectsModule module))
			{
				Type typee = module.GetType();

				MethodInfo? info = typee.GetMethod("ServerSendImpactDecal",
					BindingFlags.NonPublic | BindingFlags.Instance);

				info?.Invoke(module, new object[] { hit, ray.origin, type });
			}
		}
	}
}