public class OperationListSec
{
	public static StoreEvent IncreasePatrols()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "sec_increase_patrols";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_patrols_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_patrols_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_patrols_desc");
		storeEvent.duration = 4;
		storeEvent.cooldown = 6;
		storeEvent.importance = 6;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_SECURITY");
		storeEvent.revPowerMod = -15;
		storeEvent.bmPowerMod = -15;
		storeEvent.secPowerMod = -20;
		storeEvent.civilUnrestMod = -40;
		return storeEvent;
	}

	public static StoreEvent Crackdown()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "crackdown";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_crackdown_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_crackdown_desc");
		storeEvent.duration = 2;
		storeEvent.cooldown = 3;
		storeEvent.importance = 6;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_SECURITY");
		storeEvent.bmPowerMod = -40;
		storeEvent.secPowerMod = -20;
		storeEvent.civilUnrestMod = -20;
		return storeEvent;
	}

	public static StoreEvent RaidBlackMarket()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "sec_raid_black_market";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_raid_bm_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_raid_bm_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_raid_bm_desc");
		storeEvent.duration = 2;
		storeEvent.cooldown = 5;
		storeEvent.importance = 5;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_SECURITY");
		storeEvent.secPowerMod = -15;
		storeEvent.bmPowerMod = -40;
		storeEvent.civilUnrestMod = -10;
		storeEvent.negociationDatas.Add(new NegociationData("NARCOTIC", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WEAPON", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("AMMUNITION", 40, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent RaidRevolution()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "sec_raid_revolution";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_raid_rev_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_raid_rev_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_raid_rev_desc");
		storeEvent.duration = 1;
		storeEvent.cooldown = 8;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_SECURITY");
		storeEvent.secPowerMod = -20;
		storeEvent.revPowerMod = -40;
		storeEvent.civilUnrestMod = 0;
		return storeEvent;
	}

	public static StoreEvent MartialLaw()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "sec_martial_law";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_martial_law_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_martial_law_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_martial_law_desc");
		storeEvent.duration = 3;
		storeEvent.cooldown = 10;
		storeEvent.importance = 9;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_SECURITY");
		storeEvent.secPowerMod = -50;
		storeEvent.revPowerMod = -50;
		storeEvent.bmPowerMod = -50;
		storeEvent.civilUnrestMod = -50;
		storeEvent.isLockDown = true;
		return storeEvent;
	}

	public static StoreEvent BudgetIncrease()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "sec_budget_increase";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_budget_increase_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_budget_increase_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_budget_increase_desc");
		storeEvent.duration = 1;
		storeEvent.cooldown = 10;
		storeEvent.importance = 4;
		storeEvent.InitNormalEvent();
		storeEvent.InitOperation("FACTION_SECURITY");
		storeEvent.secPowerMod = 50;
		return storeEvent;
	}
}
