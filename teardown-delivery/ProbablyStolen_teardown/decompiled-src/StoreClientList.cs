using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreClientList
{
	public static readonly RandomClientPool generalPool = new RandomClientPool("FACTION_LOWER_LEVEL").Add(CreateMerchant, 100).Add(StoreClientListBarter.CreateAddictGeneral, 75, 30, 1).Add(StoreClientListBarter.CreateNormalFoodBarter, 75, 30, 1)
		.Add(CreateMiner, 100)
		.Add(CreateShadyPharmacist, 50)
		.Add(CreateLowerLevelChemist, 150)
		.Add(CreateSpacerClientFood, 100)
		.Add(CreateSpacerClientFoodVariety, 100)
		.Add(CreateSpacerClientAmenityVariety, 100)
		.Add(CreateJunker, 100, 80, 60)
		.Add(CreateScavGeneral, 200)
		.Add(CreateScavCrate, 150)
		.Add(CreateWaterSeller, 100)
		.Add(CreateScavBlood, 100, 66, 33)
		.Add(CreateStampSeller, 70)
		.Add(CreateMercBuyer, 100)
		.Add(CreateFoodBankBuyer, 50)
		.Add(CreateInsScav, 100);

	public static readonly RandomClientPool blackMarketPool = new RandomClientPool("FACTION_BLACK_MARKET", StoreClientManager.GetBlackMarketClientChance).Add(CreateThief, 50).Add(CreateShadyMerchant, 50).Add(CreatePettyThief, 75, 50, 25)
		.Add(CreateFoodThief, 75, 50, 25);

	public static readonly RandomClientPool upperLevelPool = new RandomClientPool("FACTION_UPPER_LEVEL", StoreClientManager.GetUpperLevelClientChance).Add(CreateUpperLowerVisitor, 100).Add(CreateUpperLowerVisitorLuxury, 50).Add(CreateCluelessTouristBuyer, 50)
		.Add(CreateBrokeUpperLevel, 30);

	public static readonly RandomClientPool revolutionPool = new RandomClientPool("FACTION_REVOLUTION", StoreClientManager.GetRevolutionClientChance).Add(StoreClientListRev.CreateRevolutionaryBuyer, 50);

	public static readonly List<RandomClientPool> factionPools = new List<RandomClientPool> { blackMarketPool, upperLevelPool, revolutionPool };

	public static List<Func<StoreClient>> buyClientList = new List<Func<StoreClient>>();

	public static List<int> buyClientListProba = new List<int>();

	public static List<Func<StoreClient>> sellClientList = new List<Func<StoreClient>> { CreateMiner, CreateShadyPharmacist, CreateLowerLevelChemist, CreateFarmerClientStandard };

	public static List<int> sellClientListProba = new List<int> { 100, 100, 100, 100 };

	public const string DISCERNING_SHOPPER_ID = "ulDiscerningShopper";

	public const string MODIFIER_INVENTOR_SELL_PRICE = "inventor_sell_price";

	public const int STORAGE_KIT_CHANCE_BASE = 15;

	public const int STORAGE_KIT_CHANCE_JACKSON1 = 45;

	public const int STORAGE_KIT_CHANCE_JACKSON2 = 75;

	public const int STORAGE_COORDINATOR_AFTER_DAY = 14;

	public const int HOPPER_AFTER_DAY = 21;

	public const int STORAGE_COORDINATOR_CHANCE = 40;

	private static readonly Dictionary<string, string> lockToCryptoModule = new Dictionary<string, string>
	{
		{ "LOCK_ID_SEC", "crypto_module_sec" },
		{ "LOCK_ID_COMMAND", "crypto_module_cmd" },
		{ "LOCK_ID_MED", "crypto_module_med" },
		{ "LOCK_ID_ENG", "crypto_module_eng" },
		{ "LOCK_ID_SUP", "crypto_module_sup" },
		{ "LOCK_ID_SER", "crypto_module_ser" }
	};

	private static readonly Dictionary<string, string> lockToKeycard = new Dictionary<string, string>
	{
		{ "LOCK_ID_SEC", "sec_keycard" },
		{ "LOCK_ID_COMMAND", "cmd_keycard" },
		{ "LOCK_ID_MED", "med_keycard" },
		{ "LOCK_ID_ENG", "eng_keycard" },
		{ "LOCK_ID_SUP", "sup_keycard" },
		{ "LOCK_ID_SER", "ser_keycard" }
	};

	public static RandomClientPool GetRandomClientPool(string factionId)
	{
		foreach (RandomClientPool factionPool in factionPools)
		{
			if (factionPool.factionId.Equals(factionId))
			{
				return factionPool;
			}
		}
		return generalPool;
	}

	public static StoreClient CreateSpacerClientFood()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerFood";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.useClientBudget = true;
		int budget = RNG.GetRandomInt(35, 50) + StoreStation.GetDayCounter() * 2;
		storeClient.SetBudget(budget);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.canClientExposeFeature = ClientCanExposeFunc.BasicClientExposeCapacity;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_food_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_food_2", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerSpacerFoodClient()
	{
		StoreClient storeClient = CreateSpacerClientFood();
		storeClient.spriteName = "trailer_scav5";
		return storeClient;
	}

	public static StoreClient CreateUpperLowerVisitor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperLowerVisitor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_visitor");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.SetBudget(RNG.GetRandomInt(250, 300) + StoreStation.GetDayCounter() * 2);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_lower_visitor_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_lower_visitor_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_lower_visitor_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUpperLowerVisitorLuxury()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperLowerVisitorLuxury";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_visitor");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.SetBudget(RNG.GetRandomInt(250, 300) + StoreStation.GetDayCounter() * 2);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_lower_visitor_luxury_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_lower_visitor_luxury_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("LUXURY_ITEM");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateDiscerningShopper()
	{
		StoreClient storeClient = CreateUpperLowerVisitorLuxury();
		storeClient.identifier = "ulDiscerningShopper";
		storeClient.SetBudget(750 + RNG.GetRandomInt(0, 250) + StoreStation.GetDayCounter());
		storeClient.cooldown = 5;
		storeClient.cooldownStartsOnArrival = true;
		storeClient.attractedType = "LUXURY_ITEM";
		return storeClient;
	}

	public static StoreClient CreatePoisonBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "poisonBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shady_buyer");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.SetBudget(RNG.GetRandomInt(150, 250));
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_poison_buyer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_poison_buyer_2"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("POISON");
		storeClient.clientBuyingTagList.Add("CHEMICAL_SUPPLIES");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSpacerClientFoodVariety()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerFoodVariety";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(RNG.GetRandomInt(100, 120) + StoreStation.GetDayCounter() * 2);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_food_variety_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_food_variety_2", storeClient.GetBudget()))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_food_variety_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.SetVariety();
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSpacerClientAmenityVariety()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "spacerAmenityVariety";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_spacer");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(RNG.GetRandomInt(50, 85) + StoreStation.GetDayCounter() * 2);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetVariety();
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_amenity_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_amenity_2", storeClient.GetBudget()))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_spacer_amenity_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("HOUSEHOLD_GOOD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateJunker()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "junker";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_junker");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -15;
		storeClient.SetClientBudget(50 + StoreStation.GetDayCounter(), 25);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("nuts_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("nuts_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
				}
				if (NetworkUpgrade.IsWinemakerUnlocked())
				{
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
					}
				}
			});
		storeClient.placedWrongItemWhenSellingToDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_wrong_item"));
		storeClient.placeRightItemWhenSellingToDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_right_item"));
		storeClient.repeatDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_repeat"));
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_accept"));
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateJunkerSellOnly()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "junker2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_junker");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_sell_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_sell_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_sell_3"))
			.SetEndAction(delegate
			{
				storeClient.isOfferWholesale = true;
				storeClient.wholesaleDiscount = -33;
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
				PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				storeClient.OnIntroduced();
			});
		storeClient.AddBasicDialog();
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_junker_sell_accept")).SetStartAction(delegate
		{
			TutorialUIManager.Instance.OnWholesaleCustomerBought();
		}).SetEndAction(delegate
		{
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFarmerClientStandard()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "farmer_standard";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_hydroponic_farmer");
		storeClient.spriteName = SpriteDict.GetRandomFarmerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetWholesale(-15);
		storeClient.SetBudget(RNG.GetRandomInt(80, 150));
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_farmer_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_greet_2"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_greet_3"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_greet_4"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_greet_5")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_farmer_harvest_1"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_harvest_2"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_harvest_3"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_harvest_4"),
			LocHelper.GetLocalizedDialogue("dialog_farmer_harvest_5")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomIntExculsive(0, list.Count)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomIntExculsive(0, list2.Count)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_farmer_seed"))
			.SetEndAction(delegate
			{
				if (NetworkUpgrade.IsWinemakerUnlocked())
				{
					int randomInt = RNG.GetRandomInt(5, 6);
					for (int i = 0; i < randomInt; i++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("wine_berry");
					}
					int randomInt2 = RNG.GetRandomInt(4, 5);
					for (int j = 0; j < randomInt2; j++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
					}
				}
				else
				{
					int randomInt3 = RNG.GetRandomInt(5, 9);
					for (int k = 0; k < randomInt3; k++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
					}
				}
			});
		storeClient.acceptDealDialogue = new Dialogue().SetText(storeClient.displayName, StoreClientDialogList.GetRandomDialogAcceptedString());
		storeClient.clientBuyingTagList.Add("SEED");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateStampSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavStamp";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scav");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_stamp_seller_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomStamp());
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomStamp());
			}
			if (StoreStation.GetDayCounter() >= 20 && RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomStamp());
			}
			if (StoreStation.GetDayCounter() >= 50 && RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomStamp());
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateInventor()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "inventor";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_dr._jackson");
		storeClient.spriteName = SpriteDict.GetInventorSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.isBargainAvailable = false;
		storeClient.nonShootable = true;
		storeClient.SetBudget(50 + StoreStation.Instance.dayCounter * 2 + StoreClientData.GetDrJacksonInvestmentBonus() * 10);
		storeClient.budgetConversionRate = 100;
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_inventor_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_greet_2"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_greet_3"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_greet_4"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_greet_5")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_inventor_offer_1"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_offer_2"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_offer_3"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_offer_4"),
			LocHelper.GetLocalizedDialogue("dialog_inventor_offer_5")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_back")).SetStartAction(delegate
		{
			PlaceInventorInventory(isVisitingPlayerStore: true);
		}).NextDialogue()
			.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)])
			.NextDialogue()
			.SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)]);
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingIdList.Add("system_module_corrupt");
		storeClient.clientBuyingIdList.Add("system_module_ruined");
		storeClient.onItemAddedToWeighted = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				ItemFeature itemFeature = ItemFeatureList.InventorMarkup();
				if (itemFeature != null)
				{
					item.AddItemFeature(itemFeature);
				}
				if (StoreReputation.IsPerkUnlocked("LLPILLAR"))
				{
					item.AddItemFeature(ItemFeatureList.DiscountBuy15());
				}
				ItemFeature itemFeature2 = ItemFeatureList.InventorEventPrice(Mathf.RoundToInt(GetInventorShopPriceChange()));
				if (itemFeature2 != null)
				{
					item.AddItemFeature(itemFeature2);
				}
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static void PlaceInventorSpecialStock()
	{
		List<GameItem> allOwnedItems = EmporiumEntry.Instance.GetAllOwnedItems();
		ModHook.FireOnPlaceInventorInventoryItemEarly(allOwnedItems);
		ReactorHelper.TryOfferStockReactor(allOwnedItems);
		PlayerStore.Instance.AddDirectSellingItemToTable("machine_bay_ext");
		PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay_large");
		PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_breeder");
		PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_breeder");
		PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_dis");
		PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_dis");
		for (int i = 0; i < CurrentConverterHelper.GetJacksonStockCount(); i++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("current_converter");
		}
		if (StoreStation.GetDayCounter() >= 21)
		{
			for (int j = 0; j < 2; j++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("intake_hopper"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("outlet_hopper"));
			}
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_cmd");
		PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_eng");
		PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_med");
		PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_sec");
		PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_ser");
		PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit");
		PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit");
		PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor_advanced");
		PlayerStore.Instance.AddDirectSellingItemToTable("advanced_flux_agent");
		PlayerStore.Instance.AddDirectSellingItemToTable("turbo_booster_adv");
		PlayerStore.Instance.AddDirectSellingItemToTable("turbo_booster_adv");
		int num = 5;
		for (int k = 0; k < num; k++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_node"));
		}
		for (int l = 0; l < num; l++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
		}
		for (int m = 0; m < num; m++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
		}
		for (int n = 0; n < num; n++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
		}
		for (int num2 = 0; num2 < num; num2++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_overclock_module"));
		}
		for (int num3 = 0; num3 < num; num3++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_eco_module"));
		}
		for (int num4 = 0; num4 < num; num4++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_fineness_module"));
		}
		ModHook.FireOnPlaceInventorInventoryItemLate(allOwnedItems);
	}

	private static float GetInventorShopPriceChange()
	{
		if (MapUIManager.Instance == null || MapUIManager.Instance.Current == null || MapUIManager.Instance.Current.Id != "dr_jackson")
		{
			return 0f;
		}
		return MapUIManager.Instance.Current.GetModifier("inventor_sell_price");
	}

	public static int GetStorageKitChance()
	{
		if (NetworkUpgrade.IsJackson2Unlocked())
		{
			return 75;
		}
		if (NetworkUpgrade.IsJackson1Unlocked())
		{
			return 45;
		}
		return 15;
	}

	public static void PlaceInventorInventory(bool isVisitingPlayerStore = false)
	{
		int chance = 66;
		List<GameItem> allOwnedItems = EmporiumEntry.Instance.GetAllOwnedItems();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GameItem item in allOwnedItems)
		{
			hashSet.Add(item.identifier);
		}
		ModHook.FireOnPlaceInventorInventoryItemEarly(allOwnedItems);
		ReactorHelper.TryOfferStockReactor(allOwnedItems);
		if (!isVisitingPlayerStore)
		{
			if (!hashSet.Contains("satchel"))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("satchel");
			}
			int drJacksonInvestmentBonus = StoreClientData.GetDrJacksonInvestmentBonus();
			if (!hashSet.Contains("backpack_large") && RNG.Roll(StoreStation.Instance.dayCounter - 30 + drJacksonInvestmentBonus))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("backpack_large");
			}
			else if (!hashSet.Contains("backpack_medium") && RNG.Roll(StoreStation.Instance.dayCounter - 12 + drJacksonInvestmentBonus))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("backpack_medium");
			}
			else if (!hashSet.Contains("backpack_small") && RNG.Roll(2 * StoreStation.Instance.dayCounter + 10 + drJacksonInvestmentBonus))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("backpack_small");
			}
		}
		RNG.Roll(StoreStation.Instance.dayCounter - 25 + StoreClientData.GetDrJacksonInvestmentBonus());
		if (!hashSet.Contains("mirage_projector") && StoreStation.Instance.dayCounter >= 10 && !PlayerStore.Instance.IsPlayerOwnThisItem("mirage_projector") && RNG.Roll(StoreStation.Instance.dayCounter))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateProjector());
		}
		if (!hashSet.Contains("security_alarm") && RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateAlarm());
		}
		if (hashSet.Contains("storage_bay"))
		{
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay");
			}
		}
		else
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay");
		}
		if (hashSet.Contains("machine_bay"))
		{
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("machine_bay");
			}
		}
		else
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("machine_bay");
		}
		if (hashSet.Contains("recharger"))
		{
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("recharger");
			}
		}
		else if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("recharger");
		}
		if (hashSet.Contains("smuggler_bay"))
		{
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("smuggler_bay");
			}
		}
		else if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("smuggler_bay");
		}
		if (hashSet.Contains("furnace"))
		{
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateFurnace());
			}
		}
		else
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateFurnace());
		}
		if (hashSet.Contains("fridge"))
		{
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("fridge");
			}
		}
		else
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("fridge");
		}
		if (hashSet.Contains("desequencer"))
		{
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("desequencer");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_sec");
			}
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_cmd");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_med");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_eng");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_ser");
			}
			PlayerStore.Instance.AddDirectSellingItemToTable("blank_keycard");
			PlayerStore.Instance.AddDirectSellingItemToTable("blank_keycard");
		}
		else if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("desequencer");
			PlayerStore.Instance.AddDirectSellingItemToTable("blank_keycard");
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_sec");
			}
			if (RNG.Roll(2))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_cmd");
			}
			if (RNG.Roll(7))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_med");
			}
			if (RNG.Roll(7))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_eng");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_ser");
			}
		}
		if (RNG.Roll(75))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("powerblock");
		}
		for (int i = 0; i < CurrentConverterHelper.GetJacksonStockCount(); i++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("current_converter");
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("alarm_module_transmitter");
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("furnace_module_blast");
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("furnace_module_junk");
		}
		int num = 1;
		for (int j = 0; j < num; j++)
		{
			if (RNG.Roll(33))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
			}
			if (RNG.Roll(33))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
			}
			if (RNG.Roll(33))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
			}
			if (RNG.Roll(16))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_overclock_module"));
			}
			if (RNG.Roll(16))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_eco_module"));
			}
			if (RNG.Roll(16))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_fineness_module"));
			}
			if (RNG.Roll(33))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_node"));
			}
			if (RNG.Roll(8))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_breeder");
			}
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCreditExt());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(100));
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(0));
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(100));
			}
		}
		if (RNG.Roll(50))
		{
			AddIfNotOwn("metal_scanner", hashSet);
		}
		if (RNG.Roll(50))
		{
			AddIfNotOwn("flashlight", hashSet);
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit");
		}
		if (StoreStation.GetDayCounter() >= 20 && RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit");
		}
		if (StoreStation.GetDayCounter() >= 40 && RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit");
		}
		if (RNG.Roll(GetStorageKitChance()))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay_expansion_kit");
		}
		AddIfNotOwn("magnifier", hashSet);
		AddIfNotOwn("labeler", hashSet);
		AddIfNotOwn("labeler_blue", hashSet);
		AddIfNotOwn("ingot_book", hashSet);
		if (StoreStation.GetDayCounter() >= 14 || PlayerStore.Instance.skipIntro)
		{
			AddIfNotOwn("toolbox", hashSet);
		}
		if (StoreStation.GetDayCounter() > 14 && RNG.Roll(40))
		{
			AddIfNotOwn("storage_coordinator", hashSet);
		}
		if (StoreStation.GetDayCounter() >= 21)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("intake_hopper"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("outlet_hopper"));
		}
		if (RNG.Roll(4))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor_advanced");
		}
		AddIfNotOwn("water_guide", hashSet);
		if (NetworkUpgrade.IsWaterTraderUnlocked())
		{
			AddIfNotOwn("aquascan", hashSet);
			if (RNG.Roll(15))
			{
				AddIfNotOwn("water_filter_adv", hashSet);
			}
			if (RNG.Roll(15))
			{
				AddIfNotOwn("portable_water_purifier_used", hashSet);
			}
			if (PlayerStore.Instance.IsPlayerOwnThisItem("uv_filter"))
			{
				if (RNG.Roll(10))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("uv_filter");
				}
			}
			else if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("uv_filter");
			}
			AddIfNotOwn("bottle_printer", hashSet);
			if (PlayerStore.Instance.IsPlayerOwnThisItem("water_purifier"))
			{
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateWaterPurifier());
				}
			}
			else
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateWaterPurifier());
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_tank");
			}
			if (NetworkUpgrade.IsJackson1Unlocked() && RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_tank");
			}
			if (NetworkUpgrade.IsJackson2Unlocked() && RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_tank");
			}
			if (PlayerStore.Instance.IsPlayerOwnThisItem("moisture_farm"))
			{
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateMoistureFarm());
				}
			}
			else
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateMoistureFarm());
			}
		}
		if (NetworkUpgrade.IsRetiredFarmerUnlocked())
		{
			AddIfNotOwn("hydroponic_guide", hashSet);
			if (PlayerStore.Instance.IsPlayerOwnThisItem("hydroponic"))
			{
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHydroponic());
				}
			}
			else if (RNG.Roll(chance))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHydroponic());
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_nutrifruit");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_dream_cap");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_kotton_loom");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_nightmare_cap");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_fillerweed_production");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_hydroreed_filter");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_smuggler");
			}
			AddIfNotOwn("plant_analyser", hashSet);
		}
		if (NetworkUpgrade.IsRetiredChemistUnlocked())
		{
			if (NetworkUpgrade.IsJackson2Unlocked() && RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("chem_module");
			}
			if (NetworkUpgrade.IsJackson1Unlocked() && RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("chem_module");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("chem_module");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("chem_module");
			}
		}
		if (NetworkUpgrade.IsPlantDnaUnlocked())
		{
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateDnaManipulator());
			}
			PlayerStore.Instance.AddDirectSellingItemToTable("plant_dna_disk");
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("plant_dna_disk");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("plant_dna_disk");
			}
			if (NetworkUpgrade.IsJackson1Unlocked())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("plant_dna_disk");
			}
			if (NetworkUpgrade.IsJackson2Unlocked())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("plant_dna_disk");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("dna_manipulator_module_stabilizer");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("dna_manipulator_module_positive_filter");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("dna_manipulator_module_negative_filter");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("dna_manipulator_module_precision");
			}
		}
		bool flag = NetworkUpgrade.IsRetiredRancherUnlocked();
		if (NetworkUpgrade.IsPlantDnaUnlocked())
		{
			flag = true;
		}
		if (flag)
		{
			AddIfNotOwn("gene_scanner", hashSet);
		}
		if (NetworkUpgrade.IsWinemakerUnlocked())
		{
			if (RNG.Roll(33))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateAgeWell());
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast_infinite");
			}
		}
		if (NetworkUpgrade.IsAquacultureUnlocked())
		{
			if (hashSet.Contains("ras"))
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(MachineRAS.SpawnRASWithEmptyTank());
				}
			}
			else if (RNG.Roll(chance))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(MachineRAS.SpawnRASWithEmptyTank());
			}
			if (hashSet.Contains("livestock_feed_dispenser"))
			{
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("livestock_feed_dispenser"));
				}
			}
			else if (RNG.Roll(chance))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("livestock_feed_dispenser"));
			}
		}
		if (NetworkUpgrade.IsRetiredRancherUnlocked() || NetworkUpgrade.IsAquacultureUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("auto_butcher"));
		}
		if (RNG.Roll(8))
		{
			GameItem gameItem = StartingPerkContent.CreateUnownedDegradedCassette();
			if (gameItem != null)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
			}
		}
		if (PlayerStore.Instance.isAugIntroduced && !PlayerStore.Instance.isAugEnded && RNG.Roll(50))
		{
			AddIfNotOwn("aug_scanner", hashSet);
		}
		if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("mouse_trap");
		}
		AddIfNotOwn("sign_water", hashSet);
		AddIfNotOwn("sign_food", hashSet);
		AddIfNotOwn("sign_medical", hashSet);
		AddIfNotOwn("sign_household", hashSet);
		AddIfNotOwn("sign_material", hashSet);
		if (NetworkUpgrade.IsGunPermitIUnlocked())
		{
			AddIfNotOwn("sign_firearm", hashSet);
		}
		if (NetworkUpgrade.IsGunPermitIIIUnlocked())
		{
			AddIfNotOwn("sign_firearm_heavy", hashSet);
		}
		AddIfNotOwn("black_lamp", hashSet);
		if (RNG.Roll(15 + StoreReputation.GetBMReputation() / 2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("black_lamp"));
		}
		AddIfNotOwn("cigarette_guide", hashSet);
		AddIfNotOwn("cigarette_color", hashSet);
		if (!StartingPerk.IsBannedCommissary())
		{
			AddIfNotOwn("stamp_guide", hashSet);
			AddIfNotOwn("logo_checker", hashSet);
		}
		if (PlayerStore.Instance.isInjectorIntroduced)
		{
			AddIfNotOwn("injector_guide", hashSet);
		}
		if (NetworkUpgrade.IsJackson1Unlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_node"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit");
			}
		}
		if (NetworkUpgrade.IsJackson2Unlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_node"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_overclock_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_eco_module"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_fineness_module"));
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("module_bay_expansion_kit");
			}
		}
		GuaranteeLockedCrateKeycardItems(allOwnedItems, hashSet);
		ModHook.FireOnPlaceInventorInventoryItemLate(allOwnedItems);
	}

	private static void GuaranteeLockedCrateKeycardItems(List<GameItem> playerItems, HashSet<string> ownedIds)
	{
		HashSet<string> hashSet = new HashSet<string>();
		int num = 0;
		foreach (GameItem playerItem in playerItems)
		{
			if (LockHelper.IsCurrentlyLocked(playerItem))
			{
				string @lock = LockHelper.GetLock(playerItem);
				if ((!lockToKeycard.TryGetValue(@lock, out var value) || !ownedIds.Contains(value)) && lockToCryptoModule.TryGetValue(@lock, out var value2))
				{
					num++;
					hashSet.Add(value2);
				}
			}
		}
		if (num == 0)
		{
			return;
		}
		if (!ownedIds.Contains("desequencer"))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("desequencer");
		}
		foreach (string item in hashSet)
		{
			if (!ownedIds.Contains(item))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(item);
			}
		}
		int num2 = 0;
		foreach (GameItem playerItem2 in playerItems)
		{
			if (playerItem2.identifier == "blank_keycard")
			{
				num2++;
			}
		}
		for (int i = num2; i < num; i++)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blank_keycard");
		}
	}

	public static void AddIfNotOwn(string itemId, HashSet<string> ownedIds)
	{
		if (!ownedIds.Contains(itemId))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn(itemId));
		}
	}

	public static StoreClient CreateInventorStorage()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "inventorStorage";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_dr._jackson");
		storeClient.spriteName = SpriteDict.GetInventorSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.nonShootable = true;
		storeClient.cooldown = 10;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isBargainAvailable = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_storage_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_storage_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_storage_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_inventor_storage_4"))
			.SetStartAction(delegate
			{
				int itemCountById = EmporiumEntry.Instance.GetItemCountById("storage_bay_large");
				int itemCountById2 = EmporiumEntry.Instance.GetItemCountById("machine_bay_ext");
				if (itemCountById == 0 && StoreStation.Instance.dayCounter >= 12)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay_large");
				}
				if (itemCountById == 1 && StoreStation.Instance.dayCounter >= 40)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay_large");
				}
				if (itemCountById == 2 && StoreStation.Instance.dayCounter >= 70)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay_large");
				}
				if (itemCountById2 == 0 && StoreStation.Instance.dayCounter >= 30)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("machine_bay_ext");
				}
				if (itemCountById2 == 1 && StoreStation.Instance.dayCounter >= 55)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("machine_bay_ext");
				}
				if (itemCountById2 == 2 && StoreStation.Instance.dayCounter >= 85)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("machine_bay_ext");
				}
			});
		storeClient.onItemAddedToWeighted = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item) && StoreReputation.IsPerkUnlocked("LLPILLAR"))
			{
				item.AddItemFeature(ItemFeatureList.DiscountBuy15());
			}
		};
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMerchant()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "merchant";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_merchant");
		storeClient.spriteName = SpriteDict.GetRandomMerchantSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.IsPersonalConsumption = false;
		storeClient.cooldown = 3;
		storeClient.SetBudget(200 + RNG.GetRandomInt(0, 100) + StoreStation.Instance.dayCounter * 3);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.acceptedChemicalProductGrade = 0;
		storeClient.CanExposeChem = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merchant_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merchant_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merchant_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merchant_4", Difficulty.Rules.merchantDiscount))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merchant_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merchant_6", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.acceptRetailMarkup = false;
		storeClient.clientItemFeatureBuying.Add("merchantDiscountBuy");
		storeClient.clientBuyingTagList.Add("ALL_TYPE_EXCEPT_CONTRABAND");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateShadyMerchant()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "shadyMerchant";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shady_merchant");
		storeClient.spriteName = SpriteDict.GetShadyMerchantSpriteName();
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.IsPersonalConsumption = false;
		storeClient.SetBudget(250 + RNG.GetRandomInt(0, 50) + StoreStation.Instance.dayCounter * 3);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.acceptedChemicalProductGrade = 0;
		storeClient.CanExposeChem = true;
		storeClient.acceptRetailMarkup = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_merchant_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_merchant_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_merchant_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_merchant_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_merchant_5", storeClient.GetBudget()))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemAddedToWeighted = delegate(GameItem addedItem)
		{
			int contrabandLevel = ContrabandHelper.GetContrabandLevel(addedItem);
			if (contrabandLevel == 1)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedLowContraband());
			}
			if (contrabandLevel == 2)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedMidContraband());
			}
			if (contrabandLevel == 3)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedHighContraband());
			}
			if (contrabandLevel == 4)
			{
				addedItem.AddItemFeature(ItemFeatureList.DiscountedCriticalContraband());
			}
		};
		storeClient.clientBuyingTagList.Add("CONTRABAND");
		storeClient.clientBuyingTagList.Add("ACCESS_CARD");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateShadyPharmacist()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "shadyPharma";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_levels_pharmacist");
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(15);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_pharma_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_pharma_2"))
			.SetEndAction(delegate
			{
				if (PlayerStore.Instance.isInjectorIntroduced && RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomInjector(), isOwend: false, isStolen: true);
					if (StoreStation.GetDayCounter() >= 20)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomInjector(), isOwend: false, isStolen: true);
					}
					PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item", isOwend: false, isStolen: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item", isOwend: false, isStolen: true);
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item", isOwend: false, isStolen: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item", isOwend: false, isStolen: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
					if (StoreStation.GetDayCounter() >= 20)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
					}
					if (StoreStation.GetDayCounter() >= 40)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
					}
					if (RNG.Roll(25))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
					}
				}
			});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_shady_pharma_alldone")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScrapper()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "machine_scrapper";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scrapper");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.cooldown = 3;
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scrap_seller_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateBrokenAlarm());
			if (RNG.Roll(66))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateBrokenFurnace());
			}
			if (RNG.Roll(33))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateBrokenBrokenMoistureFarm());
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLowerLevelChemist()
	{
		return CreateLowerLevelChemist(carryContraband: true);
	}

	public static StoreClient CreateLowerLevelChemist(bool carryContraband = true)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lowerLevelChemist";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_chemist");
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.SetBudget(75, 125);
		storeClient.SetWholesale(-RNG.GetRandomInt(15, 25));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_ll_chemist_1_b")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_ll_chemist_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_ll_chemist_3"))
			.SetEndAction(delegate
			{
				if (carryContraband && RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					if (StoreStation.GetDayCounter() >= 20)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					}
					if (StoreStation.GetDayCounter() >= 40)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					}
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("salve");
					if (StoreStation.GetDayCounter() >= 40)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("salve");
					}
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				if (StoreStation.GetDayCounter() >= 20)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				}
				if (StoreStation.GetDayCounter() >= 40)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.clientBuyingTagList.Add("CHEMICAL_SUPPLIES");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMiner()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "miner";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_miner");
		storeClient.spriteName = SpriteDict.GetRandomMinerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(15, 25);
		storeClient.SetBudget(50 + RNG.GetRandomInt(15, 25));
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.budgetConversionRate = 50;
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_miner_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_miner_greet_2"),
			LocHelper.GetLocalizedDialogue("dialog_miner_greet_3"),
			LocHelper.GetLocalizedDialogue("dialog_miner_greet_4"),
			LocHelper.GetLocalizedDialogue("dialog_miner_greet_5")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_miner_offer_1"),
			LocHelper.GetLocalizedDialogue("dialog_miner_offer_2"),
			LocHelper.GetLocalizedDialogue("dialog_miner_offer_3"),
			LocHelper.GetLocalizedDialogue("dialog_miner_offer_4"),
			LocHelper.GetLocalizedDialogue("dialog_miner_offer_5")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_miner_substance"))
			.SetEndAction(delegate
			{
				int num = RNG.GetRandomInt(3, 4);
				if (StoreStation.GetDayCounter() >= 15 && RNG.Roll(50))
				{
					num++;
				}
				if (StoreStation.GetDayCounter() >= 30 && RNG.Roll(50))
				{
					num += 2;
				}
				if (StoreStation.GetDayCounter() >= 45 && RNG.Roll(50))
				{
					num++;
				}
				if (StoreStation.GetDayCounter() >= 60 && RNG.Roll(50))
				{
					num++;
				}
				for (int i = 0; i < num; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
				}
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static List<string> GetScavGreetingLines()
	{
		return new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_2"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_3"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_4"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_5"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_6"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_7"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_8"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_9"),
			LocHelper.GetLocalizedDialogue("dialog_scav_greet_10")
		};
	}

	public static List<string> GetScavOfferLines()
	{
		return new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_1"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_2"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_3"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_4"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_5"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_6"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_7"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_8"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_9"),
			LocHelper.GetLocalizedDialogue("dialog_scav_offer_10")
		};
	}

	public static StoreClient CreateScavGeneral()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavGeneral";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(15, 25);
		List<string> scavGreetingLines = GetScavGreetingLines();
		List<string> scavOfferLines = GetScavOfferLines();
		storeClient.mainDialogue.SetText(storeClient.displayName, scavGreetingLines[RNG.GetRandomInt(0, scavGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, scavOfferLines[RNG.GetRandomInt(0, scavOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				AddScavGeneralStock();
				storeClient.OnIntroduced();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static void AddScavGeneralStock()
	{
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(25))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(8))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("processed_cheese");
		}
		if (StoreStation.Instance.dayCounter >= 7)
		{
			if (RNG.Roll(8))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("kitchen_knife");
			}
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("combat_knife");
			}
			if (RNG.Roll(8))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("box_cutter");
			}
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("kitchen_cleaver");
			}
			if (RNG.Roll(12))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("pipe_weapon");
			}
			if (RNG.Roll(8))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("hatchet");
			}
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("screwdriver");
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("welder");
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("crowbar");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire_cutter");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("water_ration_small");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("water_ration_small");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_fabric");
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
		}
		if (RNG.Roll(80))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
		}
		if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
		}
		if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
		}
		if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.RandomCigarette());
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.RandomCigarette());
		}
		if (PlayerStore.Instance.isInjectorIntroduced && RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomInjector());
		}
		if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon");
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
		}
		if (RNG.Roll(8))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("portable_water_purifier_used"));
		}
		if (RNG.Roll(15) && NetworkUpgrade.IsRetiredFarmerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullNutrifruitSeedPacket());
			}
		}
		if (RNG.Roll(15) && NetworkUpgrade.IsRetiredFarmerUnlocked() && NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("bloomberry_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("bloomberry_seed");
		}
		if (RNG.Roll(25))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(20, 80));
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter(50, 100));
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("water_pitcher");
		}
		if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nudka");
		}
		if (RNG.Roll(25))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_medical");
		}
		if (RNG.Roll(25))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical");
		}
		if (RNG.Roll(25))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical");
		}
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scratcher");
		}
		if (StoreStation.Instance.dayCounter >= 10)
		{
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_corrupt");
			}
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
			}
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
			}
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_ruined");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_corrupt");
			}
			if (RNG.Roll(10))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor");
			}
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pure_white_injector");
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("rubbing_alcohol");
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("smelling_salt");
		}
		if (RNG.Roll(40))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item");
		}
		if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("surgery_tool");
		}
		if (NetworkUpgrade.IsWinemakerUnlocked())
		{
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
			}
		}
		if (EmporiumEntry.Instance.IsFrontCounterEmptyNonOwned())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
		}
	}

	public static StoreClient CreateScavCrate()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavCrate";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(15, 25);
		List<string> scavGreetingLines = GetScavGreetingLines();
		List<string> scavOfferLines = GetScavOfferLines();
		storeClient.mainDialogue.SetText(storeClient.displayName, scavGreetingLines[RNG.GetRandomInt(0, scavGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, scavOfferLines[RNG.GetRandomInt(0, scavOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				string itemID = TableMaster.Roll("packedFoodTable");
				int minInclusive = 5;
				int maxInclusive = 6;
				if (StoreStation.Instance.dayCounter >= 20)
				{
					minInclusive = 6;
					maxInclusive = 7;
				}
				else if (StoreStation.Instance.dayCounter >= 40)
				{
					minInclusive = 7;
					maxInclusive = 8;
				}
				else if (StoreStation.Instance.dayCounter >= 60)
				{
					minInclusive = 8;
					maxInclusive = 9;
				}
				for (int i = 0; i < RNG.GetRandomInt(minInclusive, maxInclusive); i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn(itemID));
				}
				if (EmporiumEntry.Instance.frontInvinvElement.childItems.Count != 0)
				{
					long baseValue = EmporiumEntry.Instance.frontInvinvElement.childItems[0].GetBaseValue();
					if (baseValue <= 10)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn(itemID));
					}
					if (baseValue <= 16)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn(itemID));
					}
				}
				storeClient.OnIntroduced();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTrailerScavGeneral()
	{
		StoreClient storeClient = CreateScavGeneral();
		storeClient.spriteName = "trailer_scav4";
		List<string> scavGreetingLines = GetScavGreetingLines();
		List<string> scavOfferLines = GetScavOfferLines();
		storeClient.mainDialogue.SetText(storeClient.displayName, scavGreetingLines[RNG.GetRandomInt(0, scavGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, scavOfferLines[RNG.GetRandomInt(0, scavOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
				PlayerStore.Instance.AddDirectSellingItemToTable("red_beer");
				PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("wire");
				PlayerStore.Instance.AddDirectSellingItemToTable("cassette5");
				PlayerStore.Instance.AddDirectSellingItemToTable("kitchen_knife");
				PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit_seed");
				PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit_seed");
				PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit_seed");
				storeClient.OnIntroduced();
			});
		return storeClient;
	}

	public static StoreClient CreateTrailerAug()
	{
		StoreClient storeClient = CreateScavGeneral();
		storeClient.spriteName = "maleLower4";
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.isAug = true;
		storeClient.SetBudget(150);
		storeClient.mainDialogue.SetText(storeClient.displayName, "Greetings, this unit requires surgical equipment.").NextDialogue().SetText(storeClient.displayName, "I mean, do you sell any surgical equipment, fellow human?")
			.SetEndAction(delegate
			{
			});
		return storeClient;
	}

	public static StoreClient CreateInsScav()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavInspectable";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.cooldown = 3;
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(10, 20);
		List<string> scavGreetingLines = GetScavGreetingLines();
		List<string> scavOfferLines = GetScavOfferLines();
		storeClient.mainDialogue.SetText(storeClient.displayName, scavGreetingLines[RNG.GetRandomInt(0, scavGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, scavOfferLines[RNG.GetRandomInt(0, scavOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				List<Action> list = new List<Action>();
				if (PlayerStore.Instance.isStampIntroduced)
				{
					list.Add(delegate
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomStamp());
					});
				}
				if (PlayerStore.Instance.isVisualInspectionIntroduced)
				{
					list.Add(delegate
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.RandomCigarette());
					});
				}
				if (PlayerStore.Instance.isInjectorIntroduced)
				{
					list.Add(delegate
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomInjector());
					});
				}
				int randomIntExculsive = RNG.GetRandomIntExculsive(0, list.Count);
				list[randomIntExculsive]();
				if (StoreStation.Instance.dayCounter >= 15)
				{
					list[randomIntExculsive]();
				}
				if (StoreStation.Instance.dayCounter >= 40)
				{
					list[randomIntExculsive]();
				}
				if (StoreStation.Instance.dayCounter >= 75)
				{
					list[randomIntExculsive]();
				}
				if (EmporiumEntry.Instance.IsFrontCounterEmptyNonOwned())
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
				}
				storeClient.OnIntroduced();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static float GetBMRepStockMult()
	{
		int reputation = StoreReputation.GetBMFaction().GetReputation();
		if (reputation >= 100)
		{
			return 2.5f;
		}
		if (reputation >= 80)
		{
			return 1.75f;
		}
		if (reputation >= 60)
		{
			return 1.5f;
		}
		if (reputation >= 40)
		{
			return 1.25f;
		}
		return 1f;
	}

	public static StoreClient CreateThief()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "thief";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_thief");
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 2;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thief_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thief_2"))
			.SetEndAction(delegate
			{
				float bMRepStockMult = GetBMRepStockMult();
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("bottled_water_premium", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("skincare_cream", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(6f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("neuroactive_perfume", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(4f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pheromone_perfume", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("turbo_booster_adv", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("cmd_keycard", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(15f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("scratcher_stack", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("sec_keycard", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(15f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("med_keycard", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(15f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("eng_keycard", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(20f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("ser_keycard", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(12f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(12f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(12f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(8f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_overclock_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(8f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_eco_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(8f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_fineness_module"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(12f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("portable_water_purifier"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(12f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_filter_adv"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(20f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealGenuineInjector(), isOwend: false, isStolen: true);
				}
				if (StoreReputation.IsPerkUnlocked("BM_RELIABLE"))
				{
					int randomInt = RNG.GetRandomInt(0, 100);
					if (randomInt <= 20)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateEvidence(), isOwend: false, isStolen: true);
					}
					else if (randomInt <= 40)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateMedical(), isOwend: false, isStolen: true);
					}
					else if (randomInt <= 60)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateEngineering(), isOwend: false, isStolen: true);
					}
					else if (randomInt <= 75)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateSecurity(), isOwend: false, isStolen: true);
					}
					else
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateService(), isOwend: false, isStolen: true);
					}
				}
				else if (RNG.Roll(30f * bMRepStockMult))
				{
					int randomInt2 = RNG.GetRandomInt(0, 100);
					if (randomInt2 <= 20)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateEvidence(), isOwend: false, isStolen: true);
					}
					else if (randomInt2 <= 40)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateMedical(), isOwend: false, isStolen: true);
					}
					else if (randomInt2 <= 60)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateEngineering(), isOwend: false, isStolen: true);
					}
					else if (randomInt2 <= 75)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateSecurity(), isOwend: false, isStolen: true);
					}
					else
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.LootCrateService(), isOwend: false, isStolen: true);
					}
				}
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_sec", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(2f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_cmd", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(7f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_med", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(7f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_eng", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("crypto_module_ser", isOwend: false, isStolen: true);
				}
				if (EmporiumEntry.Instance.IsFrontCounterEmptyNonOwned())
				{
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical", isOwend: false, isStolen: true);
						PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical", isOwend: false, isStolen: true);
						PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical", isOwend: false, isStolen: true);
					}
					else
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen: true);
						PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen: true);
						PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen: true);
					}
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreatePettyThief()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "pettyThief";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_petty_thief");
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.cooldown = 2;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-10);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_petty_thief_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_petty_thief_2"))
			.SetEndAction(delegate
			{
				float bMRepStockMult = GetBMRepStockMult();
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("householdTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("householdTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("householdTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("householdTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("medicalTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("medicalTable"), isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult) && NetworkUpgrade.IsWinemakerUnlocked())
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast_red", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult) && NetworkUpgrade.IsWinemakerUnlocked())
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast_red", isOwend: false, isStolen: true);
				}
				if (RNG.Roll(25f * bMRepStockMult) && NetworkUpgrade.IsWinemakerUnlocked())
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast_red", isOwend: false, isStolen: true);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"), isOwend: false, isStolen: true);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFoodThief()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "foodThief";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_food_thief");
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.cooldown = 2;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-RNG.GetRandomInt(15, 20));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_food_thief_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_food_thief_2"))
			.SetEndAction(delegate
			{
				float bMRepStockMult = GetBMRepStockMult();
				if (RNG.Roll(15f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(8f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(8f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(8f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(12f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_milk", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(20f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(10f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				if (RNG.Roll(5f * bMRepStockMult))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("processed_juice", isOwend: false, isStolen: true, ignorePerk: false, 25);
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: false, isStolen: true, ignorePerk: false, 25);
				RemoveCommissaryStock();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static void RemoveCommissaryStock()
	{
		List<GameItem> childItems = EmporiumEntry.Instance.frontInvinvElement.childItems;
		int num = 0;
		foreach (GameItem item in childItems)
		{
			foreach (CommissaryItemData commissaryItemData in PlayerStore.Instance.commissaryData.commissaryItemDatas)
			{
				if (item.identifier == commissaryItemData.identifier && commissaryItemData.StockCount > 0)
				{
					commissaryItemData.StockCount--;
					num++;
					break;
				}
			}
		}
		Debug.Log("Removed " + num + " items from Commissary stock due to Thief.");
		if (num > 0)
		{
			AfterhourEvents.Start("commissary_burglary");
		}
	}

	public static void PlaceSupplierInventory()
	{
		PlayerStore.Instance.AddDirectSellingItemToTable("bandage_item");
		PlayerStore.Instance.AddDirectSellingItemToTable("bandage_item");
		PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(100));
		PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor");
		if (NetworkUpgrade.IsWaterTraderUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullNutrientTabletBox());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullNutrientTabletBox());
			PlayerStore.Instance.AddDirectSellingItemToTable("water_pitcher");
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
		}
		if (NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
		}
		if (NetworkUpgrade.IsRetiredFarmerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("fillerweed_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("fillerweed_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullNutrifruitSeedPacket());
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
		PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
		PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
		PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
		if (NetworkUpgrade.IsChemistUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("50ml_cylinder");
			PlayerStore.Instance.AddDirectSellingItemToTable("mini_bottle");
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		PlayerStore.Instance.AddDirectSellingItemToTable("kotton_fabric");
		GameItem gameItem = DirectoryMaster.Item("large_bottled_water");
		WaterHelper.FillWithBaseWater(gameItem);
		PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
		PlayerStore.Instance.AddDirectSellingItemToTable("cassette" + RNG.GetRandomInt(1, 14) + "d");
		if (RNG.Roll(StoreStation.GetDayCounter()))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("cassette" + RNG.GetRandomInt(1, 14) + "l");
		}
		if (RNG.Roll(StoreStation.GetDayCounter() - 20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("cassette" + RNG.GetRandomInt(1, 14));
		}
	}

	public static StoreClient CreateSupplier()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "supplier";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_supply_runner");
		storeClient.spriteName = SpriteDict.GetRandomMerchantSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientItemFeatureBuying.Add("premiumBuy");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_supplier_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_supplier_2"))
			.SetEndAction(delegate
			{
				PlaceSupplierInventory();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSupplierIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "supplierIntro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_supplier");
		storeClient.spriteName = SpriteDict.GetRandomMerchantSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientItemFeatureBuying.Add("premiumBuy");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_supplier_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_supplier_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_supplier_intro_3"))
			.SetEndAction(delegate
			{
				PlaceSupplierInventory();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateWaterSeller()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "waterSeller";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_water_trader");
		storeClient.spriteName = SpriteDict.GetRandomMerchantSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(15, 20);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_seller_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_water_seller_2"))
			.SetEndAction(delegate
			{
				if (NetworkUpgrade.IsWaterTraderUnlocked())
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater("large_bottled_water"));
					if (StoreStation.GetDayCounter() >= 20)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater("large_bottled_water"));
					}
					if (StoreStation.GetDayCounter() >= 50)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater("large_bottled_water"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater("large_bottled_water"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater("large_bottled_water"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater("large_bottled_water"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.GetRandomWater("water_jug"));
					}
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration"));
					if (StoreStation.GetDayCounter() >= 20)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration"));
					}
					if (StoreStation.GetDayCounter() >= 50)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration_small"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration_small"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration_small"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration_small"));
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_ration_small"));
					}
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLandlordClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "landlord";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_landlord");
		storeClient.spriteName = "landlord";
		storeClient.clientIntent = StoreClient.ClientIntent.RENT;
		storeClient.dismissable = false;
		Dialogue onRentPaidDialogue = new Dialogue();
		Dialogue onRentNoPaidDialogue = new Dialogue();
		onRentPaidDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_landlord_player_pay", PlayerStore.Instance.GetRentValue()));
		onRentNoPaidDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_landlord_player_nopay"));
		Dialogue buyoutDialogue = new Dialogue();
		buyoutDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_landlord_player_buyout", PlayerStore.Instance.GetStoreBuyPrice()));
		string text = ((!NetworkUpgrade.IsLandlordGraceUnlocked()) ? LocHelper.GetLocalizedDialogue("dialog_landlord_first_line_grace") : LocHelper.GetLocalizedDialogue("dialog_landlord_first_line_normal"));
		bool areChoicesAdded = false;
		storeClient.mainDialogue.SetText(storeClient.displayName, text).SetStartAction(delegate
		{
			if (!areChoicesAdded)
			{
				areChoicesAdded = true;
				storeClient.mainDialogue.AddChoice(DirectoryMaster.Item("$"), "pay rent choice", "", onRentPaidDialogue, delegate
				{
					PlayerStore.Instance.OnRentPaid();
					onRentPaidDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_rent_paid_1", PlayerStore.Instance.GetRentValue())).SetStartAction(delegate
					{
					})
						.NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_rent_paid_2"))
						.SetEndAction(delegate
						{
							DialogUIManager.Instance.CurrentClientLeave();
						});
				}, () => PlayerStore.Instance.CanPayRent());
				storeClient.mainDialogue.AddChoice(DirectoryMaster.Item("$"), "Don't pay the rent", "", onRentNoPaidDialogue, delegate
				{
					if (!NetworkUpgrade.IsLandlordGraceUnlocked())
					{
						onRentNoPaidDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_no_grace_1")).SetStartAction(delegate
						{
						})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_no_grace_2"))
							.SetEndAction(delegate
							{
								PlayerStore.Instance.ExecuteGameOver();
							});
					}
					else
					{
						onRentNoPaidDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_grace_1")).SetStartAction(delegate
						{
						})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_grace_2"))
							.SetStartAction(delegate
							{
							})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_grace_3"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_grace_4"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_grace_5"))
							.SetEndAction(delegate
							{
								DialogUIManager.Instance.CurrentClientLeave();
							});
					}
					PlayerStore.Instance.OnRentNotPaid();
				});
				if (PlayerStore.Instance.CanBuyOutStore())
				{
					storeClient.mainDialogue.AddChoice(DirectoryMaster.Item("$"), "buyout choice", "", buyoutDialogue, delegate
					{
						PlayerStore.Instance.BuyOutStore();
						buyoutDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_buyout_1")).NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_buyout_2"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_buyout_3"))
							.SetStartAction(delegate
							{
								PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("deed"), isOwend: true);
							})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_buyout_4"))
							.SetEndAction(delegate
							{
								DialogUIManager.Instance.CurrentClientLeave();
								PlayerStore.Instance.ExecuteGameOver("buyout");
							});
					});
				}
			}
		});
		return storeClient;
	}

	public static StoreClient CreateLandlordVendingMachineClient()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "landlordVending";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_the_landlord");
		storeClient.spriteName = "landlord";
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.dismissable = false;
		Dialogue vendingFountainDialogue = new Dialogue();
		Dialogue vendingMachineDialogue = new Dialogue();
		Dialogue noDialogue = new Dialogue();
		vendingFountainDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_landlord_vending_player_water"));
		vendingMachineDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_landlord_vending_player_machine"));
		noDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_landlord_vending_player_decline"));
		Dialogue lastLineDialogue = new Dialogue();
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_3"))
			.SetNextDialogue(lastLineDialogue);
		lastLineDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_last")).SetStartAction(delegate
		{
			if (!PlayerStore.Instance.isVendingFountainUnlocked)
			{
				lastLineDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", vendingFountainDialogue, delegate
				{
					vendingFountainDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_yes_1")).NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_yes_2"))
						.SetEndAction(delegate
						{
							VendingFountainHelper.UnlockVendingFountain();
							PlayerStore.Instance.rentValue += 150;
							DialogUIManager.Instance.CurrentClientLeave();
						});
				});
			}
			if (!PlayerStore.Instance.isVendingMachineUnlocked)
			{
				lastLineDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", vendingMachineDialogue, delegate
				{
					vendingMachineDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_yes_1")).NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_yes_2"))
						.SetEndAction(delegate
						{
							VendingMachineHelper.UnlockVendingMachine();
							PlayerStore.Instance.rentValue += 150;
							DialogUIManager.Instance.CurrentClientLeave();
						});
				});
			}
			lastLineDialogue.AddChoice(DirectoryMaster.Item("x"), "", "", noDialogue, delegate
			{
				noDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_no_1")).NextDialogue()
					.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_landlord_vending_no_2"))
					.SetEndAction(delegate
					{
						DialogUIManager.Instance.CurrentClientLeave();
					});
			});
		});
		return storeClient;
	}

	public static StoreClient CreateInspectionClient(int personality = 0, bool fromShowcase = false)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "securityInspector";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_inspector");
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		storeClient.dismissable = false;
		if (personality == 1)
		{
			storeClient.displayName = LocHelper.GetLocalizedClientName("name_shady_security_inspector");
		}
		if (personality == 2)
		{
			storeClient.displayName = LocHelper.GetLocalizedClientName("name_lazy_security_inspector");
			storeClient.isLazyInspector = true;
		}
		storeClient.clientIntent = StoreClient.ClientIntent.INSPECTION;
		storeClient.dismissable = false;
		storeClient.arrestable = false;
		Dialogue nextDialogue = StoreClientDialogList.SecurityDialogInspectionLastLine(personality, fromShowcase);
		if (fromShowcase)
		{
			storeClient.mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_inspection_showcase_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_inspection_showcase_2"))
				.SetNextDialogue(nextDialogue);
		}
		else
		{
			storeClient.mainDialogue.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_inspection_routine_1")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_inspection_routine_2"))
				.SetNextDialogue(nextDialogue);
		}
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateTrailerInspectionClient()
	{
		StoreClient storeClient = CreateInspectionClient();
		storeClient.spriteName = "officer_lun";
		return storeClient;
	}

	public static StoreClient CreateScavBlood()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scavBlood";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_pale_scav");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.SetBudget(0);
		storeClient.budgetConversionRate = 100;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scav_blood_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scav_blood_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
				PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
				if (StoreStation.GetDayCounter() >= 25)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
				}
				if (StoreStation.GetDayCounter() >= 50)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
				}
				if (StoreStation.GetDayCounter() >= 75)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
				}
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scav_blood_3"));
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
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scav_blood_looks_good"));
				dialogue.priority = 1;
				DialogUIManager.Instance.StartDialogue(dialogue);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLoanSharkIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "loan_shark";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_loan_shark");
		storeClient.spriteName = SpriteDict.GetLoanSharkSpriteName();
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_intro_10"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.storeClientManager.uniqueClientTracker.Add(storeClient.identifier, StoreStation.Instance.dayCounter);
				PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLoanShark2()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "loan_shark2";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_loan_shark");
		storeClient.spriteName = SpriteDict.GetLoanSharkSpriteName();
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue yesDialogue = new Dialogue();
		Dialogue noDialogue = new Dialogue();
		Dialogue soonDialogue = new Dialogue();
		yesDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_loan_shark2_player_agree", PlayerStore.Instance.GetMortgageDownPayment()));
		noDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_loan_shark2_player_decline"));
		soonDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_loan_shark2_player_soon"));
		Dialogue lastLine = new Dialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_loan_shark2_last_line"));
		if (PlayerStore.Instance.IsPropertyPaid)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_paid_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_paid_2"))
				.NextDialogue()
				.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_loan_shark2_paid_player"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_paid_4"))
				.SetEndAction(delegate
				{
					DialogUIManager.Instance.CurrentClientLeave();
				});
		}
		else if (PlayerStore.Instance.storeClientManager.uniqueClientTracker.ContainsKey(storeClient.identifier))
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_2"))
				.SetNextDialogue(lastLine);
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_4", PlayerStore.Instance.GetStoreBuyPrice()))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_5"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_6", PlayerStore.Instance.GetMortgageDownPayment()))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_7_rate", Difficulty.Rules.mortgageInterestRate))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_intro_8_weeks", PlayerStore.Instance.GetMortgageWeeklyPaymentPreview(), PlayerStore.GetMortgageWeeks()))
				.SetNextDialogue(lastLine);
		}
		lastLine.SetStartAction(delegate
		{
			PlayerStore.Instance.storeClientManager.uniqueClientTracker.TryAdd(storeClient.identifier, StoreStation.Instance.dayCounter);
			if (lastLine.choices.Count <= 0)
			{
				lastLine.AddChoice(DirectoryMaster.Item("$"), "yes choice", "", yesDialogue, delegate
				{
					yesDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_yes_1")).NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_yes_2"))
						.NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_yes_3"))
						.SetStartAction(delegate
						{
							PlayerStore.Instance.TakeMortgage();
							EmporiumEntry.Instance.TransferOwnershipFrontInv();
							EmporiumEntry.Instance.TransferOwnershipBackInv();
						})
						.NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_yes_4"))
						.SetEndAction(delegate
						{
							DialogUIManager.Instance.CurrentClientLeave();
						});
				}, () => PlayerStore.Instance.playerCash >= PlayerStore.Instance.GetMortgageDownPayment());
				lastLine.AddChoice(DirectoryMaster.Item("$"), "no choice", "", noDialogue, delegate
				{
					noDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_no_1")).NextDialogue()
						.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_no_2"))
						.SetEndAction(delegate
						{
							PlayerStore.Instance.IsMortgageOfferDeclined = true;
							DialogUIManager.Instance.CurrentClientLeave();
						});
				});
				lastLine.AddChoice(DirectoryMaster.Item("$"), "soon choice", "", soonDialogue, delegate
				{
					soonDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark2_soon_1")).SetEndAction(delegate
					{
						DialogUIManager.Instance.CurrentClientLeave();
					});
				});
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLoanSharkCollect()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "loan_shark_collect";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_loan_shark");
		storeClient.spriteName = SpriteDict.GetLoanSharkSpriteName();
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue yesDialogue = new Dialogue();
		Dialogue noDialogue = new Dialogue();
		yesDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_player_pay", PlayerStore.Instance.mortgageWeeklyPayment));
		noDialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_player_no"));
		Dialogue lastDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_2"));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_1")).SetNextDialogue(lastDialogue);
		lastDialogue.SetStartAction(delegate
		{
			if (lastDialogue.choices.Count <= 0)
			{
				lastDialogue.AddChoice(DirectoryMaster.Item("$"), "yes choice", "", yesDialogue, delegate
				{
					if (PlayerStore.Instance.mortgageWeekLeft == 1)
					{
						yesDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_last_1")).SetStartAction(delegate
						{
							PlayerStore.Instance.PayWeeklyMortgage();
							PlayerStore.Instance.OnMortgagePaidOff();
						})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_last_2"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_last_3"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_last_4"))
							.SetStartAction(delegate
							{
								PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("deed"), isOwend: true);
							})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_last_5"))
							.SetEndAction(delegate
							{
								PlayerStore.Instance.ExecuteGameOver("buyout_mortgage");
								DialogUIManager.Instance.CurrentClientLeave();
							});
					}
					else
					{
						yesDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_reg_1")).SetStartAction(delegate
						{
							PlayerStore.Instance.PayWeeklyMortgage();
						})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_reg_2"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_reg_3"))
							.SetEndAction(delegate
							{
								DialogUIManager.Instance.CurrentClientLeave();
							});
					}
				}, () => PlayerStore.Instance.playerCash >= PlayerStore.Instance.mortgageWeeklyPayment);
				if (PlayerStore.Instance.IsEnoughForRepo())
				{
					lastDialogue.AddChoice(DirectoryMaster.Item("$"), "no choice", "", noDialogue, delegate
					{
						noDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_repo_1")).NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_repo_2"))
							.SetEndAction(delegate
							{
								PlayerStore.Instance.Repo();
							})
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_repo_3"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_repo_4"))
							.SetEndAction(delegate
							{
								DialogUIManager.Instance.CurrentClientLeave();
							});
					});
				}
				else
				{
					lastDialogue.AddChoice(DirectoryMaster.Item("$"), "no choice", "", noDialogue, delegate
					{
						noDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_gameover_1")).NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_gameover_2"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_gameover_3"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_gameover_4"))
							.NextDialogue()
							.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_loan_shark_collect_gameover_5"))
							.SetEndAction(delegate
							{
								PlayerStore.Instance.ExecuteGameOver("repo");
							});
					});
				}
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateMercBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "mercenaryBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_mercenary");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(300 + StoreStation.Instance.dayCounter * 2);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merc_buyer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merc_buyer_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_merc_buyer_3"))
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

	public static StoreClient CreateGunsmith()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lower_level_gunsmith";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_gunsmith");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-20);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_gunsmith_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_gunsmith_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandmadeGun());
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHeavyHandmadeGun());
				}
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHeavyHandmadeGun(ItemFeatureList.EquipmentCondition.Poor));
				}
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandmadeGun());
				}
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHandmadeGun(ItemFeatureList.EquipmentCondition.Poor));
				}
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRevolver());
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("small_pistol_ammo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLowerLevelRareMerchant()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lower_level_rare_merchant";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_rare_merchant");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rare_merchant_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rare_merchant_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rare_merchant_3"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_breeder");
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_overclock_module"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_eco_module"));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_fineness_module"));
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("advanced_flux_agent");
				}
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("neuroactive_perfume");
				}
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor_advanced");
				}
				if (RNG.Roll(33))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("turbo_booster_adv");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRareLowerLevelChemist()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "rareLowerLevelChemist";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_pharmacist");
		storeClient.spriteName = SpriteDict.GetRandomScientistSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.SetBudget(75, 125);
		storeClient.SetWholesale(-25);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rare_chemist_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rare_chemist_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_rare_chemist_3"))
			.SetEndAction(delegate
			{
				int chance = 25;
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					if (RNG.Roll(75))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					}
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
					if (RNG.Roll(75))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
					}
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("black_injector");
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("black_injector");
					}
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
					PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
					if (RNG.Roll(75))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("fanta_pill");
					}
				}
			});
		storeClient.AddBasicDialog();
		storeClient.clientBuyingTagList.Add("CHEMICAL_SUPPLIES");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateSecSurplusMerchant()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "secSurplusMerchant";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_requisition_officer");
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.tradeReputationMultiplier = 2.0;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.SetWholesale(-15);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_surplus_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_surplus_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_surplus_3"))
			.SetEndAction(delegate
			{
				if (PermitHelper.IsGunPermitIII())
				{
					if (RNG.Roll(40))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceSMG(ItemFeatureList.EquipmentCondition.Poor));
					}
					if (RNG.Roll(40))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceSMG(ItemFeatureList.EquipmentCondition.Fair));
					}
					if (RNG.Roll(30))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceShotgun(ItemFeatureList.EquipmentCondition.Poor));
					}
					if (RNG.Roll(30))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceShotgun(ItemFeatureList.EquipmentCondition.Fair));
					}
					if (RNG.Roll(30))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("stock");
					}
					if (RNG.Roll(30))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("short_stock");
					}
					if (RNG.Roll(30))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("gl");
					}
					if (RNG.Roll(30))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("bipod");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_smg");
					}
					if (RNG.Roll(40))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_smg");
					}
					if (RNG.Roll(30))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_smg_high");
					}
					if (RNG.Roll(15))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_smg_box");
					}
				}
				if (PermitHelper.IsGunPermitII())
				{
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun(ItemFeatureList.EquipmentCondition.Poor));
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePoliceHandgun(ItemFeatureList.EquipmentCondition.Fair));
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_p");
					}
					if (RNG.Roll(66))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm");
					}
					if (RNG.Roll(33))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_high");
					}
					if (RNG.Roll(18))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_ext");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("grip");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("laser2");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("silencer2");
					}
				}
				if (PermitHelper.IsGunPermitI())
				{
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("rds");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("laser");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("compensator");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("silencer");
					}
				}
				if (StoreReputation.IsPerkUnlocked("SEC_RELIABLE_VENDOR"))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("stun_gun");
					PlayerStore.Instance.AddDirectSellingItemToTable("tazer");
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
				else
				{
					if (RNG.Roll(15))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("stun_gun");
					}
					if (RNG.Roll(15))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("tazer");
					}
					if (RNG.Roll(50))
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
					}
				}
				if (PermitHelper.IsNoGunPermit())
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
					PlayerStore.Instance.AddDirectSellingItemToTable("heavy_pistol_ammo_nl");
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateSecRequisitionOfficer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "secRequisitionOfficer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_security_requisition_officer");
		storeClient.spriteName = SpriteDict.GetRandomSecSpriteName();
		storeClient.tradeReputationMultiplier = 2.0;
		StoreClientFactionSetup.InitSecurity(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(300);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_req_officer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_req_officer_2"))
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_sec_req_officer_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.CompleteClientCreation();
		storeClient.acceptDealDialogue = StoreClientDialogList.GetRandomDialogAccepted();
		storeClient.CompleteSecClient();
		return storeClient;
	}

	public static StoreClient CreateCluelessTouristBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "cluelessTouristBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_tourist");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitTourist(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(RNG.GetRandomInt(180, 250));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_tourist_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_greet_2"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_greet_3"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_greet_4"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_greet_5"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_greet_6")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_tourist_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_deal_2"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_deal_3"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_deal_4"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_deal_5"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_deal_6")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.clientBuyingTagList.Add("WATER");
		storeClient.clientBuyingTagList.Add("WATER_RATION");
		storeClient.clientBuyingTagList.Add("SUBSTANCE");
		storeClient.refusesHazardousWaterLabel = true;
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateBrokeUpperLevel()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "brokeUpperLevel";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_citizen");
		storeClient.spriteName = SpriteDict.GetRandomUpperMaleSpriteName();
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 3;
		storeClient.SetWholesale(35);
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_broke_upper_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_broke_upper_greet_2")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_broke_upper_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_broke_upper_deal_2")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
				float chance = 14.28f;
				if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("tazer");
					PlayerStore.Instance.AddDirectSellingItemToTable("stun_gun");
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("water_filter_adv");
					PlayerStore.Instance.AddDirectSellingItemToTable("portable_water_purifier");
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_breeder");
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_efficiency_module"));
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_quality_module"));
				}
				else if (RNG.Roll(chance))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
					PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("random_performance_module"));
				}
				else
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("blue_blood_bag");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTouristWeaponSouvenirBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "touristWeaponSouvenirBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_station_tourist");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitTourist(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(RNG.GetRandomInt(200, 300));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_greet_2"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_greet_3")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_deal_2"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_deal_3"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_deal_4"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_weapon_deal_5")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateTouristJunkSouvenirBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "touristJunkSouvenirBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_station_tourist");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		StoreClientFactionSetup.InitTourist(storeClient);
		storeClient.SetVariety();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(RNG.GetRandomInt(125, 200));
		List<string> list = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_greet_1"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_greet_2"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_greet_3"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_greet_4")
		};
		List<string> list2 = new List<string>
		{
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_deal_1"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_deal_2"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_deal_3"),
			LocHelper.GetLocalizedDialogue("dialog_tourist_junk_deal_4")
		};
		storeClient.mainDialogue.SetText(storeClient.displayName, list[RNG.GetRandomInt(0, list.Count - 1)]).NextDialogue().SetText(storeClient.displayName, list2[RNG.GetRandomInt(0, list2.Count - 1)])
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_tourist_junk_variety"))
			.SetEndAction(delegate
			{
			});
		storeClient.clientBuyingTagList.Add("MATERIAL");
		storeClient.clientBuyingTagList.Add("MAKESHIFT");
		storeClient.clientBuyingTagList.Add("JUNK");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFoodBankBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "foodBankBuyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_food_bank_supplier");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.tradeReputationMultiplier = 3.0;
		storeClient.cooldown = 6;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		int budget = RNG.GetRandomInt(150, 200) + StoreStation.Instance.dayCounter * 2;
		storeClient.SetBudget(budget);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_food_bank_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_food_bank_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_food_bank_3"))
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_food_bank_4"))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemAddedToCounters = delegate(GameItem item)
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				item.AddItemFeature(ItemFeatureList.DiscountSell50());
			}
		};
		storeClient.clientBuyingTagList.Add("FOOD");
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCriminalBuyerThief()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "criminalBuyerThief";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shopping_thief");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(RNG.GetRandomInt(500, 750));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_criminal_thief_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_criminal_thief_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_criminal_thief_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemAddedToWeighted = delegate
		{
		};
		storeClient.clientBuyingTagList.Add("ACCESS_CARD");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.clientBuyingTagList.Add("MELEE_WEAPON");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCriminelMerc()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "criminalBuyerMerc";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_shopping_mercenary");
		storeClient.spriteName = SpriteDict.GetRandomMercSpriteName();
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(RNG.GetRandomInt(600, 650));
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_criminal_merc_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_criminal_merc_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_criminal_merc_3"))
			.SetEndAction(delegate
			{
			});
		storeClient.onItemAddedToWeighted = delegate
		{
		};
		storeClient.clientBuyingTagList.Add("ARMOR");
		storeClient.clientBuyingTagList.Add("MEDICAL");
		storeClient.clientBuyingTagList.Add("WEAPON");
		storeClient.clientBuyingTagList.Add("MUNITION");
		storeClient.clientBuyingTagList.Add("AMMUNITION");
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateUpperLevelCharity()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "upperLevelCharity";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_upper_level_volunteer");
		storeClient.spriteName = SpriteDict.GetRandomUpperSpriteName();
		StoreClientFactionSetup.InitUpperLevelCitizen(storeClient);
		storeClient.cooldown = 5;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_charity_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_upper_charity_2"))
			.SetEndAction(delegate
			{
				if (CommissaryData.GetTotalTypeMarketPriceModifier("FOOD") >= 66)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: true);
				}
				if (CommissaryData.GetTotalTypeMarketPriceModifier("ENERGY_CELL") >= 66)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(43), isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(26), isOwend: true);
				}
				if (CommissaryData.GetTotalTypeMarketPriceModifier("WEAPON") >= 66)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("tazer", isOwend: true);
				}
				if (CommissaryData.GetTotalTypeMarketPriceModifier("WATER") >= 66)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateHighQualityWater(), isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable(WaterPremadeHelper.AccurateHighQualityWater(), isOwend: true);
				}
				if (CommissaryData.GetTotalTypeMarketPriceModifier("MEDICAL") >= 66)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("phagimycin_pill", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item", isOwend: true);
				}
				if (EmporiumEntry.Instance.IsFrontCounterEmpty())
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("phagimycin_pill", isOwend: true);
					PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item", isOwend: true);
				}
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateLotteryRep()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lotteryRep";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lottery_representative");
		storeClient.spriteName = LocHelper.GetLocalizedClientName("name_trade_board_official");
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.acceptRetailMarkup = false;
		storeClient.tradeReputationMultiplier = 0.0;
		storeClient.isBargainAvailable = false;
		storeClient.clientBuyingTagList.Add("LOTTERY_IS_WINNING_TAG");
		storeClient.onItemSold = delegate(GameItem item, int cost)
		{
			if (item.identifier.Equals("lottery_ticket"))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("lottery_ticket");
			}
		};
		if (PlayerStore.GetStoreClientData().lotteryMetTime == 0)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_1")).NextDialogue().SetStartAction(delegate
			{
				PlayerStore.GetStoreClientData().lotteryMetTime++;
			})
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_4"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_5"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_6"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_7"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_intro_8"))
				.SetEndAction(delegate
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("lottery_info", isOwend: true);
					for (int k = 0; k < 10; k++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("lottery_ticket");
					}
					for (int l = 0; l < 5; l++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("scratcher");
					}
				});
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_return_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_lottery_return_2"))
				.SetEndAction(delegate
				{
					_ = PlayerStore.GetStoreClientData().lotteryMetTime;
					_ = 2;
					PlayerStore.GetStoreClientData().lotteryMetTime++;
					for (int i = 0; i < 10; i++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("lottery_ticket");
					}
					for (int j = 0; j < 5; j++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("scratcher");
					}
				});
		}
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateFlexiBuyer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "lower_level_flexi_buyer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_lower_level_citizen");
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.SetBudget(90 + RNG.GetRandomInt(0, 20) + StoreStation.Instance.dayCounter);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_flexi_buyer_1")).SetEndAction(delegate
		{
			List<GameItem> allOwnedItems = EmporiumEntry.Instance.GetAllOwnedItems();
			allOwnedItems.RemoveAll((GameItem item) => item.IsTag("CONTAINER_TAG") || item.IsGameItemType("MACHINE"));
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (GameItem item in allOwnedItems)
			{
				foreach (string item2 in item.GetGameItemType())
				{
					switch (item2)
					{
					case "MATERIAL":
					case "MEDICAL":
					case "FOOD":
					case "HOUSEHOLD_GOOD":
					{
						int num = (int)item.unitValue;
						if (dictionary.ContainsKey(item2))
						{
							dictionary[item2] += num;
						}
						else
						{
							dictionary[item2] = num;
						}
						break;
					}
					}
				}
			}
			List<KeyValuePair<string, int>> list = dictionary.ToList();
			list.Sort((KeyValuePair<string, int> pair1, KeyValuePair<string, int> pair2) => pair2.Value.CompareTo(pair1.Value));
			string text;
			if (list.Count > 0)
			{
				text = list[0].Key;
				storeClient.clientBuyingTagList.Add(text);
				Dialogue dialogue = new Dialogue();
				dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_flexi_buyer_type", TypeHelper.GetTypeDisplayName(text)));
				DialogUIManager.Instance.StartDialogue(dialogue, forcedStart: true);
			}
			else
			{
				text = "FOOD";
				storeClient.clientBuyingTagList.Add(text);
				Dialogue dialogue2 = new Dialogue();
				dialogue2.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_flexi_buyer_food"));
				DialogUIManager.Instance.StartDialogue(dialogue2, forcedStart: true);
			}
			if (RNG.Roll(66))
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (GameItem item3 in allOwnedItems)
				{
					if (item3.IsGameItemType(text))
					{
						hashSet.Add(item3.identifier);
					}
				}
				if (hashSet.Count >= 4)
				{
					storeClient.SetVariety();
				}
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateBusinessPermit()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "businessPermitOfficial";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_trade_board_official");
		storeClient.spriteName = SpriteDict.GetClerkSpriteName();
		storeClient.tradeReputationMultiplier = 0.0;
		storeClient.dismissable = false;
		storeClient.isBargainAvailable = false;
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.clientBuyingTagList.Add("ALL_TYPE");
		Dialogue lastDialogue = new Dialogue();
		lastDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_last1"));
		Dialogue lastDialogue2 = new Dialogue();
		lastDialogue2.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_last2"));
		storeClient.canClientExposeFeature = ClientCanExposeFunc.TradeBoardExposeCapacity;
		storeClient.resumeAfterConfrontDialogue = lastDialogue2;
		Dialogue option1 = new Dialogue();
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		if (PlayerStore.Instance.storeClientData.businessPermitOfficialState == 0)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state0_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state0_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state0_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state0_4", PlayerStore.GetBusinessPermitPrice()))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state0_5"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state0_6"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state0_7"))
				.SetStartAction(delegate
				{
					PlayerStore.Instance.storeClientData.businessPermitOfficialState = 1;
					if (!lastDialogue.choices.Exists(((GameItem, Dialogue, Action) c) => c.Item2 == option1))
					{
						lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", option1, delegate
						{
						}, () => PlayerStore.Instance.playerCash >= PlayerStore.GetBusinessPermitPrice());
					}
				})
				.nextDialogue = lastDialogue;
			lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue2, delegate
			{
			});
		}
		else if (PlayerStore.Instance.storeClientData.businessPermitOfficialState == 1)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_3", PlayerStore.GetBusinessPermitPrice()))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_4"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_5"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_6"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_7"))
				.SetStartAction(delegate
				{
				})
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state1_8", PlayerStore.Instance.storeClientData.businessPermitBribeAmount))
				.SetStartAction(delegate
				{
					PlayerStore.Instance.storeClientData.businessPermitOfficialState = 2;
					storeClient.clientIntent = StoreClient.ClientIntent.BUY;
					if (!lastDialogue2.choices.Exists(((GameItem, Dialogue, Action) c) => c.Item2 == option1))
					{
						lastDialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", option1, delegate
						{
						}, () => PlayerStore.Instance.playerCash >= PlayerStore.GetBusinessPermitPrice());
					}
				})
				.nextDialogue = lastDialogue2;
			lastDialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, delegate
			{
			});
		}
		else if (PlayerStore.Instance.storeClientData.businessPermitOfficialState == 2)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state2_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state2_2"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state2_3"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state2_4"))
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state2_5"))
				.SetStartAction(delegate
				{
				})
				.NextDialogue()
				.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_biz_permit_state2_6", PlayerStore.Instance.storeClientData.businessPermitBribeAmount))
				.SetStartAction(delegate
				{
					storeClient.clientIntent = StoreClient.ClientIntent.BUY;
					if (!lastDialogue2.choices.Exists(((GameItem, Dialogue, Action) c) => c.Item2 == option1))
					{
						lastDialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", option1, delegate
						{
						}, () => PlayerStore.Instance.playerCash >= PlayerStore.GetBusinessPermitPrice());
					}
				})
				.nextDialogue = lastDialogue2;
			lastDialogue2.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, delegate
			{
			});
		}
		option1.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt1_player", PlayerStore.GetBusinessPermitPrice())).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt1_1"))
			.SetStartAction(delegate
			{
				storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
				PlayerStore.Instance.ModCash(-PlayerStore.GetBusinessPermitPrice());
				RunStats.OnPermitBought(PlayerStore.GetBusinessPermitPrice());
				PlayerStore.Instance.hasBusinessPermit = true;
				StoreClientData.ScheduleLouFirstVisit(isPermitPurchase: true);
				PlayerStore.Instance.AddDirectSellingItemToTableMiddle(DirectoryMaster.Item("business_permit"), isOwend: true);
				EmporiumAnalytics.TrackBusinessPermitBought();
				AchievementTracker.Instance.CheckAuthorized();
			})
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt1_2"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt2_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt2_1"))
			.SetStartAction(delegate
			{
				storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
			})
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt2_2"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt2_3"))
			.SetStartAction(delegate
			{
				int item2 = RNG.ModuloWithCount(StoreStation.Instance.dayCounter, 7).Item1;
				PlayerStore.Instance.secData.CommitCrime("OPERATING_WITHOUT_LICENSE", item2 * 20 + 50);
			})
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt2_4"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt4_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_opt4_1"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.clientItemFeatureBuying.Add("bribeDonation");
		storeClient.onItemBought = delegate(GameItem item, int value)
		{
			item.RemoveItemFeatureByID("bribeDonation");
			int num = (int)item.GetNegociatedValue();
			PlayerStore.Instance.storeClientData.businessPermitBribeAmount -= num;
			RunStats.OnPermitBribe(num);
			Debug.Log("Bribe amount remaining: " + PlayerStore.Instance.storeClientData.businessPermitBribeAmount);
			if (PlayerStore.Instance.storeClientData.businessPermitBribeAmount <= 0)
			{
				DialogUIManager.Instance.HideChoiceBubble();
				storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
				Dialogue dialogue3 = new Dialogue();
				dialogue3.SetPriority(7);
				dialogue3.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_bribe_success_1")).SetEndAction(delegate
				{
					storeClient.dismissable = true;
				}).NextDialogue()
					.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_bribe_success_2"))
					.SetEndAction(delegate
					{
						DialogUIManager.Instance.CurrentClientLeave();
					});
				DialogUIManager.Instance.StartDialogue(dialogue3);
			}
			else
			{
				Dialogue dialogue4 = new Dialogue();
				dialogue4.SetPriority(6);
				dialogue4.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_biz_permit_bribe_progress", PlayerStore.Instance.storeClientData.businessPermitBribeAmount));
				dialogue4.SetNextDialogue(lastDialogue2);
				DialogUIManager.Instance.StartDialogue(dialogue4);
			}
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation(IgnoreExposeFunc: true);
		return storeClient;
	}

	public static int GetBribeAmount()
	{
		int item = RNG.ModuloWithCount(StoreStation.Instance.dayCounter, 7).Item1;
		return 25 + item * 10;
	}

	public static StoreClient CreateAugMed(int forcedTier = -1)
	{
		StoreClient obj = new StoreClient
		{
			identifier = "aug_med"
		};
		int num = (obj.augTier = ((forcedTier > 0) ? forcedTier : AugHelper.RollAugTier()));
		AugHelper.SetRandomAugFaction(obj, num);
		obj.isAug = true;
		obj.clientIntent = StoreClient.ClientIntent.BUY;
		obj.SetBudget(RNG.GetRandomInt(150, 250));
		obj.CanExposeChem = true;
		obj.mainDialogue = AugHelper.CreateRandomMedAugDialogue(obj.GetBudget(), num);
		obj.clientBuyingTagList.Add("MEDICAL");
		obj.CompleteClientCreation();
		return obj;
	}

	public static StoreClient CreateAugMaterial(int forcedTier = -1)
	{
		StoreClient obj = new StoreClient
		{
			identifier = "aug_material"
		};
		int num = (obj.augTier = ((forcedTier > 0) ? forcedTier : AugHelper.RollAugTier()));
		AugHelper.SetRandomAugFaction(obj, num);
		obj.isAug = true;
		obj.clientIntent = StoreClient.ClientIntent.BUY;
		obj.SetBudget(RNG.GetRandomInt(150, 250));
		obj.mainDialogue = AugHelper.CreateRandomMaterialAugDialogue(obj.GetBudget(), num);
		obj.clientBuyingTagList.Add("MATERIAL");
		obj.clientBuyingTagList.Add("ENERGY_CELL");
		obj.CompleteClientCreation();
		return obj;
	}
}
