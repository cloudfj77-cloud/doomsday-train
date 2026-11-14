public class StoreTablet : TooltipInterface, IClickable
{
	public override string GetTooltip()
	{
		return LocHelper.GetLocalizedUI("inte_tablet_hover");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		TutorialUIManager.Instance.BuyTabletOpened();
		NegociationUIManager.Instance.ToggleUI();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
