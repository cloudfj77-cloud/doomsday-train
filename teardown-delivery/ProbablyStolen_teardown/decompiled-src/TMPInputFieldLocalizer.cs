using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(TMP_InputField))]
public class TMPInputFieldLocalizer : MonoBehaviour
{
	private TMP_InputField inputField;

	public FontLoader.FontRole fontRole;

	private void Awake()
	{
		inputField = GetComponent<TMP_InputField>();
	}

	protected virtual void OnLocaleChanged(Locale newLocale)
	{
		if (inputField == null || newLocale == null)
		{
			return;
		}
		if (FontLoader.Instance == null)
		{
			Debug.LogError("TMPInputFieldLocalizer: FontLoader.Instance is missing in the scene.");
			return;
		}
		TMP_FontAsset fontByRole = FontLoader.Instance.GetFontByRole(fontRole, newLocale);
		if (!(fontByRole == null))
		{
			inputField.fontAsset = fontByRole;
		}
	}

	protected virtual void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
		OnLocaleChanged(LocalizationSettings.SelectedLocale);
	}

	protected virtual void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}
}
