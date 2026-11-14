using System.Collections.Generic;
using TMPro;
using UnityEngine.Localization;

public class StoreClientCounter : TooltipInterface
{
	public TextMeshProUGUI clientCounterTMP;

	public override string GetTooltip()
	{
		int count = PlayerStore.Instance.storeClientManager.clientStack.Count;
		return new LocalizedString("UI", "inte_client_counter").GetLocalizedString(new Dictionary<string, object> { { "count", count } });
	}

	public override bool IsTooltipEnabled()
	{
		return PlayerStore.Instance.storeState == PlayerStore.StoreState.OPEN;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (PlayerStore.Instance.storeState == PlayerStore.StoreState.CLOSED || PlayerStore.Instance.storeState == PlayerStore.StoreState.MORNING)
		{
			if (clientCounterTMP != null)
			{
				clientCounterTMP.text = "";
			}
		}
		else if (PlayerStore.Instance.storeState == PlayerStore.StoreState.OPEN && clientCounterTMP != null)
		{
			clientCounterTMP.text = PlayerStore.Instance.storeClientManager.clientStack.Count.ToString() ?? "";
		}
	}
}
