using UnityEngine;

public class StoreSecButton : TooltipInterface, IHoldClickable, IClickable
{
	public AudioSource audioSource;

	public SpriteRenderer light;

	public Sprite normale;

	public Sprite lit;

	public float HoldDuration => 0.4f;

	public static StoreSecButton Instance { get; private set; }

	public bool ShouldRequireHold()
	{
		if (!EmporiumOption.Instance.instantCallSecurity && PlayerStore.Instance.IsClientPresent() && !StoreReputation.IsPerkUnlocked("SEC_ALARM_IGNORED") && !PlayerStore.Instance.currentClientInstance.GetClientBlueprint().reported)
		{
			return IsClientArrestable();
		}
		return false;
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public override string GetTooltip()
	{
		if (PlayerStore.Instance.IsClientPresent())
		{
			if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().reported)
			{
				return LocHelper.GetLocalizedUI("inte_sec_reported");
			}
			if (BargainUIManager.Instance != null && BargainUIManager.Instance.IsCurrentClientCoerced())
			{
				return LocHelper.GetLocalizedUI("inte_sec_cant");
			}
			if (!PlayerStore.Instance.currentClientInstance.GetClientBlueprint().dismissable && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().nonDismissableArrestable)
			{
				return LocHelper.GetLocalizedUI("inte_sec_call");
			}
			if (!PlayerStore.Instance.currentClientInstance.GetClientBlueprint().dismissable || !PlayerStore.Instance.currentClientInstance.GetClientBlueprint().arrestable)
			{
				return LocHelper.GetLocalizedUI("inte_sec_cant");
			}
			return LocHelper.GetLocalizedUI("inte_sec_call");
		}
		return LocHelper.GetLocalizedUI("inte_sec_call");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		audioSource.Play();
		if (!StoreReputation.IsPerkUnlocked("SEC_ALARM_IGNORED") && PlayerStore.Instance.IsClientPresent())
		{
			PlayerStore.Instance.OnCustomerReported();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!PlayerStore.Instance.IsClientPresent())
		{
			light.sprite = lit;
			return;
		}
		bool isClientBeingArrested = PlayerStore.Instance.isClientBeingArrested;
		bool reported = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().reported;
		if (PlayerStore.Instance.IsClientPresent() && IsClientArrestable() && !isClientBeingArrested && !reported && !PlayerStore.Instance.isHandlingShootout)
		{
			light.sprite = normale;
		}
		else
		{
			light.sprite = lit;
		}
	}

	public bool IsClientArrestable()
	{
		if (PlayerStore.Instance.isHandlingShootout || StoreUIManager.Instance.isShot)
		{
			return false;
		}
		if (BargainUIManager.Instance != null && BargainUIManager.Instance.IsCurrentClientCoerced())
		{
			return false;
		}
		if (!PlayerStore.Instance.currentClientInstance.GetClientBlueprint().dismissable && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().nonDismissableArrestable)
		{
			return true;
		}
		if (!PlayerStore.Instance.currentClientInstance.GetClientBlueprint().dismissable || !PlayerStore.Instance.currentClientInstance.GetClientBlueprint().arrestable)
		{
			return false;
		}
		return true;
	}

	public void ResetLight()
	{
	}
}
