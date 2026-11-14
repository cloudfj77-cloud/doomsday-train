using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class NewGameCommonButton
{
	public Button button;

	public Image background;

	public Image outer;

	public Image inner;

	public TextMeshProUGUI label;

	[NonSerialized]
	public bool hovered;

	public static NewGameCommonButton Build(NewGamePanelFrame frame, Transform parent, string name, float x, float y, float width, float height, string labelText, UnityAction onClick)
	{
		NewGameCommonButton newGameCommonButton = new NewGameCommonButton();
		RectTransform rectTransform = NewGameUIElements.NewRect(parent, name, x, y, width, height);
		newGameCommonButton.background = rectTransform.gameObject.AddComponent<Image>();
		newGameCommonButton.background.color = NewGameStyle.BUTTON_BG;
		newGameCommonButton.background.raycastTarget = true;
		newGameCommonButton.outer = NewGameUIElements.NewImage(rectTransform, "Outer", 0f, 0f, width, height, frame.frame2Square, NewGameStyle.BUTTON_BORDER_COLOR);
		float num = 4f;
		newGameCommonButton.inner = NewGameUIElements.NewImage(rectTransform, "Inner", num, num, width - 2f * num, height - 2f * num, frame.frame2Square, NewGameStyle.BUTTON_BORDER_COLOR);
		newGameCommonButton.label = NewGameUIElements.NewText(frame, rectTransform, "Label", num, num, width - 2f * num, height - 2f * num, FontLoader.FontRole.Lubrifont, 22f, NewGameStyle.BUTTON_BORDER_COLOR, TextAlignmentOptions.Center, wrap: false);
		WildNetworkUI.AutoSize(newGameCommonButton.label, 12f);
		newGameCommonButton.label.text = labelText;
		newGameCommonButton.button = rectTransform.gameObject.AddComponent<Button>();
		newGameCommonButton.button.targetGraphic = newGameCommonButton.background;
		newGameCommonButton.button.transition = Selectable.Transition.None;
		if (onClick != null)
		{
			newGameCommonButton.button.onClick.AddListener(onClick);
		}
		newGameCommonButton.WireHover(name);
		return newGameCommonButton;
	}

	public void WireHover(string id)
	{
		if (!(button == null))
		{
			WildNetworkSlot wildNetworkSlot = button.GetComponent<WildNetworkSlot>();
			if (wildNetworkSlot == null)
			{
				wildNetworkSlot = button.gameObject.AddComponent<WildNetworkSlot>();
			}
			wildNetworkSlot.id = id;
			wildNetworkSlot.onHover = delegate(string _, bool entering)
			{
				hovered = entering;
				Refresh();
			};
			Refresh();
		}
	}

	public void SetInteractable(bool interactable)
	{
		if (!(button == null))
		{
			button.interactable = interactable;
			Refresh();
		}
	}

	public void Refresh()
	{
		if (!(button == null))
		{
			Color color = ((!button.interactable) ? NewGameStyle.WithAlpha(NewGameStyle.BUTTON_BORDER_COLOR, 0.3f) : (hovered ? NewGameStyle.BUTTON_HOVER : NewGameStyle.BUTTON_BORDER_COLOR));
			if (outer != null)
			{
				outer.color = color;
			}
			if (inner != null)
			{
				inner.color = color;
			}
			if (label != null)
			{
				label.color = color;
			}
		}
	}
}
