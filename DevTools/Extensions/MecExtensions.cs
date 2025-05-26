using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using UnityEngine;

namespace CoreLib.DevTools.Extensions
{
	public static class MecExtensions
	{
		public static bool Kill(this CoroutineHandle handle)
		{
			if (handle.IsRunning)
			{
				handle.IsRunning = false;
				return true;
			}
			return false;
		}
		
		public static void Kill(this IEnumerable<CoroutineHandle> handles)
		{
			Timing.KillCoroutines((handles as CoroutineHandle[]) ?? handles.ToArray());
		}
		
		public static CoroutineHandle Run(this IEnumerator<float> coroutine)
		{
			return Timing.RunCoroutine(coroutine);
		}
		
		public static CoroutineHandle Run(this IEnumerator<float> coroutine, GameObject gameObject)
		{
			return Timing.RunCoroutine(coroutine, gameObject);
		}
		
		public static CoroutineHandle RunSafely(this IEnumerator<float> coroutine)
		{
			return Timing.RunCoroutine(coroutine.Safe());
		}
		
		public static IEnumerator<float> Safe(this IEnumerator<float> coroutine)
		{
			int step = 0;
			for (;;)
			{
				float current;
				try
				{
					if (!coroutine.MoveNext())
					{
						yield break;
					}
					current = coroutine.Current;
					int num = step;
					step = num + 1;
				}
				catch (Exception ex)
				{
					Log.Error(
						$"Error in MecExtensions.Safe({coroutine.GetType().FullName}) after {step}'th yield: {ex}");
					yield break;
				}
				yield return current;
			}
		}
		
		public static IEnumerator<float> While(this IEnumerator<float> coroutine, Func<bool> condition)
		{
			while (condition() && coroutine.MoveNext())
			{
				yield return coroutine.Current;
			}
		}
		
		public static IEnumerator<float> WhileOwnsItem(this IEnumerator<float> coroutine, Player player, Item item)
		{
			while (player.IsConnected && item.Base.Owner == player.ReferenceHub && coroutine.MoveNext())
			{
				yield return coroutine.Current;
			}
		}
		
		public static CoroutineHandle InvokePeriodically(this Action action, float interval)
		{
			return Timing.RunCoroutine(MecExtensions.PeriodicInvocation(action, interval));
		}
		
		private static IEnumerator<float> PeriodicInvocation(Action action, float interval)
		{
			for (;;)
			{
				try
				{
					action();
				}
				catch (Exception e)
				{
					MethodInfo method = action.Method;
					string format = "Error in MecExtensions.PeriodicInvocation: {0}.{1} has caused an exception: {2}";
					Type? declaringType = method.DeclaringType;
					Log.Error(string.Format(format, declaringType?.FullName ?? "[null]", method.Name, e));
				}
				yield return Timing.WaitForSeconds(interval);
			}
		}
		
		public static void CallDelayedSafely(float delay, Action action)
		{
			DelayedInvocation(delay, action).RunSafely();
		}
		
		private static IEnumerator<float> DelayedInvocation(float delay, Action action)
		{
			yield return Timing.WaitForSeconds(delay);
			try
			{
				action();
			}
			catch (Exception e)
			{
				MethodInfo method = action.Method;
				string format = "Error in MecExtensions.CallDelayedSafely: {0}.{1} has caused an exception: {2}";
				Type declaringType = method.DeclaringType!;
				Log.Error(string.Format(format, declaringType.FullName ?? "[null]", method.Name, e));
			}
		}
	}
}