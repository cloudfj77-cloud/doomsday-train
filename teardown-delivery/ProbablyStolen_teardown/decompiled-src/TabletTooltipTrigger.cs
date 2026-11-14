using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabletTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Func<string> provider;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!(TooltipManager._instance == null) && provider != null)
		{
			string text = provider();
			if (!string.IsNullOrEmpty(text))
			{
				TooltipManager._instance.SetAndShowTooltip(text, this);
			}
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
