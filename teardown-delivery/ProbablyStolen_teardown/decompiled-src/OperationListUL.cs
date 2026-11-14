public class OperationListUL
{
	public static StoreEvent MedicalAid()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "ul_medical_aid";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_medical_aid_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_medical_aid_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_medical_aid_desc");
		storeEvent.duration = 2;
		storeEvent.cooldown = 5;
		storeEvent.importance = 5;
		storeEvent.InitNormalEvent();
		storeEvent.revPowerMod = -10;
		storeEvent.civilUnrestMod = -20;
		storeEvent.upperLevelFriendlinessMod = -25;
		storeEvent.negociationDatas.Add(new NegociationData("MEDICAL", -25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("FOOD", -10, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent FoodDrive()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "ul_food_drive";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_food_drive_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_food_drive_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_food_drive_desc");
		storeEvent.duration = 3;
		storeEvent.importance = 5;
		storeEvent.InitNormalEvent();
		storeEvent.secPowerMod = 10;
		storeEvent.revPowerMod = -5;
		storeEvent.civilUnrestMod = -15;
		storeEvent.negociationDatas.Add(new NegociationData("FOOD", -50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER", -20, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_RATION", -20, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent HouseholdRelief()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "ul_household_relief";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_op_household_relief_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_op_household_relief_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_op_household_relief_desc");
		storeEvent.duration = 2;
		storeEvent.importance = 5;
		storeEvent.InitNormalEvent();
		storeEvent.upperLevelFriendlinessMod = -20;
		storeEvent.civilUnrestMod = -15;
		storeEvent.negociationDatas.Add(new NegociationData("HOUSEHOLD_GOOD", -40, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}
}
