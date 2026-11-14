public class OperationListLL
{
	public static StoreEvent Riot()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "ll_riot";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_riot_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_riot_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_riot_desc");
		storeEvent.duration = 1;
		storeEvent.cooldown = 8;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.revPowerMod = 10;
		storeEvent.civilUnrestMod = -40;
		storeEvent.negociationDatas.Add(new NegociationData("FOOD", 30, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WEAPON", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("AMMUNITION", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.isBlockByLockDown = true;
		return storeEvent;
	}

	public static StoreEvent WorkerStrike()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "ll_worker_strike";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_worker_strike_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_worker_strike_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_worker_strike_desc");
		storeEvent.duration = 3;
		storeEvent.cooldown = 5;
		storeEvent.importance = 6;
		storeEvent.InitNormalEvent();
		storeEvent.revPowerMod = 5;
		storeEvent.civilUnrestMod = 20;
		storeEvent.negociationDatas.Add(new NegociationData("WATER", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_RATION", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("FOOD", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.isBlockByLockDown = true;
		return storeEvent;
	}
}
