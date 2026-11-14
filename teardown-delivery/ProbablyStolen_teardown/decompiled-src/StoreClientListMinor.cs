using System;
using System.Collections.Generic;

public class StoreClientListMinor
{
	private const int NUCLEAR_WASTE_FEE_PER_BARREL = 100;

	public static void InitMinorClientList()
	{
		PlayerStore.Instance.unseenMinorClient = new List<string>
		{
			"stationEngineerMinor", "minerMinor", "nurseMinor", "scavWaterMinor", "scavHaul", "patrolOfficer", "woundedMan", "upperLevelVisitorMinor", "minorMerc", "annoyedEngineer",
			"salvagePilot", "sickLowers", "lowerLevelElder", "oldScav1", "nurseMinor2", "lover", "farmerMinor", "lotteryWinnerMinor", "peatMinorClient", "conspiracyMinor",
			"radioactiveWasteCustomer"
		};
		InitUnseenMinorDict();
	}

	public static void InitUnseenMinorDict()
	{
		PlayerStore.Instance.unseenMinorDict = new Dictionary<string, Func<StoreClient>>
		{
			{ "stationEngineerMinor", CreateStationEngineer },
			{ "minerMinor", CreateMinerMinor },
			{ "nurseMinor", CreateNurse1 },
			{ "scavWaterMinor", CreateScavWater },
			{ "scavHaul", CreateScavHaul },
			{ "patrolOfficer", CreatePatrolOfficer },
			{ "woundedMan", CreateWounded },
			{ "upperLevelVisitorMinor", CreateUpperLevelVisitor },
			{ "minorMerc", CreateMinorMerc },
			{ "annoyedEngineer", CreateAnnoyedEngineer },
			{ "salvagePilot", CreateSalvagePilot },
			{ "sickLowers", CreateSickLowers },
			{ "lowerLevelElder", CreateLowerLevelElder },
			{ "oldScav1", CreateOldScav },
			{ "nurseMinor2", CreateNurse2 },
			{ "lover", CreateLover },
			{ "farmerMinor", CreateFarmerMinor },
			{ "lotteryWinnerMinor", CreateLotteryWinner },
			{ "peatMinorClient", CreatePeatClient },
			{ "conspiracyMinor", CreateConspiracyClient },
			{ "upperLevelServantHoboParty", CreateUpperLevelServant },
			{ "radioactiveWasteCustomer", CreateRadioactiveWasteCustomer }
		};
	}

	public static StoreClient CreateClientFromId(string id)
	{
		if (PlayerStore.Instance.unseenMinorDict != null && PlayerStore.Instance.unseenMinorDict.ContainsKey(id) && PlayerStore.Instance.unseenMinorDict[id] != null)
		{
			return PlayerStore.Instance.unseenMinorDict[id]();
		}
		Debug.Log("Warning: Possible Error in Minor Client Generation");
		return null;
	}

	public static StoreClient CreateStationEngineer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "stationEngineerMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_station_engineer");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomEngSpriteName();
		storeClient.SetBudget(250 + RNG.GetRandomInt(0, 250));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_engineer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_engineer_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_engineer_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_engineer_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_engineer_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_engineer_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_engineer_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("TOOL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMinerMinor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "minerMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_miner");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomMinerSpriteName();
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 25));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_miner_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_miner_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_miner_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_miner_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_miner_5"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("NARCOTIC");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateNurse1()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nurseMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_mid_level_nurse");
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -33;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse1_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse1_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse1_4"))
			.SetEndAction(delegate
			{
				bool isStolen = false;
				if (PlayerStore.Instance.isStolenItemIntroduced)
				{
					isStolen = true;
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen);
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen);
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill", isOwend: false, isStolen);
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill", isOwend: false, isStolen);
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill", isOwend: false, isStolen);
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill", isOwend: false, isStolen);
				PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateNurse2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "nurseMinor2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_nurse");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(125);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse2_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse2_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse2_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse2_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse2_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nurse2_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScavWater()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavWaterMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sick_scav");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.SetBudget(175);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_water_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_water_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_water_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_water_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_water_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_water_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water");
			});
		storeClient.willAcceptItem = (GameItem targetItem) => WaterHelper.WillAcceptWater(targetItem, new List<string> { WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY });
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_water_right"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScavHaul()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavHaul";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -25;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_scav_haul_9"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePatrolOfficer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "patrolOfficer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_officer");
		storeClient.clientFaction = "FACTION_SECURITY";
		storeClient.isSecurity = true;
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(75);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_patrol_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_patrol_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_patrol_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_patrol_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_patrol_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_patrol_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_patrol_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("CIGARETTE_TAG");
		storeClient.clientBuyingIdList.Add("caffeine_pill");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateWounded()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "woundedMan";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_wounded_man");
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.spriteName = SpriteDict.GetRandomBMMaleSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(100);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_wounded_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_wounded_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_wounded_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_wounded_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_wounded_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_wounded_6"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUpperLevelVisitor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperLevelVisitorMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_citizen");
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(500);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_visitor_11"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MAKESHIFT");
		storeClient.clientBuyingTagList.Add("LOWTECH");
		storeClient.clientBuyingTagList.Add("PROCESSED_FOOD");
		storeClient.clientBuyingTagList.Add("NARCOTIC");
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.clientBlackTagList.Add("LUXURY_ITEM");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMinorMerc()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "minorMerc";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_mercenary");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(300);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_merc_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_merc_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_merc_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_merc_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_merc_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_merc_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_merc_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.clientBlackTagList.Add("LUXURY_ITEM");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateAnnoyedEngineer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "annoyedEngineer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_annoyed_engineer");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomEngSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(80);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsTag("FOOD"))
			{
				return true;
			}
			return targetItem.IsGameItemType("WATER_RATION") || WaterHelper.WillAcceptWater(targetItem, new List<string>
			{
				WaterFeatureHelper.PURE_WATER_DISPLAY,
				WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY,
				WaterFeatureHelper.BASE_WATER_DISPLAY
			});
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_annoyed_eng_right"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSalvagePilot()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "salvagePilot";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_salvage_ship_pilot");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.getRandomPilotSpriteName();
		storeClient.SetWholesale(-33);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_salvage_pilot_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_salvage_pilot_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_salvage_pilot_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_salvage_pilot_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_salvage_pilot_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_salvage_pilot_6"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSickLowers()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "sickLowers";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sick_lower_levels_citizen");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(100);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_sick_lowers_8"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLowerLevelElder()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowerLevelElder";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_levels_elder");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomElderSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(125);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_elder_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_elder_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_elder_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_elder_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_elder_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_elder_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_elder_7"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateOldScav()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "OldScav1";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_old_scav");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomElderSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.SetBudget(0);
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_6"))
			.NextDialogue()
			.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_player"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_12"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHeavyHandmadeGun(ItemFeatureList.EquipmentCondition.Fair));
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
				PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsTag("FOOD"))
			{
				return true;
			}
			if (targetItem.IsTag("BREVAGE"))
			{
				return true;
			}
			if (targetItem.IsTag("HOUSEHOLD_GOOD"))
			{
				return true;
			}
			return targetItem.IsGameItemType("WATER_RATION") || WaterHelper.WillAcceptWater(targetItem, new List<string>
			{
				WaterFeatureHelper.PURE_WATER_DISPLAY,
				WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY,
				WaterFeatureHelper.BASE_WATER_DISPLAY
			});
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_old_scav_right"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLover()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lover";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_desperate_lover");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(150);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lover_9"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("TOOL");
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFarmerMinor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "farmerMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_hydroponic_farmer");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomFarmerSpriteName();
		storeClient.SetWholesale(-25);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_farmer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_farmer_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_farmer_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_farmer_4"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 8; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLotteryWinner()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lotteryWinnerMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_citizen");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.SetBudget(300);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lottery_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lottery_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lottery_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lottery_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lottery_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_lottery_6"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePeatClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "peatMinorClient";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(25);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_peat_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_peat_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_peat_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_peat_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_peat_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_peat_6"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 6; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateConspiracyClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "conspiracyMinor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_conspiracy_theorist");
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomScavMaleSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.SetWholesale(33);
		storeClient.SetBudget(88);
		storeClient.AcceptingContrabandOverride = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_9"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("glass_shard_shiv");
				PlayerStore.Instance.AddDirectSellingItemToTable("glass_shard_shiv");
				PlayerStore.Instance.AddDirectSellingItemToTable("glass_shard_shiv");
				PlayerStore.Instance.AddDirectSellingItemToTable("pipe_weapon");
				PlayerStore.Instance.AddDirectSellingItemToTable("pipe_weapon");
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_11"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_conspiracy_12"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUpperLevelServant()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperLevelServantHoboParty";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_servant");
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(350);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_ul_servant_8"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("TREAT");
		storeClient.clientBuyingTagList.Add("BREVAGE");
		storeClient.clientBlackTagList.Add("LUXURY_ITEM");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRadioactiveWasteCustomer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "radioactiveWasteCustomer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_corporate_waste_manager");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue choiceNextDialogue = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_minor_nuclear_choice_1"));
		Dialogue choiceNextDialogue2 = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_minor_nuclear_choice_3"));
		Dialogue choiceNextDialogue3 = new Dialogue().SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_minor_nuclear_choice_5"));
		Dialogue dialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nuclear_offer"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nuclear_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_minor_nuclear_2"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 5; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nuclear_waste");
				}
			})
			.SetNextDialogue(dialogue);
		dialogue.AddChoice(DirectoryMaster.Item("nuclear_waste"), "", "", choiceNextDialogue, delegate
		{
			TakeNuclearWaste(1);
		}).AddChoice(DirectoryMaster.Item("nuclear_waste"), "", "", choiceNextDialogue2, delegate
		{
			TakeNuclearWaste(3);
		}).AddChoice(DirectoryMaster.Item("nuclear_waste"), "", "", choiceNextDialogue3, delegate
		{
			TakeNuclearWaste(5);
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static void TakeNuclearWaste(int barrels)
	{
		PlayerStore.Instance.RemoveNotOwnedItemFromTable();
		int num = 0;
		for (int i = 0; i < barrels; i++)
		{
			if (PlayerStore.Instance.TryAddDirectSellingItemToTable("nuclear_waste", isOwend: true))
			{
				num++;
			}
		}
		if (num > 0)
		{
			PlayerStore.Instance.ModCash(100 * num);
		}
		DialogUIManager.Instance.CurrentClientLeave();
	}
}
