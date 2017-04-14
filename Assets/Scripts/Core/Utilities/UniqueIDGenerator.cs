using System;

public static class UniqueIDGenerator
{
	private static long lastSeed = 0L;
	private static short count = 0;

	public static long GetID()
	{
		long seed = (long)(DateTime.Now.Ticks * 0.0001) * 10000;
		if (seed == lastSeed)
		{
			++count;
			if (count > 9999)
				count = 9999;
			return seed + count;
		}
		else
		{
			lastSeed = seed;
			count = 0;
			return seed;
		}
	}
}
