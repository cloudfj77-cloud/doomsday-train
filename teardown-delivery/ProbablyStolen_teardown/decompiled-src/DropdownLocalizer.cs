using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DropdownLocalizer : MonoBehaviour
{
	public TMP_Dropdown dropdown;

	public string[] optionKeys;

	public string localizationTable;

	private void Start()
	{
		UpdateOptions();
	}

	public void UpdateOptions()
	{
		dropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		string[] array = optionKeys;
		foreach (string text in array)
		{
			string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(localizationTable, text, null, FallbackBehavior.UseProjectSettings);
			list.Add(new TMP_Dropdown.OptionData(localizedString));
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
