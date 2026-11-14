using QFSW.QC;

public static class GunOrderDebug
{
	[Command("gun-service-test", "Queues a custom gun order customer with a service order (they bring their own gun to be modded) as the next client: tier 1 Basic, 2 Mid, 3 High", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string ServiceTest(int tier = 1)
	{
		if (tier < 1 || tier > 3)
		{
			return "Tier must be 1 (Basic), 2 (Mid) or 3 (High).";
		}
		GunOrderData gunOrderData = GunOrderData.CreateServiceOrder(tier);
		StoreClient storeClient = tier switch
		{
			2 => StoreClientListGun.CreateMercGunBuyer(gunOrderData), 
			1 => StoreClientListGun.CreateLLGunBuyer(gunOrderData), 
			_ => StoreClientListGun.CreateULBuyer(gunOrderData), 
		};
		PlayerStore.Instance.storeClientManager.AddNextClient(storeClient);
		return $"Queued '{storeClient.identifier}' with a service order (fee {gunOrderData.GetBaseReward()} credits). Their gun and its requirements appear when they reach the counter.";
	}

	[Command("gun-order-test", "Queues a custom gun order customer as the next client (tier 1 Basic, 2 Mid, 3 High) and starts a gun price event: mob justice (+33%), or the Black Market weapon shipment (-35%) when surplus is true", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Test(int tier = 1, bool surplus = false)
	{
		StoreClient storeClient;
		switch (tier)
		{
		case 1:
			storeClient = StoreClientListGun.CreateLLGunBuyer();
			break;
		case 2:
			storeClient = StoreClientListGun.CreateMercGunBuyer();
			break;
		case 3:
			storeClient = StoreClientListGun.CreateULBuyer();
			break;
		default:
			return "Tier must be 1 (Basic), 2 (Mid) or 3 (High).";
		}
		StoreEvent storeEvent = (surplus ? OperationListBM.IllegalWeaponShipment() : EventListNormal.CreateLowerLevelMobJustice());
		StoreEventManager storeEventManager = StoreStation.Instance.storeEventManager;
		string arg;
		if (storeEventManager.IsEventActive(storeEvent))
		{
			arg = "'" + storeEvent.identifier + "' was already running";
		}
		else
		{
			storeEventManager.ActivateEvent(storeEvent, debugLog: false, force: true);
			arg = $"started '{storeEvent.identifier}' for {storeEvent.duration} day(s)";
		}
		PlayerStore.Instance.storeClientManager.AddNextClient(storeClient);
		return $"Queued '{storeClient.identifier}' ({storeClient.gunOrderData?.tier} tier order) as the next client; {arg}. " + $"Gun market modifier now {GunOrderData.GetMarketModifierPercent():+0;-0;0}%, order base reward {storeClient.gunOrderData?.GetBaseReward()} credits.";
	}
}
