using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabletScrollRect : ScrollRect
{
	public override void OnInitializePotentialDrag(PointerEventData eventData)
	{
		Forward(eventData, ExecuteEvents.initializePotentialDrag);
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		Forward(eventData, ExecuteEvents.beginDragHandler);
	}

	public override void OnDrag(PointerEventData eventData)
	{
		Forward(eventData, ExecuteEvents.dragHandler);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		Forward(eventData, ExecuteEvents.endDragHandler);
	}

	private void Forward<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> handler) where T : IEventSystemHandler
	{
		if (base.transform.parent != null)
		{
			ExecuteEvents.ExecuteHierarchy(base.transform.parent.gameObject, eventData, handler);
		}
	}
}
