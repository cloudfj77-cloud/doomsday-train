public class StoreMagnifyingGlass : TooltipInterface, IClickable
{
	public override string GetTooltip()
	{
		return LocHelper.GetLocalizedInteractable("magnifying_screen");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		if (CanEnterMode())
		{
			StoreUIManager.Instance.EnterMagnifyingMode();
		}
	}

	public bool CanEnterMode()
	{
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return false;
		}
		if (PlayerStore.Instance.isClientBeingArrested)
		{
			return false;
		}
		if (PlayerStore.Instance.isHandlingShootout)
		{
			return false;
		}
		return true;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
