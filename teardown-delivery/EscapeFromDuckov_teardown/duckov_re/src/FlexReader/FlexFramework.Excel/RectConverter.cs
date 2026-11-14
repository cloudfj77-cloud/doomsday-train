using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FlexFramework.Excel;

public sealed class RectConverter : CustomConverter<Rect>
{
	public override Rect Convert(string input)
	{
		if (!Regex.IsMatch(input, "^\\([-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b,[-+]?[0-9]*\\.?[0-9]+\\b\\)$"))
		{
			throw new FormatException("Rect value expression invalid: " + input);
		}
		string[] array = Split(input.Trim('(', ')'), ',');
		float x = ValueConverter.Convert<float>(array[0]);
		float y = ValueConverter.Convert<float>(array[1]);
		float width = ValueConverter.Convert<float>(array[2]);
		float height = ValueConverter.Convert<float>(array[3]);
		return new Rect(x, y, width, height);
	}
}
