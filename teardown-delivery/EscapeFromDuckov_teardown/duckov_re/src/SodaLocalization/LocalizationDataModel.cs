using SodaCraft.Localizations;

public class LocalizationDataModel
{
	internal LanguageSettings settings;

	public string DisplayName
	{
		get
		{
			if (settings == null)
			{
				return "?";
			}
			return settings.GetDisplayName();
		}
	}

	public LocalizationDataModel(LanguageSettings settings)
	{
		this.settings = settings;
		settings.Reinitialize();
	}

	public string GetPlainText(string key)
	{
		return settings.GetPlainText(key);
	}
}
