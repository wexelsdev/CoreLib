using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace CoreLib.DevTools.Extensions
{
	public static class CollectionExtensions
	{
		[Pure]
		public static bool AnyOfType<TAny>(this IEnumerable enumerable, out TAny? match)
		{
			foreach (object item in enumerable)
			{
				if (item is TAny any)
				{
					match = any;
					return true;
				}
			}
			
			match = default;
			return false;
		}
	
		[Pure]
		public static bool AnyOfType<TAny>(this IEnumerable enumerable)
		{
			IEnumerator enumerator = enumerable.GetEnumerator();
			using var enumerator1 = enumerator as IDisposable;
			
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is TAny)
				{
					return true;
				}
			}
			
			return false;
		}
	
		[ContractAnnotation("=> true, match:notnull; => false, match:null")]
		[Pure]
		public static bool Any<T>(this IEnumerable<T?> enumerable, Func<T, bool> predicate, out T? match)
		{
			foreach (T? item in enumerable)
			{
				if (item != null && predicate(item))
				{
					match = item;
					return true;
				}
			}
			match = default;
			return false;
		}
		
		[ContractAnnotation("=> true, match:notnull; => false, match:null")]
		[Pure]
		public static bool Any<T>(this IEnumerable<T?> enumerable, out T? match)
		{
			if (enumerable is IList<T> list)
			{
				if (list.Count > 0)
				{
					match = list[0];
					return true;
				}
			}
			else
			{
				using IEnumerator<T?> enumerator = enumerable.GetEnumerator();
				if (enumerator.MoveNext())
				{
					match = enumerator.Current;
					return true;
				}
			}
			match = default;
			return false;
		}
	
		[Pure]
		public static T? MinBy<T, TKey>(this IEnumerable<T?> enumerable, Func<T, TKey> selector)
		{
			return enumerable.MinBy(selector, out _);
		}
	
		[Pure]
		public static T? MinBy<T, TKey>(this IEnumerable<T?> enumerable, Func<T, TKey> selector, out TKey? value)
		{
			Comparer<TKey> comparer = Comparer<TKey>.Default;
			T? t;
			using IEnumerator<T?> enumerator = enumerable.GetEnumerator();
			
			if (!enumerator.MoveNext())
			{
				value = default;
				t = default;
			}
			else
			{
				T? min = enumerator.Current;
				value = selector(min!);
				
				while (enumerator.MoveNext())
				{
					T? item = enumerator.Current;
					TKey? key = selector(item!);
					
					if (comparer.Compare(key, value) < 0)
					{
						min = item;
						value = key;
					}
				}
				t = min;
			}

			return t;
		}
	
		[Pure]
		public static T? MaxBy<T, TKey>(this IEnumerable<T?> enumerable, Func<T, TKey> selector)
		{
			return enumerable.MaxBy(selector, out _);
		}
	
		[Pure]
		public static T? MaxBy<T, TKey>(this IEnumerable<T?> enumerable, Func<T, TKey> selector, out TKey? value)
		{
			Comparer<TKey> comparer = Comparer<TKey>.Default;
			T? t;
			using IEnumerator<T?> enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				value = default;
				t = default;
			}
			else
			{
				T? max = enumerator.Current;
				value = selector(max!);
				while (enumerator.MoveNext())
				{
					T? item = enumerator.Current;
					if (item != null)
					{
						TKey? key = selector(item);
						if (comparer.Compare(key, value) > 0)
						{
							max = item;
							value = key;
						}
					}
				}
				t = max;
			}

			return t;
		}
	
		[Pure]
		public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			foreach (TSource item in enumerable)
			{
				if (predicate(item))
				{
					yield return selector(item);
				}
			}
		}
	
		[Pure]
		public static bool None<T>(this IEnumerable<T> enumerable)
		{
			return !enumerable.Any();
		}
	
		[Pure]
		public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			return !enumerable.Any(predicate);
		}
	
		[Pure]
		public static T NextInLoop<T>(this IList<T> list, T current)
		{
			return list[(list.IndexOf(current) + 1) % list.Count];
		}
	
		[Pure]
		public static T PreviousInLoop<T>(this IList<T> list, T current)
		{
			return list[(list.IndexOf(current) - 1 + list.Count) % list.Count];
		}
	
		[Pure]
		public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T except)
		{
			return from t in enumerable
				where !Equals(t, except)
				select t;
		}
	
		[Pure]
		public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
		{
			return from t in enumerable
				where !predicate(t)
				select t;
		}
	
		public static IEnumerable<ValueTuple<T, int>> ForReversed<T>(this IList<T> list)
		{
			int num;
			for (int i = list.Count - 1; i >= 0; i = num - 1)
			{
				yield return new ValueTuple<T, int>(list[i], i);
				num = i;
			}
		}
	
		public static TList Shuffle<TList>(this TList list) where TList : IList
		{
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = Rand.Next(i + 1);
				TList ptr = list;
				TList ptr2;
				if (default(TList) == null)
				{
					TList tlist = ptr;
					ptr2 = tlist;
				}
				else
				{
					ptr2 = ptr;
				}
				ref TList ptr3 = ref ptr2;
				int index = i;
				ref TList ptr4 = ref list;
				TList ptr5;
				if (default(TList) == null)
				{
					TList tlist2 = ptr4;
					ptr5 = tlist2;
				}
				else
				{
					ptr5 = ptr4;
				}
				int index2 = j;
				object value = list[j];
				object value2 = list[i];
				ptr3[index] = value;
				ptr5[index2] = value2;
			}
			return list;
		}
	
		[ContractAnnotation("=> true, item:notnull; => false, item:null")]
		public static bool TryPullLast<T>(this IList<T?> list, out T? item)
		{
			if (list.Count == 0)
			{
				item = default;
				return false;
			}
			
			item = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			
			return true;
		}
	
		[ContractAnnotation("=> true, item:notnull; => false, item:null")]
		public static bool TryPullLast<T>(this IList<T?> list, Func<T, bool> predicate, out T? item)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T? current = list[i];
				if (predicate(current!))
				{
					item = current;
					list[i] = list[list.Count - 1];
					
					list.RemoveAt(list.Count - 1);
					return true;
				}
			}
			
			item = default;
			return false;
		}
	
		public static T PullRandom<T>(this IList<T> list)
		{
			if (list.Count == 0)
			{
				throw new InvalidOperationException("The list is empty.");
			}
			
			int randomIndex = Rand.Next(list.Count);
			T result = list[randomIndex];
			
			list[randomIndex] = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			
			return result;
		}
	
		public static bool Toggle<T>([ItemNotNull] this ISet<T> set, T item)
		{
			if (set.Add(item))
			{
				return true;
			}
			
			set.Remove(item);
			return false;
		}
	
		public static void Set<T>(this ISet<T> set, T item, bool state)
		{
			if (state)
			{
				set.Add(item);
				return;
			}
			
			set.Remove(item);
		}
	
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SwapRemoveAt<T>(this IList<T> list, int index)
		{
			list[index] = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
		}
	
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class, new()
		{
			if (!dictionary.TryGetValue(key, out var obj))
			{
				return dictionary[key] = Activator.CreateInstance<TValue>();
			}
			return obj;
		}
	
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
		{
			foreach (TValue value in values)
			{
				dictionary[keySelector(value)] = value;
			}
		}
	
		public static IEnumerable<ArraySegment<T>> SlidingWindow<T>(this IEnumerable<T> source, int windowSize)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			
			if (windowSize <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(windowSize), "Window size must be greater than zero.");
			}
			
			T[] array = (source as T[]) ?? source.ToArray();
			if (windowSize > array.Length)
			{
				yield break;
			}
			
			int num;
			for (int i = 0; i <= array.Length - windowSize; i = num + 1)
			{
				yield return new ArraySegment<T>(array, i, windowSize);
				num = i;
			}
		}
		
		private static readonly Random Rand = new();
	}
}