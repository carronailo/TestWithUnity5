using UnityEngine;

public static class Touch_ToString_Extension
{
	public static string ToStringEx(this Touch t)
	{
		return string.Format("id|{0},pos|{1},tc|{2}", t.fingerId, t.position, t.tapCount);
	}
}
