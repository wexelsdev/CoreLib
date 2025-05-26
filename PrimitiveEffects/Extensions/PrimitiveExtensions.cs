using System;
using System.Collections.Generic;
using CoreLib.DevTools.PrimitiveHealth;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using ProjectMER.Features.Objects;
using MEC;
using CoreLib.DevTools.Extensions;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace CoreLib.PrimitiveEffects.Extensions
{
	public static class PrimitiveExtensions
	{
		public static AdminToy DestroyDelayed(this AdminToy toy, float delay)
		{
			Timing.CallDelayed(delay, toy.Destroy);
			return toy;
		}
		
		public static AdminToy[] DestroyDelayed(this AdminToy[] toys, float delay)
		{
			Timing.CallDelayed(delay, delegate
			{
				AdminToy[] toys2 = toys;
				foreach (var t in toys2)
				{
					t.Destroy();
				}
			});
			return toys;
		}
		
		public static Primitive DecayLine(this Primitive primitive, float time = 0.6f, float initialDelay = 0f)
		{
			return primitive.Decay(time, initialDelay, true, false);
		}
		
		public static Primitive Decay(this Primitive primitive, float lifetime = 0.6f, float initialDelay = 0f, bool x = true, bool y = true, bool z = true)
		{
			primitive._Decay(lifetime, initialDelay, x, y, z).Run();
			return primitive;
		}
		
		private static IEnumerator<float> _Decay(this Primitive primitive, float time, float initialDelay, bool x, bool y, bool z)
		{
			if (initialDelay > 0f)
			{
				yield return Timing.WaitForSeconds(initialDelay);
			}
			Vector3 scaleDelta = primitive.Scale / -(time / 0.2f);
			if (!x)
			{
				scaleDelta.x = 0f;
			}
			if (!y)
			{
				scaleDelta.y = 0f;
			}
			if (!z)
			{
				scaleDelta.z = 0f;
			}
			while (time > 0f)
			{
				yield return Timing.WaitForSeconds(0.2f);
				time -= 0.2f;
				primitive.Scale += scaleDelta;
			}
			primitive.Destroy();
		}
		
		public static void Decay(this Primitive[] primitives, Vector3 center, float time = 0.6f, bool x = true, bool y = true, bool z = true)
		{
			primitives._Decay(center, time, x, y, z).Run();
		}
		
		private static IEnumerator<float> _Decay(this Primitive[] primitives, Vector3 center, float time, bool x, bool y, bool z)
		{
			if (primitives.Length == 0)
			{
				yield break;
			}
			ValueTuple<Primitive, Vector3, Vector3>[] data = new ValueTuple<Primitive, Vector3, Vector3>[primitives.Length];
			float intervalCount = time / 0.2f;
			for (int i = 0; i < primitives.Length; i++)
			{
				Vector3 positionDelta = (center - primitives[i].Position) / intervalCount;
				Vector3 scaleDelta = primitives[i].Scale / -intervalCount;
				if (!x)
				{
					scaleDelta.x = 0f;
				}
				if (!y)
				{
					scaleDelta.y = 0f;
				}
				if (!z)
				{
					scaleDelta.z = 0f;
				}
				data[i] = new ValueTuple<Primitive, Vector3, Vector3>(primitives[i], scaleDelta, positionDelta);
			}
			while (time > 0f)
			{
				yield return Timing.WaitForSeconds(0.2f);
				time -= 0.2f;
				foreach (ValueTuple<Primitive, Vector3, Vector3> valueTuple in data)
				{
					Primitive primitive = valueTuple.Item1;
					Vector3 scaleDelta2 = valueTuple.Item2;
					Vector3 positionDelta2 = valueTuple.Item3;
					primitive.Scale += scaleDelta2;
					primitive.Position += positionDelta2;
				}
			}
			foreach (var t in primitives)
			{
				t.Destroy();
			}
		}
		
		public static void DecayLines(this Primitive[] primitives, float time = 0.6f)
		{
			primitives.Decay(time, true, false);
		}
		
		public static void Decay(this Primitive[] primitives, float time = 0.6f, bool x = true, bool y = true, bool z = true)
		{
			primitives._Decay(time, x, y, z).RunSafely();
		}
		
		private static IEnumerator<float> _Decay(this Primitive[] primitives, float time, bool x, bool y, bool z)
		{
			if (primitives.Length == 0)
			{
				yield break;
			}
			ValueTuple<Primitive, Vector3>[] data = new ValueTuple<Primitive, Vector3>[primitives.Length];
			float intervalCount = time / 0.2f;
			for (int i = 0; i < primitives.Length; i++)
			{
				Vector3 scaleDelta = primitives[i].Scale / -intervalCount;
				if (!x)
				{
					scaleDelta.x = 0f;
				}
				if (!y)
				{
					scaleDelta.y = 0f;
				}
				if (!z)
				{
					scaleDelta.z = 0f;
				}
				data[i] = new ValueTuple<Primitive, Vector3>(primitives[i], scaleDelta);
			}
			while (time > 0f)
			{
				yield return Timing.WaitForSeconds(0.2f);
				time -= 0.2f;
				foreach (ValueTuple<Primitive, Vector3> valueTuple in data)
				{
					Primitive primitive = valueTuple.Item1;
					Vector3 scaleDelta2 = valueTuple.Item2;
					primitive.Scale += scaleDelta2;
				}
			}
			foreach (var t in primitives)
			{
				t.Destroy();
			}
		}
		
		public static HealthObject MakeDestroyable(this AdminToy toy, Action? onKilled = null, float maxHealth = 300f, Player? owner = null)
		{
			if (toy == null)
			{
				throw new ArgumentNullException(nameof(toy));
			}

			if (toy.AdminToyBase.gameObject.TryGetComponent(out HealthObject healthObject))
			{
				Object.DestroyImmediate(healthObject);
			}

			if (onKilled == null) return healthObject;
			if (owner != null)
				healthObject = toy.AdminToyBase.gameObject.AddComponent<HealthObject>()
					.Init(onKilled, maxHealth, owner);
			return healthObject;
		}
		
		public static void Explode(this MapEditorObject mapEditorObject, ItemType projectileType = ItemType.GrenadeFlash)
		{
			ExplosionUtils.ServerSpawnEffect(mapEditorObject.gameObject.transform.position, projectileType);
			mapEditorObject.Destroy();
		}
		
		public static HealthObject MakeDestroyable(this MapEditorObject toy, Action? onKilled = null, float maxHealth = 300f, Player? owner = null)
		{
			if (toy == null)
			{
				throw new ArgumentNullException(nameof(toy));
			}

			if (toy.gameObject.TryGetComponent(out HealthObject healthObject))
			{
				Object.DestroyImmediate(healthObject);
			}

			if (onKilled == null) return healthObject;
			if (owner != null)
				healthObject = toy.gameObject.AddComponent<HealthObject>().Init(onKilled, maxHealth, owner);
			return healthObject;
		}
		
		public static bool Destroy(this AdminToy toy)
		{
			toy.Destroy();
			return true;
		}
		
		public static HealthObject HealthObject(this MapEditorObject mapEditorObject)
		{
			return mapEditorObject.gameObject.GetComponentInParent<HealthObject>();
		}
	}
}