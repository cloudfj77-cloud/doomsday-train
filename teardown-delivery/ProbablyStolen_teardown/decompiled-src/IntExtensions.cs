using System;

internal static class IntExtensions
{
	internal static int Mod(this int a, int n)
	{
		if (n == 0)
		{
			throw new ArgumentOutOfRangeException("n", "(a mod 0) is undefined.");
		}
		int num = a % n;
		if ((n > 0 && num < 0) || (n < 0 && num > 0))
		{
			return num + n;
		}
		return num;
	}
}
