using AdminToys;
using CoreLib.DevTools.Pooling;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using MEC;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace CoreLib.DevTools.Other
{
	public static class Prefabs
	{
		private static PrimitiveObjectToy _primitivePrefab = PrefabHelper.GetPrefab<PrimitiveObjectToy>((PrefabType)20);
		private static PrimitiveObjectToy PrimitivePrefab
		{
			get
			{
				PrimitiveObjectToy result;
				if ((result = _primitivePrefab) == null)
				{
					result = _primitivePrefab = PrefabHelper.GetPrefab<PrimitiveObjectToy>((PrefabType)20);
				}
				return result;
			}
			set => _primitivePrefab = value;
		}
		
		public static void GetLineData(Vector3 from, Vector3 to, float thickness, bool cube, out Vector3 position, out Vector3 scale, out Quaternion rotation)
		{
			Vector3 direction = to - from;
			float distance = direction.magnitude;
			scale = new Vector3(thickness, distance * (cube ? 1f : 0.5f), thickness);
			position = from + direction * 0.5f;
			rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);
		}
		
		public static Primitive SpawnPrimitive(PrimitiveType type, Vector3 position, Vector3 scale, Color color, Quaternion? rotation = null, PrimitiveFlags flags = (PrimitiveFlags)2, bool isStatic = true, float syncInterval = 0.05f)
		{
			var primitive = Primitive.Create(type, flags, position, rotation!.Value.eulerAngles, scale, false, ToGlowColor(color));
			primitive.Color = ToGlowColor(color);
			primitive.IsStatic = isStatic;
			primitive.Base.syncInterval = syncInterval;
			primitive.Spawn();
			if (!isStatic)
			{
				Timing.CallDelayed(0.15f, delegate
				{
					primitive.MovementSmoothing = 60;
				});
			}
			return primitive;
		}
		
		public static PooledPrimitive RentLine(Vector3 from, Vector3 to, Color color, float thickness = 0.01f, bool cube = false, PrimitiveFlags flags = (PrimitiveFlags)2, bool movementSmoothing = true)
		{
			GetLineData(from, to, thickness, cube, out var position, out var scale, out var rotation);
			return RentPrimitive(cube ? (PrimitiveType)3 : (PrimitiveType)2, position, scale, color, rotation, flags, movementSmoothing);
		}

		public static PooledPrimitive RentPrimitive(PrimitiveType type, Vector3 position, Vector3 scale, Color color, Quaternion? rotation = null, PrimitiveFlags flags = (PrimitiveFlags)2, bool movementSmoothing = true)
		{
			return SharedPool.Rent(type, position, scale, color, rotation, flags, movementSmoothing);
		}
		
		public static Light SpawnLight(Vector3 position, Color color, float intensity = 1f, float range = 1f, bool shadows = false, bool isStatic = true)
		{
			if (color.a > 1f)
			{
				color /= color.a;
			}
			Light light = Light.Create(Vector3.zero, null, null, false, Color.black);
			light.Position = position;
			light.Color = color;
			light.Range = range;
			light.Intensity = intensity;
			light.ShadowStrength = 1f;
			light.ShadowType = (shadows ? (LightShadows)2 : 0);
			light.MovementSmoothing = (isStatic ? (byte)0 : (byte)60);
			light.IsStatic = isStatic;
			light.Base.syncInterval = (isStatic ? 0.2f : 0.05f);
			light.Spawn();
			return light;
		}
		
		public static void DrawPoint(Vector3 position, Color color = default(Color), float duration = 10f)
		{
			color = ((color == default(Color)) ? Color.red : color);
			color = ToGlowColor(color);
			RentPrimitive(0, position, Vector3.one * 0.1f, color, null, (PrimitiveFlags)2, false).DestroyDelayed(duration);
		}
		
		public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 10f)
		{
			color = ((color == default(Color)) ? Color.red : color);
			color = ToGlowColor(color);
			RentLine(start, end, color, 0.01f, false, (PrimitiveFlags)2, false).DestroyDelayed(duration);
		}
		
		public static void ShowTransformLocalAxis(this Transform transform, float time = 30f, float scale = 1f, Vector3 offset = default(Vector3))
		{
			Rigidbody rb = transform.gameObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				Vector3 velocity = rb.velocity;
				rb.isKinematic = true;
				Timing.CallDelayed(time, delegate
				{
					rb.isKinematic = false;
					rb.velocity = velocity;
				});
			}

			var toys = new AdminToy[]
			{
				RentLine(transform.position + offset, transform.position + offset + transform.right * scale,
					Color.red * 50f).Primitive,
				RentLine(transform.position + offset, transform.position + offset + transform.up * scale, Color.green * 50f)
					.Primitive,
				RentLine(transform.position + offset, transform.position + offset + transform.forward * scale,
					Color.blue * 50f).Primitive,
				RentPrimitive(0, transform.position + offset, Vector3.one * 0.05f, Color.black).Primitive
			};

			Timing.CallDelayed(time, () =>
			{
				foreach (AdminToy toy in toys)
				{
					toy.Destroy();
				}
			});
		}
		
		public static void ShowVector(Vector3 position, Vector3 direction, float time = 30f, float scale = 1f)
		{
			RentLine(position, position + direction * scale, Color.red).DestroyDelayed(time);
			RentPrimitive(0, position, Vector3.one * 0.05f, Color.black).DestroyDelayed(time);
		}
		
		public static Color ToGlowColor(Color color)
		{
			if (color.r > 1f || color.g > 1f || color.b > 1f)
			{
				Color result = color;
				result.a = 0.1f;
				return result;
			}
			return color;
		}
		
		internal static void Init()
		{
			PrefabHelper.GetPrefab<PrimitiveObjectToy>((PrefabType)20).PrimitiveFlags = 0;
			LightSourceToy prefab = PrefabHelper.GetPrefab<LightSourceToy>((PrefabType)21);
			prefab.LightIntensity = 0f;
			prefab.LightRange = 0f;
		}
		
		internal static readonly PrimitivePool SharedPool = new PrimitivePool();
	}
}