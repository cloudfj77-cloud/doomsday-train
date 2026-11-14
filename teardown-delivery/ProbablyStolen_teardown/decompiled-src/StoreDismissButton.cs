using UnityEngine;

public class StoreDismissButton : TooltipInterface, IHoldClickable, IClickable
{
	public AudioSource audioSource;

	public float HoldDuration => 0.4f;

	public bool ShouldRequireHold()
	{
		if (!EmporiumOption.Instance.instantDismissCustomer && PlayerStore.Instance.lockNextBellFlag == 0 && PlayerStore.Instance.IsClientPresent())
		{
			if (!PlayerStore.Instance.IsClientDimissable())
			{
				return PlayerStore.Instance.devMode;
			}
			return true;
		}
		return false;
	}

	public override string GetTooltip()
	{
		if (PlayerStore.Instance.lockNextBellFlag > 0)
		{
			return LocHelper.GetLocalizedUI("inte_dimiss_cant");
		}
		if (PlayerStore.Instance.IsClientPresent())
		{
			if (PlayerStore.Instance.IsClientDimissable())
			{
				return LocHelper.GetLocalizedUI("inte_dimiss_no_call");
			}
			return LocHelper.GetLocalizedUI("inte_dimiss_cant");
		}
		return LocHelper.GetLocalizedUI("inte_dimiss_no_call");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		if (PlayerStore.Instance.lockNextBellFlag <= 0)
		{
			audioSource.Play();
			if (PlayerStore.Instance.IsClientPresent() && (PlayerStore.Instance.IsClientDimissable() || PlayerStore.Instance.devMode))
			{
				PlayerStore.Instance.TryPlayerDismissCurrentClient(callNextWhenPossible: false);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
