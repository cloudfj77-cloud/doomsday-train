using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FlexFramework.Excel;

public abstract class CustomConverter<T> : IConverter
{
	Type IConverter.Type => typeof(T);

	public abstract T Convert(string input);

	public bool TryConvert(string input, out T value)
	{
		value = default(T);
		try
		{
			value = Convert(input);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	protected string[] Split(string input, params char[] separators)
	{
		string text = string.Empty;
		for (int i = 0; i < separators.Length; i++)
		{
			text = ((i > 0) ? "|" : (string.Empty + text + "(?<!\\\\)" + separators[i]));
		}
		string[] array = Regex.Split(input, text, RegexOptions.Multiline);
		for (int j = 0; j < array.Length; j++)
		{
			for (int k = 0; k < separators.Length; k++)
			{
				char c = separators[k];
				array[j] = array[j].Replace("\\" + c, c.ToString());
			}
		}
		return array;
	}

	protected IEnumerable<string> SplitGroup(string input, char start, char end)
	{
		string pattern = "\\G\\[(\\\\\\[|\\\\\\]|[^\\[\\]])+\\]".Replace('[', start).Replace(']', end);
		foreach (Match item in Regex.Matches(input, pattern, RegexOptions.Multiline))
		{
			string value = item.Value;
			if (!item.Success)
			{
				throw new ArgumentException("Group expression invalid", input);
			}
			value = value.TrimStart(start).TrimEnd(end);
			value = value.Replace("\\" + start, start.ToString()).Replace("\\" + end, end.ToString());
			if (Regex.IsMatch(value, "(?<=\\([^]]*),(?=[^]]*\\))"))
			{
				value = Regex.Replace(value, "(?<=\\([^]]*),(?=[^]]*\\))", "\\,");
			}
			yield return value;
		}
	}

	object IConverter.Convert(string input)
	{
		return Convert(input);
	}
}
