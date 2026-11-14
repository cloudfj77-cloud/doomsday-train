using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	private bool isCurrentlySelected;

	public static bool isHoveringInteractable;

	public static InteractableUI currentHoveringInteractable;

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		OnSelected();
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
	{
		isHoveringInteractable = true;
		currentHoveringInteractable = this;
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		isHoveringInteractable = false;
		currentHoveringInteractable = null;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnSelected()
	{
		Debug.Log("Selected InteractableUI");
		RTSController.selectedObjects.Add(base.gameObject);
		isCurrentlySelected = true;
	}

	public void OnDeselected()
	{
		isCurrentlySelected = false;
		RTSController.selectedObjects.Remove(base.gameObject);
		EmporiumRTSController.Instance.OnDeselected();
	}

	public static void DeselectCurrents()
	{
		RTSController.previousSelectedObjects.Clear();
		foreach (GameObject item in RTSController.selectedObjects.ToList())
		{
			InteractableUI component2;
			if (item.TryGetComponent<Interactable>(out var component))
			{
				RTSController.previousSelectedObjects.Add(component.gameObject);
				component.OnDeselected();
			}
			else if (item.TryGetComponent<InteractableUI>(out component2))
			{
				RTSController.previousSelectedObjects.Add(component2.gameObject);
				component2.OnDeselected();
			}
		}
	}
}
