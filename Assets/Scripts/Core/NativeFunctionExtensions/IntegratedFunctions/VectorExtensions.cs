using System;
using UnityEngine;

public static class Vector_Math_Extension
{
	private static float precision = 0.01f;

	public static bool RoughlyEquals(this Vector3 thisVector, Vector3 otherVector)
	{
		return Math.Abs(thisVector.x - otherVector.x) <= precision
			&& Math.Abs(thisVector.y - otherVector.y) <= precision
			&& Math.Abs(thisVector.z - otherVector.z) <= precision;
	}

	public static bool RoughlyEquals(this Vector2 thisVector, Vector2 otherVector)
	{
		return Math.Abs(thisVector.x - otherVector.x) <= precision
			&& Math.Abs(thisVector.y - otherVector.y) <= precision;
	}

	public static bool RoughlyEquals(this Vector4 thisVector, Vector4 otherVector)
	{
		return Math.Abs(thisVector.x - otherVector.x) <= precision
			&& Math.Abs(thisVector.y - otherVector.y) <= precision
			&& Math.Abs(thisVector.z - otherVector.z) <= precision
			&& Math.Abs(thisVector.w - otherVector.w) <= precision;
	}

	public static bool RoughlyOrthoTo(this Vector3 thisVector, Vector3 otherVector)
	{
		return (Math.Abs(thisVector.x - otherVector.x) <= precision
			&& Math.Abs(thisVector.y - otherVector.y) <= precision)
			|| (Math.Abs(thisVector.x - otherVector.x) <= precision
			&& Math.Abs(thisVector.z - otherVector.z) <= precision)
			|| (Math.Abs(thisVector.y - otherVector.y) <= precision
			&& Math.Abs(thisVector.z - otherVector.z) <= precision);
	}

	public static Vector2 ClampPosition(this Vector2 position, Vector2 xBoundary, Vector2 yBoundary)
	{
		if (xBoundary != Vector2.zero)
			position.x = Mathf.Clamp(position.x, xBoundary[0], xBoundary[1]);
		if (yBoundary != Vector2.zero)
			position.y = Mathf.Clamp(position.y, yBoundary[0], yBoundary[1]);
		return position;
	}

	public static Vector3 ClampPosition(this Vector3 position, Vector2 xBoundary, Vector2 yBoundary, Vector2 zBoundary)
	{
		if (xBoundary != Vector2.zero)
			position.x = Mathf.Clamp(position.x, xBoundary[0], xBoundary[1]);
		if (yBoundary != Vector2.zero)
			position.y = Mathf.Clamp(position.y, yBoundary[0], yBoundary[1]);
		if (zBoundary != Vector2.zero)
			position.z = Mathf.Clamp(position.z, zBoundary[0], zBoundary[1]);
		return position;
	}

	public static Vector4 ClampPosition(this Vector4 position, Vector2 xBoundary, Vector2 yBoundary, Vector2 zBoundary, Vector2 wBoundary)
	{
		if (xBoundary != Vector2.zero)
			position.x = Mathf.Clamp(position.x, xBoundary[0], xBoundary[1]);
		if (yBoundary != Vector2.zero)
			position.y = Mathf.Clamp(position.y, yBoundary[0], yBoundary[1]);
		if (zBoundary != Vector2.zero)
			position.z = Mathf.Clamp(position.z, zBoundary[0], zBoundary[1]);
		if (wBoundary != Vector2.zero)
			position.w = Mathf.Clamp(position.w, wBoundary[0], wBoundary[1]);
		return position;
	}
}
