using System.Linq;

namespace ScuffedZombies;

public class MathEx
{
	public static float Min(params float[] values)
	{
		return values.Min();
	}

	public static float Max(params float[] values)
	{
		return values.Max();
	}
}
