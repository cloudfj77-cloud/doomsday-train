using System;
using System.Collections.Generic;
using UnityEngine;

public class RNG
{
	public static System.Random random = new System.Random();

	public static bool Roll(int chance)
	{
		if (UnityEngine.Random.Range(0, 100) < chance)
		{
			return true;
		}
		return false;
	}

	public static bool Roll(float chance)
	{
		return UnityEngine.Random.Range(0f, 100f) < chance;
	}

	public static bool Roll(int chance, int topRange)
	{
		if (UnityEngine.Random.Range(0, topRange) < chance)
		{
			return true;
		}
		return false;
	}

	public static float GetRandomFloat(float minInclusive, float maxInclusive)
	{
		return UnityEngine.Random.Range(minInclusive, maxInclusive);
	}

	public static double GetRandomDouble(double minInclusive, double maxInclusive)
	{
		double num = maxInclusive - minInclusive;
		return minInclusive + random.NextDouble() * num;
	}

	public static int GetRandomInt(int minInclusive, int maxInclusive)
	{
		return UnityEngine.Random.Range(minInclusive, maxInclusive + 1);
	}

	public static int GetRandomIntExculsive(int minInclusive, int maxExclusive)
	{
		return UnityEngine.Random.Range(minInclusive, maxExclusive);
	}

	public static int GetRandomInt()
	{
		return UnityEngine.Random.Range(0, 101);
	}

	public static T GetRandomElement<T>(List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			throw new ArgumentException("List cannot be null or empty.");
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T GetRandomElement<T>(T[] array)
	{
		if (array == null || array.Length == 0)
		{
			throw new ArgumentException("Array cannot be null or empty.");
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	public static T GetRandomElement<T>(IReadOnlyList<T> list)
	{
		if (list == null || list.Count == 0)
		{
			throw new ArgumentException("List cannot be null or empty.");
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static int GetOdd(int[] oddArray)
	{
		int num = 0;
		int[] array = oddArray;
		foreach (int num2 in array)
		{
			num += num2;
		}
		int randomInt = GetRandomInt(0, num);
		int num3 = 0;
		int num4 = 0;
		array = oddArray;
		foreach (int num5 in array)
		{
			num3 += num5;
			if (randomInt < num3)
			{
				return num4;
			}
			num4++;
		}
		return 0;
	}

	public static T GetOdd<T>((T Value, int Weight)[] options)
	{
		int num = 0;
		(T, int)[] array = options;
		for (int i = 0; i < array.Length; i++)
		{
			(T, int) tuple = array[i];
			num += tuple.Item2;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		int num3 = 0;
		array = options;
		for (int i = 0; i < array.Length; i++)
		{
			(T, int) tuple2 = array[i];
			num3 += tuple2.Item2;
			if (num2 < num3)
			{
				return tuple2.Item1;
			}
		}
		return options[0].Value;
	}

	public static int GetOdd(List<int> oddArray)
	{
		int num = 0;
		foreach (int item in oddArray)
		{
			num += item;
		}
		int randomInt = GetRandomInt(0, num);
		int num2 = 0;
		int num3 = 0;
		foreach (int item2 in oddArray)
		{
			num2 += item2;
			if (randomInt < num2)
			{
				return num3;
			}
			num3++;
		}
		return 0;
	}

	public static string GetOddString(int[] oddArray, string[] ids)
	{
		return ids[GetOdd(oddArray)];
	}

	public static List<int> DivideWithDeviation(int total, int count, float deviationRange)
	{
		if (count <= 0)
		{
			throw new ArgumentException("Count must be greater than 0.");
		}
		if (deviationRange < 0f || deviationRange > 1f)
		{
			throw new ArgumentException("Deviation range must be between 0 and 1.");
		}
		int num = total / count;
		List<int> list = new List<int>(count);
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			float num3 = 1f + UnityEngine.Random.Range(0f - deviationRange, deviationRange);
			int num4 = Mathf.RoundToInt((float)num * num3);
			if (i == count - 1)
			{
				num4 = total - num2;
			}
			list.Add(num4);
			num2 += num4;
		}
		return list;
	}

	public static int CalculatePercentage(int value, int percentage)
	{
		return value * percentage / 100;
	}

	public static int CalculatePercentageOff(int value, int percentage)
	{
		return CalculatePercentage(value, percentage) + value;
	}

	public static void RunTest()
	{
		int[] array = new int[6] { 1, 4, 4, 8, 16, 32 };
		Debug.Log("Starting test for GetOdd function.");
		Debug.Log("Input array: " + string.Join(", ", array));
		int[] array2 = new int[array.Length];
		for (int i = 0; i < 1000000; i++)
		{
			int odd = GetOdd(array);
			array2[odd]++;
		}
		string text = "";
		for (int j = 0; j < array.Length; j++)
		{
			text += $"{array[j]}({array2[j]})";
			if (j < array.Length - 1)
			{
				text += ", ";
			}
		}
		Debug.Log("Result after 100 rolls: " + text);
		Debug.Log("Test for GetOdd completed.");
	}

	public static (int, int) ModuloWithCount(int value, int modulo)
	{
		if (value < 0 || modulo <= 0)
		{
			Debug.Log("Error, Input may cause infinite loop");
			return (0, 0);
		}
		int num = 0;
		while (value >= modulo)
		{
			num++;
			value -= modulo;
		}
		return (num, value);
	}

	public static int MultiplyAndRound(int a, double b, bool roundUp = false)
	{
		double a2 = (double)a * b;
		if (!roundUp)
		{
			return (int)Math.Round(a2);
		}
		return (int)Math.Ceiling(a2);
	}

	public static string FormatPercentage(int value)
	{
		if (value >= 0)
		{
			return $"+{value}%";
		}
		return $"{value}%";
	}

	public static string FormatIntDisplay(int value)
	{
		if (value >= 0)
		{
			return $"+{value}";
		}
		return $"{value}";
	}
}
