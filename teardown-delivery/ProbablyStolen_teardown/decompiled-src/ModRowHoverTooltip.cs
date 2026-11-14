using UnityEngine;
using UnityEngine.EventSystems;

public class ModRowHoverTooltip : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public string text;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!string.IsNullOrEmpty(text) && !(TooltipManager._instance == null))
		{
			TooltipManager._instance.SetAndShowTooltip(text, this);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (TooltipManager._instance != null)
		{
			TooltipManager._instance.HideTooltip(this);
		}
	}

	private void OnDisable()
	{
		if (TooltipManager._instance != null)
		{
			TooltipManager._instance.HideTooltip(this);
		}
	}
}
