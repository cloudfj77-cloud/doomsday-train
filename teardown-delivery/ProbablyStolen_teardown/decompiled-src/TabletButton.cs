using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabletButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public enum Kind
	{
		Large,
		Small
	}

	public Kind kind;

	public Image background;

	public Image outer;

	public Image inner;

	public Image face;

	public TextMeshProUGUI label;

	public Button button;

	public CanvasGroup group;

	private bool hovered;

	private bool selected;

	private bool interactable = true;

	public bool Interactable => interactable;

	public void SetInteractable(bool value)
	{
		interactable = value;
		if (button != null)
		{
			button.interactable = value;
		}
		if (!value)
		{
			hovered = false;
		}
		Refresh();
	}

	public void SetSelected(bool value)
	{
		selected = value;
		Refresh();
	}

	public void SetLabel(string text)
	{
		if (label != null)
		{
			label.text = text;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovered = true;
		Refresh();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovered = false;
		Refresh();
	}

	private void OnDisable()
	{
		hovered = false;
		Refresh();
	}

	private void Refresh()
	{
		bool flag = hovered && interactable;
		if (group != null)
		{
			group.alpha = ((interactable || selected) ? 1f : ((kind == Kind.Large) ? 0.3f : 0.5f));
		}
		if (kind == Kind.Large)
		{
			if (background != null)
			{
				background.color = TabletStyle.INK;
			}
			if (face != null)
			{
				face.color = (flag ? TabletStyle.INK_HOVER : TabletStyle.INK);
			}
			return;
		}
		Color color = (selected ? TabletStyle.GREEN : (flag ? TabletStyle.INK_HOVER : TabletStyle.GREY_FACE));
		if (background != null)
		{
			background.color = color;
		}
		if (face != null)
		{
			face.color = color;
		}
	}
}
