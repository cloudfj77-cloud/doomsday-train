using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Interactable))]
public class TooltipContainer : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[TextArea]
	public string content;

	private Interactable interactable;

	private bool showTooltip;

	private TooltipInterface tooltipInterfaceInstance;

	public static bool isPaused;

	private void Awake()
	{
		interactable = GetComponent<Interactable>();
		tooltipInterfaceInstance = GetComponent<TooltipInterface>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!isPaused && (!PlayerStore.IsInstanceExist() || !PlayerStore.Instance.cinematicMode))
		{
			showTooltip = true;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		showTooltip = false;
		if (TooltipManager._instance != null)
		{
			TooltipManager._instance.HideTooltip();
		}
	}

	private void Update()
	{
		if (showTooltip)
		{
			UpdateTooltip();
		}
	}

	public void UpdateTooltip()
	{
		if (tooltipInterfaceInstance == null)
		{
			if (content == null || string.IsNullOrWhiteSpace(content) || content.Equals(""))
			{
				TooltipManager._instance.HideTooltip();
			}
			else if (!interactable.isAvailable)
			{
				TooltipManager._instance.SetAndShowTooltip("<color=grey><i>*Unavailable*</color> </i>" + content, this);
			}
			else
			{
				TooltipManager._instance.SetAndShowTooltip(content, this);
			}
		}
		else if (tooltipInterfaceInstance.IsTooltipEnabled())
		{
			TooltipManager._instance.SetAndShowTooltip(tooltipInterfaceInstance.GetTooltip(), this);
		}
		else
		{
			TooltipManager._instance.HideTooltip();
		}
	}

	public void OnDisable()
	{
		showTooltip = false;
		if (TooltipManager._instance != null)
		{
			TooltipManager._instance.HideTooltip();
		}
	}
}
