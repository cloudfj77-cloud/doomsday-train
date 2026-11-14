using System.Text.RegularExpressions;

namespace FlexFramework.Excel;

public class ObjectConverter : CustomConverter<object>
{
	public override object Convert(string input)
	{
		if (Regex.IsMatch(input, "^[+-]?[\\d]+[\\d,]*\\.{0,1}[\\d]*$", RegexOptions.IgnoreCase))
		{
			input = Regex.Replace(input, ",|\\+", string.Empty);
			if (Regex.IsMatch(input, "\\."))
			{
				if (float.TryParse(input, out var result))
				{
					return result;
				}
				if (double.TryParse(input, out var result2))
				{
					return result2;
				}
			}
			if (ushort.TryParse(input, out var result3))
			{
				return result3;
			}
			if (short.TryParse(input, out var result4))
			{
				return result4;
			}
			if (uint.TryParse(input, out var result5))
			{
				return result5;
			}
			if (int.TryParse(input, out var result6))
			{
				return result6;
			}
			if (ulong.TryParse(input, out var result7))
			{
				return result7;
			}
			if (long.TryParse(input, out var result8))
			{
				return result8;
			}
		}
		if (Regex.IsMatch(input, "^(true|false)$", RegexOptions.IgnoreCase) && bool.TryParse(input, out var result9))
		{
			return result9;
		}
		return input;
	}
}
