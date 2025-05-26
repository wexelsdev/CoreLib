using System;
using CoreLib.DevTools.DataTypes;
using CoreLib.DevTools.Other;
using CoreLib.DevTools.Pooling;
using Exiled.API.Features.Toys;
using CoreLib.PrimitiveEffects.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreLib.PrimitiveEffects.SparkEffects
{
	public class SparkEffects
	{
		public static void Create(int amount, Vector3 position, Vector3 direction, Color color, float spread = 15f, float forceMin = 3f, float forceMax = 4.8f)
		{
			Create(amount, _ => position, direction, _ => color, spread, forceMin, forceMax);
		}
    
		public static void Create(int amount, Func<int, Vector3> positionGetter, Vector3 direction, Func<int, Color> colorGetter, float spread = 15f, float forceMin = 3f, float forceMax = 4.8f)
		{
			direction.Normalize();
			Primitive[] primitives = new Primitive[amount];
			for (int i = 0; i < amount; i++)
			{
				Primitive spark = SparkPool.Rent((PrimitiveType)3, positionGetter(i), Vector3.one * Random.Range(0.03f, 0.07f), colorGetter(i), Random.rotation).Primitive;
				Vector3 force = Helpers.GetSpread(spread) * direction * Random.Range(forceMin, forceMax);
				spark.Base.gameObject.GetOrAddComponent<SparkBehaviour>().Init(force);
				primitives[i] = spark;
			} 
			primitives.Decay(2.6f);
		}
	
		private static readonly PrimitivePool SparkPool = new();
		
		private class SparkBehaviour : MonoBehaviour
		{
			private void Start()
			{
				_existenceSeconds = new SecondCounter();
				_rigidbody = gameObject.AddComponent<Rigidbody>();
				_rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				_rigidbody.drag = 0.7f;
				_rigidbody.AddForce(_force, ForceMode.VelocityChange);
				_collider = gameObject.AddComponent<BoxCollider>();
				_collider.material = Material;
			}
		
			private void OnCollisionStay()
			{
				if (_existenceSeconds > 0.2f)
				{
					_rigidbody.velocity *= 0.8f;
				}
			}
		
			public void Init(Vector3 force)
			{
				_force = force;
				if (_rigidbody != null)
				{
					_existenceSeconds.Reset();
					_rigidbody.velocity = Vector3.zero;
					_rigidbody.AddForce(force, ForceMode.VelocityChange);
				}
			}
		
			private static readonly PhysicMaterial Material = new()
			{
				bounciness = 0.2f,
				dynamicFriction = 1f,
				staticFriction = 1f,
				bounceCombine = PhysicMaterialCombine.Maximum,
				frictionCombine = PhysicMaterialCombine.Maximum
			};
		
			private BoxCollider _collider = null!;
		
			private SecondCounter _existenceSeconds = null!;
		
			private Vector3 _force;
		
			private Rigidbody _rigidbody = null!;
		}
	}
}