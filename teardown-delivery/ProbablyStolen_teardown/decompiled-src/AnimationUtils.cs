using System;

public class AnimationUtils
{
	private static double Square(double x)
	{
		if (x == 0.0)
		{
			return 1.0;
		}
		return Math.Sign(x);
	}

	private static double Triangular(double x, double skew = 0.5)
	{
		x = Math.Clamp(x, 0.0, 1.0);
		skew = Math.Clamp(skew, 0.0, 1.0);
		if (skew <= 0.0)
		{
			return 1.0 * (1.0 - x) + -1.0 * x;
		}
		if (skew >= 1.0)
		{
			return -1.0 * (1.0 - x) + 1.0 * x;
		}
		if (x < skew)
		{
			x /= skew;
			return -1.0 * (1.0 - x) + 1.0 * x;
		}
		x = (x - skew) / (1.0 - skew);
		return 1.0 * (1.0 - x) + -1.0 * x;
	}

	public static float SineWave(double currentTime, double period = 1.0, double relativePhase = 0.0, double relativeOffset = 0.0, double amplitude = 1.0)
	{
		return (float)((Math.Sin(currentTime * Math.PI * 2.0 / period + relativePhase * Math.PI * 2.0) + relativeOffset) * amplitude);
	}

	public static float PulseWave(double currentTime, double period = 1.0, double relativePhase = 0.0, double relativeDuty = 0.5, double relativeOffset = 0.0, double amplitude = 1.0)
	{
		double num = (currentTime / period + relativePhase) % 1.0;
		num = (num + 1.0) % 1.0;
		return (float)((Square(relativeDuty - num) + relativeOffset) * amplitude);
	}

	public static float SpikeWave(double currentTime, double period = 1.0, double relativePhase = 0.0, double relativeSkew = 0.5, double relativeOffset = 0.0, double amplitude = 1.0)
	{
		relativeSkew = Math.Clamp(relativeSkew, 0.0, 1.0);
		double num = ((!(relativeSkew < 0.5)) ? (relativeSkew / 2.0) : ((relativeSkew + 1.0) / 2.0));
		return (float)((Triangular(((currentTime / period + relativePhase + num) % 1.0 + 1.0) % 1.0, relativeSkew) + relativeOffset) * amplitude);
	}

	public static float ScaledCosineWave(double currentTime, double period = 1.0, double relativePhase = 0.0, double relativeOffset = 0.0, double amplitude = 1.0)
	{
		return SineWave(currentTime, period, relativePhase + 0.25, relativeOffset + 1.0, amplitude * 0.5);
	}

	public static float ScaledPulseWave(double currentTime, double period = 1.0, double relativePhase = 0.0, double relativeDuty = 0.5, double relativeOffset = 0.0, double amplitude = 1.0)
	{
		return PulseWave(currentTime, period, relativePhase + 0.25, relativeDuty, relativeOffset + 1.0, amplitude * 0.5);
	}
}
