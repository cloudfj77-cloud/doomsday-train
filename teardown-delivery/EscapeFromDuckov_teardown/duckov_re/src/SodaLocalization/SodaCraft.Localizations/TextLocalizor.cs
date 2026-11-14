using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SodaCraft.Localizations;

public class TextLocalizor : MonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private TMP_Text tmpText;

	[LocalizationKey("UIText")]
	[SerializeField]
	private string key;

	public string Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
			Refresh();
		}
	}

	private void Awake()
	{
		LocalizationManager.OnSetLanguage += OnSetLanguage;
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void OnDestroy()
	{
		LocalizationManager.OnSetLanguage -= OnSetLanguage;
	}

	private void OnSetLanguage(SystemLanguage language)
	{
		Refresh();
	}

	private void Refresh()
	{
		RefreshReferences();
		RefreshTexts();
	}

	private void RefreshDataModel()
	{
		LocalizationManager.SetLanguage();
	}

	private void RefreshReferences()
	{
		bool flag = string.IsNullOrEmpty(key);
		if (text == null)
		{
			text = GetComponent<Text>();
			if (flag && (bool)text)
			{
				key = text.text;
			}
		}
		if (tmpText == null)
		{
			tmpText = GetComponent<TMP_Text>();
			if (flag && (bool)tmpText)
			{
				key = tmpText.text;
			}
		}
	}

	private void RefreshTexts()
	{
		string text = key.ToPlainText();
		if (this.text != null)
		{
			this.text.text = text;
		}
		if (tmpText != null)
		{
			tmpText.text = text;
		}
	}

	private void OnValidate()
	{
		Refresh();
	}
}
