using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartingPerkElement : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public string id;

	public bool isSelected;

	public Image icon;

	public Image background;

	public StartingPerk perk;

	public static bool showTooltip;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (NewGameData.Instance.isInMainMenu && !PerkUIController.Instance.isFirstRun)
		{
			MainMenuUIController.Instance.OnAnyButtonClick();
			if (isSelected)
			{
				PerkUIController.Instance.DeselectPerk(this);
				isSelected = false;
			}
			else if (CanSelect())
			{
				PerkUIController.Instance.SelectPerk(this);
				isSelected = true;
			}
		}
	}

	public bool CanSelect()
	{
		foreach (StartingPerk startingPerk in NewGameData.Instance.startingPerks)
		{
			if (startingPerk.incompatiblePerks.Contains(id))
			{
				return false;
			}
		}
		return true;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		showTooltip = true;
		SetTooltipContent(perk);
		PerkUIController.Instance.ShowTooltip();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		showTooltip = false;
		PerkUIController.Instance.HideTooltip();
	}

	private void Start()
	{
		icon.sprite = StartingPerkIconLoader.GetIcon(perk);
		if (perk.type == StartingPerk.StartingPerkType.POSITIVE)
		{
			icon.color = PerkUIController.Instance.green;
		}
		else if (perk.type == StartingPerk.StartingPerkType.NEGATIVE)
		{
			icon.color = PerkUIController.Instance.red;
		}
		else if (perk.type == StartingPerk.StartingPerkType.NEUTRAL)
		{
			icon.color = PerkUIController.Instance.orange;
		}
	}

	private void Update()
	{
		if (showTooltip)
		{
			Vector3 tooltipPositionSimple = UIHelper.GetTooltipPositionSimple();
			tooltipPositionSimple.z = PerkUIController.Instance.startingPerkTooltip.transform.position.z;
			PerkUIController.Instance.startingPerkTooltip.transform.position = tooltipPositionSimple;
		}
	}

	public void SetTooltipContent(StartingPerk perk)
	{
		PerkUIController.Instance.startingPerkTooltipTitle.text = perk.GetLocalizedDisplayName();
		PerkUIController.Instance.startingPerkTooltipDescription.text = perk.GetLocalizedDescription();
		PerkUIController.Instance.startingPerkTooltipCost.text = perk.cost + " " + LocHelper.GetLocalizedPerkTable("perk_points");
		PerkUIController.Instance.startingPerkTooltipIncompatible.text = "";
	}
}
