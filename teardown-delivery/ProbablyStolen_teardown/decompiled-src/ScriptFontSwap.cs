using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ScriptFontSwap : MonoBehaviour
{
	private TextMeshProUGUI tmp;

	private TMP_FontAsset scriptFont;

	public static void Apply(TextMeshProUGUI tmp, string text)
	{
		if (!(tmp == null))
		{
			tmp.text = text;
			ScriptFontSwap scriptFontSwap = tmp.GetComponent<ScriptFontSwap>();
			if (scriptFontSwap == null)
			{
				scriptFontSwap = tmp.gameObject.AddComponent<ScriptFontSwap>();
			}
			scriptFontSwap.tmp = tmp;
			scriptFontSwap.scriptFont = FontForText(text);
			scriptFontSwap.ApplyFont();
		}
	}

	public static TMP_FontAsset FontForText(string text)
	{
		if (string.IsNullOrEmpty(text) || FontLoader.Instance == null)
		{
			return null;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (char c in text)
		{
			if ((c >= '가' && c <= '\ud7af') || (c >= 'ᄀ' && c <= 'ᇿ') || (c >= '\u3130' && c <= '\u318f'))
			{
				flag3 = true;
			}
			else if ((c >= '\u3040' && c <= 'ヿ') || (c >= 'ㇰ' && c <= 'ㇿ'))
			{
				flag2 = true;
			}
			else if ((c >= '一' && c <= '鿿') || (c >= '㐀' && c <= '䶿') || (c >= '豈' && c <= '\ufaff') || (c >= '\u3000' && c <= '〿') || (c >= '\uff00' && c <= '\uffef') || char.IsSurrogate(c))
			{
				flag = true;
			}
		}
		if (flag3)
		{
			return FontLoader.Instance.koreanFont;
		}
		if (flag2)
		{
			return FontLoader.Instance.japaneseFont;
		}
		if (flag)
		{
			return FontLoader.Instance.simplifiedChineseFont;
		}
		return null;
	}

	private void ApplyFont()
	{
		if (tmp == null)
		{
			return;
		}
		if (scriptFont != null)
		{
			tmp.font = scriptFont;
		}
		else if (!(FontLoader.Instance == null) && !(LocalizationSettings.SelectedLocale == null))
		{
			TMPLocalizer component = GetComponent<TMPLocalizer>();
			FontLoader.FontRole role = ((component != null) ? component.fontRole : FontLoader.FontRole.Noto_Sans);
			TMP_FontAsset fontByRole = FontLoader.Instance.GetFontByRole(role, LocalizationSettings.SelectedLocale);
			if (fontByRole != null)
			{
				tmp.font = fontByRole;
			}
		}
	}

	private void OnLocaleChanged(Locale locale)
	{
		ApplyFont();
	}

	private void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
		if (scriptFont != null)
		{
			ApplyFont();
		}
	}

	private void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}
}
