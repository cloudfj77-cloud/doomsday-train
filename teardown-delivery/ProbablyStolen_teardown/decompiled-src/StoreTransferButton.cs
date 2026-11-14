using System.Collections.Generic;
using UnityEngine;

public class StoreTransferButton : TooltipInterface, IClickable
{
	public AudioSource audioSource;

	public override string GetTooltip()
	{
		return LocHelper.GetLocalizedUI("inte_transfer");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		audioSource.Play();
		EmporiumEntry instance = EmporiumEntry.Instance;
		bool flag = instance.frontInvWindow != null && instance.frontInvWindow.IsVisible();
		if (flag && instance.frontInvinvElement.childItems.Count != 0)
		{
			foreach (GameItem item in new List<GameItem>(instance.frontInvinvElement.childItems))
			{
				GraphUtils.TryAcceptAll(instance.backInvinvElement, item);
			}
		}
		else
		{
			instance.TransferOwnedItemBackToInv();
			if (flag)
			{
				instance.TransferOwnedItemFrontToInv();
			}
		}
		TutorialUIManager.Instance.OnTransferButtonClicked();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
