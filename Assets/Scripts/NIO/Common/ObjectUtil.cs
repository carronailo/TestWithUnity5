
namespace NIO
{
	using System;

	public static class ObjectUtil
	{
		public static T CheckNotNull<T>(T arg, string text)
		{
			if (arg == null)
				throw new NullReferenceException(text);
			return arg;
		}

		public static int CheckPositive(int i, string name)
		{
			if (i <= 0)
				throw new ArgumentException(string.Format("{0}: {1} (expected: > 0)", name, i));
			return i;
		}

		public static long CheckPositive(long i, string name)
		{
			if (i <= 0)
				throw new ArgumentException(string.Format("{0}: {1} (expected: > 0)", name, i));
			return i;
		}

		public static int CheckPositiveOrZero(int i, string name)
		{
			if (i < 0)
				throw new ArgumentException(string.Format("{0}: {1} (expected: > 0)", name, i));
			return i;
		}

		public static long CheckPositiveOrZero(long i, string name)
		{
			if (i < 0)
				throw new ArgumentException(string.Format("{0}: {1} (expected: > 0)", name, i));
			return i;
		}

		public static T[] CheckNotEmpty<T>(T[] array, string name)
		{
			CheckNotNull(array, name);
			CheckPositive(array.Length, name + ".length");
			return array;
		}

	}
}
