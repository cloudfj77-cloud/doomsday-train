using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FlexFramework.Excel;

public class Color32Converter : CustomConverter<Color32>
{
	public override Color32 Convert(string input)
	{
		if (Regex.IsMatch(input, "^\\(([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5]),([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5]),([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5]),([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\\)$"))
		{
			string[] array = Split(input.Trim('(', ')'), ',');
			int num = ValueConverter.Convert<int>(array[0]);
			int num2 = ValueConverter.Convert<int>(array[1]);
			int num3 = ValueConverter.Convert<int>(array[2]);
			int num4 = ValueConverter.Convert<int>(array[3]);
			return new Color(num, num2, num3, num4);
		}
		if (Regex.IsMatch(input, "^\\(([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5]),([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5]),([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\\)$"))
		{
			string[] array2 = Split(input.Trim('(', ')'), ',');
			int num5 = ValueConverter.Convert<int>(array2[0]);
			int num6 = ValueConverter.Convert<int>(array2[1]);
			int num7 = ValueConverter.Convert<int>(array2[2]);
			return new Color(num5, num6, num7);
		}
		throw new FormatException("Color32 value expression invalid: " + input);
	}
}
