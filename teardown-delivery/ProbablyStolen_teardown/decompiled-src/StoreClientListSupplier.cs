using System;
using System.Collections.Generic;

public class StoreClientListSupplier
{
	public const string HYPERTECH_SMUGGLER_ID = "hypertech_smuggler";

	public const int HYPERTECH_SMUGGLER_STOCK_COUNT = 5;

	private static readonly string[] HYPERTECH_SMUGGLER_STOCK = new string[6] { "hypertech_electronic", "energy_credit_hyp", "turbo_booster_hypertech", "hypertech_bay", "mind_reprogrammer", "system_uncapped_neural_core" };

	private static readonly HashSet<string> HYPERTECH_SMUGGLER_ONE_PER_VISIT = new HashSet<string> { "hypertech_bay", "system_uncapped_neural_core", "mind_reprogrammer" };

	public static StoreClient CreateCourrierBeer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierRedImp";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_red_imp_courrier");
		storeClient.spriteName = SpriteDict.GetCourrierSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.wholesaleDiscount = ShareholderHelper.WholesaleDiscount("BREWER_SHAREHOLDER", -20);
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_beer_1")).SetEndAction(delegate
		{
			if (StorePhoneClient.GetBrewerPhoneClient().stockType == 0)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullBeer());
			}
			else if (StorePhoneClient.GetBrewerPhoneClient().stockType == 1)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullBeer());
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullBeer());
			}
		});
		storeClient.onItemSold = delegate(GameItem item, int cost)
		{
			StorePhoneClient.GetBrewerPhoneClient().amountSpent += cost;
		};
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_beer_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCourrierBeerFree()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierRedImpFree";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_red_imp_courrier");
		storeClient.spriteName = SpriteDict.GetCourrierSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.wholesaleDiscount = -20;
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_beer_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullBeer(), isOwend: true);
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_beer_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCourrierMining()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierMining";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_gp_mining_courrier");
		storeClient.spriteName = SpriteDict.GetCourrierSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.wholesaleDiscount = ShareholderHelper.WholesaleDiscount("MINER_SHAREHOLDER", -20);
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_mining_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_mining_2"))
			.SetEndAction(delegate
			{
				if (StorePhoneClient.GetMiningPhoneClient().stockType == 0)
				{
					for (int i = 0; i < 6; i++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
					}
				}
				else if (StorePhoneClient.GetMiningPhoneClient().stockType == 1)
				{
					for (int j = 0; j < 12; j++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("common_ore");
					}
				}
			});
		storeClient.onItemSold = delegate(GameItem item, int cost)
		{
			StorePhoneClient.GetMiningPhoneClient().amountSpent += cost;
		};
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_mining_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCourrierMiningFree()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierMiningFree";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_gp_mining_courrier");
		storeClient.spriteName = SpriteDict.GetCourrierSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.wholesaleDiscount = -20;
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_mining_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_mining_2"))
			.SetEndAction(delegate
			{
				for (int i = 0; i < 8; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("common_ore", isOwend: true);
				}
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_mining_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCourrierChem()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierChem";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_ashish");
		storeClient.spriteName = SpriteDict.GetAshishSpriteName();
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELLNBUY;
		storeClient.wholesaleDiscount = ShareholderHelper.WholesaleDiscount("CHEMIST_SHAREHOLDER", -20);
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.SetBudget(200);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_chem_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_chem_2"))
			.SetEndAction(delegate
			{
				if (StorePhoneClient.GetChemPhoneClient().stockType == 0)
				{
					for (int i = 0; i < 8; i++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					}
				}
				else if (StorePhoneClient.GetChemPhoneClient().stockType == 1)
				{
					for (int j = 0; j < 16; j++)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust");
					}
				}
			});
		storeClient.onItemSold = delegate(GameItem item, int cost)
		{
			StorePhoneClient.GetChemPhoneClient().amountSpent += cost;
		};
		storeClient.clientBuyingTagList.Add("CHEMICAL_SUPPLIES");
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_chem_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.onReportAction = (Action)Delegate.Combine(storeClient.onReportAction, (Action)delegate
		{
			StorePhoneClient.GetChemPhoneClient().phoneState = StorePhoneClient.PhoneState.NotResponding;
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCourrierChemFree()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierChemFree";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_ashish");
		storeClient.spriteName = SpriteDict.GetAshishSpriteName();
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.wholesaleDiscount = -20;
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_chem_1")).SetEndAction(delegate
		{
			for (int i = 0; i < 8; i++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("dream_dust", isOwend: true);
			}
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_chem_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.onReportAction = (Action)Delegate.Combine(storeClient.onReportAction, (Action)delegate
		{
			StorePhoneClient.GetChemPhoneClient().phoneState = StorePhoneClient.PhoneState.NotResponding;
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCourrierPharma()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierPharma";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_courrier");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.wholesaleDiscount = ShareholderHelper.WholesaleDiscount("PHARMA_SHAREHOLDER", -20);
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_pharma_1")).SetEndAction(delegate
		{
			if (StorePhoneClient.GetPharmaPhoneClient().stockType == 0)
			{
				for (int i = 0; i < 5; i++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
			}
			else if (StorePhoneClient.GetPharmaPhoneClient().stockType == 1)
			{
				for (int j = 0; j < 10; j++)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
				}
			}
		});
		storeClient.onItemSold = delegate(GameItem item, int cost)
		{
			StorePhoneClient.GetPharmaPhoneClient().amountSpent += cost;
		};
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_pharma_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateCourrierPharmaFree()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "courrierPharmaFree";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_courrier");
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientFaction = "FACTION_UPPER_LEVEL";
		storeClient.isBargainAvailable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.wholesaleDiscount = -20;
		storeClient.isOfferWholesale = true;
		storeClient.isRegularSellDisable = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_pharma_1")).SetEndAction(delegate
		{
			for (int i = 0; i < 6; i++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: true);
			}
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_courrier_pharma_done")).SetEndAction(delegate
		{
			DialogUIManager.Instance.CurrentClientLeave();
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScavFoodSeller()
	{
		return CreateScavGenericSeller("FOOD", "scavGenericFoodSeller");
	}

	public static StoreClient CreateScavHouseholdSeller()
	{
		return CreateScavGenericSeller("HOUSEHOLD_GOOD", "scavGenericHouseholdSeller");
	}

	public static StoreClient CreateScavMaterialSeller()
	{
		return CreateScavGenericSeller("MATERIAL", "scavGenericMaterialSeller");
	}

	public static StoreClient CreateScavMedSeller()
	{
		return CreateScavGenericSeller("MEDICAL", "scavGenericMedSeller");
	}

	public static void AddScavFoodStock()
	{
		int chance = 25 + Math.Max(-15, StoreReputation.GetLLReputation() / 2);
		int chance2 = 10 + Math.Max(-5, StoreReputation.GetLLReputation() / 5);
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.SpawnFromTable("packedFoodTable"));
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("zerochew");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("zerochew");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("processed_cheese");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("processed_cheese");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("cat_bar");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("cat_bar");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("li_eat_snackbar");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("li_eat_snackbar");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("processed_cheese");
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit");
	}

	public static void AddScavHouseholdStock()
	{
		int chance = 25 + Math.Max(-15, StoreReputation.GetLLReputation() / 2);
		int chance2 = 10 + Math.Max(-5, StoreReputation.GetLLReputation() / 5);
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("shampoo");
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("water_pitcher");
		}
		if (RNG.Roll(chance) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast", isOwend: false, isStolen: true);
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper");
	}

	public static void AddScavMaterialStock()
	{
		int chance = 25 + Math.Max(-15, StoreReputation.GetLLReputation() / 2);
		int chance2 = 10 + Math.Max(-5, StoreReputation.GetLLReputation() / 5);
		if (RNG.Roll(20))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("screwdriver");
		}
		if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("welder");
		}
		if (RNG.Roll(15))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire_cutter");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
		}
		if (RNG.Roll(80))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("box_cutter");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_fabric");
		}
		if (RNG.Roll(50))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
		}
		if (RNG.Roll(50))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
		}
		if (RNG.Roll(50))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
		}
		if (RNG.Roll(50))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
	}

	public static void AddScavMedStock()
	{
		int chance = 25 + Math.Max(-15, StoreReputation.GetLLReputation() / 2);
		int chance2 = 10 + Math.Max(-5, StoreReputation.GetLLReputation() / 5);
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pure_white_injector");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("rubbing_alcohol");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("smelling_salt");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item");
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("surgery_tool");
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("surgery_tool");
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_medical");
		}
		if (PlayerStore.Instance.isInjectorIntroduced && RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRandomInjector());
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("hemostatic_bandage_item");
	}

	public static StoreClient CreateScavGenericSeller(string type, string id)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = id;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scavenger");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isOfferWholesale = true;
		storeClient.wholesaleDiscount = -RNG.GetRandomInt(15, 25);
		List<string> scavGreetingLines = StoreClientList.GetScavGreetingLines();
		List<string> scavOfferLines = StoreClientList.GetScavOfferLines();
		storeClient.mainDialogue.SetText(storeClient.displayName, scavGreetingLines[RNG.GetRandomInt(0, scavGreetingLines.Count - 1)]).NextDialogue().SetText(storeClient.displayName, scavOfferLines[RNG.GetRandomInt(0, scavOfferLines.Count - 1)])
			.SetEndAction(delegate
			{
				if (type.Equals("FOOD"))
				{
					AddScavFoodStock();
				}
				if (type.Equals("HOUSEHOLD_GOOD"))
				{
					AddScavHouseholdStock();
				}
				else if (type.Equals("MATERIAL"))
				{
					AddScavMaterialStock();
				}
				else if (type.Equals("MEDICAL"))
				{
					AddScavMedStock();
				}
				storeClient.OnIntroduced();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateThiefFoodSeller()
	{
		return CreateThiefGenericSeller("FOOD", "thiefGenericFoodSeller");
	}

	public static StoreClient CreateThiefHouseholdSeller()
	{
		return CreateThiefGenericSeller("HOUSEHOLD_GOOD", "thiefGenericHouseholdSeller");
	}

	public static StoreClient CreateThiefMaterialSeller()
	{
		return CreateThiefGenericSeller("MATERIAL", "thiefGenericMaterialSeller");
	}

	public static StoreClient CreateThiefMedSeller()
	{
		return CreateThiefGenericSeller("MEDICAL", "thiefGenericMedSeller");
	}

	public static StoreClient CreateThiefLuxurySeller()
	{
		return CreateThiefGenericSeller("LUXURY_ITEM", "thiefGenericLuxurySeller");
	}

	public static void AddThiefFoodStock()
	{
		int chance = 25 + Math.Max(-15, StoreReputation.GetBMReputation() / 2);
		int chance2 = 10 + Math.Max(-5, StoreReputation.GetBMReputation() / 5);
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nudka", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nudka", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("raw_meat", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("processed_cheese");
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat", isOwend: false, isStolen: true);
	}

	public static void AddThiefHouseholdStock()
	{
		int chance = 25 + Math.Max(-15, StoreReputation.GetBMReputation() / 2);
		int chance2 = 10 + Math.Max(-5, StoreReputation.GetBMReputation() / 5);
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pack_condom", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pack_condom", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("aquascan", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("kitchen_cleaver", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_superyeast", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_superyeast", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast_red", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast_red", isOwend: false, isStolen: true);
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper", isOwend: false, isStolen: true);
	}

	public static void AddThiefMaterialStock()
	{
		int chance = 50 + Math.Max(-25, StoreReputation.GetBMReputation() / 2);
		Math.Max(-15, StoreReputation.GetBMReputation() / 2);
		Math.Max(-5, StoreReputation.GetBMReputation() / 5);
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("gun_part", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("gun_part", isOwend: false, isStolen: true);
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("common_electronic", isOwend: false, isStolen: true);
	}

	public static void AddThiefMedStock()
	{
		int chance = 50 + Math.Max(-25, StoreReputation.GetBMReputation() / 2);
		Math.Max(-15, StoreReputation.GetBMReputation() / 2);
		int chance2 = 10 + Math.Max(-5, StoreReputation.GetBMReputation() / 5);
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("rubbing_alcohol", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("rubbing_alcohol", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_medical", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("surgery_tool", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("surgery_tool", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("salve", isOwend: false, isStolen: true);
		}
		if (RNG.Roll(chance2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("salve", isOwend: false, isStolen: true);
		}
		PlayerStore.Instance.AddDirectSellingItemToTable("pink_injector", isOwend: false, isStolen: true);
	}

	public static void AddThiefLuxuryStock()
	{
	}

	public static StoreClient CreateThiefGenericSeller(string type, string id)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = id;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_thief");
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.spriteName = SpriteDict.GetRandomShadySpriteName();
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.cooldown = 2;
		storeClient.SetWholesale(15);
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thief_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_thief_2"))
			.SetEndAction(delegate
			{
				if (type.Equals("FOOD"))
				{
					AddThiefFoodStock();
				}
				if (type.Equals("HOUSEHOLD_GOOD"))
				{
					AddThiefHouseholdStock();
				}
				else if (type.Equals("MATERIAL"))
				{
					AddThiefMaterialStock();
				}
				else if (type.Equals("MEDICAL"))
				{
					AddThiefMedStock();
				}
				else if (type.Equals("LUXURY_ITEM"))
				{
					AddThiefLuxuryStock();
				}
				storeClient.OnIntroduced();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateHypertechSmuggler()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "hypertech_smuggler";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_hypertech_smuggler");
		storeClient.spriteName = (RNG.Roll(50) ? SpriteDict.GetRandomBMMaleSpriteName() : SpriteDict.GetRandomBMFemaleSpriteName());
		StoreClientFactionSetup.InitBlackMarket(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.isBargainAvailable = false;
		storeClient.clientItemFeatureBuying.Add("premiumBuyHypertech");
		storeClient.chargesContrabandMarkup = true;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hypertech_smuggler_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hypertech_smuggler_2"))
			.SetEndAction(delegate
			{
				List<string> list = new List<string>(HYPERTECH_SMUGGLER_STOCK);
				for (int i = 0; i < 5; i++)
				{
					if (list.Count <= 0)
					{
						break;
					}
					string text = list[RNG.GetRandomInt(0, list.Count - 1)];
					if (HYPERTECH_SMUGGLER_ONE_PER_VISIT.Contains(text))
					{
						list.Remove(text);
					}
					PlayerStore.Instance.AddDirectSellingItemToTable(text);
				}
			});
		storeClient.onReportAction = (Action)Delegate.Combine(storeClient.onReportAction, (Action)delegate
		{
			StoreService hypertechSmugglerService = StoreService.GetHypertechSmugglerService();
			if (hypertechSmugglerService != null)
			{
				hypertechSmugglerService.unlocked = false;
				hypertechSmugglerService.introduced = false;
				hypertechSmugglerService.isUsing = false;
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}
