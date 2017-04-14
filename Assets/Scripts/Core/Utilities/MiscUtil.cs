using UnityEngine;
using System.Collections;

public static class MiscUtil
{
	public static int CompareTransformName(Transform x, Transform y)
	{
		if (x == null)
		{
			if (y == null)
				return 0;
			return 1;
		}
		else if (y == null)
			return -1;
		else
			return string.Compare(x.name, y.name);
	}

	public static int CompareGameObjectName(GameObject x, GameObject y)
	{
		if (x == null)
		{
			if (y == null)
				return 0;
			return 1;
		}
		else if (y == null)
			return -1;
		else
			return string.Compare(x.name, y.name);
	}

	public static bool IsTouchDevice()
	{
		return Application.platform == RuntimePlatform.Android
			|| Application.platform == RuntimePlatform.IPhonePlayer;
	}

}
