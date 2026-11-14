using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class FontLoader : MonoBehaviour
{
	public enum FontRole
	{
		Noto_Sans,
		Noto_Serif,
		Rubik,
		Caveat,
		Lubrifont,
		Tektur,
		Tektur_Bold,
		Noto_Sans_Normal,
		Noto_Serif_Medium,
		Noto_Sans_Bold,
		Noto_Serif_Bold
	}

	public TMP_FontAsset westernFont;

	public TMP_FontAsset japaneseFont;

	public TMP_FontAsset koreanFont;

	public TMP_FontAsset simplifiedChineseFont;

	public TMP_FontAsset simplifiedChineseHandwrittenFont;

	public TMP_FontAsset koreanHandwrittenFont;

	public TMP_FontAsset koreanScifiFont;

	public TMP_FontAsset japaneseHandwrittenFont;

	public TMP_FontAsset japaneseScifiFont;

	public TMP_FontAsset notoSans;

	public TMP_FontAsset notoSans_Normal;

	public TMP_FontAsset notoSerif;

	public TMP_FontAsset notoSerif_Medium;

	public TMP_FontAsset rubik;

	public TMP_FontAsset caveat;

	public TMP_FontAsset lubrifont;

	public TMP_FontAsset tektur;

	public TMP_FontAsset tektur_Bold;

	public TMP_FontAsset notoSans_Bold;

	public TMP_FontAsset notoSerif_Bold;

	public TMP_FontAsset polishDisplayFont;

	public static FontLoader Instance { get; private set; }

	private void Awake()
	{
		Debug.Log("FontLoader Awake");
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public TMP_FontAsset GetFontByRole(FontRole role, Locale locale)
	{
		string code = locale.Identifier.Code;
		if (code == "pl" && role == FontRole.Lubrifont && polishDisplayFont != null)
		{
			return polishDisplayFont;
		}
		switch (code)
		{
		case "en":
		case "fr":
		case "de":
		case "ptBR":
		case "it":
		case "es":
		case "ru":
		case "pl":
			return role switch
			{
				FontRole.Noto_Sans => notoSans, 
				FontRole.Noto_Serif => notoSerif, 
				FontRole.Rubik => rubik, 
				FontRole.Caveat => caveat, 
				FontRole.Lubrifont => lubrifont, 
				FontRole.Tektur => tektur, 
				FontRole.Tektur_Bold => tektur_Bold, 
				FontRole.Noto_Sans_Normal => notoSans_Normal, 
				FontRole.Noto_Serif_Medium => notoSerif_Medium, 
				FontRole.Noto_Sans_Bold => notoSans_Bold, 
				FontRole.Noto_Serif_Bold => notoSerif_Bold, 
				_ => notoSans, 
			};
		case "ja":
			return role switch
			{
				FontRole.Caveat => japaneseHandwrittenFont, 
				FontRole.Tektur => japaneseScifiFont, 
				FontRole.Tektur_Bold => japaneseScifiFont, 
				FontRole.Lubrifont => japaneseScifiFont, 
				_ => japaneseFont, 
			};
		case "ko":
			return role switch
			{
				FontRole.Caveat => koreanHandwrittenFont, 
				FontRole.Tektur => koreanScifiFont, 
				FontRole.Tektur_Bold => koreanScifiFont, 
				FontRole.Lubrifont => koreanScifiFont, 
				_ => koreanFont, 
			};
		case "zh-Hans":
		case "zh":
			return role switch
			{
				FontRole.Caveat => simplifiedChineseHandwrittenFont, 
				FontRole.Tektur => lubrifont, 
				FontRole.Tektur_Bold => lubrifont, 
				FontRole.Lubrifont => lubrifont, 
				_ => simplifiedChineseFont, 
			};
		default:
			return notoSans;
		}
	}
}
