public class StoreClientInstance
{
	private GameGridInventory gridInv;

	private StoreClient storeClient;

	public StoreClientInstance()
	{
		gridInv = new GameGridInventory(10, 10);
	}

	public static StoreClientInstance CreateClientInstance(StoreClient storeClient)
	{
		StoreClientInstance result = new StoreClientInstance
		{
			storeClient = storeClient
		};
		LocGenderHelper.ResolveClientNames(storeClient);
		return result;
	}

	public StoreClient GetClientBlueprint()
	{
		return storeClient;
	}

	public static bool IsClientBusinessDone(StoreClientInstance currentInstance)
	{
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.BUY)
		{
			if (currentInstance.storeClient.useClientBudget && currentInstance.storeClient.GetBudget() <= 0)
			{
				return true;
			}
			return false;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.SELL)
		{
			if (IsCurrentClientStillHaveThingToSell())
			{
				return false;
			}
			return true;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.SELLNBUY)
		{
			return false;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.BARTER)
		{
			return true;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.INFORMATION_DEALER)
		{
			return true;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.PROCUREMENT_OFFER)
		{
			return true;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.PROCUREMENT_COLLECT)
		{
			return true;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.GUNSMITH)
		{
			return true;
		}
		if (currentInstance.storeClient.clientIntent == StoreClient.ClientIntent.EXPEDITION)
		{
			return true;
		}
		return false;
	}

	public static bool IsCurrentClientStillHaveThingToSell()
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			foreach (GameItem childItem in EmporiumEntry.Instance.frontInvinvElement.childItems)
			{
				if (!GeneralHelper.IsItemOwned(childItem))
				{
					return true;
				}
			}
			foreach (GameItem childItem2 in EmporiumEntry.Instance.backInvinvElement.childItems)
			{
				if (!GeneralHelper.IsItemOwned(childItem2))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}
}
