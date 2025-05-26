using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreLib.DevTools.Extensions
{
	public static class MathExtensions
	{
		public static float GetClosestT(Vector3 a, Vector3 b, Vector3 target)
		{
			if (a == b)
			{
				return 0f;
			}
			Vector3 ab = b - a;
			return Vector3.Dot(target - a, ab) / ab.sqrMagnitude;
		}
	
		public static float Randomize(this float value, float randomness)
		{
			return value * Random.Range(1f - randomness, 1f + randomness);
		}
	
		public static void GetPlaneDirections(Vector3 normal, out Vector3 right, out Vector3 up)
		{
			normal.Normalize();
			Vector3 arbitrary = (Mathf.Abs(normal.y) < 0.9f) ? Vector3.up : Vector3.right;
			right = Vector3.Cross(normal, arbitrary).normalized;
			up = Vector3.Cross(normal, right).normalized;
		}
	
		public static void GetTrianglePoints(Vector3 center, Vector3 normal, float sideLength, out Vector3 p0, out Vector3 p1, out Vector3 p2)
		{
			GetPlaneDirections(normal, out var right, out var up);
			float height = sideLength / 1.7320508f;
			float halfSide = sideLength * 0.5f;
			float verticalDown = sideLength / 3.4641016f;
			p0 = center + up * height;
			p1 = center - right * halfSide - up * verticalDown;
			p2 = center + right * halfSide - up * verticalDown;
		}
	
		public static void GetSquarePoints(Vector3 normal, Vector3 center, float sideLength, out Vector3 p0, out Vector3 p1, out Vector3 p2, out Vector3 p3)
		{
			GetPlaneDirections(normal, out var right, out var up);
			float halfSide = sideLength * 0.5f;
			p0 = center + right * halfSide + up * halfSide;
			p1 = center - right * halfSide + up * halfSide;
			p2 = center - right * halfSide - up * halfSide;
			p3 = center + right * halfSide - up * halfSide;
		}
		
		public static void GetNGonPoints(Vector3 normal, Vector3 center, float radius, int sides, out Vector3[] points)
		{
			GetPlaneDirections(normal, out var right, out _);
			points = new Vector3[sides];
			Quaternion rotation = Quaternion.AngleAxis(360f / sides, normal);
			for (int i = 0; i < sides; i++)
			{
				right = rotation * right;
				points[i] = center + right * radius;
			}
		}
	
		public static void GetPentagramPoints(Vector3 normal, Vector3 center, float radius, out Vector3[] points)
		{
			GetNGonPoints(normal, center, radius, 5, out points);
			
			Vector3[] starPoints = new Vector3[5];
			starPoints[0] = points[0];
			starPoints[1] = points[2];	
			starPoints[2] = points[4];
			starPoints[3] = points[1];
			starPoints[4] = points[3];
    
			points = starPoints;
		}
	
		public static void GetPlaneUp(Vector3 normal, out Vector3 up)
		{
			GetPlaneDirections(normal, out _, out up);
		}
	
		public static void GetPlaneRight(Vector3 normal, out Vector3 right)
		{
			GetPlaneDirections(normal, out right, out _);
		}
	
		public static Vector3 RandomPoint(this Bounds bounds)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
		}
	
		[Pure]
		public static float AtMost(this float value, float max)
		{
			if (value > max)
			{
				return max;
			}
			return value;
		}
	
		[Pure]
		public static float AtLeast(this float value, float min)
		{
			if (value < min)
			{
				return min;
			}
			return value;
		}
	
		[Pure]
		public static float Between(this float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}
	
		public static Vector3 FromToRotation(Vector3 current, Vector3 target)
		{
			return new Vector3(Mathf.DeltaAngle(current.x, target.x), Mathf.DeltaAngle(current.y, target.y), Mathf.DeltaAngle(current.z, target.z));
		}
	}
}