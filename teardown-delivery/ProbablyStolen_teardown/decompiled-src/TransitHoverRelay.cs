using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransitHoverRelay : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Action<bool> onHover;

	public void OnPointerEnter(PointerEventData eventData)
	{
		onHover?.Invoke(obj: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onHover?.Invoke(obj: false);
	}

	private void OnDisable()
	{
		onHover?.Invoke(obj: false);
	}
}
