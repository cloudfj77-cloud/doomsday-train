using System;
using UnityEngine;

public static class TabletStyle
{
	public static readonly Color INK = Hex("414554");

	public static readonly Color SCREEN = Hex("d1c8a5");

	public static readonly Color INK_HOVER = Hex("6c7182");

	public static readonly Color GREY_FACE = Hex("6e6d6b");

	public static readonly Color GREEN = Hex("388156");

	public static readonly Color RED = Hex("cc4849");

	public static readonly Color BARGAIN_RED = Hex("9c3738");

	public static readonly Color SUB_HEADER_TEXT = Hex("d1c8a5");

	public static readonly Color TAG_VARIETY_BG = Hex("aa77c4");

	public static readonly Color TAG_VARIETY_TEXT = Hex("ddedf7");

	public static readonly Color TAG_RED_TEXT = Hex("dba873");

	public static readonly Color TAG_INTEREST_BG = Hex("404040");

	public static readonly Color TAG_INTEREST_TEXT = Hex("9cdb73");

	public static readonly Color ROW_SEPARATOR = Hex("414554", 0.1f);

	public static readonly Color ROW_SELECTED_BG = Hex("414554", 0.15f);

	public static readonly Color SCROLL_TRACK = Hex("414554", 0.2f);

	public const float REFUSED_ALPHA = 0.3f;

	public const float BUTTON_DISABLED_ALPHA = 0.3f;

	public const float SMALL_BUTTON_DISABLED_ALPHA = 0.5f;

	public const float ARROW_DISABLED_ALPHA = 0.3f;

	public const float SCANLINES_ALPHA = 0.03f;

	public const float PANEL_W = 644f;

	public const float PANEL_H = 818f;

	public const float ART_PPU_MULTIPLIER = 0.5f;

	public static readonly Vector2 NEGOTIATION_POSITION = new Vector2(550f, 0f);

	public static readonly Vector2 END_OF_DAY_POSITION = Vector2.zero;

	public const float SCREEN_X = 32f;

	public const float SCREEN_Y = 54f;

	public const float SCREEN_W = 562f;

	public const float SCREEN_H = 710f;

	public const float SCREEN_PAD = 16f;

	public const float INNER_X = 48f;

	public const float INNER_Y = 70f;

	public const float INNER_W = 530f;

	public const float CONTROL_SIZE = 32f;

	public const float CONTROL_Y = 8f;

	public const float CONTROL_GAP = 4f;

	public const float CONTROL_CLOSE_X = 558f;

	public const float CONTROL_BARGAIN_X = 522f;

	public const float CONTROL_RESIZE_X = 486f;

	public const float CONTROL_ICON_PPU_MULTIPLIER = 0.5f;

	public const float CLIENT_HEADER_Y = 70f;

	public const float CLIENT_HEADER_H = 52f;

	public const float CLIENT_NAME_H = 26f;

	public const float CLIENT_NAME_SIZE = 22f;

	public const float CLIENT_NAME_SIZE_MIN = 14f;

	public const float CLIENT_NAME_ALONE_Y = 13f;

	public const float CLIENT_SUBTITLE_Y = 30f;

	public const float CLIENT_SUBTITLE_H = 22f;

	public const float CLIENT_SUBTITLE_SIZE = 16f;

	public const float SECTION_X = 48f;

	public const float SECTION_Y = 138f;

	public const float SECTION_H = 552f;

	public const float END_SECTION_Y = 70f;

	public const float END_SECTION_H = 620f;

	public const float FRAME_X = 46f;

	public const float FRAME_Y = 138f;

	public const float FRAME_W = 534f;

	public const float FRAME_H = 554f;

	public const float END_FRAME_Y = 68f;

	public const float END_FRAME_H = 624f;

	public const int FRAME_NOTCHED_BORDER = 6;

	public const float SECTION_HEADER_H = 38f;

	public const float SECTION_HEADER_SIZE = 24f;

	public const float SECTION_HEADER_SIZE_MIN = 14f;

	public const float BLOCK_GAP = 16f;

	public const float SEPARATOR_H = 2f;

	public const float SEPARATOR_TO_BODY = 18f;

	public const float INFO_Y = 54f;

	public const float BODY_BOTTOM = 432f;

	public const float SEPARATOR_BOTTOM_Y = 448f;

	public const float FOOTER_Y = 466f;

	public const float FOOTER_H = 70f;

	public const float END_BODY_BOTTOM = 500f;

	public const float END_INFO_H = 22f;

	public const float END_INFO_SIZE = 16f;

	public const float INFO_PAD_X = 24f;

	public const float INFO_ROW_H = 23f;

	public const float INFO_ROW_GAP = 8f;

	public const float INFO_LABEL_W = 102f;

	public const float INFO_TAGS_X = 138f;

	public const float INFO_TAGS_W = 368f;

	public const float INFO_LABEL_SIZE = 16f;

	public const float INFO_PLAIN_SIZE = 18f;

	public const float TAG_H = 23f;

	public const float TAG_PAD_X = 4f;

	public const float TAG_GAP_X = 8f;

	public const float TAG_GAP_Y = 4f;

	public const float TAG_TEXT_SIZE = 16f;

	public const float TAG_ICON_SIZE = 14f;

	public const float TAG_ICON_GAP = 2f;

	public const int TAG_ROUNDED_BORDER = 3;

	public const float BODY_PAD_X = 12f;

	public const float BODY_ROW_W = 502f;

	public const float BODY_ROW_GAP = 8f;

	public const float SCROLLBAR_X = 520f;

	public const float SCROLLBAR_W = 6f;

	public const float SCROLL_TRACK_W = 2f;

	public const float SCROLL_SENSITIVITY = 30f;

	public const float ROW_H = 28f;

	public const float ROW_TOTAL_H = 30f;

	public const float ROW_PAD_X = 4f;

	public const float ROW_NAME_SIZE = 18f;

	public const float ROW_PRICE_SIZE = 20f;

	public const float ROW_REASON_SIZE = 16f;

	public const float ROW_NAME_W = 360f;

	public const float ROW_PRICE_W = 130f;

	public const float ROW_HOVER_LEFT_BAR = 8f;

	public const float ROW_SELECTED_H = 32f;

	public const int FRAME2_BORDER = 2;

	public const float BREAKDOWN_PAD_X = 12f;

	public const float BREAKDOWN_PAD_Y = 8f;

	public const float BREAKDOWN_GAP = 8f;

	public const float BREAKDOWN_SUB_HEADER_H = 28f;

	public const float BREAKDOWN_SUB_HEADER_SIZE = 18f;

	public const float BREAKDOWN_FEATURE_H = 24f;

	public const float BREAKDOWN_FEATURE_SIZE = 18f;

	public const float SUB_HEADER_H = 28f;

	public const float SUB_HEADER_BANNER_H = 38f;

	public const float SUB_HEADER_SIZE = 16f;

	public const float SUB_HEADER_ICON_GAP = 4f;

	public const float FOOTER_PAD_X = 12f;

	public const float FOOTER_ROW_W = 506f;

	public const float FOOTER_ROW_H = 35f;

	public const float FOOTER_TEXT_H = 33f;

	public const float FOOTER_LABEL_SIZE = 20f;

	public const float FOOTER_VALUE_SIZE = 24f;

	public const float FOOTER_LABEL_W = 260f;

	public const float FOOTER_VALUE_W = 300f;

	public const float FOOTER_DOTS_X = 2f;

	public const float FOOTER_DOTS_W = 502f;

	public const float FOOTER_SINGLE_ROW_Y = 17.5f;

	public const float SELECTOR_Y = 21f;

	public const float SELECTOR_ARROW_SIZE = 28f;

	public const float SELECTOR_PREV_X = 16f;

	public const float SELECTOR_NEXT_X = 462f;

	public const float SELECTOR_TEXT_SIZE = 18f;

	public const float BUTTON_W = 256f;

	public const float BUTTON_H = 42f;

	public const float BUTTON_Y = 706f;

	public const float BUTTON_SINGLE_X = 185f;

	public const float BUTTON_LEFT_X = 51f;

	public const float BUTTON_RIGHT_X = 319f;

	public const float BUTTON_INNER_INSET = 4f;

	public const float BUTTON_TEXT_SIZE = 22f;

	public const float BUTTON_TEXT_SIZE_MIN = 12f;

	public const float ARROWS_W = 194f;

	public const float ARROWS_H = 259f;

	public const float ARROWS_X = 216f;

	public const float CHECKBOX_SIZE = 20f;

	public const float CHECKBOX_FILL = 8f;

	public const float CHECKBOX_Y = 4f;

	public const float END_ROW_NAME_X = 32f;

	public const float END_ROW_W = 506f;

	public const float CONFIRM_TEXT_SIZE = 18f;

	public const float CONFIRM_BUTTON_W = 200f;

	public const float CONFIRM_BUTTON_GAP = 16f;

	public const float BARGAIN_W = 400f;

	public const float BARGAIN_H = 622f;

	public const float BARGAIN_PLATE_W = 350f;

	public const float BARGAIN_LINE_W = 2f;

	public const float BARGAIN_SPINE_X = 352f;

	public const float BARGAIN_SPINE_W = 48f;

	public static readonly Vector4 BARGAIN_PLATE_BORDER = new Vector4(4f, 11f, 2f, 11f);

	public const float TABLET_OPEN_EDGE = 598f;

	public const float BARGAIN_OVERLAP = 46f;

	public const float BARGAIN_INNER_X = 18f;

	public const float BARGAIN_INNER_Y = 40f;

	public const float BARGAIN_INNER_W = 314f;

	public const float BARGAIN_INNER_H = 542f;

	public const float BARGAIN_FRAME_X = 15.5f;

	public const float BARGAIN_FRAME_Y = 38f;

	public const float BARGAIN_FRAME_W = 318f;

	public const float BARGAIN_FRAME_H = 546f;

	public const float BARGAIN_SCAN_Y = 22f;

	public const float BARGAIN_SCAN_H = 578f;

	public const float BARGAIN_SEPARATOR_1_Y = 48f;

	public const float BARGAIN_GROUPS_Y = 60f;

	public const float BARGAIN_GROUPS_H = 399f;

	public const float BARGAIN_SCROLLBAR_X = 306f;

	public const float BARGAIN_SEPARATOR_2_Y = 469f;

	public const float BARGAIN_FOOTER_Y = 481f;

	public const float BARGAIN_FOOTER_H = 57f;

	public const float BARGAIN_REP_LINE_Y = 32f;

	public const float BARGAIN_REP_LINE_H = 25f;

	public const float BARGAIN_REP_LINE_SIZE = 18f;

	public const float BARGAIN_GROUP_X = 12f;

	public const float BARGAIN_GROUP_W = 290f;

	public const float BARGAIN_GROUP_H = 74f;

	public const float BARGAIN_GROUP_GAP = 8f;

	public const float BARGAIN_GROUP_HEADER_H = 28f;

	public const float BARGAIN_GROUP_HEADER_SIZE = 18f;

	public const float BARGAIN_GROUP_HEADER_ICON_GAP = 8f;

	public const float BARGAIN_GROUP_ROW_Y = 34f;

	public const float BARGAIN_GROUP_ROW_H = 36f;

	public const float BARGAIN_GROUP_BUTTON_Y = 2f;

	public const float BARGAIN_GROUP_BUTTON_H = 32f;

	public const float BARGAIN_GROUP_BUTTON_PAD = 6f;

	public const float BARGAIN_GROUP_BUTTON_GAP = 8f;

	public const float SMALL_BUTTON_TEXT_SIZE = 16f;

	public const float SMALL_BUTTON_INNER_INSET = 4f;

	public const float TEXT_SIZE_MIN = 10f;

	public static Color Hex(string rgb, float alpha = 1f)
	{
		int num = Convert.ToInt32(rgb.Substring(0, 2), 16);
		int num2 = Convert.ToInt32(rgb.Substring(2, 2), 16);
		return new Color(b: (float)Convert.ToInt32(rgb.Substring(4, 2), 16) / 255f, r: (float)num / 255f, g: (float)num2 / 255f, a: alpha);
	}

	public static Color WithAlpha(Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}
}
