using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FlexFramework.Excel;

public sealed class Vector4Converter : CustomConverter<Vector4>
{
	public override Vector4 Convert(string input)
	{
		if (!Regex.IsMatch(input, "^\\([-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b\\),[-+]?[0-9]*\\.?[0-9]+\\b$"))
		{
			throw new FormatException("Vector4 value expression invalid: " + input);
		}
		string[] array = Split(input.Trim('(', ')'), ',');
		float x = ValueConverter.Convert<float>(array[0]);
		float y = ValueConverter.Convert<float>(array[1]);
		float z = ValueConverter.Convert<float>(array[2]);
		float w = ValueConverter.Convert<float>(array[3]);
		return new Vector4(x, y, z, w);
	}
}
