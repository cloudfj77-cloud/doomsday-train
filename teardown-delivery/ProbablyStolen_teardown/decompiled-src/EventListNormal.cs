public class EventListNormal
{
	public static StoreEvent CreateNutrifruitHarvest(int duration = 3)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "nutrifruitHarvest";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_nutrifruit_harvest_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_nutrifruit_harvest_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_nutrifruit_harvest_desc");
		storeEvent.duration = duration;
		storeEvent.importance = 4;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.negociationDatas.Add(new NegociationData("FOOD", -20, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent CreateBadNutrifruitHarvest()
	{
		return CreateBadNutrifruitHarvestDuration();
	}

	public static StoreEvent CreateBadNutrifruitHarvestDuration(int duration = 3)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "badNutrifruitHarvest";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_bad_harvest_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_bad_harvest_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_bad_harvest_desc");
		storeEvent.duration = duration;
		storeEvent.onActiveActionId = "badNutrifruitHarvestOnActive";
		storeEvent.importance = 5;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.negociationDatas.Add(new NegociationData("FOOD", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateSeedHeist()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "seedShortage";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_seed_heist_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_seed_heist_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_seed_heist_desc");
		storeEvent.duration = 2;
		storeEvent.importance = 4;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.negociationDatas.Add(new NegociationData("FOOD", 33, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("SEED", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateCounterfeitMedicalSuppliesDiscovered()
	{
		return CreateCounterfeitMedicalSuppliesDiscoveredDuration();
	}

	public static StoreEvent CreateCounterfeitMedicalSuppliesDiscoveredDuration(int duration = 2)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "medicalSuppliesShortage";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_counterfeit_med_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_counterfeit_med_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_counterfeit_med_desc");
		storeEvent.duration = duration;
		storeEvent.importance = 6;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.onActiveActionId = "medicalSuppliesShortageOnActive";
		storeEvent.negociationDatas.Add(new NegociationData("MEDICAL", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateWaterTreatmentPlantBreakdown()
	{
		return CreateWaterTreatmentPlantBreakdownDuration();
	}

	public static StoreEvent CreateWaterTreatmentPlantBreakdownDuration(int duration = 3)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "water_treatment_breakdown";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_water_breakdown_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_water_breakdown_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_water_breakdown_desc");
		storeEvent.duration = duration;
		storeEvent.importance = 6;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.negociationDatas.Add(new NegociationData("WATER", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_RATION", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("BREVAGE", 45, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_PURIFICATION_SUPPLY", 40, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateScheduledTreatmentPlantMaintenance()
	{
		return CreateScheduledTreatmentPlantMaintenanceDuration();
	}

	public static StoreEvent CreateScheduledTreatmentPlantMaintenanceDuration(int duration = 2)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "scheduled_treatment_maintenance";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_water_maintenance_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_water_maintenance_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_water_maintenance_desc");
		storeEvent.duration = duration;
		storeEvent.importance = 4;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.negociationDatas.Add(new NegociationData("WATER", 35, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_RATION", 35, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("BREVAGE", 30, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_PURIFICATION_SUPPLY", 25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateMinerReturnFromIceAsteroid()
	{
		return CreateMinerReturnFromIceAsteroidDuration();
	}

	public static StoreEvent CreateMinerReturnFromIceAsteroidDuration(int duration = 2)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "miner_return_ice_asteroid";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_ice_asteroid_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_ice_asteroid_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_ice_asteroid_desc");
		storeEvent.duration = duration;
		storeEvent.importance = 4;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.LOWER;
		storeEvent.negociationDatas.Add(new NegociationData("WATER", -25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_RATION", -25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("BREVAGE", -20, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WATER_PURIFICATION_SUPPLY", -15, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateLowerLevelMobJustice()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "lower_level_mob_justice";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_mob_justice_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_mob_justice_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_mob_justice_desc");
		storeEvent.duration = 1;
		storeEvent.cooldown = 10;
		storeEvent.importance = 5;
		storeEvent.eventArea = StoreEvent.EventArea.LOWER;
		storeEvent.negociationDatas.Add(new NegociationData("WEAPON", 33, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("AMMUNITION", 33, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("ARMOR", 33, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateStoreRoomExcavated()
	{
		return CreateStoreRoomExcavatedDuration();
	}

	public static StoreEvent CreateStoreRoomExcavatedDuration(int duration = 1)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "storeroom_excavated";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_storeroom_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_storeroom_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_storeroom_desc");
		storeEvent.duration = duration;
		storeEvent.importance = 3;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.LOWER;
		storeEvent.negociationDatas.Add(new NegociationData("HOUSEHOLD_GOOD", -20, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateMiningExpeditionReturn()
	{
		return CreateMiningExpeditionReturnDuration();
	}

	public static StoreEvent CreateMiningExpeditionReturnDuration(int duration = 1)
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "miningExpeditionReturn";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_mining_return_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_mining_return_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_mining_return_desc");
		storeEvent.duration = duration;
		storeEvent.importance = 3;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.LOWER;
		storeEvent.negociationDatas.Add(new NegociationData("MATERIAL", -25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}

	public static StoreEvent CreateConstructionProject()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "construction_project";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_construction_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_construction_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_construction_desc");
		storeEvent.duration = 3;
		storeEvent.importance = 4;
		storeEvent.onActiveActionId = "construction_projectOnActive";
		storeEvent.InitNormalEvent();
		storeEvent.negociationDatas.Add(new NegociationData("MATERIAL", 30, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent CreateMeteorShower()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "meteorShower";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_meteor_shower_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_meteor_shower_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_meteor_shower_desc");
		storeEvent.duration = 3;
		storeEvent.importance = 7;
		storeEvent.onActiveActionId = "meteorShowerOnActive";
		storeEvent.InitNormalEvent();
		storeEvent.negociationDatas.Add(new NegociationData("MATERIAL", 50, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent CreatePowerShortage()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "powerShortage";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_power_shortage_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_power_shortage_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_power_shortage_desc");
		storeEvent.duration = 2;
		storeEvent.importance = 7;
		storeEvent.InitNormalEvent();
		storeEvent.negociationDatas.Add(new NegociationData("ENERGY_CELL", 66, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent SchedulePowerPlantMaintenance()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "powerPlantMaintenance";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_power_maintenance_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_power_maintenance_display");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_power_maintenance_desc");
		storeEvent.duration = 1;
		storeEvent.importance = 4;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		storeEvent.negociationDatas.Add(new NegociationData("ENERGY_CELL", 33, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent StationWideBlackout()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "stationWideBlackout";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_blackout_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_blackout_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_blackout_desc");
		storeEvent.duration = 2;
		storeEvent.importance = 9;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		storeEvent.negociationDatas.Add(new NegociationData("ENERGY_CELL", 50, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("WEAPON", 25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.negociationDatas.Add(new NegociationData("AMMUNITION", 25, storeEvent.newsName, storeEvent.displayName));
		return storeEvent;
	}

	public static StoreEvent CruiseShipDocked()
	{
		StoreEvent obj = new StoreEvent
		{
			identifier = "cruiseShipDocked",
			newsName = LocHelper.GetLocalizedMechanic("event_normal_cruise_ship_news"),
			displayName = LocHelper.GetLocalizedMechanic("event_normal_cruise_ship_display"),
			newsDescription = LocHelper.GetLocalizedMechanic("event_normal_cruise_ship_desc"),
			duration = 2,
			importance = 3,
			eventType = StoreEvent.EventType.NORMALE,
			eventArea = StoreEvent.EventArea.ALL
		};
		obj.addClientFromEventActionId = obj.identifier + "addClientAction";
		return obj;
	}

	public static StoreEvent CreateOxymoreWave()
	{
		StoreEvent storeEvent = new StoreEvent();
		storeEvent.identifier = "oxymoreWave";
		storeEvent.newsName = LocHelper.GetLocalizedMechanic("event_normal_oxymore_news");
		storeEvent.displayName = LocHelper.GetLocalizedMechanic("event_normal_oxymore_news");
		storeEvent.newsDescription = LocHelper.GetLocalizedMechanic("event_normal_oxymore_desc");
		storeEvent.duration = 2;
		storeEvent.cooldown = 5;
		storeEvent.importance = 5;
		storeEvent.eventType = StoreEvent.EventType.NORMALE;
		storeEvent.eventArea = StoreEvent.EventArea.ALL;
		storeEvent.negociationDatas.Add(new NegociationData("MEDICAL", 25, storeEvent.newsName, storeEvent.displayName));
		storeEvent.addClientFromEventActionId = storeEvent.identifier + "addClientAction";
		return storeEvent;
	}
}
