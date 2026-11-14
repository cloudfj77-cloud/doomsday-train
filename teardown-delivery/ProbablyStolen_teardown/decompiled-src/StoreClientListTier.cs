using System;

public class StoreClientListTier
{
	private static Func<StoreClient>[] t1FoodClient = new Func<StoreClient>[3] { CreateScavFood, CreateHungrySpacer, CreateSpacerChef };

	private static Func<StoreClient>[] t1MedicalClient = new Func<StoreClient>[3] { CreateInjuredSpacer, CreateSickChildCaretaker, CreateLowLevelDoctor };

	private static Func<StoreClient>[] t1MaterialClient = new Func<StoreClient>[3] { CreateLowerRepairman, CreateScrapper, CreateMaintenanceWorker };

	private static Func<StoreClient>[] t1HouseholdClient = new Func<StoreClient>[3] { CreateSpacerHousehold, CreateYoungSpacer, CreateFamilyMother };

	private static Func<StoreClient>[] t1WaterClient = new Func<StoreClient>[2] { CreateThirstySpacer, CreateHydrationSeeker };

	private static Func<StoreClient>[] clientCategories = new Func<StoreClient>[5] { GetRandomT1FoodClient, GetRandomT1MedicalClient, GetRandomT1MaterialClient, GetRandomT1HouseholdClient, GetRandomT1WaterClient };

	public static StoreClient GetRandomT1WaterClient()
	{
		int randomInt = RNG.GetRandomInt(0, t1WaterClient.Length - 1);
		return t1WaterClient[randomInt]();
	}

	public static StoreClient GetRandomT1FoodClient()
	{
		int randomInt = RNG.GetRandomInt(0, t1FoodClient.Length - 1);
		return t1FoodClient[randomInt]();
	}

	public static StoreClient GetRandomT1MedicalClient()
	{
		int randomInt = RNG.GetRandomInt(0, t1MedicalClient.Length - 1);
		return t1MedicalClient[randomInt]();
	}

	public static StoreClient GetRandomT1MaterialClient()
	{
		int randomInt = RNG.GetRandomInt(0, t1MaterialClient.Length - 1);
		return t1MaterialClient[randomInt]();
	}

	public static StoreClient GetRandomT1HouseholdClient()
	{
		int randomInt = RNG.GetRandomInt(0, t1HouseholdClient.Length - 1);
		return t1HouseholdClient[randomInt]();
	}

	public static StoreClient CreateWaterScavenger()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "waterScavenger";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBudget(60 + RNG.GetRandomInt(0, 40));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_scavenger_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_scavenger_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_scavenger_3", storeClient.GetBudget()))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_scavenger_4"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("small_bottled_water");
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("WATER_RATION"))
			{
				return true;
			}
			if (!targetItem.IsTag("WATER"))
			{
				return false;
			}
			if (WaterHelper.IsEmpty(targetItem))
			{
				return false;
			}
			string currentWaterDisplay = WaterFeatureHelper.GetCurrentWaterDisplay(targetItem);
			return (currentWaterDisplay.Equals(WaterFeatureHelper.BASE_WATER_DISPLAY) || currentWaterDisplay.Equals(WaterFeatureHelper.GHOSTWATER_DISPLAY)) ? true : false;
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_water_scavenger_wrong_item");
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem, localizedDialogue);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_scavenger_right_item"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateThirstySpacer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "thirstySpacer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_thirsty_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(90 + RNG.GetRandomInt(0, 20));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thirsty_spacer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thirsty_spacer_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thirsty_spacer_3", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water");
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("WATER_RATION"))
			{
				return true;
			}
			if (!targetItem.IsTag("WATER"))
			{
				return false;
			}
			if (WaterHelper.IsEmpty(targetItem))
			{
				return false;
			}
			string currentWaterDisplay = WaterFeatureHelper.GetCurrentWaterDisplay(targetItem);
			return (currentWaterDisplay.Equals(WaterFeatureHelper.BASE_WATER_DISPLAY) || currentWaterDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY) || currentWaterDisplay.Equals(WaterFeatureHelper.GHOSTWATER_DISPLAY)) ? true : false;
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_thirsty_spacer_wrong_item");
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem, localizedDialogue);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thirsty_spacer_right_item"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateHydrationSeeker()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavWaterShopper";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBudget(120 + RNG.GetRandomInt(0, 30));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hydration_seeker_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hydration_seeker_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water");
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("WATER_RATION"))
			{
				return true;
			}
			if (!targetItem.IsTag("WATER"))
			{
				return false;
			}
			if (WaterHelper.IsEmpty(targetItem))
			{
				return false;
			}
			string currentWaterDisplay = WaterFeatureHelper.GetCurrentWaterDisplay(targetItem);
			return (currentWaterDisplay.Equals(WaterFeatureHelper.BASE_WATER_DISPLAY) || currentWaterDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY) || currentWaterDisplay.Equals(WaterFeatureHelper.PURE_WATER_DISPLAY)) ? true : false;
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_hydration_seeker_wrong_item");
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem, localizedDialogue);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hydration_seeker_right_item"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWaterShopper()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "waterSpace2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(145 + RNG.GetRandomInt(0, 50) + StoreStation.GetDayCounter() / 4);
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_shopper_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_shopper_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_shopper_3", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("large_bottled_water");
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("WATER_RATION"))
			{
				return true;
			}
			if (!targetItem.IsTag("WATER"))
			{
				return false;
			}
			if (WaterHelper.IsEmpty(targetItem))
			{
				return false;
			}
			string currentWaterDisplay = WaterFeatureHelper.GetCurrentWaterDisplay(targetItem);
			return (currentWaterDisplay.Equals(WaterFeatureHelper.BASE_WATER_DISPLAY) || currentWaterDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY)) ? true : false;
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_water_shopper_wrong_item");
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem, localizedDialogue);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_shopper_right_item"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWaterStockPiler()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "waterStockpiler";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_water_stockpiler");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.AddBasicCheck();
		storeClient.SetBudget(500 + RNG.GetRandomInt(0, 100) + StoreStation.GetDayCounter());
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.IsPersonalConsumption = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_stockpiler_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_stockpiler_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_stockpiler_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_stockpiler_4", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_jug");
				PlayerStore.Instance.AddDirectSellingItemToTable("large_bottled_water");
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (!targetItem.IsTag("WATER"))
			{
				return false;
			}
			if (targetItem.IsID("bottled_water_premium"))
			{
				return true;
			}
			if (WaterHelper.IsEmpty(targetItem))
			{
				return false;
			}
			string currentWaterDisplay = WaterFeatureHelper.GetCurrentWaterDisplay(targetItem);
			return (currentWaterDisplay.Equals(WaterFeatureHelper.PURE_WATER_DISPLAY) || currentWaterDisplay.Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY)) ? true : false;
		};
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_water_stockpiler_wrong_item");
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem, localizedDialogue);
			}
			else
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_stockpiler_right_item"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.canClientExposeFeature = ClientCanExposeFunc.ALLExposeCapacity;
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation(IgnoreExposeFunc: true);
		return storeClient;
	}

	public static StoreClient GetRandomT1Client()
	{
		int randomInt = RNG.GetRandomInt(0, clientCategories.Length - 1);
		return clientCategories[randomInt]();
	}

	public static StoreClient CreateScavFood()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerFoodI";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 25));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scav_food_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scav_food_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateHungrySpacer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "hungrySpacer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_hungry_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 20));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hungry_spacer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hungry_spacer_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSpacerChef()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerChef";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer_cook");
		storeClient.spriteName = SpriteDict.GetRandomChefSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 20));
		storeClient.AddBasicCheck();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_chef_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_chef_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_chef_3", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateExpeditionSupplier()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "expeditionSupplier";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_expedition_supplier");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(145 + RNG.GetRandomInt(0, 50) + StoreStation.GetDayCounter() / 4);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_expedition_supplier_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_expedition_supplier_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUpperPrivateChef()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "privateChef";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_private_chef");
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomChefSpriteName();
		storeClient.SetBudget(400 + RNG.GetRandomInt(0, 100) + StoreStation.GetDayCounter());
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_chef_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_chef_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_chef_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateInjuredSpacer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerMedical";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_injured_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 25));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_injured_spacer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_injured_spacer_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSickChildCaretaker()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "sickChildCaretaker";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_sick_child_caretaker");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 20));
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sick_child_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sick_child_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLowLevelDoctor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowLevelDoctor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_low_level_doctor");
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 50));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.IsPersonalConsumption = false;
		string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_low_doctor_no_scanner");
		if (RNG.Roll(50))
		{
			storeClient.CanExposeChem = true;
			localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_low_doctor_scanner");
		}
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_low_doctor_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_low_doctor_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_low_doctor_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_low_doctor_4", storeClient.GetBudget()))
			.NextDialogue()
			.SetText(storeClient.displayName, localizedDialogue)
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateClinicNurse()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "clinicNurse";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_mid_level_nurse");
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(145 + RNG.GetRandomInt(0, 50) + StoreStation.GetDayCounter() / 4);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.IsPersonalConsumption = false;
		string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_clinic_nurse_no_scanner");
		if (RNG.Roll(75))
		{
			storeClient.CanExposeChem = true;
			localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_clinic_nurse_scanner");
		}
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_clinic_nurse_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_clinic_nurse_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, localizedDialogue)
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateHospitalAdmin()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "hospitalAdmin";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_hospital_supplier");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.SetBudget(750 + RNG.GetRandomInt(0, 250) + StoreStation.GetDayCounter());
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.CanExposeChem = true;
		storeClient.IsPersonalConsumption = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hospital_admin_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hospital_admin_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hospital_admin_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hospital_admin_4"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLowerRepairman()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowerRepairman";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_repairman");
		storeClient.spriteName = SpriteDict.GetRandomEngSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 25));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lower_repairman_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lower_repairman_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingTagList.Add("TOOL");
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScrapper()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scrap_byer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scrap_buyer");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 40));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scrapper_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scrapper_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scrapper_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scrapper_4", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingTagList.Add("TOOL");
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMaintenanceWorker()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "maintenanceWorker";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_maintenance_worker");
		storeClient.spriteName = SpriteDict.GetRandomEngSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 25));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_maintenance_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_maintenance_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_maintenance_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_maintenance_4", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingTagList.Add("TOOL");
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLowerInventor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowerInventor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_inventor");
		storeClient.spriteName = SpriteDict.GetRandomSciFemaleSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(145 + RNG.GetRandomInt(0, 150) + StoreStation.GetDayCounter() / 4);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lower_inventor_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lower_inventor_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("TOOL");
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.clientNegociationCheck.Add("CHECK_INGOT_FLASHLIGHT");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStationEngineer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "stationEngineer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_station_engineer");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomEngSpriteName();
		storeClient.SetBudget(750 + RNG.GetRandomInt(0, 250) + StoreStation.GetDayCounter());
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_station_engineer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_station_engineer_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingTagList.Add("MODULE");
		storeClient.clientBuyingTagList.Add("NODE");
		storeClient.clientBuyingTagList.Add("TOOL");
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSpacerHousehold()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerHouseholdItem";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 25));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_household_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_household_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("HOUSEHOLD_GOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateYoungSpacer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "youngSpacerHousehold";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_young_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(80 + RNG.GetRandomInt(0, 20));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_young_spacer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_young_spacer_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("HOUSEHOLD_GOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFamilyMother()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "familyMother";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_parent");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 15));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_family_mother_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_family_mother_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_family_mother_3", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("HOUSEHOLD_GOOD");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCitizen()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "citizenHousehold";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(145 + RNG.GetRandomInt(0, 50) + StoreStation.GetDayCounter() / 4);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_citizen_household_1")).SetEndAction(delegate
		{
		});
		storeClient.clientBuyingTagList.Add("HOUSEHOLD_GOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUpperMaid()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperMaid";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_servant");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.SetBudget(500 + RNG.GetRandomInt(0, 150) + StoreStation.GetDayCounter());
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_maid_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_maid_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("HOUSEHOLD_GOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}
