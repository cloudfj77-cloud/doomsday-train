using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LocSmartStringHelper
{
	private static readonly string[] tables = new string[14]
	{
		"UI", "Mechanic", "Dialogue", "PhoneDialogue", "ClientName", "Item", "Interactable", "NetworkUpgrade", "MainMenu", "NewGame",
		"PerkTable", "RepPerkTable", "Hint", "Tutorial"
	};

	private static readonly string[] smartMarkers = new string[2] { ":plural:", ":choose(" };

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		AsyncOperationHandle<LocalizationSettings> initializationOperation = LocalizationSettings.InitializationOperation;
		initializationOperation.Completed += delegate
		{
			ApplySmartFlags();
		};
		LocalizationSettings.SelectedLocaleChanged += delegate
		{
			ApplySmartFlags();
		};
	}

	public static void ApplySmartFlags()
	{
		string[] array = tables;
		foreach (string text in array)
		{
			AsyncOperationHandle<StringTable> tableAsync = LocalizationSettings.StringDatabase.GetTableAsync(text);
			tableAsync.Completed += delegate(AsyncOperationHandle<StringTable> op)
			{
				ApplyToTable(op.Result);
			};
		}
	}

	private static void ApplyToTable(StringTable table)
	{
		if (table == null)
		{
			return;
		}
		foreach (StringTableEntry value2 in table.Values)
		{
			if (value2.IsSmart || string.IsNullOrEmpty(value2.Value))
			{
				continue;
			}
			string[] array = smartMarkers;
			foreach (string value in array)
			{
				if (value2.Value.Contains(value))
				{
					value2.IsSmart = true;
					break;
				}
			}
		}
	}

	public static List<string> GetSmartEntryReport()
	{
		List<string> list = new List<string>();
		string[] array = tables;
		foreach (string text in array)
		{
			StringTable table = LocalizationSettings.StringDatabase.GetTable(text);
			if (table == null)
			{
				continue;
			}
			foreach (StringTableEntry value in table.Values)
			{
				if (value.IsSmart)
				{
					list.Add(text + " / " + value.Key);
				}
			}
		}
		return list;
	}
}
