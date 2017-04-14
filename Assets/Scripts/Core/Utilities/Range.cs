
public enum ERange
{
	Destroyed = -1,
	R1 = 0, //2.2
	R2, //3.0
	R3, //4.2
	R4, //5.0
	R5, //5.8
	R6, //6.2
	R7, //7.0
	R8, //8.8
	RI, //50
	Count,
	R0,
}

public static class Range
{
	public static float GetRangeValue(ERange range)
	{
		switch (range)
		{
			case ERange.R1:
				return 2.2f;
			case ERange.R2:
				return 3.0f;
			case ERange.R3:
				return 4.2f;
			case ERange.R4:
				return 5.0f;
			case ERange.R5:
				return 5.8f;
			case ERange.R6:
				return 6.2f;
			case ERange.R7:
				return 7.0f;
			case ERange.R8:
				return 8.8f;
			case ERange.RI:
				return 50f;
			default:
				return 0f;
		}
	}

	public static float GetRangeSquare(ERange range)
	{
		float value = GetRangeValue(range);
		return value * value;
	}
}
