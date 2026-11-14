using System;
using System.Collections.Generic;

namespace FlexFramework.Excel;

public sealed class DictionaryConverter<TKey, TValue> : CustomConverter<Dictionary<TKey, TValue>>
{
	public override Dictionary<TKey, TValue> Convert(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return null;
		}
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		foreach (string item in SplitGroup(input, '[', ']'))
		{
			string[] array = Split(item, ',');
			TKey val = ValueConverter.Convert<TKey>(array[0]);
			TValue value = ValueConverter.Convert<TValue>(array[1]);
			if (dictionary.ContainsKey(val))
			{
				TKey val2 = val;
				throw new FormatException("Dictionary key is duplicate: " + val2);
			}
			dictionary.Add(val, value);
		}
		return dictionary;
	}
}
