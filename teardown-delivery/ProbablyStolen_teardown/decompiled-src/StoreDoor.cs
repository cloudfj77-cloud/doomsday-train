public class StoreDoor : TooltipInterface, IClickable
{
	private Interactable interactable;

	public override string GetTooltip()
	{
		if (HealthData.IsSeriouslyWounded())
		{
			return LocHelper.GetLocalizedInteractable("door_wounded");
		}
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED && PlayerStore.Instance.visitLeftTonight == 0)
		{
			return LocHelper.GetLocalizedInteractable("door_too_late");
		}
		if (StartingPerkContent.IsAgoraphobiaResting())
		{
			int agoraphobiaRestNightsLeft = StartingPerkContent.GetAgoraphobiaRestNightsLeft();
			if (agoraphobiaRestNightsLeft > 1)
			{
				return LocHelper.GetLocalizedInteractable("door_agoraphobia_rest", agoraphobiaRestNightsLeft);
			}
			return LocHelper.GetLocalizedInteractable("door_agoraphobia_rest_last");
		}
		if (StoreStation.Instance.storeEventManager.IsEventActive("crackdown"))
		{
			return LocHelper.GetLocalizedInteractable("door_crackdown");
		}
		if (PlayerStore.Instance.storeState != 0)
		{
			return LocHelper.GetLocalizedInteractable("door_day_not_over");
		}
		return LocHelper.GetLocalizedInteractable("door_leave_store");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.OnStoreDoorClicked();
		}
		if (PlayerStore.Instance.devMode)
		{
			Clicked();
		}
		else if (!HealthData.IsSeriouslyWounded() && !StoreStation.Instance.storeEventManager.IsEventActive("crackdown") && !StartingPerkContent.IsAgoraphobiaResting() && PlayerStore.Instance.isAfterhoursIntroduced && PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED && PlayerStore.Instance.visitLeftTonight > 0)
		{
			Clicked();
		}
	}

	public void Clicked()
	{
		interactable.OnDeselected();
		if (MapUIManager.Instance.IsShowingGoOutsideConfirm())
		{
			MapUIManager.Instance.OpenGoOutsideConfirm();
		}
		else
		{
			StoreResponse.ShowAfterhourResponse();
		}
	}

	public void Awake()
	{
		interactable = GetComponent<Interactable>();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
