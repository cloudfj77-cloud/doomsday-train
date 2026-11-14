using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogBubbleClickAdvance : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public Action onClick;

	public static DialogBubbleClickAdvance Attach(GameObject target, Action onClick)
	{
		if (target == null)
		{
			return null;
		}
		DialogBubbleClickAdvance dialogBubbleClickAdvance = target.GetComponent<DialogBubbleClickAdvance>();
		if (dialogBubbleClickAdvance == null)
		{
			dialogBubbleClickAdvance = target.AddComponent<DialogBubbleClickAdvance>();
		}
		dialogBubbleClickAdvance.onClick = onClick;
		return dialogBubbleClickAdvance;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && (!(ItemSelectHandler.current != null) || !ItemSelectHandler.current.IsEquipped))
		{
			onClick?.Invoke();
		}
	}
}
