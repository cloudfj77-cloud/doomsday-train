using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public static class TabletUI
{
	public struct TagSpec
	{
		public string text;

		public Color background;

		public Color textColor;

		public Sprite icon;

		public Func<string> tooltip;

		public TagSpec(string text, Color background, Color textColor, Sprite icon = null, Func<string> tooltip = null)
		{
			this.text = text;
			this.background = background;
			this.textColor = textColor;
			this.icon = icon;
			this.tooltip = tooltip;
		}
	}

	public struct FooterRow
	{
		public TextMeshProUGUI label;

		public TextMeshProUGUI value;

		public Image dots;

		public void SetColor(Color color)
		{
			if (label != null)
			{
				label.color = color;
			}
			if (value != null)
			{
				value.color = color;
			}
		}
	}

	public static void ClearChildren(RectTransform container)
	{
		if (!(container == null))
		{
			for (int num = container.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(container.GetChild(num).gameObject);
			}
		}
	}

	public static RectTransform NewRect(Transform parent, string name, float x, float y, float width, float height)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		RectTransform obj = gameObject.transform as RectTransform;
		WildNetworkUI.SetRect(obj, x, y, width, height);
		return obj;
	}

	public static Image NewImage(Transform parent, string name, float x, float y, float width, float height, Sprite sprite, Color color, bool raycast = false)
	{
		Image image = NewRect(parent, name, x, y, width, height).gameObject.AddComponent<Image>();
		WildNetworkUI.ApplySprite(image, sprite, color);
		image.raycastTarget = raycast;
		return image;
	}

	public static Image NewFlat(Transform parent, string name, float x, float y, float width, float height, Color color, bool raycast = false)
	{
		return NewImage(parent, name, x, y, width, height, null, color, raycast);
	}

	public static Image NewStretched(Transform parent, string name, Sprite sprite, Color color)
	{
		Image image = NewImage(parent, name, 0f, 0f, 0f, 0f, sprite, color);
		Stretch(image.rectTransform);
		return image;
	}

	public static void Stretch(RectTransform rect, float inset = 0f)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = new Vector2(inset, inset);
		rect.offsetMax = new Vector2(0f - inset, 0f - inset);
	}

	public static TMP_FontAsset Font(TabletSkin skin, FontLoader.FontRole role)
	{
		switch (role)
		{
		case FontLoader.FontRole.Lubrifont:
			return skin.fontLubrifont;
		case FontLoader.FontRole.Noto_Sans_Normal:
			if (skin.fontNotoRegular != null)
			{
				return skin.fontNotoRegular;
			}
			break;
		}
		return skin.fontNotoMedium;
	}

	public static TextMeshProUGUI NewText(TabletSkin skin, Transform parent, string name, float x, float y, float width, float height, FontLoader.FontRole role, float size, Color color, TextAlignmentOptions alignment, bool wrap = false)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.SetActive(value: false);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		WildNetworkUI.SetRect(gameObject.transform as RectTransform, x, y, width, height);
		TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
		textMeshProUGUI.font = Font(skin, role);
		textMeshProUGUI.fontSize = size;
		textMeshProUGUI.color = color;
		textMeshProUGUI.alignment = alignment;
		textMeshProUGUI.enableWordWrapping = wrap;
		textMeshProUGUI.enableAutoSizing = false;
		textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
		textMeshProUGUI.raycastTarget = false;
		textMeshProUGUI.richText = true;
		gameObject.AddComponent<LocalizeStringEvent>();
		TMPLocalizer tMPLocalizer = gameObject.AddComponent<TMPLocalizer>();
		tMPLocalizer.ChangeFontOnly = true;
		tMPLocalizer.fontRole = role;
		gameObject.SetActive(value: true);
		return textMeshProUGUI;
	}

	public static void AutoSize(TextMeshProUGUI text, float minSize = 10f)
	{
		WildNetworkUI.AutoSize(text, minSize);
	}

	public static float MeasureWidth(TextMeshProUGUI text, string content)
	{
		if (!text.gameObject.activeInHierarchy)
		{
			Debug.LogWarning("[TabletUI] Measuring '" + content + "' under an inactive parent (" + text.transform.parent.name + "): the width will be wrong. Activate the host before building into it.");
		}
		if (text.text != content)
		{
			text.text = content;
		}
		text.ForceMeshUpdate(ignoreActiveState: true, forceTextReparsing: true);
		return Mathf.Ceil(text.preferredWidth);
	}

	public static string Credits(long value)
	{
		return value + GeneralHelper.CreditIcon();
	}

	public static TabletTooltipTrigger AddTooltip(GameObject target, Func<string> provider)
	{
		if (target.GetComponent<Graphic>() == null)
		{
			Image image = target.AddComponent<Image>();
			image.color = Color.clear;
			image.raycastTarget = true;
		}
		else
		{
			target.GetComponent<Graphic>().raycastTarget = true;
		}
		TabletTooltipTrigger tabletTooltipTrigger = target.AddComponent<TabletTooltipTrigger>();
		tabletTooltipTrigger.provider = provider;
		return tabletTooltipTrigger;
	}

	public static Image InfoIcon(Transform parent, float x, float y, Sprite sprite, Func<string> tooltip)
	{
		Image image = NewImage(parent, "Info", x, y, 14f, 14f, sprite, Color.white, raycast: true);
		if (tooltip != null)
		{
			AddTooltip(image.gameObject, tooltip);
		}
		return image;
	}

	public static float Tag(TabletSkin skin, Transform parent, float x, float y, TagSpec spec)
	{
		RectTransform rectTransform = NewRect(parent, "Tag", x, y, 10f, 23f);
		Image image = NewStretched(rectTransform, "Plate", skin.tagRounded, spec.background);
		image.raycastTarget = spec.tooltip != null;
		TextMeshProUGUI textMeshProUGUI = NewText(skin, rectTransform, "Text", 4f, 0f, 10f, 23f, FontLoader.FontRole.Noto_Sans, 16f, spec.textColor, TextAlignmentOptions.MidlineLeft);
		textMeshProUGUI.text = spec.text;
		float num = MeasureWidth(textMeshProUGUI, spec.text);
		textMeshProUGUI.rectTransform.sizeDelta = new Vector2(num + 2f, 23f);
		float num2 = 4f + num + 4f;
		if (spec.icon != null)
		{
			float num3 = 4f + num + 2f;
			NewImage(rectTransform, "Icon", num3, 4.5f, 14f, 14f, spec.icon, Color.white);
			num2 = num3 + 14f + 4f;
		}
		rectTransform.sizeDelta = new Vector2(num2, 23f);
		if (spec.tooltip != null)
		{
			AddTooltip(image.gameObject, spec.tooltip);
		}
		return num2;
	}

	public static float InfoRow(TabletSkin skin, Transform host, float y, string label, IList<TagSpec> tags)
	{
		TextMeshProUGUI textMeshProUGUI = NewText(skin, host, "InfoLabel", 24f, y, 102f, 23f, FontLoader.FontRole.Noto_Sans, 16f, TabletStyle.INK, TextAlignmentOptions.MidlineLeft);
		textMeshProUGUI.text = label;
		AutoSize(textMeshProUGUI);
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < tags.Count; i++)
		{
			RectTransform rectTransform = NewRect(host, "Probe", 0f, 0f, 0f, 0f);
			float num3 = Tag(skin, rectTransform, 0f, 0f, tags[i]);
			UnityEngine.Object.Destroy(rectTransform.gameObject);
			if (num > 0f && num + num3 > 368f)
			{
				num = 0f;
				num2 += 27f;
			}
			Tag(skin, host, 138f + num, y + num2, tags[i]);
			num += num3 + 8f;
		}
		return num2 + 23f;
	}

	public static float InfoLine(TabletSkin skin, Transform host, float y, string label, string value)
	{
		if (!string.IsNullOrEmpty(label))
		{
			TextMeshProUGUI textMeshProUGUI = NewText(skin, host, "InfoLabel", 24f, y, 102f, 23f, FontLoader.FontRole.Noto_Sans, 16f, TabletStyle.INK, TextAlignmentOptions.MidlineLeft);
			textMeshProUGUI.text = label;
			AutoSize(textMeshProUGUI);
		}
		float num = (string.IsNullOrEmpty(label) ? 24f : 138f);
		float width = 506f - num;
		TextMeshProUGUI textMeshProUGUI2 = NewText(skin, host, "InfoValue", num, y, width, 23f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.INK, TextAlignmentOptions.MidlineLeft);
		textMeshProUGUI2.text = value;
		AutoSize(textMeshProUGUI2);
		return 23f;
	}

	public static RectTransform RowRoot(Transform content, string name, float height)
	{
		RectTransform rectTransform = NewRect(content, name, 0f, 0f, 502f, height);
		LayoutElement layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
		layoutElement.preferredHeight = height;
		layoutElement.minHeight = height;
		return rectTransform;
	}

	public static GameObject RowFrame(TabletSkin skin, RectTransform row, float boxHeight, float barHeight)
	{
		RectTransform rectTransform = NewRect(row, "Frame", 0f, 0f, 502f, boxHeight);
		NewStretched(rectTransform, "Border", skin.frame2, TabletStyle.INK);
		NewFlat(rectTransform, "Bar", 0f, 0f, 8f, barHeight, TabletStyle.INK);
		return rectTransform.gameObject;
	}

	public static TabletListRow ListRow(TabletSkin skin, Transform content, string name, string right, bool refused, Func<string> tooltip, Action onClick)
	{
		RectTransform rectTransform = RowRoot(content, "Row", 30f);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		image.color = Color.clear;
		image.raycastTarget = true;
		TabletListRow tabletListRow = rectTransform.gameObject.AddComponent<TabletListRow>();
		tabletListRow.onClick = onClick;
		tabletListRow.selectable = onClick != null;
		RectTransform rectTransform2 = NewRect(rectTransform, "Visuals", 0f, 0f, 502f, 30f);
		if (refused)
		{
			CanvasGroup canvasGroup = rectTransform2.gameObject.AddComponent<CanvasGroup>();
			canvasGroup.alpha = 0.3f;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
		}
		TextMeshProUGUI textMeshProUGUI = NewText(skin, rectTransform2, "Name", 12f, 0f, 360f, 28f, FontLoader.FontRole.Noto_Sans, 18f, TabletStyle.INK, TextAlignmentOptions.MidlineLeft);
		textMeshProUGUI.text = name;
		AutoSize(textMeshProUGUI);
		TextMeshProUGUI textMeshProUGUI2 = NewText(skin, rectTransform2, "Right", 368f, 0f, 130f, 28f, FontLoader.FontRole.Noto_Sans, refused ? 16f : 20f, TabletStyle.INK, TextAlignmentOptions.MidlineRight);
		textMeshProUGUI2.text = right;
		AutoSize(textMeshProUGUI2);
		NewFlat(rectTransform2, "Separator", 0f, 28f, 502f, 2f, TabletStyle.ROW_SEPARATOR);
		tabletListRow.hoverVisual = RowFrame(skin, rectTransform, 28f, 28f);
		tabletListRow.hoverVisual.SetActive(value: false);
		if (tooltip != null)
		{
			AddTooltip(rectTransform.gameObject, tooltip);
		}
		return tabletListRow;
	}

	public static RectTransform SubHeader(TabletSkin skin, Transform content, string text, float height, Sprite icon = null, Func<string> tooltip = null, Color? textColor = null)
	{
		RectTransform rectTransform = RowRoot(content, "SubHeader", height);
		NewFlat(rectTransform, "Plate", 0f, 0f, 502f, height, TabletStyle.INK);
		Color color = textColor ?? TabletStyle.SUB_HEADER_TEXT;
		TextMeshProUGUI textMeshProUGUI = NewText(skin, rectTransform, "Text", 0f, 0f, 502f, height, FontLoader.FontRole.Noto_Sans, (height >= 38f) ? 16f : 18f, color, TextAlignmentOptions.Center);
		textMeshProUGUI.text = text;
		AutoSize(textMeshProUGUI);
		if (icon != null)
		{
			float num = MeasureWidth(textMeshProUGUI, text);
			float x = (502f + num) * 0.5f + 4f;
			InfoIcon(rectTransform, x, (height - 14f) * 0.5f, icon, tooltip);
			textMeshProUGUI.rectTransform.anchoredPosition = new Vector2(-9f, 0f);
			(rectTransform.GetChild(rectTransform.childCount - 1) as RectTransform).anchoredPosition += new Vector2(-9f, 0f);
		}
		return rectTransform;
	}

	public static RectTransform TextRow(TabletSkin skin, Transform content, string text, float size, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.MidlineLeft)
	{
		RectTransform rectTransform = RowRoot(content, "TextRow", 30f);
		TextMeshProUGUI textMeshProUGUI = NewText(skin, rectTransform, "Text", 4f, 0f, 494f, 28f, FontLoader.FontRole.Noto_Sans, size, color, alignment);
		textMeshProUGUI.text = text;
		AutoSize(textMeshProUGUI);
		NewFlat(rectTransform, "Separator", 0f, 28f, 502f, 2f, TabletStyle.ROW_SEPARATOR);
		return rectTransform;
	}

	public static FooterRow FooterLine(TabletSkin skin, Transform footer, float y, string label, string value, Color? color = null)
	{
		Color color2 = color ?? TabletStyle.INK;
		RectTransform parent = NewRect(footer, "FooterRow", 12f, y, 506f, 35f);
		FooterRow result = default(FooterRow);
		result.label = NewText(skin, parent, "Label", 0f, 0f, 260f, 33f, FontLoader.FontRole.Noto_Sans, 20f, color2, TextAlignmentOptions.MidlineLeft);
		result.label.text = label;
		AutoSize(result.label);
		result.value = NewText(skin, parent, "Value", 206f, 0f, 300f, 33f, FontLoader.FontRole.Noto_Sans, 24f, color2, TextAlignmentOptions.MidlineRight);
		result.value.text = value;
		AutoSize(result.value);
		result.dots = NewImage(parent, "Dots", 2f, 33f, 502f, 2f, skin.dottedLine, TabletStyle.INK);
		result.dots.type = Image.Type.Tiled;
		return result;
	}

	public static void NoKeyboardFocus(params Selectable[] selectables)
	{
		foreach (Selectable selectable in selectables)
		{
			if (selectable != null)
			{
				selectable.navigation = new Navigation
				{
					mode = Navigation.Mode.None
				};
			}
		}
	}

	private static TabletButton BuildButton(TabletSkin skin, Transform parent, string name, float x, float y, float width, float height, TabletButton.Kind kind, string label, Action onClick)
	{
		RectTransform rectTransform = NewRect(parent, name, x, y, width, height);
		TabletButton tabletButton = rectTransform.gameObject.AddComponent<TabletButton>();
		tabletButton.kind = kind;
		tabletButton.background = rectTransform.gameObject.AddComponent<Image>();
		tabletButton.background.raycastTarget = true;
		tabletButton.group = rectTransform.gameObject.AddComponent<CanvasGroup>();
		tabletButton.button = rectTransform.gameObject.AddComponent<Button>();
		tabletButton.button.targetGraphic = tabletButton.background;
		tabletButton.button.transition = Selectable.Transition.None;
		NoKeyboardFocus(tabletButton.button);
		if (onClick != null)
		{
			tabletButton.button.onClick.AddListener(delegate
			{
				onClick();
			});
		}
		float num = ((kind == TabletButton.Kind.Large) ? 4f : 4f);
		tabletButton.outer = NewStretched(rectTransform, "Outer", skin.frame2, TabletStyle.SCREEN);
		tabletButton.face = NewImage(rectTransform, "Face", num + 2f, num + 2f, width - 2f * (num + 2f), height - 2f * (num + 2f), null, TabletStyle.INK);
		tabletButton.inner = NewImage(rectTransform, "Inner", num, num, width - 2f * num, height - 2f * num, skin.frame2, TabletStyle.SCREEN);
		tabletButton.label = NewText(skin, rectTransform, "Label", num, num, width - 2f * num, height - 2f * num, (kind == TabletButton.Kind.Large) ? FontLoader.FontRole.Lubrifont : FontLoader.FontRole.Noto_Sans, (kind == TabletButton.Kind.Large) ? 22f : 16f, TabletStyle.SCREEN, TextAlignmentOptions.Center);
		tabletButton.label.text = label;
		AutoSize(tabletButton.label, 12f);
		tabletButton.SetInteractable(value: true);
		return tabletButton;
	}

	public static TabletButton BigButton(TabletSkin skin, Transform parent, float x, float y, string label, Action onClick, float width = 256f)
	{
		return BuildButton(skin, parent, "Button", x, y, width, 42f, TabletButton.Kind.Large, label, onClick);
	}

	public static TabletButton SmallButton(TabletSkin skin, Transform parent, float x, float y, float width, float height, string label, Action onClick)
	{
		return BuildButton(skin, parent, "SmallButton", x, y, width, height, TabletButton.Kind.Small, label, onClick);
	}

	public static Button ArrowButton(Transform parent, string name, float x, float y, float size, Sprite normal, Sprite hover, Action onClick)
	{
		RectTransform rectTransform = NewRect(parent, name, x, y, size, size);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		image.sprite = normal;
		image.color = Color.white;
		image.raycastTarget = true;
		Button button = rectTransform.gameObject.AddComponent<Button>();
		button.targetGraphic = image;
		button.transition = Selectable.Transition.SpriteSwap;
		SpriteState spriteState = button.spriteState;
		spriteState.highlightedSprite = hover;
		spriteState.pressedSprite = hover;
		spriteState.selectedSprite = normal;
		spriteState.disabledSprite = normal;
		button.spriteState = spriteState;
		NoKeyboardFocus(button);
		if (onClick != null)
		{
			button.onClick.AddListener(delegate
			{
				onClick();
			});
		}
		return button;
	}

	public static void SetArrowEnabled(Button button, bool enabled)
	{
		if (!(button == null))
		{
			button.interactable = enabled;
			Image image = button.targetGraphic as Image;
			if (image != null)
			{
				image.color = new Color(1f, 1f, 1f, enabled ? 1f : 0.3f);
			}
		}
	}

	public static TabletCheckbox Checkbox(TabletSkin skin, Transform parent, float x, float y, bool on, Action<bool> onChanged)
	{
		RectTransform rectTransform = NewRect(parent, "Checkbox", x, y, 20f, 20f);
		Image image = rectTransform.gameObject.AddComponent<Image>();
		image.color = Color.clear;
		image.raycastTarget = true;
		TabletCheckbox tabletCheckbox = rectTransform.gameObject.AddComponent<TabletCheckbox>();
		tabletCheckbox.frame = NewStretched(rectTransform, "Frame", skin.frame2, TabletStyle.INK);
		float num = 6f;
		tabletCheckbox.fill = NewFlat(rectTransform, "Fill", num, num, 8f, 8f, TabletStyle.INK);
		tabletCheckbox.button = rectTransform.gameObject.AddComponent<Button>();
		tabletCheckbox.button.targetGraphic = image;
		tabletCheckbox.button.transition = Selectable.Transition.None;
		NoKeyboardFocus(tabletCheckbox.button);
		tabletCheckbox.button.onClick.AddListener(tabletCheckbox.Toggle);
		tabletCheckbox.onChanged = onChanged;
		tabletCheckbox.SetOn(on);
		return tabletCheckbox;
	}

	public static void LayoutSection(TabletFrame frame, float infoHeight, bool withSeparator, float bodyBottom)
	{
		float num = 54f;
		if (infoHeight > 0f)
		{
			WildNetworkUI.SetRect(frame.infoHost, 0f, num, 530f, infoHeight);
			frame.infoHost.gameObject.SetActive(value: true);
			num += infoHeight + 16f;
		}
		else
		{
			frame.infoHost.gameObject.SetActive(value: false);
		}
		if (withSeparator)
		{
			frame.separatorTop.gameObject.SetActive(value: true);
			WildNetworkUI.SetRect(frame.separatorTop.rectTransform, 0f, num, 530f, 2f);
			num += 18f;
		}
		else
		{
			frame.separatorTop.gameObject.SetActive(value: false);
		}
		float num2 = Mathf.Max(0f, bodyBottom - num);
		WildNetworkUI.SetRect(frame.body, 0f, num, 530f, num2);
		if (frame.arrowsWatermark != null)
		{
			WildNetworkUI.SetRect(frame.arrowsWatermark.rectTransform, 168f, num + (num2 - 259f) * 0.5f, 194f, 259f);
		}
	}

	public static void ScrollToTop(ScrollRect scroll)
	{
		if (!(scroll == null))
		{
			Canvas.ForceUpdateCanvases();
			scroll.verticalNormalizedPosition = 1f;
		}
	}
}
