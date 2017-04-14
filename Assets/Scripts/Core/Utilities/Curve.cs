using System.Collections.Generic;
using UnityEngine;

public static class Curve
{
	public static List<Vector3> Line(Vector3 p0, Vector3 p1, float delta)
	{
		List<Vector3> line = new List<Vector3>();
		float t = 0f;
		float limit = 1f + delta;
		while (t < limit)
		{
			line.Add(Vector3.Lerp(p0, p1, t));
			t += delta;
		}
		return line;
	}

	public static List<Vector3> Polyline(Vector3 p0, Vector3 p1, Vector3 p2, float delta)
	{
		List<Vector3> line = new List<Vector3>();
		float t = 0f;
		float limit = 0.5f + delta;
		while (t < limit)
		{
			line.Add(Vector3.Lerp(p0, p1, t * 2f));
			t += delta;
		}
		t = delta;
		while (t < limit)
		{
			line.Add(Vector3.Lerp(p1, p2, t * 2f));
			t += delta;
		}
		return line;
	}

	public static List<Vector3> Arc(Vector3 p0, Vector3 p1, Vector3 p2, float delta)
	{
		List<Vector3> line = new List<Vector3>();
		float t = 0f;
		float limit = 1f + delta;
		Vector3 p4 = (p1 - p0 - p2) * -1f;
		Vector3 v0 = p0 - p4;
		Vector3 v1 = p2 - p4;
		while (t < limit)
		{
			line.Add(Vector3.Slerp(v0, v1, t) + p4);
			t += delta;
		}
		return line;
	}

	public static List<Vector3> Bezier2Curve(Vector3 p0, Vector3 p1, Vector3 p2, float delta)
	{
		List<Vector3> curve = new List<Vector3>();
		float t = 0f;
		float limit = 1f + delta;
		while (t < limit)
		{
			float x = Bezier2(p0.x, p1.x, p2.x, t);
			float y = Bezier2(p0.y, p1.y, p2.y, t);
			float z = Bezier2(p0.z, p1.z, p2.z, t);
			curve.Add(new Vector3(x, y, z));
			t += delta;
		}
		return curve;
	}

	public static List<Vector3> Bezier3Curve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float delta)
	{
		List<Vector3> curve = new List<Vector3>();
		float t = 0f;
		float limit = 1f + delta;
		while (t < limit)
		{
			float x = Bezier3(p0.x, p1.x, p2.x, p3.x, t);
			float y = Bezier3(p0.y, p1.y, p2.y, p3.y, t);
			float z = Bezier3(p0.z, p1.z, p2.z, p3.z, t);
			curve.Add(new Vector3(x, y, z));
			t += delta;
		}
		return curve;
	}

	private static float Bezier2(float p0, float p1, float p2, float t)
	{
		float tSquare = t * t;
		return (p2 - 2 * p1 + p0) * tSquare + 2 * (p1 - p0) * t + p0;
	}

	private static float Bezier3(float p0, float p1, float p2, float p3, float t)
	{
		float tSquare = t * t;
		float tCube = t * tSquare;
		return (p3 - 3 * p2 + 3 * p1 - p0) * tCube + 3 * (p2 - 2 * p1 + p0) * tSquare + 3 * (p1 - p0) * t + p0;
	}
}
