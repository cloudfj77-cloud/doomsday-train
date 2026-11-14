using System;
using UnityEngine;

public static class NewGameStyle
{
	public const float PANEL_W = 1110f;

	public const float PANEL_H = 958f;

	public static readonly Vector4 PANEL_BORDER = new Vector4(30f, 169f, 135f, 46f);

	public const float PANEL_PPU_MULTIPLIER = 0.5f;

	public const float HEADER_TITLE_X = 43f;

	public const float HEADER_TITLE_Y = 13f;

	public const float HEADER_TITLE_H = 66f;

	public const float HEADER_TITLE_SIZE = 56f;

	public const float HEADER_TITLE_SIZE_MIN = 28f;

	public const float BACK_X = 1062f;

	public const float BACK_Y = 19f;

	public const float ARROW_SIZE = 32f;

	public const float STRIP_X = 137f;

	public const float STRIP_Y = 136f;

	public const float STRIP_W = 836f;

	public const float STRIP_H = 64f;

	public const float STRIP_ARROW_Y = 16f;

	public const float STRIP_GAP = 12f;

	public const int STRIP_PAGE_SIZE = 10;

	public const float SLOT_SIZE = 64f;

	public const float SLOT_STEP = 76f;

	public const float STRIP_SLOTS_X = 44f;

	public const float STRIP_SLOTS_W = 748f;

	public const float STRIP_NEXT_X = 804f;

	public const float SLOT_ICON_SIZE = 38f;

	public const float SLOT_ICON_INSET = 13f;

	public const float ARROW_DISABLED_ALPHA = 0.3f;

	public const float SEPARATOR_X = 80f;

	public const float SEPARATOR_1_Y = 232f;

	public const float SEPARATOR_2_Y = 468f;

	public const float SEPARATOR_12_W = 972f;

	public const float SEPARATOR_3_Y = 865f;

	public const float SEPARATOR_3_W = 900f;

	public const float SEPARATOR_H = 2f;

	public const float TITLE_X = 78f;

	public const float TITLE_Y = 258f;

	public const float TITLE_W = 560f;

	public const float TITLE_H = 57f;

	public const float TITLE_SIZE = 48f;

	public const float TITLE_SIZE_MIN = 24f;

	public const float TAG_X = 652f;

	public const float TAG_Y = 277f;

	public const float TAG_W = 400f;

	public const float TAG_H = 27f;

	public const float TAG_SIZE = 20f;

	public const float DESC_X = 80f;

	public const float DESC_Y = 327f;

	public const float DESC_W = 975f;

	public const float DESC_H = 113f;

	public const float DESC_SIZE = 24f;

	public const float DESC_SIZE_MIN = 16f;

	public const float START_WITH_X = 80f;

	public const float START_WITH_Y = 498f;

	public const float START_WITH_W = 950f;

	public const float START_WITH_H = 38f;

	public const float START_WITH_SIZE = 28f;

	public const float BULLETS_X = 80f;

	public const float BULLETS_Y = 548f;

	public const float BULLETS_W = 902f;

	public const float BULLET_ROW_H = 35f;

	public const float BULLET_ROW_GAP = 4f;

	public const float BULLET_SIZE = 4f;

	public const float BULLET_DOT_Y = 15f;

	public const float BULLET_TEXT_X = 16f;

	public const float BULLET_TEXT_SIZE = 24f;

	public const float BULLET_TEXT_SIZE_MIN = 14f;

	public const float WARNING_X = 80f;

	public const float WARNING_Y = 822f;

	public const float WARNING_W = 950f;

	public const float WARNING_H = 27f;

	public const float WARNING_SIZE = 20f;

	public const float BUTTON_X = 427f;

	public const float BUTTON_Y = 891f;

	public const float BUTTON_W = 256f;

	public const float BUTTON_H = 42f;

	public const float BUTTON_BORDER = 2f;

	public const float BUTTON_INNER_INSET = 4f;

	public const float BUTTON_TEXT_SIZE = 22f;

	public const float TEXT_SIZE_MIN = 12f;

	public const int FRAME2_BORDER = 2;

	public static readonly Color SHADE = Hex("292334", 0.8f);

	public static readonly Color HEADER_TEXT = Hex("292234");

	public static readonly Color SEPARATOR = Hex("8abfbb", 0.1f);

	public static readonly Color TITLE = Color.white;

	public static readonly Color DESCRIPTION = Hex("a8aea5");

	public static readonly Color START_WITH = Hex("c4c4b0");

	public static readonly Color BULLET = Hex("8abfbb");

	public static readonly Color BULLET_TEXT = Hex("d9d9d9");

	public static readonly Color TAG_POSITIVE = Hex("9cdb73");

	public static readonly Color TAG_WARNING = Hex("dba873");

	public static readonly Color TAG_DANGER = Hex("cc4849");

	public static readonly Color WARNING = Hex("dba873");

	public static readonly Color WARNING_DANGER = Hex("cc4849");

	public static readonly Color BUTTON_BG = Hex("292234");

	public static readonly Color BUTTON_BORDER_COLOR = Hex("8abfbb");

	public static readonly Color BUTTON_HOVER = Hex("fff09b");

	public const float BUTTON_DISABLED_ALPHA = 0.3f;

	public const float PERKS_INSTRUCTION_X = 80f;

	public const float PERKS_INSTRUCTION_Y = 136f;

	public const float PERKS_INSTRUCTION_W = 975f;

	public const float PERKS_INSTRUCTION_H = 65f;

	public const float PERKS_INSTRUCTION_SIZE = 24f;

	public const float PERKS_INSTRUCTION_SIZE_MIN = 16f;

	public const float PERKS_RULE_W = 972f;

	public const float PERKS_RULE_1_Y = 240f;

	public const float PERKS_RULE_2_Y = 588f;

	public const float PERKS_LABEL_X = 80f;

	public const float PERKS_LABEL_W = 950f;

	public const float PERKS_LABEL_H = 38f;

	public const float PERKS_LABEL_SIZE = 28f;

	public const float PERKS_AVAILABLE_LABEL_Y = 270f;

	public const float PERKS_SELECTED_LABEL_Y = 618f;

	public const float PERKS_GRID_X = 59f;

	public const float PERKS_GRID_Y = 336f;

	public const float PERKS_GRID_W = 996f;

	public const int PERKS_COLUMNS = 12;

	public const int PERKS_ROWS = 3;

	public const int PERKS_PAGE_SIZE = 36;

	public const float PERK_SLOT_SIZE = 64f;

	public const float PERK_SLOT_SHADOW = 8f;

	public const float PERK_SLOT_H = 72f;

	public const float PERK_STEP_X = 76f;

	public const float PERK_STEP_Y = 80f;

	public const float PERKS_SLOTS_X = 48f;

	public const float PERKS_SLOTS_W = 900f;

	public const float PERKS_GRID_H = 224f;

	public const float PERKS_ARROW_Y = 96f;

	public const float PERKS_NEXT_X = 964f;

	public const float PERKS_SELECTED_ROW_Y = 684f;

	public const int PERKS_SELECTED_PAGE_SIZE = 10;

	public const float PERKS_SELECTED_ROW_MAX_W = 748f;

	public const float PERKS_SELECTED_PREV_X = 137f;

	public const float PERKS_SELECTED_NEXT_X = 941f;

	public const float PERKS_SELECTED_ARROW_Y = 700f;

	public const float PERKS_COUNTERS_Y = 805f;

	public const float PERKS_COUNTER_W = 384f;

	public const float PERKS_COUNTER_H = 42f;

	public const float PERKS_COUNTER_GAP = 24f;

	public const float PERKS_COUNTER_TEXT_SIZE = 18f;

	public const float PERK_ICON_SIZE = 64f;

	public const float PERK_LOCK_W = 16f;

	public const float PERK_LOCK_H = 18f;

	public const float PERK_LOCK_X = 24f;

	public const float PERK_LOCK_Y = 23f;

	public const float PERK_TAKEN_ALPHA = 0.3f;

	public const float PERK_TOOLTIP_W = 512f;

	public const float PERK_TOOLTIP_OUTER_BORDER = 2f;

	public const float PERK_TOOLTIP_BORDER = 2f;

	public const float PERK_TOOLTIP_PADDING = 12f;

	public const float PERK_TOOLTIP_CONTENT_X = 16f;

	public const float PERK_TOOLTIP_CONTENT_W = 480f;

	public const float PERK_TOOLTIP_GAP = 8f;

	public const float PERK_TOOLTIP_HEADER_H = 28f;

	public const float PERK_TOOLTIP_NAME_SIZE = 24f;

	public const float PERK_TOOLTIP_NAME_SIZE_MIN = 14f;

	public const float PERK_TOOLTIP_COST_W = 204f;

	public const float PERK_TOOLTIP_COST_SIZE = 18f;

	public const float PERK_TOOLTIP_LINES_PADDING = 8f;

	public const float PERK_TOOLTIP_LINE_GAP = 16f;

	public const float PERK_TOOLTIP_BULLET_SIZE = 4f;

	public const float PERK_TOOLTIP_BULLET_TEXT_GAP = 8f;

	public const float PERK_TOOLTIP_LINE_SIZE = 18f;

	public const float PERK_TOOLTIP_LOCK_W = 16f;

	public const float PERK_TOOLTIP_LOCK_H = 18f;

	public const float PERK_TOOLTIP_LOCK_ROW_H = 22f;

	public const float PERK_TOOLTIP_LOCK_TEXT_GAP = 8f;

	public const float PERK_TOOLTIP_LOCK_TEXT_SIZE = 16f;

	public const float PERK_TOOLTIP_MOUSE_OFFSET = 25f;

	public static readonly Color PERK_TOOLTIP_OUTER = Hex("292234", 0.75f);

	public static readonly Color PERK_TOOLTIP_BG = Hex("292234");

	public static readonly Color PERK_TOOLTIP_BORDER_COLOR = Hex("8abfbb");

	public static readonly Color PERK_TOOLTIP_NAME = Hex("d5d2b8");

	public static readonly Color PERK_TOOLTIP_COST = Hex("fff09b");

	public static readonly Color PERK_TOOLTIP_RULE = Hex("414554");

	public static readonly Color PERK_TOOLTIP_BULLET = Hex("8abfbb");

	public static readonly Color PERK_TOOLTIP_LINE = Hex("c4c4b0");

	public static readonly Color PERK_TOOLTIP_LOCKED = Hex("cf6f6f");

	public static readonly Color PERKS_COUNTER_BG = new Color(1f / 17f, 1f / 17f, 1f / 17f, 0.2f);

	public static readonly Color PERKS_COUNTER_TEXT = Hex("c4c4b0");

	public static readonly Color PERKS_COUNTER_VALUE = Hex("fff09b");

	public static readonly Color PERKS_COUNTER_OVER = Hex("cc4849");

	public static readonly Color PERK_POSITIVE = Hex("9cdb73");

	public static readonly Color PERK_NEGATIVE = Hex("cc4849");

	public static readonly Color PERK_NEUTRAL = Hex("dba873");

	public static readonly Color PERK_LOCKED = Hex("88948a");

	public const float DIFF_ROW_X = 80f;

	public const float DIFF_ROW_Y = 116f;

	public const float DIFF_ROW_W = 900f;

	public const float DIFF_PLATE_W = 288f;

	public const float DIFF_PLATE_H = 643f;

	public const float DIFF_PLATE_STEP = 306f;

	public const int DIFF_PLATES_PER_PAGE = 3;

	public const float DIFF_PLATE_BORDER = 2f;

	public const float DIFF_PREV_X = 36f;

	public const float DIFF_NEXT_X = 992f;

	public const float DIFF_ARROW_Y = 421f;

	public const float DIFF_IMAGE_X = 60f;

	public const float DIFF_IMAGE_Y = 10f;

	public const float DIFF_IMAGE_SIZE = 164f;

	public const float DIFF_RULE_X = 16f;

	public const float DIFF_RULE_Y = 175f;

	public const float DIFF_RULE_W = 256f;

	public const float DIFF_TITLE_Y = 201f;

	public const float DIFF_TITLE_H = 38f;

	public const float DIFF_TITLE_SIZE = 28f;

	public const float DIFF_SUBTITLE_Y = 237f;

	public const float DIFF_SUBTITLE_H = 22f;

	public const float DIFF_SUBTITLE_SIZE = 16f;

	public const float DIFF_BULLETS_X = 18f;

	public const float DIFF_BULLETS_Y = 271f;

	public const float DIFF_BULLETS_W = 248f;

	public const float DIFF_BULLETS_BOTTOM_PAD = 16f;

	public const float DIFF_BULLET_TEXT_SIZE = 16f;

	public const float DIFF_BULLET_ROW_PAD = 6f;

	public const float DIFF_BULLET_ROW_GAP = 2f;

	public const float DIFF_BULLETS_SCROLL_SENSITIVITY = 30f;

	public const float DIFF_COMPACT_TITLE_Y = 16f;

	public const float DIFF_COMPACT_SUBTITLE_Y = 52f;

	public const float DIFF_COMPACT_RULE_Y = 90f;

	public const float DIFF_COMPACT_BULLETS_Y = 104f;

	public const float DIFF_OPTIONS_RULE_Y = 783f;

	public const float DIFF_OPTIONS_Y = 809f;

	public const float DIFF_OPTIONS_H = 32f;

	public const float DIFF_CHECKBOX_SIZE = 32f;

	public const float DIFF_CHECKBOX_MARK = 12f;

	public const float DIFF_CHECKBOX_LABEL_GAP = 12f;

	public const float DIFF_OPTIONS_GAP = 64f;

	public const float DIFF_OPTION_LABEL_SIZE = 20f;

	public static readonly Color DIFF_PLATE_BORDER_COLOR = Hex("8abfbb");

	public static readonly Color DIFF_PLATE_BORDER_SELECTED = Hex("fff09b");

	public static readonly Color DIFF_TITLE = Hex("fefefe");

	public static readonly Color DIFF_SUBTITLE = Hex("c4c4b0");

	public static readonly Color DIFF_CHECKBOX_BG = Hex("292234");

	public static readonly Color DIFF_CHECKBOX_BORDER = Hex("8abfbb");

	public static readonly Color DIFF_CHECKBOX_BORDER_HOVER = Hex("e0d09b");

	public static readonly Color DIFF_CHECKBOX_MARK_COLOR = Hex("8abfbb");

	public static readonly Color DIFF_OPTION_LABEL = Hex("8abfbb");

	public const float DIFF_OPTION_LOCKED_ALPHA = 0.3f;

	public const float CREDITS_X = 80f;

	public const float CREDITS_Y = 120f;

	public const float CREDITS_W = 900f;

	public const float CREDITS_H = 786f;

	public const float CREDITS_GAP = 16f;

	public const float CREDITS_SECTION_TITLE_SIZE = 28f;

	public const float CREDITS_SECTION_TITLE_H = 38f;

	public const float CREDITS_ROWS_GAP = 8f;

	public const float CREDITS_ROW_H = 33f;

	public const float CREDITS_ROW_SIZE = 24f;

	public const float CREDITS_COLUMN_W = 450f;

	public const float CREDITS_PARAGRAPH_SIZE = 22f;

	public const float CREDITS_NOTE_SIZE = 24f;

	public const float CREDITS_SCROLL_SENSITIVITY = 40f;

	public const float CREDITS_SCROLLBAR_X = 992f;

	public const float CREDITS_SCROLLBAR_W = 16f;

	public const float CREDITS_SCROLLBAR_TRACK_W = 8f;

	public static readonly Color CREDITS_SECTION_TITLE = Hex("d9d9d9");

	public static readonly Color CREDITS_TEXT = Hex("a8aea5");

	public static readonly Color CREDITS_SCROLL_TRACK = Hex("8abfbb");

	public static readonly Color CREDITS_SCROLL_HANDLE_BG = Hex("292234");

	public static readonly Color CREDITS_SCROLL_HANDLE_BORDER = Hex("8abfbb");

	public const float LOAD_LIST_X = 80f;

	public const float LOAD_LIST_Y = 136f;

	public const float LOAD_LIST_W = 900f;

	public const float LOAD_LIST_H = 797f;

	public const float LOAD_PLATE_H = 96f;

	public const float LOAD_PLATE_GAP = 8f;

	public const float LOAD_SCROLL_SENSITIVITY = 40f;

	public const float LOAD_ICON_PLATE_X = 2f;

	public const float LOAD_ICON_PLATE_Y = 2f;

	public const float LOAD_ICON_PLATE_W = 64f;

	public const float LOAD_ICON_PLATE_H = 92f;

	public const float LOAD_ICON_RING_X = 12f;

	public const float LOAD_ICON_RING_Y = 27f;

	public const float LOAD_ICON_RING_SIZE = 42f;

	public const float LOAD_ICON_SIZE = 38f;

	public const float LOAD_TITLE_X = 82f;

	public const float LOAD_TITLE_Y = 13f;

	public const float LOAD_TITLE_W = 308f;

	public const float LOAD_TITLE_H = 38f;

	public const float LOAD_TITLE_SIZE = 32f;

	public const float LOAD_TITLE_SIZE_MIN = 18f;

	public const float LOAD_INFO_Y = 56f;

	public const float LOAD_INFO_H = 27f;

	public const float LOAD_INFO_SIZE = 20f;

	public const float LOAD_DATE_X = 310f;

	public const float LOAD_DATE_W = 260f;

	public const float LOAD_DATE_Y = 21f;

	public const float LOAD_DATE_H = 25f;

	public const float LOAD_DATE_SIZE = 18f;

	public const float LOAD_VERSION_Y = 50f;

	public const float LOAD_PLATE_SEPARATOR_X = 586f;

	public const float LOAD_PLATE_SEPARATOR_Y = 16f;

	public const float LOAD_PLATE_SEPARATOR_H = 64f;

	public const float LOAD_BUTTON_X = 604f;

	public const float LOAD_BUTTON_Y = 27f;

	public const float LOAD_BUTTON_W = 192f;

	public const float LOAD_ICON_BUTTON_Y = 34f;

	public const float LOAD_DUPLICATE_X = 812f;

	public const float LOAD_DELETE_X = 856f;

	public const float LOAD_ICON_SPRITE_INSET = -2f;

	public const float LOAD_EXPANDED_X = 12f;

	public const float LOAD_EXPANDED_W = 876f;

	public const float LOAD_EXPANDED_OVERLAP = 2f;

	public const float LOAD_EXPANDED_BORDER_BOTTOM = 8f;

	public const float LOAD_EXPANDED_ROWS_X = 70f;

	public const float LOAD_EXPANDED_ROWS_Y = 18f;

	public const float LOAD_EXPANDED_ROWS_W = 716f;

	public const float LOAD_EXPANDED_PAD_BOTTOM = 10f;

	public const float LOAD_ROW_H = 64f;

	public const float LOAD_ROW_GAP = 4f;

	public const float LOAD_ROW_STEP = 74f;

	public const float LOAD_ROW_INFO_Y = 25f;

	public const float LOAD_ROW_DATE_X = 228f;

	public const float LOAD_ROW_DATE_W = 260f;

	public const float LOAD_ROW_SEPARATOR_X = 504f;

	public const float LOAD_ROW_BUTTON_X = 522f;

	public const float LOAD_ROW_BUTTON_Y = 12f;

	public const float LOAD_MAGNIFY_X = 1026f;

	public const float LOAD_MAGNIFY_Y = 19f;

	public const float LOAD_DIALOG_X = 171f;

	public const float LOAD_DIALOG_Y = 372f;

	public const float LOAD_DIALOG_W = 768f;

	public const float LOAD_DIALOG_H = 214f;

	public const float LOAD_DIALOG_HEADER_H = 42f;

	public const float LOAD_DIALOG_CAP_W = 12f;

	public const float LOAD_DIALOG_BODY_Y = 40f;

	public const float LOAD_DIALOG_TITLE_X = 24f;

	public const float LOAD_DIALOG_TITLE_Y = 2f;

	public const float LOAD_DIALOG_TITLE_W = 700f;

	public const float LOAD_DIALOG_TITLE_H = 38f;

	public const float LOAD_DIALOG_TITLE_SIZE = 32f;

	public const float LOAD_DIALOG_TEXT_X = 24f;

	public const float LOAD_DIALOG_TEXT_Y = 60f;

	public const float LOAD_DIALOG_TEXT_W = 720f;

	public const float LOAD_DIALOG_TEXT_H = 56f;

	public const float LOAD_DIALOG_TEXT_SIZE = 24f;

	public const float LOAD_DIALOG_TEXT_SIZE_MIN = 16f;

	public const float LOAD_DIALOG_SEPARATOR_X = 64f;

	public const float LOAD_DIALOG_SEPARATOR_Y = 122f;

	public const float LOAD_DIALOG_SEPARATOR_W = 640f;

	public const float LOAD_DIALOG_BUTTON_Y = 148f;

	public const float LOAD_DIALOG_BUTTON_W = 256f;

	public const float LOAD_DIALOG_BUTTON_GAP = 16f;

	public static readonly Color LOAD_PLATE_BORDER_COLOR = Hex("8abfbb");

	public static readonly Color LOAD_ICON_PLATE_COLOR = Hex("8abfbb");

	public static readonly Color LOAD_TITLE_COLOR = Hex("8abfbb");

	public static readonly Color LOAD_INFO_COLOR = Hex("d9d9d9");

	public static readonly Color LOAD_DATE_COLOR = Hex("a8aea5");

	public static readonly Color LOAD_VERSION_COLOR = Hex("88948a");

	public static readonly Color LOAD_EXPANDED_FILL = Hex("8abfbb", 0.15f);

	public static readonly Color LOAD_DIALOG_BG = Hex("292334");

	public static readonly Color LOAD_DIALOG_HEADER = Hex("8abfbb");

	public static readonly Color LOAD_DIALOG_TITLE = Hex("292234");

	public static readonly Color LOAD_DIALOG_TEXT = Hex("a8aea5");

	public static Color PerkColor(StartingPerk.StartingPerkType type)
	{
		return type switch
		{
			StartingPerk.StartingPerkType.POSITIVE => PERK_POSITIVE, 
			StartingPerk.StartingPerkType.NEGATIVE => PERK_NEGATIVE, 
			_ => PERK_NEUTRAL, 
		};
	}

	public static Color TagColor(StartingScenario.TagTone tone)
	{
		return tone switch
		{
			StartingScenario.TagTone.Positive => TAG_POSITIVE, 
			StartingScenario.TagTone.Danger => TAG_DANGER, 
			_ => TAG_WARNING, 
		};
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
