using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransitCardView : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IScrollHandler
{
	public AfterhourLocation location;

	public Image outline;

	public bool available;

	public ScrollRect scroll;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && !eventData.dragging)
		{
			TransitUI.Select(location);
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (scroll != null && scroll.gameObject.activeInHierarchy)
		{
			scroll.OnScroll(eventData);
		}
	}
}
