using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class MachineryStyle
{
	public const string ATLAS = "Window/machinery";

	public const int HEADER_H = 20;

	public const int TITLE_SIZE = 10;

	public const int TITLE_MARGIN = 4;

	public const int RULE_H = 1;

	public const int RULE_INSET = 4;

	public const int CLOSE_SIZE = 16;

	public const int CLOSE_MARGIN = 2;

	public const int HEADER_BUTTON_GAP = 2;

	public const int TOP_PADDING = 21;

	public const int PANEL_PAD = 4;

	public const int LABEL_Y = 4;

	public const int LABEL_H = 10;

	public const int CONTENT_Y = 18;

	public const int LABEL_TEXT_SIZE = 6;

	public const int LABEL_TEXT_PAD = 6;

	public const int LABEL_MIN_W = 32;

	public const int LABEL_ICON_W = 21;

	public const int LABEL_ICON_PAD = 3;

	public const int SEPARATOR_W = 1;

	public const int SEPARATOR_BOTTOM_PAD = 4;

	public const int INTAKE_PAD = 4;

	public const int VENT_H = 3;

	public const int VENT_GAP = 1;

	public const int VENT_STEP = 4;

	public const int VENT_EDGE_PAD = 7;

	public const int VENT_CONTENT_GAP = 5;

	public const int VENT_MIN_SPACE = 16;

	public const int BUTTON_H = 16;

	public const int BUTTON_MIN_W = 16;

	public const int BUTTON_TEXT_SIZE = 7;

	public const int BUTTON_TEXT_PAD = 6;

	public const int BUTTON_ICON_GAP = 3;

	public const int RADIO_SIZE = 16;

	public const int RADIO_LABEL_GAP = 6;

	public const int RADIO_LIST_GAP = 6;

	public const int PROGRESS_H = 8;

	public const int PROGRESS_W = 70;

	public const int PROGRESS_INSET = 2;

	public const int SLIDER_W = 96;

	public const int SLIDER_H = 8;

	public const int SLIDER_TRACK_H = 4;

	public const int SLIDER_TRACK_Y = 2;

	public const int SLIDER_THUMB_W = 16;

	public const int SLIDER_THUMB_H = 8;

	public const int TAB_H = 10;

	public const int TAB_MIN_W = 32;

	public const int TAB_TEXT_PAD = 4;

	public const int TAB_GAP = 2;

	public const int TAB_INSET = 4;

	public const int TAB_UNDERLINE_H = 1;

	public const int TAB_PAGE_GAP = 3;

	public const int TEXT_SIZE = 6;

	public const int STACK_GAP = 4;

	public static readonly Color PANEL = Hex("424242");

	public static readonly Color TITLE_TEXT = Hex("c4c4b0");

	public static readonly Color CELL_COLOR = Hex("201e24", 0.8f);

	public static readonly Color RULE = Hex("181a19");

	public static readonly Color LABEL_PLATE = Hex("a8aea5");

	public static readonly Color LABEL_TEXT = Hex("201e24");

	public static readonly Color BODY_TEXT = Hex("c4c4b0");

	public static readonly Color VALUE_TEXT = Hex("d9d9d9");

	public static readonly Color BUTTON_TEXT = Hex("292334");

	public static readonly Color BUTTON_TEXT_DISABLED = Hex("79746e");

	public static readonly Color BUTTON_DISABLED_TINT = new Color(0.6f, 0.6f, 0.6f, 1f);

	public static readonly Color PROGRESS_FILL = Hex("9cdb73");

	public static readonly Color PROGRESS_FILL_WARNING = Hex("dba873");

	public static readonly Color PROGRESS_FILL_DANGER = Hex("cc4849");

	public static readonly Color SLIDER_TRACK = Hex("292234");

	public static readonly Color SLIDER_FILL = Hex("79746e");

	public static readonly Color SLIDER_EMPTY = Hex("201e24");

	public static readonly Color TAB_PLATE = Hex("d5d2b8");

	public const float TAB_PLATE_ALPHA = 0.15f;

	public const float TAB_PLATE_ALPHA_ACTIVE = 0.3f;

	public static readonly Color TAB_TEXT = Hex("c4c4b0");

	public static readonly Color TAB_TEXT_ACTIVE = Hex("d9d9d9");

	public static readonly Color TAB_TEXT_HOVER = Hex("fff09b");

	public static readonly Color TAB_UNDERLINE = Hex("d5d2b8", 0.2f);

	public static Sprite BevelFrame => Sprite("bevel_frame");

	public static Sprite Intake => Sprite("intake");

	public static Sprite PlateNotched => Sprite("plate_notched");

	public static Sprite TabPlate => Sprite("tab_plate");

	public static Sprite ButtonNormal => Sprite("button_normal");

	public static Sprite ButtonHover => Sprite("button_hover");

	public static Sprite ButtonPressed => Sprite("button_pressed");

	public static Sprite RadioOn => Sprite("radio_on");

	public static Sprite RadioOnHover => Sprite("radio_on_hover");

	public static Sprite RadioOff => Sprite("radio_off");

	public static Sprite RadioOffHover => Sprite("radio_off_hover");

	public static Sprite RadioPressed => Sprite("radio_pressed");

	public static Sprite ProgressFrame => Sprite("progress_frame");

	public static Sprite SliderThumbNormal => Sprite("slider_thumb_normal");

	public static Sprite SliderThumbHover => Sprite("slider_thumb_hover");

	public static Sprite Cell => Sprite("cell");

	public static Sprite Vent => Sprite("vent");

	public static Sprite IconPower => Sprite("icon_power");

	public static Sprite IconNote => Sprite("icon_note");

	public static Sprite White => Sprite("flat");

	public static Sprite Sprite(string name)
	{
		return RenderHandler.LoadFromAtlas("Window/machinery", name);
	}

	public static Color Hex(string rgb, float alpha = 1f)
	{
		int num = Convert.ToInt32(rgb.Substring(0, 2), 16);
		int num2 = Convert.ToInt32(rgb.Substring(2, 2), 16);
		return new Color(b: (float)Convert.ToInt32(rgb.Substring(4, 2), 16) / 255f, r: (float)num / 255f, g: (float)num2 / 255f, a: alpha);
	}

	public static void SetSliced(TreeNodeRender node, Sprite sprite, Color color)
	{
		node.image.sprite = sprite;
		node.image.type = Image.Type.Sliced;
		node.image.color = color;
		node.image.enabled = true;
	}

	public static void SetSliced(TreeNodeRender node, Sprite sprite)
	{
		SetSliced(node, sprite, Color.white);
	}

	public static void SetFlat(TreeNodeRender node, Color color)
	{
		node.image.sprite = White;
		node.image.type = Image.Type.Simple;
		node.image.color = color;
		node.image.enabled = true;
	}

	public static void SetSimple(TreeNodeRender node, Sprite sprite, Color color)
	{
		node.image.sprite = sprite;
		node.image.type = Image.Type.Simple;
		node.image.color = color;
		node.image.enabled = true;
		if (sprite != null)
		{
			node.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
		}
	}

	public static void Place(TreeNodeRender node, int x, int y, int width, int height)
	{
		node.rectTransform.anchoredPosition = new Vector2(x, -y);
		node.rectTransform.sizeDelta = new Vector2(width, height);
	}

	public static void InitText(TreeNodeRender node, int size, Color color, HorizontalAlignmentOptions horizontal = HorizontalAlignmentOptions.Center, bool uppercase = false)
	{
		TextMeshProUGUI text = node.text;
		text.text = "";
		text.fontSize = size;
		text.color = color;
		text.fontStyle = (uppercase ? FontStyles.UpperCase : FontStyles.Normal);
		text.horizontalAlignment = horizontal;
		text.verticalAlignment = VerticalAlignmentOptions.Middle;
		text.enableWordWrapping = false;
		text.overflowMode = TextOverflowModes.Overflow;
		text.enableAutoSizing = false;
		text.richText = true;
		text.raycastTarget = false;
		text.enabled = true;
	}

	public static int TextWidth(TreeNodeRender node)
	{
		node.text.ForceMeshUpdate();
		float preferredWidth = node.text.preferredWidth;
		if (!(preferredWidth <= 0f))
		{
			return Mathf.CeilToInt(preferredWidth);
		}
		return 0;
	}

	public static int TextHeight(TreeNodeRender node)
	{
		node.text.ForceMeshUpdate();
		float preferredHeight = node.text.preferredHeight;
		if (!(preferredHeight <= 0f))
		{
			return Mathf.CeilToInt(preferredHeight);
		}
		return 0;
	}

	public static void ApplyInventoryStyle(PixelElement element)
	{
		if (element == null)
		{
			return;
		}
		if (!(element is GameGridInventory gameGridInventory))
		{
			if (!(element is GameSlotInventory gameSlotInventory))
			{
				if (!(element is GridPixelElement gridPixelElement))
				{
					if (!(element is MachineryIntake machineryIntake))
					{
						if (!(element is MachineryPanel machineryPanel))
						{
							if (element is MachineryStack machineryStack)
							{
								foreach (PixelElement element2 in machineryStack.elements)
								{
									ApplyInventoryStyle(element2);
								}
								return;
							}
							if (element is MachineryTabs machineryTabs)
							{
								for (int i = 0; i < machineryTabs.count; i++)
								{
									ApplyInventoryStyle(machineryTabs.GetPage(i));
								}
							}
						}
						else
						{
							ApplyInventoryStyle(machineryPanel.content);
						}
					}
					else
					{
						ApplyInventoryStyle(machineryIntake.content);
					}
					return;
				}
				for (int j = 0; j < gridPixelElement.gridHeight; j++)
				{
					for (int k = 0; k < gridPixelElement.gridWidth; k++)
					{
						ApplyInventoryStyle(gridPixelElement.GetElement(k, j));
					}
				}
			}
			else
			{
				gameSlotInventory.SetMachineryStyle();
			}
		}
		else
		{
			gameGridInventory.SetMachineryStyle();
		}
	}

	public static int Even(int value)
	{
		if ((value & 1) != 1)
		{
			return value;
		}
		return value + 1;
	}
}
