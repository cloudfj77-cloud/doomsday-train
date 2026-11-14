using System;
using System.Collections.Generic;
using System.Text;
using QFSW.QC;
using UnityEngine.Localization.Settings;

public static class LocSmartStringDebug
{
	private static readonly int[] testNumbers = new int[13]
	{
		0, 1, 2, 3, 4, 5, 11, 12, 21, 22,
		25, 100, 101
	};

	[Command("loc-plural-test", "Prints a localized entry formatted with a range of test numbers, to check every plural form in the current language", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string PluralTest(string table, string key)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Locale: " + LocHelper.GetCurrentLocaleCode());
		int[] array = testNumbers;
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i];
			string text;
			try
			{
				text = LocalizationSettings.StringDatabase.GetLocalizedString(table, key, new object[4] { num, num, num, num });
			}
			catch (Exception ex)
			{
				text = "ERROR: " + ex.Message;
			}
			stringBuilder.AppendLine(num.ToString().PadLeft(3) + " -> " + text);
		}
		return stringBuilder.ToString();
	}

	[Command("loc-smart-list", "Lists every localization entry currently using smart formatting in the loaded tables", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string SmartList()
	{
		List<string> smartEntryReport = LocSmartStringHelper.GetSmartEntryReport();
		if (smartEntryReport.Count == 0)
		{
			return "No smart entries found in the loaded tables.";
		}
		return string.Join("\n", smartEntryReport);
	}
}
