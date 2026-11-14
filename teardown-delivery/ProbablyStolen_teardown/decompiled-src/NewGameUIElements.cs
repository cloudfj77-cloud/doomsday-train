using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public static class NewGameUIElements
{
	public static void ClearChildren(RectTransform container)
	{
		if (!(container == null))
		{
			for (int num = container.childCount - 1; num >= 0; num--)
			{
				Object.Destroy(container.GetChild(num).gameObject);
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

	public static void FitPixelArt(Image image, float slotX, float slotY, float slotSize, float targetSize)
	{
		if (!(image == null) && !(image.sprite == null))
		{
			float width = image.sprite.rect.width;
			float height = image.sprite.rect.height;
			float num = Mathf.Max(width, height);
			int num2 = Mathf.Max(1, Mathf.RoundToInt(targetSize / num));
			float num3 = width * (float)num2;
			float num4 = height * (float)num2;
			float x = slotX + Mathf.Floor((slotSize - num3) * 0.5f);
			float y = slotY + Mathf.Floor((slotSize - num4) * 0.5f);
			WildNetworkUI.SetRect(image.rectTransform, x, y, num3, num4);
			image.preserveAspect = false;
			image.type = Image.Type.Simple;
		}
	}

	public static TextMeshProUGUI NewText(NewGamePanelFrame frame, Transform parent, string name, float x, float y, float width, float height, FontLoader.FontRole role, float size, Color color, TextAlignmentOptions alignment, bool wrap)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.SetActive(value: false);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		WildNetworkUI.SetRect(gameObject.transform as RectTransform, x, y, width, height);
		TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
		textMeshProUGUI.font = ((role == FontLoader.FontRole.Lubrifont) ? frame.fontLubrifont : ((role == FontLoader.FontRole.Noto_Sans_Normal && frame.fontNotoRegular != null) ? frame.fontNotoRegular : frame.fontNotoMedium));
		textMeshProUGUI.fontSize = size;
		textMeshProUGUI.color = color;
		textMeshProUGUI.alignment = alignment;
		textMeshProUGUI.enableWordWrapping = wrap;
		textMeshProUGUI.enableAutoSizing = false;
		textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
		textMeshProUGUI.raycastTarget = false;
		gameObject.AddComponent<LocalizeStringEvent>();
		TMPLocalizer tMPLocalizer = gameObject.AddComponent<TMPLocalizer>();
		tMPLocalizer.ChangeFontOnly = true;
		tMPLocalizer.fontRole = role;
		gameObject.SetActive(value: true);
		return textMeshProUGUI;
	}

	public static TMP_InputField NewInputField(NewGamePanelFrame frame, Transform parent, string name, float x, float y, float width, float height, FontLoader.FontRole role, float size, Color color, int characterLimit)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.SetActive(value: false);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		WildNetworkUI.SetRect(gameObject.transform as RectTransform, x, y, width, height);
		Image image = gameObject.AddComponent<Image>();
		image.color = Color.clear;
		image.raycastTarget = true;
		RectTransform rectTransform = NewRect(gameObject.transform, "TextArea", 0f, 0f, width, height);
		rectTransform.gameObject.AddComponent<RectMask2D>();
		TextMeshProUGUI textMeshProUGUI = NewText(frame, rectTransform, "Text", 0f, 0f, width, height, role, size, color, TextAlignmentOptions.MidlineLeft, wrap: false);
		TMP_InputField tMP_InputField = gameObject.AddComponent<TMP_InputField>();
		tMP_InputField.targetGraphic = image;
		tMP_InputField.textViewport = rectTransform;
		tMP_InputField.textComponent = textMeshProUGUI;
		tMP_InputField.fontAsset = textMeshProUGUI.font;
		tMP_InputField.pointSize = size;
		tMP_InputField.lineType = TMP_InputField.LineType.SingleLine;
		tMP_InputField.characterLimit = characterLimit;
		tMP_InputField.customCaretColor = true;
		tMP_InputField.caretColor = color;
		tMP_InputField.selectionColor = NewGameStyle.WithAlpha(color, 0.35f);
		tMP_InputField.transition = Selectable.Transition.None;
		gameObject.AddComponent<TMPInputFieldLocalizer>().fontRole = role;
		gameObject.SetActive(value: true);
		return tMP_InputField;
	}

	public static Button NewIconButton(Transform parent, string name, float x, float y, float size, Sprite normal, Sprite hover, string tooltip, UnityAction onClick)
	{
		Image image = NewImage(parent, name, x, y, size, size, normal, Color.white, raycast: true);
		Button button = image.gameObject.AddComponent<Button>();
		button.targetGraphic = image;
		button.transition = Selectable.Transition.SpriteSwap;
		SpriteState spriteState = button.spriteState;
		spriteState.highlightedSprite = hover;
		spriteState.pressedSprite = hover;
		spriteState.selectedSprite = normal;
		spriteState.disabledSprite = normal;
		button.spriteState = spriteState;
		if (onClick != null)
		{
			button.onClick.AddListener(onClick);
		}
		SetTooltip(image.gameObject, tooltip);
		return button;
	}

	public static void SetTooltip(GameObject target, string tooltip)
	{
		if (target == null)
		{
			return;
		}
		CustomUITooltipTrigger customUITooltipTrigger = target.GetComponent<CustomUITooltipTrigger>();
		if (string.IsNullOrEmpty(tooltip))
		{
			if (customUITooltipTrigger != null)
			{
				customUITooltipTrigger.tooltipText = null;
			}
			return;
		}
		if (customUITooltipTrigger == null)
		{
			customUITooltipTrigger = target.AddComponent<CustomUITooltipTrigger>();
		}
		customUITooltipTrigger.tooltipText = tooltip;
	}

	public static void RefreshContinueLook(NewGamePanelFrame frame, bool hovered)
	{
		if (!(frame == null) && !(frame.continueButton == null))
		{
			Color color = ((!frame.continueButton.interactable) ? NewGameStyle.WithAlpha(NewGameStyle.BUTTON_BORDER_COLOR, 0.3f) : (hovered ? NewGameStyle.BUTTON_HOVER : NewGameStyle.BUTTON_BORDER_COLOR));
			frame.continueOuter.color = color;
			frame.continueInner.color = color;
			frame.continueLabel.color = color;
		}
	}

	public static void SetArrow(Button button, Image image, bool enabled)
	{
		if (button != null)
		{
			button.interactable = enabled;
		}
		if (image != null)
		{
			image.color = new Color(1f, 1f, 1f, enabled ? 1f : 0.3f);
		}
	}

	public static void PlayClick()
	{
		if (MainMenuUIController.Instance != null)
		{
			MainMenuUIController.Instance.OnAnyButtonClick();
		}
	}
}
