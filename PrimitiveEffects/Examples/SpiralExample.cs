using System.Collections.Generic;
using CoreLib.DevTools.Extensions;
using CoreLib.DevTools.Other;
using CoreLib.PrimitiveEffects.Other;
using Decals;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using CoreLib.PrimitiveEffects.Extensions;
using UnityEngine;

namespace CoreLib.PrimitiveEffects.Examples
{
	public class SpiralExample : PrimitiveEffect
	{
		public override bool IsPenetrating { get; } = true;
		public override Penetrate? Penetrate { get; } = new() { MaxPenetrationDistance = 7f };
		public override float MaxDistance { get; set; } = 70f;
		public override SparksMaker? Sparks { get; } = new() {Color = new Color(1, 1, 4) * 50, Min = 4, Max = 6};

		protected override void Effect(Vector3 from, Vector3 to, IntervalRecord? penetrationRecord, bool debugLine = false)
		{
			if (penetrationRecord != null) SpiralCoroutine(from, to, penetrationRecord).Run();
			base.Effect(from, to, penetrationRecord, debugLine);
		}

		public override void Hit(Ray ray, RaycastHit hit)
		{
			Helpers.SpawnDecal(ray, hit, DecalPoolType.Bullet);
			MathExtensions.GetTrianglePoints(hit.point, hit.normal, 0.3f, out var p0, out var p, out var p2);
			Primitive[] primitives = new Primitive[]
			{
				Prefabs.RentLine(p0, p, new Color(1f, 1f, 1.4f) * 50, 0.01f, true).Primitive,
				Prefabs.RentLine(p, p2, new Color(1f, 1f, 1.4f) * 50, 0.01f, true).Primitive,
				Prefabs.RentLine(p2, p0, new Color(1f, 1f, 1.4f) * 50, 0.01f, true).Primitive,
				Prefabs.RentPrimitive(PrimitiveType.Cylinder, hit.point, new Vector3(0.1f, 0.02f, 0.1f), new Color(1f, 1f, 1.4f) * 50, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90f, 0f, 0f)).Primitive
			};
			Primitive[] array = primitives;
			foreach (var t in array)
			{
				t.Base.transform.SetParentSync(hit.transform);
			}
			primitives.Decay(hit.point, 2f);
			base.Hit(ray, hit);
		}

		private static IEnumerator<float> SpiralCoroutine(Vector3 from, Vector3 to, IntervalRecord penetrationRecord)
		{
			if (float.IsNaN(from.x)) yield break;
			yield return float.NegativeInfinity;
			Prefabs.RentLine(from, to, new Color(1f, 1f, 1.4f) * 50, 0.03f).Primitive.DecayLine(0.8f);
			int frameCounter = 0;
			Vector3 position = from;
			Vector3 direction = (to - from).normalized;
			MathExtensions.GetPlaneUp(direction, out var up);
			up *= 0.06f;
			Vector3 p0 = position + up;
			Quaternion rotation = Quaternion.AngleAxis(40f, direction);
			LinkedList<Primitive> primitives = new LinkedList<Primitive>();
			float distance = 0f;
			float maxDistance = Vector3.Distance(from, to);
			while (distance <= maxDistance + 0.01f)
			{
				distance = Mathf.Round(distance * 1000) / 1000;
				position += direction * 0.2f;
				distance += 0.2f;
				up = rotation * up;
				Vector3 p = position + up;
				if (!penetrationRecord.ContainsPoint(distance))
				{
					Primitive item = Prefabs.RentLine(p0, p, new Color(1f, 1f, 1.4f) * 50, 0.01f, true).Primitive;
					primitives.AddLast(item);
				}
				p0 = p;
				int num = frameCounter + 1;
				frameCounter = num;
				if (num % 25 == 0)
				{
					yield return float.NegativeInfinity;
				}
			}
			float deltaPerSecond = (float)Server.Tps;
			float scaleDiff = -0.013f / (deltaPerSecond * 0.8f);
			Vector3 scaleDelta = new Vector3(scaleDiff, 0f, scaleDiff);
			float decayIndex = 0f;
			float decayIndexDelta = 1.5f;
			while (primitives.Count > 0)
			{
				decayIndex += decayIndexDelta;
				decayIndexDelta += 0.2f;
				int endIndex = (int)Mathf.Min(decayIndex, primitives.Count - 1);
				int currentIndex = 0;
				LinkedListNode<Primitive>? node = primitives.First;
				while (node != null && currentIndex <= endIndex)
				{
					LinkedListNode<Primitive>? next = node.Next;
					Primitive primitive = node.Value;
					primitive.Scale += scaleDelta;
					if (primitive.Scale.x <= 0f)
					{
						primitive.Destroy();
						primitives.Remove(node);
						endIndex--;
					}
					else
					{
						currentIndex++;
					}
					node = next;
				}
				if (primitives.Count == 0)
				{
					break;
				}
				yield return float.NegativeInfinity;
			}
		}
	}
}