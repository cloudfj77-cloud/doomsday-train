using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FlexFramework.Excel;

public sealed class Vector3Converter : CustomConverter<Vector3>
{
	public override Vector3 Convert(string input)
	{
		if (!Regex.IsMatch(input, "^\\([-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b\\)$"))
		{
			throw new FormatException("Vector3 value expression invalid: " + input);
		}
		string[] array = Split(input.Trim('(', ')'), ',');
		float x = ValueConverter.Convert<float>(array[0]);
		float y = ValueConverter.Convert<float>(array[1]);
		float z = ValueConverter.Convert<float>(array[2]);
		return new Vector3(x, y, z);
	}
}
