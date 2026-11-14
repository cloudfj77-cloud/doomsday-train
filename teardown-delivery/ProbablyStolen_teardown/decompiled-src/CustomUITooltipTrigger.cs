using UnityEngine;
using UnityEngine.EventSystems;

public class CustomUITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public string tooltipText;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (CustomUIManager.Instance != null && !string.IsNullOrEmpty(tooltipText))
		{
			CustomUIManager.Instance.ShowTooltip(tooltipText, this);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (CustomUIManager.Instance != null)
		{
			CustomUIManager.Instance.HideTooltip(this);
		}
	}

	private void OnDisable()
	{
		if (CustomUIManager.Instance != null)
		{
			CustomUIManager.Instance.HideTooltip(this);
		}
	}
}
