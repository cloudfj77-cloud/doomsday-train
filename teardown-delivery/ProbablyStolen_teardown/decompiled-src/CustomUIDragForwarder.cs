using UnityEngine;
using UnityEngine.EventSystems;

public class CustomUIDragForwarder : MonoBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public UIDragHandler target;

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		eventData.useDragThreshold = true;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (target != null && target.isActiveAndEnabled)
		{
			target.OnBeginDrag(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (target != null && target.isActiveAndEnabled)
		{
			target.OnDrag(eventData);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (target != null && target.isActiveAndEnabled)
		{
			target.OnEndDrag(eventData);
		}
	}
}
