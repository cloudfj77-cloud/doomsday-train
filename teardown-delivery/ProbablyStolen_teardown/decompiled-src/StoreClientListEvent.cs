using System.Collections.Generic;

public class StoreClientListEvent
{
	private static readonly LocalizedLines iceMinerGreetingLines = LocalizedLines.Dialogue("dialog_ice_miner_greeting_1", "dialog_ice_miner_greeting_2", "dialog_ice_miner_greeting_3", "dialog_ice_miner_greeting_4", "dialog_ice_miner_greeting_5");

	private static readonly LocalizedLines iceMinerOfferLines = LocalizedLines.Dialogue("dialog_ice_miner_offer_1", "dialog_ice_miner_offer_2", "dialog_ice_miner_offer_3", "dialog_ice_miner_offer_4", "dialog_ice_miner_offer_5");

	private static readonly LocalizedLines foremanGreetingLines = LocalizedLines.Dialogue("dialog_foreman_greeting_1", "dialog_foreman_greeting_2", "dialog_foreman_greeting_3", "dialog_foreman_greeting_4", "dialog_foreman_greeting_5");

	private static readonly LocalizedLines foremanDealLines = LocalizedLines.Dialogue("dialog_foreman_deal_1", "dialog_foreman_deal_2", "dialog_foreman_deal_3", "dialog_foreman_deal_4");

	private static readonly LocalizedLines scavengerGreetingLines = LocalizedLines.Dialogue("dialog_scav_household_greeting_1", "dialog_scav_household_greeting_2", "dialog_scav_household_greeting_3", "dialog_scav_household_greeting_4", "dialog_scav_household_greeting_5");

	private static readonly LocalizedLines scavengerOfferLines = LocalizedLines.Dialogue("dialog_scav_household_offer_1", "dialog_scav_household_offer_2", "dialog_scav_household_offer_3", "dialog_scav_household_offer_4", "dialog_scav_household_offer_5");

	private static readonly LocalizedLines panicBuyerGreetingLines = LocalizedLines.Dialogue("dialog_panic_buyer_greeting_1", "dialog_panic_buyer_greeting_2", "dialog_panic_buyer_greeting_3", "dialog_panic_buyer_greeting_4", "dialog_panic_buyer_greeting_5");

	private static readonly LocalizedLines panicBuyerOfferLines = LocalizedLines.Dialogue("dialog_panic_buyer_offer_1", "dialog_panic_buyer_offer_2", "dialog_panic_buyer_offer_3", "dialog_panic_buyer_offer_4", "dialog_panic_buyer_offer_5");

	private static readonly LocalizedLines corporateBuyerOfferLines = LocalizedLines.Dialogue("dialog_corporate_buyer_offer_1", "dialog_corporate_buyer_offer_2");

	private static readonly LocalizedLines foodBuyerGreetingLines = LocalizedLines.Dialogue("dialog_food_buyer_greeting_1", "dialog_food_buyer_greeting_2", "dialog_food_buyer_greeting_3", "dialog_food_buyer_greeting_4", "dialog_food_buyer_greeting_5");

	private static readonly LocalizedLines foodBuyerOfferLines = LocalizedLines.Dialogue("dialog_food_buyer_offer_1", "dialog_food_buyer_offer_2", "dialog_food_buyer_offer_3", "dialog_food_buyer_offer_4", "dialog_food_buyer_offer_5");

	private static readonly LocalizedLines pharmacistGreetingLines = LocalizedLines.Dialogue("dialog_pharmacist_greeting_1", "dialog_pharmacist_greeting_2", "dialog_pharmacist_greeting_3", "dialog_pharmacist_greeting_4", "dialog_pharmacist_greeting_5");

	private static readonly LocalizedLines farmerOfferLines = LocalizedLines.Dialogue("dialog_farmer_seed_1", "dialog_farmer_seed_2", "dialog_farmer_seed_3", "dialog_farmer_seed_4");

	public static StoreClient CreateIceMiner()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "iceMiner";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_ice_miner");
		storeClient.spriteName = SpriteDict.getRandomPilotSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetWholesale(-RNG.GetRandomInt(15, 25));
		storeClient.SetBudget(50 + RNG.GetRandomInt(15, 25));
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.mainDialogue.SetText(storeClient.displayName, iceMinerGreetingLines[RNG.GetRandomInt(0, iceMinerGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, iceMinerOfferLines[RNG.GetRandomInt(0, iceMinerOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateHighQualityWater("large_bottled_water"));
				int num = RNG.GetRandomInt(0, 1);
				if (StoreStation.Instance.dayCounter >= 15)
				{
					num++;
				}
				if (StoreStation.Instance.dayCounter >= 45)
				{
					num++;
				}
				for (int i = 0; i < num; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateHighQualityWater());
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateForeman()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "foreman";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_foreman");
		storeClient.spriteName = SpriteDict.GetRandomEngSpriteName();
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(400);
		storeClient.mainDialogue.SetText(storeClient.displayName, foremanGreetingLines[RNG.GetRandomInt(0, foremanGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, foremanDealLines[RNG.GetRandomInt(0, foremanDealLines.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScavengerHouseholdClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavengerHousehold";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(15, 20);
		storeClient.mainDialogue.SetText(storeClient.displayName, scavengerGreetingLines[RNG.GetRandomInt(0, scavengerGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, scavengerOfferLines[RNG.GetRandomInt(0, scavengerOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePanicBuyerClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "panicBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 20));
		storeClient.mainDialogue.SetText(storeClient.displayName, panicBuyerGreetingLines[RNG.GetRandomInt(0, panicBuyerGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, panicBuyerOfferLines[RNG.GetRandomInt(0, panicBuyerOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("small_bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water");
			});
		storeClient.willAcceptItem = (GameItem targetItem) => targetItem.IsGameItemType("WATER_RATION") || WaterHelper.WillAcceptWater(targetItem, new List<string>
		{
			WaterFeatureHelper.PURE_WATER_DISPLAY,
			WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY,
			WaterFeatureHelper.BASE_WATER_DISPLAY,
			WaterFeatureHelper.GHOSTWATER_DISPLAY
		});
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				string text = "";
				string currentWaterDisplay = WaterFeatureHelper.GetCurrentWaterDisplay(targetItem);
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(wrongItemString: targetItem.IsTag("WATER") ? WaterHelper.GetReactionString(currentWaterDisplay) : LocHelper.GetLocalizedDialogue("dialog_panic_buyer_wrong_item"), storeClient: storeClient, targetItem: targetItem);
			}
			else if (GeneralHelper.IsItemOwned(targetItem))
			{
				Dialogue dialogue = new Dialogue();
				dialogue.priority = 1;
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_panic_buyer_right_item"));
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCorporateBuyerClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "corporateBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_corporate_representative");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(450 + RNG.GetRandomInt(0, 50));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_corporate_buyer_1")).NextDialogue().SetText(storeClient.displayName, corporateBuyerOfferLines[RNG.GetRandomInt(0, corporateBuyerOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("large_bottled_water");
			});
		storeClient.willAcceptItem = delegate(GameItem targetItem)
		{
			if (targetItem.IsGameItemType("WATER_RATION"))
			{
				return true;
			}
			return targetItem.IsID("bottled_water_premium") || WaterHelper.WillAcceptWater(targetItem, new List<string>
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
			else if (GeneralHelper.IsItemOwned(targetItem))
			{
				Dialogue dialogue = new Dialogue();
				dialogue.priority = 1;
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_corporate_buyer_right_item"));
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateGenericWaterBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "generic_water_buyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(150 + RNG.GetRandomInt(0, 50));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_generic_water_buyer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_generic_water_buyer_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water");
				PlayerStore.Instance.AddDirectSellingItemToTable("large_bottled_water");
			});
		storeClient.willAcceptItem = (GameItem targetItem) => targetItem.IsGameItemType("WATER_RATION") || WaterHelper.WillAcceptWater(targetItem, new List<string>
		{
			WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY,
			WaterFeatureHelper.BASE_WATER_DISPLAY
		});
		storeClient.onItemAddedToWeighted = delegate(GameItem targetItem)
		{
			if (!storeClient.willAcceptItem(targetItem))
			{
				WaterFeatureHelper.OnWrongItemPlaceWaterClient(storeClient, targetItem);
			}
			else if (GeneralHelper.IsItemOwned(targetItem))
			{
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_generic_water_buyer_right_item"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFoodBuyerClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "foodBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_food_buyer");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(75 + RNG.GetRandomInt(0, 25));
		storeClient.mainDialogue.SetText(storeClient.displayName, foodBuyerGreetingLines[RNG.GetRandomInt(0, foodBuyerGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, foodBuyerOfferLines[RNG.GetRandomInt(0, foodBuyerOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePharmacistClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "pharmacist";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_pharmacist");
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(150 + RNG.GetRandomInt(0, 50));
		storeClient.mainDialogue.SetText(storeClient.displayName, pharmacistGreetingLines[RNG.GetRandomInt(0, pharmacistGreetingLines.Count - 1)]).SetEndAction(delegate
		{
		});
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.IsPersonalConsumption = false;
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFarmerSeedClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "farmerSeedBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_farmer");
		storeClient.spriteName = SpriteDict.GetRandomFarmerSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(100 + RNG.GetRandomInt(0, 50));
		storeClient.budgetConversionRate = 100;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -20;
		storeClient.mainDialogue.SetText(storeClient.displayName, farmerOfferLines[RNG.GetRandomInt(0, farmerOfferLines.Count - 1)]).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
			}
		});
		storeClient.clientBuyingTagList.Add("SEED");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLynchingCitizen()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lynching_citizen";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_angry_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(RNG.GetRandomInt(75, 150));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_lynching_greeting_1"),
			LocHelper.GetLocalizedDialogue("dialog_lynching_greeting_2"),
			LocHelper.GetLocalizedDialogue("dialog_lynching_greeting_3")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_lynching_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_lynching_deal_2"),
			LocHelper.GetLocalizedDialogue("dialog_lynching_deal_3")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePowerEventLLBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowerLevelCitizenPowerBuying";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(RNG.GetRandomInt(150, 200));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_power_ll_greeting_1"),
			LocHelper.GetLocalizedDialogue("dialog_power_ll_greeting_2"),
			LocHelper.GetLocalizedDialogue("dialog_power_ll_greeting_3"),
			LocHelper.GetLocalizedDialogue("dialog_power_ll_greeting_4"),
			LocHelper.GetLocalizedDialogue("dialog_power_ll_greeting_5")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_power_ll_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_power_ll_deal_2"),
			LocHelper.GetLocalizedDialogue("dialog_power_ll_deal_3")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePowerMaintenanceEventLLBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowerLevelCitizenMaintenanceBuying";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(RNG.GetRandomInt(150, 200));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_power_maint_greeting_1"),
			LocHelper.GetLocalizedDialogue("dialog_power_maint_greeting_2")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_power_maint_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_power_maint_deal_2"),
			LocHelper.GetLocalizedDialogue("dialog_power_maint_deal_3")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePowerEventULBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperLevelCitizenPowerBuying";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_resident");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.SetBudget(RNG.GetRandomInt(325, 400));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_power_ul_greeting_1"),
			LocHelper.GetLocalizedDialogue("dialog_power_ul_greeting_2"),
			LocHelper.GetLocalizedDialogue("dialog_power_ul_greeting_3")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_power_ul_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_power_ul_deal_2")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("ENERGY_CELL");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePanickedShopkeeperBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "panickedShopkeeperWeaponBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_panicked_shopkeeper");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.SetBudget(RNG.GetRandomInt(150, 300));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_panicked_shop_greeting_1"),
			LocHelper.GetLocalizedDialogue("dialog_panicked_shop_greeting_2"),
			LocHelper.GetLocalizedDialogue("dialog_panicked_shop_greeting_3")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_panicked_shop_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_panicked_shop_deal_2"),
			LocHelper.GetLocalizedDialogue("dialog_panicked_shop_deal_3")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_panicked_shop_intro")).NextDialogue().SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)])
			.NextDialogue()
			.SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateClinicVolunteerClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "clinicVolunteer";
		storeClient.isBargainAvailable = false;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_clinic_volunteer");
		storeClient.spriteName = SpriteDict.GetRandomMedSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_clinic_volunteer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_clinic_volunteer_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_clinic_volunteer_3"))
			.SetEndAction(delegate
			{
				int num = 2;
				if (StoreReputation.GetLLReputation() >= 33)
				{
					num++;
				}
				if (StoreReputation.GetLLReputation() >= 66)
				{
					num++;
				}
				if (StoreReputation.GetLLReputation() >= 99)
				{
					num++;
				}
				if (StoreReputation.GetLLReputation() >= 149)
				{
					num++;
				}
				for (int i = 0; i < num; i++)
				{
					GameItem gameItem = DirectoryMaster.Item("pure_white_injector");
					gameItem.AddItemFeature(ItemFeatureList.FreeFeature());
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}
