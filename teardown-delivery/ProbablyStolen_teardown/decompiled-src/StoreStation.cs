using System.Collections.Generic;

public class StoreStation
{
	public const int STARTING_ORDER = 80;

	[ES3Serializable]
	public int order = 80;

	[ES3Serializable]
	public int traffic = 100;

	[ES3Serializable]
	public int happiness = 80;

	[ES3Serializable]
	public StoreEventManager storeEventManager;

	[ES3Serializable]
	public StoreOperationManager storeOperationManager;

	[ES3Serializable]
	public int dayCounter;

	[ES3Serializable]
	public int todayGutterflowPurity = 8000;

	[ES3Serializable]
	public int todayRustwaterPurity = 9000;

	private static StoreStation instance = null;

	private static readonly object padlock = new object();

	public static StoreStation Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new StoreStation();
				}
				return instance;
			}
		}
	}

	public StoreStation()
	{
		storeEventManager = new StoreEventManager();
		storeOperationManager = new StoreOperationManager();
	}

	public static void ResetData()
	{
		instance = null;
	}

	public void StartDay()
	{
		dayCounter++;
		storeEventManager.OnDayStart();
		if (dayCounter % 5 == 0 && PlayerStore.Instance.wrongfulReportCount > 0)
		{
			PlayerStore.Instance.wrongfulReportCount--;
		}
		todayGutterflowPurity = RNG.GetRandomInt(8000, 8999);
		todayRustwaterPurity = RNG.GetRandomInt(9000, 9150);
		HandleFaction();
		HandleOrder();
		HandleStoreRequests();
		foreach (NetworkUpgrade value in PlayerStore.Instance.networkUpgrade.Values)
		{
			value.OnDayStart();
		}
	}

	public void HandleFaction()
	{
		foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
		{
			storeReputation.actionCooldown--;
			if (storeReputation.actionCooldown < 0)
			{
				storeReputation.actionCooldown = 0;
			}
			storeReputation.influence += storeReputation.strength;
		}
	}

	public void HandleOrder()
	{
	}

	public void HandleStoreRequests()
	{
		List<StoreRequest> storeRequests = PlayerStore.Instance.storeRequests;
		for (int num = storeRequests.Count - 1; num >= 0; num--)
		{
			storeRequests[num].duration--;
			if (storeRequests[num].duration <= 0)
			{
				storeRequests.RemoveAt(num);
			}
		}
		StoreRequest.StartDay();
		PCUIManager.Instance.storePC.RefreshState();
	}

	public static int GetDayCounter()
	{
		if (Instance == null)
		{
			return 0;
		}
		return Instance.dayCounter;
	}

	public bool IsStationOnLockdown()
	{
		foreach (StoreEvent activeEvent in storeEventManager.GetActiveEvents())
		{
			if (activeEvent.isLockDown)
			{
				return true;
			}
		}
		return false;
	}
}
