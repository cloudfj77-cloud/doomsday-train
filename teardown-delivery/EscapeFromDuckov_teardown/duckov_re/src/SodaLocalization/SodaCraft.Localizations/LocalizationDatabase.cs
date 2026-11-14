using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SodaCraft.Localizations;

[CreateAssetMenu]
public class LocalizationDatabase : ScriptableObject
{
	private static LocalizationDatabase _instance;

	private List<LanguageSettings> _e;

	public static LocalizationDatabase Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<LocalizationDatabase>("LocalizationDatabase");
				if (_instance == null)
				{
					Debug.LogError("需要配置文件LocalizationDatabase。放在Resources文件夹里。");
				}
			}
			return _instance;
		}
	}

	private List<LanguageSettings> Entries
	{
		get
		{
			if (_e == null || _e.Count == 0)
			{
				_e = new List<LanguageSettings>();
				string text = Application.streamingAssetsPath + "/Localization";
				Debug.Log(text);
				foreach (string item in from e in Directory.GetFiles(text)
					where e.EndsWith(".csv")
					select e)
				{
					LanguageSettings languageSettings = LanguageSettings.CreateFromPath(item);
					if (languageSettings != null)
					{
						_e.Add(languageSettings);
					}
				}
			}
			return _e;
		}
	}

	public LanguageSettings GetEntry(SystemLanguage language)
	{
		return Entries.Find((LanguageSettings e) => e != null && e.Language == language);
	}

	internal LanguageSettings GetDefaultEntry()
	{
		return GetEntry(SystemLanguage.English);
	}

	public string[] GetLanguageDisplayNameList()
	{
		if (Entries == null)
		{
			return null;
		}
		return Entries.Select((LanguageSettings e) => e.GetDisplayName()).ToArray();
	}

	internal SystemLanguage GetLanguageByIndex(int index)
	{
		if (index < 0 || index >= Entries.Count)
		{
			return SystemLanguage.Unknown;
		}
		return Entries[index].Language;
	}
}
