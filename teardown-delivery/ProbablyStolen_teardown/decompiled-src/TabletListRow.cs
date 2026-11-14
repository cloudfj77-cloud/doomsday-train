using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabletListRow : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public GameObject hoverVisual;

	public Action onClick;

	public bool selected;

	public bool selectable = true;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (selectable && !selected && hoverVisual != null)
		{
			hoverVisual.SetActive(value: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!selected && hoverVisual != null)
		{
			hoverVisual.SetActive(value: false);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && selectable)
		{
			onClick?.Invoke();
		}
	}

	private void OnDisable()
	{
		if (!selected && hoverVisual != null)
		{
			hoverVisual.SetActive(value: false);
		}
	}
}
