public class OperationListBM
{
	public static StoreEvent GangWar()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "bm_gang_war";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_gang_war_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_gang_war_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_gang_war_desc");
		storeEvent.duration = 3;
		storeEvent.cooldown = 10;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.negociationDatas.Add(new NegociationData("MEDICAL", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.bmPowerMod = -50;
		storeEvent.civilUnrestMod = 20;
		storeEvent.upperLevelFriendlinessMod = 15;
		storeEvent.isBlockByLockDown = true;
		return storeEvent;
	}

	public static StoreEvent IllegalWeaponShipment()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "bm_illegal_weapon_shipment";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_weapon_shipment_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_weapon_shipment_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_weapon_shipment_desc");
		storeEvent.duration = 2;
		storeEvent.cooldown = 8;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.bmPowerMod = 20;
		storeEvent.civilUnrestMod = 10;
		storeEvent.negociationDatas.Add(new NegociationData("WEAPON", -35, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("AMMUNITION", -35, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}
}
