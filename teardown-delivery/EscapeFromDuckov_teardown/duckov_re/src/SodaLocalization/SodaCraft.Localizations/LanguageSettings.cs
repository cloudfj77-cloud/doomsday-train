using System;
using System.IO;
using MiniLocalizor;
using UnityEngine;

namespace SodaCraft.Localizations;

[Serializable]
public class LanguageSettings
{
	[SerializeField]
	private SystemLanguage language = SystemLanguage.Unknown;

	private CSVFileLocalizor _provider;

	public string path => Path.Combine(Application.streamingAssetsPath, "Localization/" + language.ToString() + ".csv");

	public ILocalizationProvider Provider
	{
		get
		{
			if (_provider == null)
			{
				LoadProvider();
			}
			return _provider;
		}
	}

	public SystemLanguage Language => language;

	private void LoadProvider()
	{
		_provider = new CSVFileLocalizor(path);
	}

	public string GetPlainText(string key)
	{
		key = key.Trim();
		return Provider.Get(key);
	}

	public string GetDisplayName()
	{
		return GetPlainText("language_name");
	}

	internal void Reinitialize()
	{
		LoadProvider();
	}

	public static LanguageSettings CreateFromPath(string path)
	{
		if (path.EndsWith(".xlsx"))
		{
			Debug.LogError("Wrong extension");
		}
		if (!Enum.TryParse<SystemLanguage>(Path.GetFileNameWithoutExtension(path), out var result))
		{
			return null;
		}
		return new LanguageSettings
		{
			language = result
		};
	}
}
