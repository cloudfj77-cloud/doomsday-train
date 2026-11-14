using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Steamworks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocHelper
{
	private static readonly Regex leadingBullet = new Regex("^\\s*[-–—•](?!\\d)\\s*");

	private static readonly HashSet<string> missingKeyLogged = new HashSet<string>();

	private static Regex missingTranslationRegex;

	private static string missingTranslationRegexSource;

	public const string LOCALE_SAVE_KEY = "locale";

	private static readonly string[] AUTO_DETECT_LOCALES = new string[8] { "en", "ja", "de", "ru", "zh", "fr", "es", "pl" };

	private static string Get(string table, string key, object[] arguments)
	{
		string text;
		try
		{
			text = ((arguments != null && arguments.Length != 0) ? LocalizationSettings.StringDatabase.GetLocalizedString(table, key, arguments) : LocalizationSettings.StringDatabase.GetLocalizedString(table, key, null, FallbackBehavior.UseProjectSettings));
		}
		catch (Exception ex)
		{
			Debug.LogError("[LocHelper] " + table + " / " + key + " threw while formatting: " + ex.Message);
			text = null;
		}
		if (text != null)
		{
			return text;
		}
		string text2 = LocalizationSettings.StringDatabase.GetTable(table)?.GetEntry(key)?.Value;
		Debug.LogError("<color=orange>[LocHelper] LOCALIZATION FORMAT FAILED: " + table + " / " + key + " " + $"(locale {GetCurrentLocaleCode()}, {((arguments != null) ? arguments.Length : 0)} argument(s)).</color> " + "Showing the raw entry text instead. Check the entry's plural/choose syntax against docs/localization-plurals.md.");
		return text2 ?? key;
	}

	public static string GetLocalized(string table, string key)
	{
		return Get(table, key, null);
	}

	public static string GetLocalizedNewGame(string key)
	{
		return Get("NewGame", key, null);
	}

	public static string GetLocalizedMainMenu(string key, params object[] arguments)
	{
		return Get("MainMenu", key, arguments);
	}

	public static string GetLocalizedUI(string key, params object[] arguments)
	{
		return Get("UI", key, arguments);
	}

	public static bool HasLocalizedUI(string key)
	{
		return HasEntry("UI", key);
	}

	public static bool HasLocalizedNetworkUpgrade(string key)
	{
		return HasEntry("NetworkUpgrade", key);
	}

	private static bool HasEntry(string table, string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		return LocalizationSettings.StringDatabase.GetTable(table)?.GetEntry(key) != null;
	}

	public static string GetLocalizedOrDefault(string table, string key, string englishFallback, params object[] arguments)
	{
		if (HasEntry(table, key))
		{
			return Get(table, key, arguments);
		}
		if (missingKeyLogged.Add(table + "/" + key))
		{
			Debug.Log("[LocHelper] " + table + " table has no entry '" + key + "' yet, showing the English fallback (see docs/pending-localization.md).");
		}
		if (arguments != null && arguments.Length != 0)
		{
			try
			{
				return string.Format(englishFallback, arguments);
			}
			catch (FormatException)
			{
				return englishFallback;
			}
		}
		return englishFallback;
	}

	public static string GetLocalizedUIOrDefault(string key, string englishFallback, params object[] arguments)
	{
		return GetLocalizedOrDefault("UI", key, englishFallback, arguments);
	}

	public static string GetLocalizedMechanicOrDefault(string key, string englishFallback, params object[] arguments)
	{
		return GetLocalizedOrDefault("Mechanic", key, englishFallback, arguments);
	}

	public static string BulletLine(string text)
	{
		return GetLocalizedMechanic("mech_list_item", text);
	}

	public static string BulletLines(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		List<string> list = new List<string>();
		string[] array = text.Replace("\r\n", "\n").Split('\n');
		foreach (string input in array)
		{
			string text2 = leadingBullet.Replace(input, "").TrimEnd();
			if (text2.Length > 0)
			{
				list.Add(BulletLine(text2));
			}
		}
		return string.Join("\n", list);
	}

	public static string GetLocalizedInteractable(string key, params object[] arguments)
	{
		return Get("Interactable", key, arguments);
	}

	public static string GetLocalizedNetworkUpgrade(string key, params object[] arguments)
	{
		return Get("NetworkUpgrade", key, arguments);
	}

	public static string GetLocalizedName(string key)
	{
		return Get("ClientName", key, null);
	}

	public static string GetLocalizedDialoguePhone(string key, params object[] arguments)
	{
		return Get("PhoneDialogue", key, arguments);
	}

	public static string GetLocalizedClientName(string key)
	{
		return Get("ClientName", key, null);
	}

	public static string GetLocalizedDialogue(string key, params object[] arguments)
	{
		return Get("Dialogue", key, arguments);
	}

	public static string GetLocalizedItem(string key, params object[] arguments)
	{
		return Get("Item", key, arguments);
	}

	public static string GetLocalizedMechanic(string key, params object[] arguments)
	{
		return Get("Mechanic", key, arguments);
	}

	public static string GetLocalizedPerkTable(string key, params object[] arguments)
	{
		return Get("PerkTable", key, arguments);
	}

	public static string GetLocalizedRepPerkTable(string key, params object[] arguments)
	{
		return Get("RepPerkTable", key, arguments);
	}

	public static bool TryParseMissingTranslation(string text, out string table, out string key)
	{
		table = null;
		key = null;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		string text2 = LocalizationSettings.StringDatabase?.NoTranslationFoundMessage;
		if (string.IsNullOrEmpty(text2))
		{
			return false;
		}
		if (missingTranslationRegex == null || missingTranslationRegexSource != text2)
		{
			string text3 = Regex.Escape(text2);
			text3 = text3.Replace(Regex.Escape("{key}"), "(?<key>.+?)");
			text3 = text3.Replace(Regex.Escape("{table.TableCollectionName}"), "(?<table>.+?)");
			text3 = Regex.Replace(text3, "\\\\\\{[^}]*}", ".*?");
			missingTranslationRegex = new Regex("^" + text3 + "$", RegexOptions.Singleline);
			missingTranslationRegexSource = text2;
		}
		Match match = missingTranslationRegex.Match(text);
		if (!match.Success)
		{
			return false;
		}
		key = match.Groups["key"].Value;
		table = match.Groups["table"].Value;
		if (!string.IsNullOrEmpty(key))
		{
			return !string.IsNullOrEmpty(table);
		}
		return false;
	}

	public static string RelocalizeIfMissing(string text)
	{
		if (!TryParseMissingTranslation(text, out var table, out var key))
		{
			return text;
		}
		string text2 = Get(table, key, null);
		if (text2 != text)
		{
			Debug.Log("[LocHelper] Repaired saved missing translation " + table + " / " + key + " (locale " + GetCurrentLocaleCode() + ").");
		}
		return text2;
	}

	public static string GetCurrentLocaleCode()
	{
		Locale selectedLocale = LocalizationSettings.SelectedLocale;
		if (!(selectedLocale != null))
		{
			return "";
		}
		return selectedLocale.Identifier.Code;
	}

	public static string WrapInLocalizedQuotes(string text)
	{
		switch (GetCurrentLocaleCode())
		{
		case "es":
		case "it":
		case "ru":
		case "pl":
			return "«" + text + "»";
		case "fr":
			return "«\u00a0" + text + "\u00a0»";
		case "de":
			return "»" + text + "«";
		case "ja":
			return "「" + text + "」";
		case "ko":
		case "zh":
		case "zh-Hans":
			return "“" + text + "”";
		default:
			return "\"" + text + "\"";
		}
	}

	public static string ResolveStartupLocale()
	{
		try
		{
			if (ES3.KeyExists("locale"))
			{
				return ES3.LoadString("locale", "en");
			}
			if (ES3.KeyExists("welcomeVersion"))
			{
				return "en";
			}
			string text = DetectPlayerLocale();
			ES3.Save("locale", text);
			return text;
		}
		catch (Exception ex)
		{
			Debug.LogError("[LocHelper] Could not read the saved locale, using English: " + ex.Message);
			return "en";
		}
	}

	private static string DetectPlayerLocale()
	{
		string text = null;
		string text2 = "default";
		try
		{
			if (SteamManager.Initialized)
			{
				string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
				text = SteamLanguageToLocale(currentGameLanguage);
				text2 = "Steam '" + currentGameLanguage + "'";
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[LocHelper] Steam language unavailable: " + ex.Message);
		}
		if (text == null)
		{
			text = SystemLanguageToLocale(Application.systemLanguage);
			text2 = "OS " + Application.systemLanguage;
		}
		if (text == null || Array.IndexOf(AUTO_DETECT_LOCALES, text) < 0)
		{
			text = "en";
		}
		Debug.Log("[LocHelper] First launch: language from " + text2 + " -> " + text);
		return text;
	}

	private static string SteamLanguageToLocale(string steamLanguage)
	{
		switch (steamLanguage)
		{
		case "english":
			return "en";
		case "japanese":
			return "ja";
		case "german":
			return "de";
		case "russian":
			return "ru";
		case "schinese":
		case "tchinese":
			return "zh";
		case "spanish":
		case "latam":
			return "es";
		case "polish":
			return "pl";
		case "french":
			return "fr";
		case "brazilian":
			return "ptBR";
		case "koreana":
			return "ko";
		case "italian":
			return "it";
		default:
			return null;
		}
	}

	private static string SystemLanguageToLocale(SystemLanguage language)
	{
		switch (language)
		{
		case SystemLanguage.English:
			return "en";
		case SystemLanguage.Japanese:
			return "ja";
		case SystemLanguage.German:
			return "de";
		case SystemLanguage.Russian:
			return "ru";
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
		case SystemLanguage.ChineseTraditional:
			return "zh";
		case SystemLanguage.Spanish:
			return "es";
		case SystemLanguage.Polish:
			return "pl";
		case SystemLanguage.French:
			return "fr";
		case SystemLanguage.Portuguese:
			return "ptBR";
		case SystemLanguage.Korean:
			return "ko";
		case SystemLanguage.Italian:
			return "it";
		default:
			return null;
		}
	}

	public static IEnumerator SetLocale(string localeCode)
	{
		yield return LocalizationSettings.InitializationOperation;
		Locale locale = LocalizationSettings.AvailableLocales.Locales.Find((Locale l) => l.Identifier.Code == localeCode);
		if (locale != null)
		{
			LocalizationSettings.SelectedLocale = locale;
		}
		else
		{
			Debug.LogWarning("Locale not found: " + localeCode);
		}
	}
}
