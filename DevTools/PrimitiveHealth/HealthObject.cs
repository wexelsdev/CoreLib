using System;
using CoreLib.DevTools.DataTypes;
using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace CoreLib.DevTools.PrimitiveHealth
{
	public class HealthObject : DamageSensitive
	{
		public float MaxHealth { get; set; }
		
		public float Health { get; set; }
		
		public bool IsDead { get; private set; }

		public event Action<HealthObjectDyingEventArgs> OnDying;
		
		public event Action<HealthObjectHurtingEventArgs> OnHurting;
		
		public HealthObject Init(Action onKilled, float maxHealth = 300f, Player owner = null!)
		{
			Owner = owner;
			MaxHealth = maxHealth;
			Health = maxHealth;
			_onKilled = delegate
			{
				try
				{
					Action onKilled2 = onKilled;
					onKilled2();
				}
				catch
				{
					// ignored
				}
			};
			return this;
		}
		
		protected override bool Hurt(float damage, Vector3 hitPosition, Player? attacker = null, DamageType damageType = 0)
		{
			HealthObjectHurtingEventArgs args = new HealthObjectHurtingEventArgs(this, attacker!, damage, hitPosition, damageType);
			Handlers.OnHealthObjectHurting(args);
			
			try
			{
				Action<HealthObjectHurtingEventArgs> onHurting = OnHurting;
				onHurting(args);
			}
			catch (Exception e)
			{
				Log.Error($"Error caught in {GetType().Name}.Hurt: {e}");
			}
			
			if (!args.IsAllowed)
			{
				return false;
			}
			
			SecondsSinceLastHurt.Reset();
			Health -= args.Damage;
			attacker!.ShowHitMarker();
			
			if (Health <= 0f)
			{
				Kill(attacker, damageType);
			}
			
			return true;
		}
		
		private void Kill(Player? attacker = null, DamageType damageType = 0)
		{
			Health = 0f;
			
			HealthObjectDyingEventArgs args = new HealthObjectDyingEventArgs(this, attacker, damageType);
			Handlers.OnHealthObjectDestroyed(args);
			
			try
			{
				Action<HealthObjectDyingEventArgs> onDying = OnDying;
				onDying(args);
			}
			catch (Exception e)
			{
				Log.Error($"Error caught in {GetType().Name}.Kill: {e}");
			}
			
			if (!args.IsAllowed)
			{
				Health = Mathf.Epsilon;
				return;
			}
			
			IsDead = true;
			
			Action onKilled = _onKilled;
			onKilled();
		}
		
		public void Heal(float amount = 3.4028235E+38f)
		{
			Health += amount;
		}
		
		public readonly SecondCounter SecondsSinceLastHurt = new SecondCounter();
		
		private Action _onKilled;
		
		public Player Owner;
	}
}