public class StoreBedroll : TooltipInterface, IClickable
{
	public override string GetTooltip()
	{
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED)
		{
			return LocHelper.GetLocalizedUI("inte_sleep_normal");
		}
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.OPEN)
		{
			return LocHelper.GetLocalizedUI("inte_sleep_cant_open");
		}
		return LocHelper.GetLocalizedUI("inte_sleep_cant");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED)
		{
			PaperUIManager.Instance.CloseAllPaperUIForSleep();
			EndOfDayUIManager.Instance.OpenUI();
			EmporiumAudioPlayer.Instance.audioBedroll.Play();
			TutorialUIManager.Instance.OnSleep();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
