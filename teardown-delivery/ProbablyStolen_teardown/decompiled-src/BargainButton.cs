using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BargainButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private BargainType bargainType;

	[SerializeField]
	private int percent;

	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	public void Setup(BargainType type, int magnitude)
	{
		bargainType = type;
		percent = magnitude;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!(BargainUIManager.Instance == null))
		{
			BargainUIManager.Instance.SetRepPreview(bargainType, percent);
			if ((!(button != null) || button.IsInteractable()) && TooltipManager._instance != null)
			{
				TooltipManager._instance.SetAndShowTooltip(BargainUIManager.Instance.GetBargainTooltip(bargainType, percent), this);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (BargainUIManager.Instance != null)
		{
			BargainUIManager.Instance.ClearRepPreview();
		}
		if (TooltipManager._instance != null)
		{
			TooltipManager._instance.HideTooltip();
		}
	}

	private void OnDisable()
	{
		if (BargainUIManager.Instance != null)
		{
			BargainUIManager.Instance.ClearRepPreview();
		}
		if (TooltipManager._instance != null)
		{
			TooltipManager._instance.HideTooltip();
		}
	}
}
