using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FlexFramework.Excel;

public sealed class Vector2Converter : CustomConverter<Vector2>
{
	public override Vector2 Convert(string input)
	{
		if (!Regex.IsMatch(input, "^\\([-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b\\)$"))
		{
			throw new FormatException("Vector2 value expression invalid: " + input);
		}
		string[] array = Split(input.Trim('(', ')'), ',');
		float x = ValueConverter.Convert<float>(array[0]);
		float y = ValueConverter.Convert<float>(array[1]);
		return new Vector2(x, y);
	}
}
