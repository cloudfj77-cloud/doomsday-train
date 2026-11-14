using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WildNetworkSlot : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public string id;

	public Action<string> onClick;

	public Action<string, bool> onHover;

	public void OnPointerEnter(PointerEventData eventData)
	{
		onHover?.Invoke(id, arg2: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onHover?.Invoke(id, arg2: false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			onClick?.Invoke(id);
		}
	}

	private void OnDisable()
	{
		onHover?.Invoke(id, arg2: false);
	}
}
