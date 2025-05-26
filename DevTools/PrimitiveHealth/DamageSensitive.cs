using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace CoreLib.DevTools.PrimitiveHealth
{
	public abstract class DamageSensitive : MonoBehaviour, IDestructible
	{
		private void Start()
		{
			if (gameObject.TryGetComponent(out NetworkIdentity identity))
			{
				NetworkId = identity.netId;
			}
			Start_();
		}
	
		public uint NetworkId { get; private set; }
	
		public Vector3 CenterOfMass => transform.position;
	
		public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
		{
			bool result;
			try
			{
				if (handler is not FirearmDamageHandler fdh)
				{
					if (handler is not ExplosionDamageHandler edh)
					{
						Player? attacker = (handler is AttackerDamageHandler adh) ? Player.Get(adh.Attacker) : null;
						result = Hurt(damage, exactHitPos, attacker);
					}
					else
					{
						OnExploded(Player.Get(edh.Attacker), edh, edh._force, damage);
						result = this.Hurt(edh.Damage, exactHitPos, Player.Get(edh.Attacker), DamageType.Explosion);
					}
				}
				else
				{
					OnShot(Player.Get(fdh.Attacker), fdh.WeaponType, handler, exactHitPos, damage);
					result = this.Hurt(fdh.Damage, exactHitPos, Player.Get(fdh.Attacker), DamageType.Firearm);
				}
			}
			catch (Exception e)
			{
				Log.Error($"Error caught in {GetType().Name}.Damage: {e}");
				result = false;
			} 
			return result;
		}
	
		public virtual void Start_()
		{
		}
	
		protected virtual void OnShot(Player attacker, ItemType weapon, DamageHandlerBase handler, Vector3 hitPos, float damage)
		{
		}
	
		protected virtual void OnExploded(Player attacker, ExplosionDamageHandler? edh, Vector3 force, float damage)
		{
		}
	
		protected abstract bool Hurt(float damage, Vector3 hitPosition, Player? attacker = null, DamageType damageType = 0);
	}
}