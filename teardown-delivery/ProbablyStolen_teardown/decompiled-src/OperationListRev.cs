public class OperationListRev
{
	public static StoreEvent MobilizationSecret()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "rev_mobilization_secret";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_mob_secret_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_mob_secret_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_mob_secret_desc");
		storeEvent.duration = 3;
		storeEvent.cooldown = 2;
		storeEvent.importance = 4;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_REVOLUTION");
		storeEvent.negociationDatas.Add(new NegociationData("WEAPON", 25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("AMMUNITION", 25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.revPowerMod = 15;
		return storeEvent;
	}

	public static StoreEvent Mobilization()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "rev_mobilization";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_mobilization_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_mobilization_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_mobilization_desc");
		storeEvent.duration = 2;
		storeEvent.cooldown = 12;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_REVOLUTION");
		storeEvent.negociationDatas.Add(new NegociationData("WEAPON", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("AMMUNITION", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("MEDICAL", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("ARMOR", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.revPowerMod = 40;
		storeEvent.isBlockByLockDown = true;
		return storeEvent;
	}

	public static StoreEvent RaidUpperLevel()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "rev_raid_upper_level";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_raid_upper_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_raid_upper_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_raid_upper_desc");
		storeEvent.duration = 1;
		storeEvent.cooldown = 5;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_REVOLUTION");
		storeEvent.negociationDatas.Add(new NegociationData("HOUSEHOLD_GOOD", -30, storeEvent.newsName, storeEvent.displayName));
		storeEvent.revPowerMod = 15;
		storeEvent.upperLevelFriendlinessMod = -50;
		storeEvent.isBlockByLockDown = true;
		return storeEvent;
	}

	public static StoreEvent RaidSecurity()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "raid_security";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_raid_sec_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_raid_sec_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_raid_sec_desc");
		storeEvent.duration = 1;
		storeEvent.cooldown = 5;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_REVOLUTION");
		storeEvent.negociationDatas.Add(new NegociationData("MEDICAL", 75, storeEvent.newsName, storeEvent.displayName));
		storeEvent.revPowerMod = -25;
		storeEvent.secPowerMod = -35;
		storeEvent.civilUnrestMod = 15;
		storeEvent.isBlockByLockDown = true;
		return storeEvent;
	}
}
