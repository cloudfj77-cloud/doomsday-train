using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ResolutionLocalizer : MonoBehaviour
{
	public TMP_Dropdown dropdown;

	private string[] optionKeys = new string[7] { "1920x1080", "960 x 540", "3840 x 2160", "1280 x 720", "1366 x 768", "1600 x 900", "2560 x 1440" };

	private string locKey = "option_graphic_recommended";

	private void Start()
	{
		UpdateOptions();
	}

	public void UpdateOptions()
	{
		dropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < optionKeys.Length; i++)
		{
			if (i <= 2)
			{
				string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("UI", locKey, null, FallbackBehavior.UseProjectSettings);
				list.Add(new TMP_Dropdown.OptionData(optionKeys[i] + " (" + localizedString + ")"));
			}
			else
			{
				list.Add(new TMP_Dropdown.OptionData(optionKeys[i]));
			}
		}
		dropdown.AddOptions(list);
	}

	private void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}

	private void OnLocaleChanged(Locale locale)
	{
		UpdateOptions();
	}
}
