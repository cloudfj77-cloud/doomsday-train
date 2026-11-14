using System;
using System.Collections.Generic;
using UnityEngine;

namespace SodaCraft.Localizations;

public static class LocalizationManager
{
	public static Dictionary<string, string> overrideTexts = new Dictionary<string, string>();

	private static LocalizationDataModel _fallbackDataModel;

	public static bool Initialized { get; private set; }

	public static SystemLanguage CurrentLanguage { get; private set; }

	public static string CurrentLanguageDisplayName => GetDisplayName(CurrentLanguage);

	public static LocalizationDataModel DataModel { get; private set; }

	public static LocalizationDataModel FallbackDataModel
	{
		get
		{
			if (_fallbackDataModel == null)
			{
				LanguageSettings languageSettings = LocalizationDatabase.Instance.GetEntry(SystemLanguage.English);
				if (languageSettings == null)
				{
					languageSettings = LocalizationDatabase.Instance.GetDefaultEntry();
				}
				_fallbackDataModel = new LocalizationDataModel(languageSettings);
			}
			return _fallbackDataModel;
		}
	}

	public static event Action<SystemLanguage> OnSetLanguage;

	public static void SetOverrideText(string key, string value)
	{
		overrideTexts[key] = value;
	}

	public static bool TryGetOverrideText(string key, out string value)
	{
		return overrideTexts.TryGetValue(key, out value);
	}

	public static bool RemoveOverrideText(string key)
	{
		return overrideTexts.Remove(key);
	}

	public static void SetLanguage(SystemLanguage language)
	{
		LanguageSettings languageSettings = LocalizationDatabase.Instance.GetEntry(language);
		if (languageSettings == null)
		{
			languageSettings = LocalizationDatabase.Instance.GetDefaultEntry();
		}
		CurrentLanguage = languageSettings.Language;
		DataModel = new LocalizationDataModel(languageSettings);
		Initialized = true;
		PlayerPrefs.SetInt("language", (int)language);
		LocalizationManager.OnSetLanguage?.Invoke(language);
	}

	public static string GetDisplayName(SystemLanguage language)
	{
		LocalizationDatabase instance = LocalizationDatabase.Instance;
		if (instance == null)
		{
			return language.ToString() + "?";
		}
		LanguageSettings entry = instance.GetEntry(language);
		if (entry == null)
		{
			return language.ToString() + "?";
		}
		return entry.GetDisplayName();
	}

	private static void Initialize()
	{
		SetLanguage((SystemLanguage)PlayerPrefs.GetInt("language", (int)Application.systemLanguage));
	}

	public static string GetPlainText(string key)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return "";
		}
		key = key.Trim();
		if (TryGetOverrideText(key, out var value))
		{
			return value;
		}
		if (DataModel == null)
		{
			Initialize();
		}
		string text = DataModel.GetPlainText(key);
		if (text == null)
		{
			text = FallbackDataModel.GetPlainText(key);
		}
		if (text == null)
		{
			text = "*" + key + "*";
		}
		return text;
	}

	public static string ToPlainText(this string key)
	{
		return GetPlainText(key);
	}

	public static void SetLanguage(string name)
	{
		object result;
		if (LocalizationDatabase.Instance == null)
		{
			Debug.LogError("未配置本地化数据库");
		}
		else if (!Enum.TryParse(typeof(SystemLanguage), name, out result))
		{
			Debug.LogError("语言解析失败 " + name);
		}
		else if (!(result is SystemLanguage language))
		{
			Debug.LogError("语言解析失败 " + name);
		}
		else
		{
			SetLanguage(language);
		}
	}

	public static void SetLanguage(int index)
	{
		LocalizationDatabase instance = LocalizationDatabase.Instance;
		if (instance == null)
		{
			Debug.LogError("未配置本地化数据库");
		}
		else
		{
			SetLanguage(instance.GetLanguageByIndex(index));
		}
	}

	internal static void SetLanguage()
	{
		SetLanguage(CurrentLanguage);
	}
}
