using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnimalMarkHelper
{
	public const string IDENTIFIER = "marking_pen";

	public const string MARKING_PEN_TAG = "MARKING_PEN_TAG";

	public const string PEN_MODE_INT = "MARKING_PEN_MODE_INT";

	public const string ANIMAL_MARK_STRING = "ANIMAL_MARK_STRING";

	public const int PEN_VALUE = 50;

	private const string ATLAS = "Items/items_husbandry";

	private const string SPRITE = "marking_pen";

	public static readonly string[] COLOR_IDS = new string[5] { "red", "purple", "green", "blue", "orange" };

	private static readonly string[] COLOR_CAPTION_KEYS = new string[5] { "mech_marking_pen_color_red", "mech_marking_pen_color_purple", "mech_marking_pen_color_green", "mech_marking_pen_color_blue", "mech_marking_pen_color_orange" };

	private static readonly RenderHandler.ColorPalette[] COLOR_PALETTE = new RenderHandler.ColorPalette[5]
	{
		RenderHandler.ColorPalette.Red,
		RenderHandler.ColorPalette.Purple,
		RenderHandler.ColorPalette.Green,
		RenderHandler.ColorPalette.Blue,
		RenderHandler.ColorPalette.Orange
	};

	private const float CAPTION_TEXT_BLEND = 0.35f;

	public const int MARK_SIZE_PX = 5;

	public const int MARK_INSET_PX = 2;

	private static string EraserCaption => LocHelper.GetLocalizedMechanic("mech_marking_pen_eraser");

	public static int ERASER_MODE => COLOR_IDS.Length;

	private static string ColorCaption(int index)
	{
		return LocHelper.GetLocalizedMechanic(COLOR_CAPTION_KEYS[index]);
	}

	public static Color GetCaptionColor(int colorIndex)
	{
		return Color.Lerp(RenderHandler.GetColorFromPalette(COLOR_PALETTE[colorIndex]), MachineryStyle.BODY_TEXT, 0.35f);
	}

	public static bool IsMarked(GameItem item)
	{
		if (item != null && item.IsTag("ANIMAL_MARK_STRING"))
		{
			return GetColorIndex(item) >= 0;
		}
		return false;
	}

	public static int GetColorIndex(GameItem item)
	{
		if (item == null || !item.IsTag("ANIMAL_MARK_STRING"))
		{
			return -1;
		}
		string @string = item.GetTagReadonly("ANIMAL_MARK_STRING").GetString();
		if (!string.IsNullOrEmpty(@string))
		{
			return Array.IndexOf(COLOR_IDS, @string);
		}
		return -1;
	}

	public static Color? GetMarkColor(GameItem item)
	{
		int colorIndex = GetColorIndex(item);
		if (colorIndex < 0)
		{
			return null;
		}
		return RenderHandler.GetColorFromPalette(COLOR_PALETTE[colorIndex]);
	}

	public static bool IsMarkable(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (item.IsTag("ANIMAL_ITEM_TAG"))
		{
			return true;
		}
		if (AquacultureHelper.IsFish(item))
		{
			return true;
		}
		return false;
	}

	public static void SetMark(GameItem item, int colorIndex)
	{
		if (colorIndex >= 0 && colorIndex < COLOR_IDS.Length)
		{
			string id = COLOR_IDS[colorIndex];
			item.ModifyTag("ANIMAL_MARK_STRING", delegate(TagState state)
			{
				state.Enable().SetString(id);
			});
		}
	}

	public static void ClearMark(GameItem item)
	{
		if (item.IsTag("ANIMAL_MARK_STRING"))
		{
			item.ModifyTag("ANIMAL_MARK_STRING", delegate(TagState state)
			{
				state.SetString("");
			});
			item.DisableTag("ANIMAL_MARK_STRING");
		}
	}

	public static GameItem CreateMarkingPen()
	{
		List<string> captions = new List<string>();
		for (int i = 0; i < COLOR_IDS.Length; i++)
		{
			captions.Add(ColorCaption(i));
		}
		captions.Add(EraserCaption);
		MachineryRadioList modes = new MachineryRadioList(captions);
		for (int j = 0; j < COLOR_IDS.Length; j++)
		{
			modes.Get(j).SetTextColor(GetCaptionColor(j));
		}
		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_marking_pen_label_ink"), modes, "mode"));
		GameItem pen = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_husbandry", "marking_pen");
		pen.name = LocHelper.GetLocalizedItem("item_marking_pen_name");
		pen.shortDescription = LocHelper.GetLocalizedItem("item_marking_pen_desc");
		pen.unitValue = 50L;
		pen.EnableTag("MARKING_PEN_TAG");
		pen.EnableTag("TOOLBOX_ITEM_TAG");
		pen.ModifyTag("MARKING_PEN_MODE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		pen.soundDragStart = "injector_move2";
		pen.soundDragEnd = "injector_move";
		modes.group.onChanged = delegate(int index)
		{
			pen.ModifyTag("MARKING_PEN_MODE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(index);
			});
		};
		GameItem gameItem = pen;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.EnableTag("TOOLBOX_ITEM_TAG");
			int num = Mathf.Clamp(loaded.GetTagReadonly("MARKING_PEN_MODE_INT").GetInt(), 0, captions.Count - 1);
			if (modes.selected != num)
			{
				modes.Select(num);
			}
		});
		pen.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		pen.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		GameItem gameItem2 = pen;
		gameItem2.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && IsMarkable(t)));
		GameItem gameItem3 = pen;
		gameItem3.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem3.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && IsMarkable(t)));
		GameItem gameItem4 = pen;
		gameItem4.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem4.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (IsMarkable(t))
			{
				int @int = s.GetTagReadonly("MARKING_PEN_MODE_INT").GetInt();
				if (@int >= ERASER_MODE || GetColorIndex(t) == @int)
				{
					ClearMark(t);
				}
				else
				{
					SetMark(t, @int);
				}
				AudioManager.Instance.Play3("click_vintage");
			}
		});
		AudioHelper.InitPlasticContainer(pen);
		return pen;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("MARKING_PEN_TAG"))
		{
			int @int = item.GetTagReadonly("MARKING_PEN_MODE_INT").GetInt();
			string text = ((@int >= ERASER_MODE || @int < 0) ? EraserCaption : ColorCaption(@int));
			RenderHandler.ColorPalette color = ((@int >= ERASER_MODE || @int < 0) ? RenderHandler.ColorPalette.LightGray : COLOR_PALETTE[@int]);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_marking_pen_tooltip_ink", text), ignoreEmpty: true, color);
		}
	}
}
