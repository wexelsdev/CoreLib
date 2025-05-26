using System.Collections.Concurrent;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace CoreLib.DevTools.Pooling
{
	public class PrimitivePool
	{
		public int Count => _pool.Count;

		private static PrimitiveObjectToy _primitivePrefab = PrefabHelper.GetPrefab<PrimitiveObjectToy>((PrefabType)20);
		private static PrimitiveObjectToy PrimitivePrefab
		{
			get
			{
				PrimitiveObjectToy result;
				if ((result = _primitivePrefab) == null)
				{
					result = _primitivePrefab =
						PrefabHelper.GetPrefab<PrimitiveObjectToy>((PrefabType)20);
				}
				return result;
			}
			set => _primitivePrefab = value;
		}

		public PooledPrimitive Rent(PrimitiveType type, Vector3 position, Vector3 scale, Color color, Quaternion? rotation = null, PrimitiveFlags flags = (PrimitiveFlags)2, bool movementSmoothing = true)
		{
			if (!_pool.TryDequeue(out var primitive))
			{
				primitive = new PooledPrimitive(Object.Instantiate(PrimitivePrefab), this);
				primitive.SetProperties(type, position, scale, color, rotation ?? Quaternion.identity, flags);
				NetworkServer.Spawn(primitive.Base.gameObject);
			}
			else
			{
				primitive.SetProperties(type, position, scale, color, rotation ?? Quaternion.identity, flags);
				NetworkServer.Spawn(primitive.Base.gameObject);
			}
			primitive.Base.syncInterval = 0.05f;
			primitive.IsStatic = false;
			if (movementSmoothing)
			{
				Timing.CallDelayed(0.15f, delegate
				{
					primitive.MovementSmoothing = 60;
				});
			}
			return primitive;
		}
	
		internal void Return(PooledPrimitive primitive)
		{
			primitive.MovementSmoothing = 0;
			primitive.Flags = (PrimitiveFlags)2;
			primitive.IsStatic = true;
			primitive.Scale = Vector3.zero;
			primitive.Base.syncInterval = 0.5f;
			NetworkServer.Destroy(primitive.Base.gameObject);
			_pool.Enqueue(primitive);
		}
	
		public void Clear()
		{
			while (_pool.TryDequeue(out var primitive))
			{
				primitive.Kill();
			}
		}
	
		private readonly ConcurrentQueue<PooledPrimitive> _pool = new();
	}
}