using System;

public static class BetaGenerator
{
	private static readonly Random random = new Random();

	public static double NextBeta(double alpha, double beta)
	{
		double num = NextGamma(alpha, 1.0);
		double num2 = NextGamma(beta, 1.0);
		return num / (num + num2);
	}

	private static double NextGamma(double shape, double scale)
	{
		double num3;
		if (shape >= 1.0)
		{
			double num = shape - 1.0 / 3.0;
			double num2 = 1.0 / Math.Sqrt(9.0 * num);
			while (true)
			{
				num3 = NextNormal();
				double num4 = 1.0 + num2 * num3;
				if (!(num4 <= 0.0))
				{
					num4 = num4 * num4 * num4;
					double num5 = random.NextDouble();
					double num6 = num3 * num3;
					if (!(num5 >= 1.0 - 0.0331 * num6 * num6) || !(Math.Log(num5) >= 0.5 * num6 + num * (1.0 - num4 + Math.Log(num4))))
					{
						break;
					}
				}
			}
		}
		else
		{
			double num7 = NextGamma(shape + 1.0, 1.0);
			double x = random.NextDouble();
			num3 = num7 * Math.Pow(x, 1.0 / shape);
		}
		return scale * num3;
	}

	private static double NextNormal()
	{
		double num;
		double num3;
		do
		{
			num = random.NextDouble() * 2.0 - 1.0;
			double num2 = random.NextDouble() * 2.0 - 1.0;
			num3 = num * num + num2 * num2;
		}
		while (num3 >= 1.0 || num3 == 0.0);
		num3 = Math.Sqrt(-2.0 * Math.Log(num3) / num3);
		return num * num3;
	}

	public static int NextBetaInt(int alpha, int beta, int variance, int mean)
	{
		return Convert.ToInt32(NextBeta(alpha, beta) * (double)variance + (double)mean);
	}
}
