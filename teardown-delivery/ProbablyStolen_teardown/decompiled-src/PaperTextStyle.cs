using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class PaperTextStyle
{
	public const string HEADER = "header";

	public const string TITLE = "title";

	public const string SECTION = "section";

	public const string BODY = "body";

	public const string CAPTION = "caption";

	public const string LABEL = "label";

	public const string SERIF_TITLE = "serif_title";

	public const string HANDWRITTEN = "handwritten";

	public const string PAGE_NUMBER = "page_number";

	public const string STAMP = "stamp";

	public const string BUTTON = "button";

	public const string META_HEADER = "meta_header";

	public const string META_BODY = "meta_body";

	public const string META_WARNING = "meta_warning";

	public string id;

	public FontLoader.FontRole role;

	public float size;

	public Color color;

	public string description;

	public FontStyles fontStyle;

	private static readonly Color INK = new Color(0.13f, 0.11f, 0.1f, 1f);

	private static readonly Color RED = new Color(0.62f, 0.12f, 0.1f, 1f);

	private static readonly Color BUTTON_TEXT = Color.white;

	private static readonly Color META_HEADER_TEXT = new Color(0.8196f, 0.7843f, 0.6471f, 1f);

	private static readonly Color META_BODY_TEXT = new Color(0.6588f, 0.6824f, 0.6471f, 1f);

	private static readonly Color META_WARNING_TEXT = new Color(0.8f, 0.2824f, 0.2863f, 1f);

	private static readonly List<PaperTextStyle> all = new List<PaperTextStyle>
	{
		new PaperTextStyle("header", FontLoader.FontRole.Tektur, 12.2f, INK, "Commission stamp header (Tektur)"),
		new PaperTextStyle("title", FontLoader.FontRole.Noto_Sans, 11.5f, INK, "Page title (Noto Sans Medium)"),
		new PaperTextStyle("section", FontLoader.FontRole.Noto_Serif, 9f, INK, "Section heading (Noto Serif)"),
		new PaperTextStyle("body", FontLoader.FontRole.Noto_Serif, 7.5f, INK, "Paragraph (Noto Serif)"),
		new PaperTextStyle("caption", FontLoader.FontRole.Noto_Sans_Bold, 6f, INK, "Illustration caption (Noto Sans Bold)"),
		new PaperTextStyle("label", FontLoader.FontRole.Noto_Sans_Bold, 7.5f, INK, "Short field label (Noto Sans Bold)"),
		new PaperTextStyle("serif_title", FontLoader.FontRole.Noto_Serif_Bold, 17.5f, INK, "Sheet title (Noto Serif Bold)"),
		new PaperTextStyle("handwritten", FontLoader.FontRole.Caveat, 12f, INK, "Handwriting (Caveat)"),
		new PaperTextStyle("page_number", FontLoader.FontRole.Caveat, 17.5f, INK, "Page indicator (Caveat)"),
		new PaperTextStyle("stamp", FontLoader.FontRole.Lubrifont, 16f, RED, "Red stamp (Lubrifont)"),
		new PaperTextStyle("button", FontLoader.FontRole.Noto_Sans, 11.5f, BUTTON_TEXT, "Text button label (Noto Sans Medium)"),
		new PaperTextStyle("meta_header", FontLoader.FontRole.Noto_Sans, 30f, META_HEADER_TEXT, "Meta panel header (Noto Sans Medium bold, the options look)", FontStyles.Bold),
		new PaperTextStyle("meta_body", FontLoader.FontRole.Noto_Sans, 20f, META_BODY_TEXT, "Meta panel body text (Noto Sans Medium)"),
		new PaperTextStyle("meta_warning", FontLoader.FontRole.Noto_Sans, 18f, META_WARNING_TEXT, "Meta panel warning text (red)")
	};

	private static readonly HashSet<string> warned = new HashSet<string>();

	public static IReadOnlyList<PaperTextStyle> All => all;

	private PaperTextStyle(string id, FontLoader.FontRole role, float size, Color color, string description, FontStyles fontStyle = FontStyles.Normal)
	{
		this.id = id;
		this.role = role;
		this.size = size;
		this.color = color;
		this.description = description;
		this.fontStyle = fontStyle;
	}

	public static List<string> Ids()
	{
		List<string> list = new List<string>(all.Count);
		foreach (PaperTextStyle item in all)
		{
			list.Add(item.id);
		}
		return list;
	}

	public static PaperTextStyle Get(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			foreach (PaperTextStyle item in all)
			{
				if (item.id == id)
				{
					return item;
				}
			}
			if (warned.Add(id))
			{
				Debug.LogWarning("[PaperUI] Unknown text style '" + id + "', using body");
			}
		}
		return all[3];
	}

	public static bool Exists(string id)
	{
		foreach (PaperTextStyle item in all)
		{
			if (item.id == id)
			{
				return true;
			}
		}
		return false;
	}

	public TMP_FontAsset Font()
	{
		if (FontLoader.Instance == null)
		{
			return null;
		}
		Locale selectedLocale = LocalizationSettings.SelectedLocale;
		if (!(selectedLocale != null))
		{
			return null;
		}
		return FontLoader.Instance.GetFontByRole(role, selectedLocale);
	}

	public static string GuessFromFont(TMP_FontAsset font, float fontSize)
	{
		if (font == null)
		{
			return "body";
		}
		string text = font.name.ToLowerInvariant();
		if (text.Contains("tektur"))
		{
			return "header";
		}
		if (text.Contains("caveat"))
		{
			if (!(fontSize >= 25f))
			{
				return "handwritten";
			}
			return "page_number";
		}
		if (text.Contains("lcd") || text.Contains("lubrifont"))
		{
			return "stamp";
		}
		if (text.Contains("notoserif") || text.Contains("noto serif") || text.Contains("serif"))
		{
			if (text.Contains("bold"))
			{
				return "serif_title";
			}
			if (!(fontSize >= 17f))
			{
				return "body";
			}
			return "section";
		}
		if (text.Contains("bold"))
		{
			if (!(fontSize <= 13f))
			{
				return "label";
			}
			return "caption";
		}
		return "title";
	}

	public static TextAlignmentOptions Alignment(string align, string valign)
	{
		bool flag = align == "center";
		bool flag2 = align == "right";
		bool num = valign == "middle";
		bool flag3 = valign == "bottom";
		if (num)
		{
			if (!flag)
			{
				if (!flag2)
				{
					return TextAlignmentOptions.Left;
				}
				return TextAlignmentOptions.Right;
			}
			return TextAlignmentOptions.Center;
		}
		if (flag3)
		{
			if (!flag)
			{
				if (!flag2)
				{
					return TextAlignmentOptions.BottomLeft;
				}
				return TextAlignmentOptions.BottomRight;
			}
			return TextAlignmentOptions.Bottom;
		}
		if (!flag)
		{
			if (!flag2)
			{
				return TextAlignmentOptions.TopLeft;
			}
			return TextAlignmentOptions.TopRight;
		}
		return TextAlignmentOptions.Top;
	}

	public static Color ParseColor(string hex, Color fallback)
	{
		if (string.IsNullOrEmpty(hex))
		{
			return fallback;
		}
		if (!ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : ("#" + hex), out var result))
		{
			return fallback;
		}
		return result;
	}

	public static string ToHex(Color color)
	{
		return "#" + ((color.a < 0.999f) ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color));
	}
}
