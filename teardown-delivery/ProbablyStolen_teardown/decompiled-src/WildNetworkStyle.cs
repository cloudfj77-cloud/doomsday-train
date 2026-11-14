using System;
using UnityEngine;

public static class WildNetworkStyle
{
	public const float WINDOW_W = 1664f;

	public const float WINDOW_H = 1016f;

	public const float HEADER_H = 42f;

	public const float FRAME_Y = 40f;

	public const float HEADER_TITLE_X = 24f;

	public const float HEADER_TITLE_SIZE = 32f;

	public const float CLOSE_SIZE = 28f;

	public const float CLOSE_RIGHT = 7f;

	public const float CLOSE_HOVER_SIZE = 32f;

	public const float HEADER_TIP_W = 16f;

	public const float INFO_X = 24f;

	public const float INFO_Y = 128f;

	public const float INFO_W = 448f;

	public const float INFO_H = 864f;

	public const float INFO_TITLE_H = 50f;

	public const float INFO_TITLE_SIZE = 42f;

	public const float INFO_TITLE_SIZE_MIN = 22f;

	public const float TEXT_SIZE_MIN = 12f;

	public const float INFO_SUBTITLE_Y = 54f;

	public const float INFO_SUBTITLE_SIZE = 24f;

	public const float INFO_DESC_Y = 111f;

	public const float INFO_DESC_SIZE = 20f;

	public const float INFO_STATS_Y = 710f;

	public const float INFO_STAT_ROW_H = 42f;

	public const float INFO_STAT_ROW_GAP = 4f;

	public const float INFO_STAT_PAD_X = 16f;

	public const float INFO_STAT_LABEL_SIZE = 20f;

	public const float INFO_STAT_VALUE_SIZE = 24f;

	public const float INFO_BUTTON_Y = 822f;

	public const float SEPARATOR_V_X = 488f;

	public const float SEPARATOR_V_Y = 66f;

	public const float SEPARATOR_V_H = 926f;

	public const float SEPARATOR_H_X = 506f;

	public const float SEPARATOR_H_Y = 106f;

	public const float SEPARATOR_H_W = 1118f;

	public const float SEPARATOR_THICKNESS = 2f;

	public const float TABS_X = 536f;

	public const float TABS_Y = 64f;

	public const float TABS_W = 1060f;

	public const float TAB_W = 256f;

	public const float TAB_H = 42f;

	public const float TAB_GAP = 12f;

	public const float TAB_TEXT_SIZE = 22f;

	public const float GRID_X = 506f;

	public const float GRID_Y = 128f;

	public const float GRID_W = 1118f;

	public const float GRID_H = 864f;

	public const float SCROLLBAR_X = 1638f;

	public const float SCROLLBAR_W = 6f;

	public const float SCROLLBAR_TRACK_W = 2f;

	public const float SCROLL_SENSITIVITY = 40f;

	public const int CARD_COLUMNS = 3;

	public const float CARD_W = 362f;

	public const float RECOMMENDED_CARD_W = 1118f;

	public const float CARD_GAP_X = 16f;

	public const float CARD_GAP_Y = 16f;

	public const float CARD_HEADER_H = 42f;

	public const float CARD_TITLE_SIZE = 24f;

	public const float CARD_SLOTS_X = 31f;

	public const float CARD_SLOTS_Y = 74f;

	public const float CARD_BOTTOM_PAD = 34f;

	public const float SLOT_SIZE = 84f;

	public const float SLOT_STEP_X = 108f;

	public const float SLOT_STEP_Y = 116f;

	public const float SLOT_ICON_SIZE = 72f;

	public const float SLOT_ICON_INSET = 6f;

	public const float SLOT_INSET_BORDER = 4f;

	public const float SLOT_PADLOCK_SIZE = 32f;

	public const float PRICE_TAG_W = 56f;

	public const float PRICE_TAG_H = 22f;

	public const float PRICE_TAG_Y = 72f;

	public const float PRICE_TAG_TEXT_SIZE = 20f;

	public const float PRICE_TAG_TEXT_PAD = 12f;

	public const float CONNECTOR_W = 24f;

	public const float CONNECTOR_H = 4f;

	public const float BUTTON_W = 320f;

	public const float BUTTON_H = 42f;

	public const float BUTTON_BORDER = 2f;

	public const float BUTTON_TEXT_SIZE = 22f;

	public const float BUTTON_DIVIDER_H = 16f;

	public const int FRAME4_BORDER = 4;

	public const int FRAME2_BORDER = 2;

	public const int PRICE_TAG_BORDER = 6;

	public static readonly Color SHADE = Hex("292334", 0.8f);

	public static readonly Color HEADER_TEXT = Hex("292234");

	public static readonly Color SEPARATOR_H = Hex("8abfbb", 0.3f);

	public static readonly Color SEPARATOR_V = Hex("8abfbb", 0.1f);

	public static readonly Color INFO_TITLE = Color.white;

	public static readonly Color INFO_SUBTITLE = Hex("88948a");

	public static readonly Color INFO_DESC = Hex("a8aea5");

	public static readonly Color INFO_COOLDOWN = Hex("b1efd0");

	public static readonly Color INFO_WARNING = Hex("cc4849");

	public static readonly Color INFO_STAT_BG = new Color(1f / 17f, 1f / 17f, 1f / 17f, 0.2f);

	public static readonly Color INFO_STAT_LABEL = Hex("c4c4b0");

	public static readonly Color INFO_STAT_VALUE = Color.white;

	public static readonly Color TAB_TEXT = Hex("b0bdc5");

	public static readonly Color TAB_TEXT_HOVER = Hex("d9d9d9");

	public static readonly Color CARD_TITLE = Hex("d9d9d9");

	public static readonly Color RECOMMENDED_PLATE = Hex("fff09b");

	public static readonly Color RECOMMENDED_TITLE = Hex("fff09b");

	public static readonly Color SLOT_ICON_BG = Hex("292234");

	public static readonly Color SLOT_EMPTY_ALPHA = new Color(1f, 1f, 1f, 0.2f);

	public const float SLOT_ICON_ALPHA_LOCKED = 0.5f;

	public const float SLOT_ICON_ALPHA_PURCHASED = 0.7f;

	public static readonly Color PRICE_TAG_NORMAL_TEXT = Hex("292234");

	public static readonly Color PRICE_TAG_UNAVAILABLE_TEXT = Hex("d5d2b8");

	public static readonly Color BUTTON_BG = Hex("292234");

	public static readonly Color BUTTON_BORDER_COLOR = Hex("afada4");

	public static readonly Color BUTTON_BORDER_HOVER = Hex("fff09b");

	public static readonly Color BUTTON_INNER = Hex("afada4", 0.5f);

	public static readonly Color BUTTON_INNER_UNAVAILABLE = Hex("cc4849", 0.5f);

	public static readonly Color BUTTON_TEXT = Hex("afada4");

	public static readonly Color BUTTON_TEXT_UNAVAILABLE = Hex("cc4849");

	public static readonly Color BUTTON_DIVIDER = Hex("afada4", 0.5f);

	public static float CardHeight(int rows)
	{
		return 74f + (float)rows * 116f - 32f + 34f;
	}

	public static Color Hex(string rgb, float alpha = 1f)
	{
		int num = Convert.ToInt32(rgb.Substring(0, 2), 16);
		int num2 = Convert.ToInt32(rgb.Substring(2, 2), 16);
		return new Color(b: (float)Convert.ToInt32(rgb.Substring(4, 2), 16) / 255f, r: (float)num / 255f, g: (float)num2 / 255f, a: alpha);
	}

	public static Color WithAlpha(Color color, float alpha)
	{
		color.a = alpha;
		return color;
	}
}
