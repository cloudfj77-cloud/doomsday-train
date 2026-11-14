using System;
using UnityEngine;

public static class TransitStyle
{
	public const float WINDOW_W = 1856f;

	public const float WINDOW_H = 1016f;

	public const float HEADER_H = 42f;

	public const float FRAME_Y = 40f;

	public const float HEADER_TITLE_X = 24f;

	public const float HEADER_TITLE_SIZE = 32f;

	public const float HEADER_TITLE_SIZE_MIN = 16f;

	public const float HEADER_TIP_W = 16f;

	public const float CLOSE_SIZE = 28f;

	public const float CLOSE_RIGHT = 7f;

	public const float FRAME_BORDER = 2f;

	public static readonly Color SHADE = Hex("292334", 0.8f);

	public static readonly Color FRAME_BG = Hex("292334");

	public static readonly Color FRAME_BORDER_COLOR = Hex("afada4");

	public static readonly Color HEADER_TEXT = Hex("292234");

	public const float LINES_X = 24f;

	public const float LINES_Y = 65f;

	public const float LINES_W = 1808f;

	public const float LINES_H = 832f;

	public const int SLOTS = 5;

	public const float CARD_EXTRA = 64f;

	public const float CARDS_X = 64f;

	public const float CARDS_Y = 202f;

	public const float CARD_W = 320f;

	public const float CARD_H = 448f;

	public const float CARD_STEP = 352f;

	public const float CARD_SHADOW_X = -8f;

	public const float CARD_SHADOW_Y = 8f;

	public const float CARD_SHADOW_ALPHA = 0.25f;

	public const int CARD_PLATE_BORDER = 6;

	public const float CARD_HEADER_H = 42f;

	public const float CARD_HEADER_PLATE_ALPHA = 0.05f;

	public const float CARD_HEADER_TEXT_X = 2f;

	public const float CARD_HEADER_SIZE = 24f;

	public const float CARD_HEADER_SIZE_MIN = 12f;

	public const float CARD_HEADER_UNAVAILABLE_ALPHA = 0.3f;

	public const float CARD_IMAGE_X = 12f;

	public const float CARD_IMAGE_Y = 54f;

	public const float CARD_IMAGE_W = 296f;

	public const float CARD_IMAGE_H = 168f;

	public const float CARD_STATUS_Y = 240f;

	public const float CARD_STATUS_H = 25f;

	public const float CARD_STATUS_SIZE = 18f;

	public const float CARD_DESC_X = 16f;

	public const float CARD_DESC_W = 288f;

	public const float CARD_DESC_SIZE = 18f;

	public const float CARD_DESC_SIZE_MIN = 12f;

	public const float CARD_EVENTS_Y = 240f;

	public const float CARD_EVENTS_BOTTOM = 16f;

	public const float CARD_EVENTS_H = 192f;

	public const float EVENT_GAP = 10f;

	public const float EVENT_LINE_GAP = 2f;

	public const float EVENT_BULLET_INDENT = 14f;

	public const string EVENT_BULLET = "•";

	public const float SCROLLBAR_X = 306f;

	public const float SCROLLBAR_W = 6f;

	public const float SCROLLBAR_TRACK_W = 2f;

	public const float SCROLL_SENSITIVITY = 40f;

	public static readonly Color CARD_PLATE = Hex("201e24");

	public static readonly Color CARD_HEADER_TEXT = Hex("d9d9d9");

	public static readonly Color CARD_IMAGE_PLATE = new Color(1f, 1f, 1f, 0.05f);

	public static readonly Color CARD_PLACEHOLDER_TEXT = new Color(1f, 1f, 1f, 0.1f);

	public const float CARD_PLACEHOLDER_SIZE = 24f;

	public static readonly Color CARD_DESC_TEXT = Hex("a8aea5");

	public static readonly Color STATUS_ATTENTION = Hex("dba873");

	public static readonly Color STATUS_UNAVAILABLE = Hex("cc4849");

	public static readonly Color EVENT_GOOD = Hex("9cdb73");

	public static readonly Color EVENT_BAD = STATUS_UNAVAILABLE;

	public static readonly Color EVENT_MIXED = STATUS_ATTENTION;

	public static readonly Color EVENT_DESCRIPTION_TEXT = CARD_DESC_TEXT;

	public static readonly Color EVENT_EFFECT_TEXT = Hex("d9d9d9");

	public static readonly Color EVENT_DURATION_TEXT = Hex("8abfbb");

	public const float TRACK_X = 64f;

	public const float TRACK_Y = 815f;

	public const float TRACK_W = 1728f;

	public const float TRACK_H = 16f;

	public static readonly Color TRACK = Hex("afada4", 0.8f);

	public const float TRACK_HOST_Y = 640f;

	public const float STATION_SIZE = 42f;

	public const float STATION_X = 108f;

	public const float STATION_Y = 801f;

	public const float CONNECTOR_X = 129f;

	public const float CONNECTOR_Y = 650f;

	public const float CONNECTOR_W = 133f;

	public const float CONNECTOR_H = 174f;

	public const float SEPARATOR_X = 64f;

	public const float SEPARATOR_Y = 915f;

	public const float SEPARATOR_W = 1728f;

	public const float SEPARATOR_H = 2f;

	public static readonly Color SEPARATOR = Hex("8abfbb", 0.1f);

	public const float BUTTON_X = 736f;

	public const float BUTTON_Y = 942f;

	public const float BUTTON_W = 384f;

	public const float BUTTON_H = 42f;

	public const float BUTTON_OUTER_INSET = 2f;

	public const float BUTTON_INNER_INSET = 6f;

	public const float BUTTON_TEXT_SIZE = 22f;

	public const float BUTTON_TEXT_SIZE_MIN = 12f;

	public const float BUTTON_DISABLED_ALPHA = 0.3f;

	public static readonly Color BUTTON_BG = Hex("292234");

	public static readonly Color BUTTON_BORDER = Hex("afada4");

	public static readonly Color BUTTON_BORDER_HOVER = Hex("fff09b");

	public static readonly Color BUTTON_INNER = Hex("afada4", 0.5f);

	public static readonly Color BUTTON_TEXT = Hex("afada4");

	public const float PAGE_ARROW_SIZE = 32f;

	public const float PAGE_ARROW_Y = 410f;

	public const float PAGE_ARROW_PREV_X = 16f;

	public const float PAGE_ARROW_NEXT_X = 1808f;

	public const float PAGE_ARROW_DISABLED_ALPHA = 0.3f;

	public static Color Hex(string rgb, float alpha = 1f)
	{
		int num = Convert.ToInt32(rgb.Substring(0, 2), 16);
		int num2 = Convert.ToInt32(rgb.Substring(2, 2), 16);
		return new Color(b: (float)Convert.ToInt32(rgb.Substring(4, 2), 16) / 255f, r: (float)num / 255f, g: (float)num2 / 255f, a: alpha);
	}
}
