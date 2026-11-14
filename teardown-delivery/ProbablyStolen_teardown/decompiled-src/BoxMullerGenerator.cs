using System;

public static class BoxMullerGenerator
{
	private static bool hasSpare;

	private static double spare;

	private static readonly Random random = new Random();

	public static double NextNormal()
	{
		if (hasSpare)
		{
			hasSpare = false;
			return spare;
		}
		double num;
		double num2;
		double num3;
		do
		{
			num = random.NextDouble() * 2.0 - 1.0;
			num2 = random.NextDouble() * 2.0 - 1.0;
			num3 = num * num + num2 * num2;
		}
		while (num3 >= 1.0 || num3 == 0.0);
		num3 = Math.Sqrt(-2.0 * Math.Log(num3) / num3);
		spare = num2 * num3;
		hasSpare = true;
		return num * num3;
	}

	public static double NextNormalSpec(int variance, int mean)
	{
		return NextNormal() * (double)variance + (double)mean;
	}

	public static int NextNormal100()
	{
		int num = Convert.ToInt32(NextNormalSpec(50, 10));
		if (num < 0)
		{
			num = 0;
		}
		else if (num > 100)
		{
			num = 100;
		}
		return num;
	}
}
