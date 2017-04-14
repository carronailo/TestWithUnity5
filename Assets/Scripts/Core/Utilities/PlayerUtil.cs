using UnityEngine;

public static class PlayerUtil
{
	static GameObject mainPlayer = null;

	public static GameObject MainPlayer
	{
		get
		{
			if (mainPlayer == null)
				mainPlayer = GameObject.FindWithTag("Player");
			return mainPlayer;
		}
	}

}
