using System;
using System.Collections.Generic;

public static class ItemSpawnerHiddenList
{
	private static readonly string[] HIDDEN = new string[5] { "save_bag", "back_inv_save_bag", "hidden_save_bag", "showcase_save_bag", "sold_save_bag" };

	private static HashSet<string> exact;

	private static List<string> prefixes;

	public static bool IsHidden(string identifier)
	{
		if (string.IsNullOrEmpty(identifier))
		{
			return false;
		}
		if (exact == null)
		{
			Build();
		}
		if (exact.Contains(identifier))
		{
			return true;
		}
		foreach (string prefix in prefixes)
		{
			if (identifier.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static void Build()
	{
		exact = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		prefixes = new List<string>();
		string[] hIDDEN = HIDDEN;
		foreach (string text in hIDDEN)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = text.Trim();
				if (text2.EndsWith("*"))
				{
					prefixes.Add(text2.Substring(0, text2.Length - 1));
				}
				else
				{
					exact.Add(text2);
				}
			}
		}
	}
}
