using System;
using System.Collections.Generic;

public class NetworkUpgradeList
{
	public const int BREWER_COST = 200;

	public const int CHEMIST_COST = 250;

	public const int PHARMA_COST = 300;

	public const int MINER_COST = 250;

	public const int BREWER_SHAREHOLDER_COST = 1450;

	public const int MINER_SHAREHOLDER_COST = 900;

	public const int CHEMIST_SHAREHOLDER_COST = 1440;

	public const int PHARMA_SHAREHOLDER_COST = 2500;

	public const int LANDLORD_GRACE_COST = 100;

	public const int SEC_GRACE_COST = 250;

	public const int INSPECTION_INFORMANT_COST = 300;

	public const int WILD_FIXER_COST = 350;

	public const int WILD_FIXER2_COST = 450;

	public const int WILD_FIXER3_COST = 550;

	public static readonly int[] WILD_FIXER_FEE = new int[3] { 25, 45, 70 };

	public static readonly int[] WILD_FIXER_PERCENT = new int[3] { 15, 20, 25 };

	public static readonly int[] WILD_FIXER_FLAT = new int[3] { 50, 150, 400 };

	public const int EVIDENCE_REMOVAL_COST = 500;

	public const int BUY_REFERAL_COST = 200;

	public const int SELL_REFERAL_COST = 200;

	public const int CRIMINEL_NETWORK_COST = 350;

	public const int POWER_HIJACK_COST = 300;

	public const int POWER_HIJACK2_COST = 600;

	public const int POWER_HIJACK3_COST = 1000;

	public const int POWER_HIJACK1_CONVERTER_DISCOUNT = 15;

	public const int POWER_HIJACK2_CONVERTER_DISCOUNT = 25;

	public const int POWER_HIJACK3_CONVERTER_DISCOUNT = 50;

	public const int COMMERCIAL_POWER_COST = 350;

	public const int SHOWCASE_I_COST = 200;

	public const int SHOWCASE_II_COST = 500;

	public const int GUTTERFLOW_TAP_COST = 250;

	public const int RUSTWATER_TAP_COST = 350;

	public const int RUINED_MACHINE_UNLOCK_COST = 300;

	public const int RETIRED_FARMER_COST = 350;

	public const int RETIRED_GUNSMITH_COST = 650;

	public const int GUN_PERMIT_I_COST = 300;

	public const int GUN_PERMIT_II_COST = 600;

	public const int GUN_PERMIT_III_COST = 900;

	public const int RETIRED_CHEMIST_COST = 1000;

	public const int RETIRED_RANCHER_COST = 750;

	public const int WINEMAKER_COST = 200;

	public const int AQUACULTURE_COST = 400;

	public const int AQUACULTURE_II_COST = 1200;

	public const int WATER_TRADER_COST = 300;

	public const int MARKETPLACE_REQUESTS_COST = 250;

	public const int JACKSON1_COST = 400;

	public const int JACKSON2_COST = 650;

	public const int JACKSON_SPECIAL_STOCK_COST = 1250;

	public const int PLANT_DNA_COST = 1250;

	public const int PLANT_DNA_DISK_COUNT = 4;

	public const int DOSSIER_UPGRADE_COST = 300;

	public const int VENDING_MACHINE_COST = 1200;

	public const int VENDING_FOUNTAIN_COST = 1200;

	public const int TRASHCAN_UPGRADE_COST = 300;

	public const int RENOVATION1_COST = 500;

	public const int RENOVATION2_COST = 750;

	public const int RENOVATION3_COST = 1000;

	public const int HYPERTECH_SMUGGLER_COST = 2500;

	public const string ADVANCED_TAB = "Advanced";

	public const int ENDORSEMENT1_COST = 2500;

	public const int ENDORSEMENT2_COST = 5000;

	public const int ENDORSEMENT3_COST = 10000;

	public const int ENDORSEMENT_BUDGET_BONUS = 25;

	public const int GRACE_COST_INCREMENT = 100;

	public const int LANDLORD_GRACE_COST_MULTIPLIER = 2;

	public const int EVIDENCE_REMOVAL_COOLDOWN = 14;

	public const int CRIMINAL_NETWORK_COOLDOWN = 3;

	public const int COMMERCIAL_POWER_CHARGE_INCREASE = 100;

	public const int SHOWCASE_CUSTOMER_INCREASE = 1;

	public const int JACKSON_INVESTMENT1_VALUE = 10;

	public const int JACKSON_INVESTMENT2_VALUE = 20;

	public const int RENOVATION_ATTRACTIVENESS = 150;

	public const int GUN_PERMIT_I_REP = 20;

	public const int GUN_PERMIT_II_REP = 50;

	public const int GUN_PERMIT_III_REP = 80;

	public static Dictionary<string, Action> UnlockActions = new Dictionary<string, Action>
	{
		{
			"BREWER",
			delegate
			{
				PlayerStore.Instance.AddDirectToWeightedTable(DirectoryMaster.Item("card_brewer"), isOwend: true);
				StorePhoneClient.GetBrewerPhoneClient().phoneState = StorePhoneClient.PhoneState.Introduction;
			}
		},
		{
			"CHEMIST",
			delegate
			{
				PlayerStore.Instance.AddDirectToWeightedTable(DirectoryMaster.Item("card_chem"), isOwend: true);
				StorePhoneClient.GetChemPhoneClient().phoneState = StorePhoneClient.PhoneState.Introduction;
			}
		},
		{
			"PHARMA",
			delegate
			{
				PlayerStore.Instance.AddDirectToWeightedTable(DirectoryMaster.Item("card_pharma"), isOwend: true);
				StorePhoneClient.GetPharmaPhoneClient().phoneState = StorePhoneClient.PhoneState.Introduction;
			}
		},
		{
			"MINER",
			delegate
			{
				PlayerStore.Instance.AddDirectToWeightedTable(DirectoryMaster.Item("card_mining"), isOwend: true);
				StorePhoneClient.GetMiningPhoneClient().phoneState = StorePhoneClient.PhoneState.Introduction;
			}
		},
		{
			"LANDLORD_GRACE",
			delegate
			{
				PlayerStore.Instance.networkUpgrade["LANDLORD_GRACE"].cost *= 2;
			}
		},
		{
			"SEC_GRACE",
			delegate
			{
				PlayerStore.Instance.networkUpgrade["SEC_GRACE"].cost += 100;
			}
		},
		{
			"INSPECTION_INFORMANT",
			delegate
			{
				StoreService.FindServiceByID("SEC_INFORMANT").unlocked = true;
			}
		},
		{
			"WILD_FIXER",
			delegate
			{
				StoreService.FindServiceByID("WILD_FIXER").unlocked = true;
			}
		},
		{
			"EVIDENCE_REMOVAL",
			delegate
			{
				PlayerStore.Instance.secData.evidenceLevel = 0;
				PlayerStore.Instance.secData.state = 0;
				PlayerStore.Instance.networkUpgrade["EVIDENCE_REMOVAL"].cost += 100;
			}
		},
		{
			"BUY_REFERAL",
			delegate
			{
				StoreService.FindServiceByID("BUY_REFERAL").unlocked = true;
			}
		},
		{
			"SELL_REFERAL",
			delegate
			{
				StoreService.FindServiceByID("SELL_REFERAL").unlocked = true;
			}
		},
		{
			"CRIMINEL_NETWORK",
			delegate
			{
				StoreService.FindServiceByID("CRIMINEL_NETWORK").unlocked = true;
			}
		},
		{
			"POWER_HIJACK",
			delegate
			{
				StoreService.FindServiceByID("POWER_SERVICE").cost = 0;
				PlayerStore.Instance.UpdatePowerState();
				PlayerStore.Instance.LoadPower();
			}
		},
		{
			"COMMERCIAL_POWER",
			delegate
			{
			}
		},
		{
			"SHOWCASE_I",
			delegate
			{
				EmporiumEntry.Instance.UpgradeDisplayCaseI();
			}
		},
		{
			"SHOWCASE_II",
			delegate
			{
				EmporiumEntry.Instance.UpgradeDisplayCaseII();
			}
		},
		{
			"GUTTERFLOW_TAP",
			delegate
			{
				EmporiumEntry.Instance.faucet.shortDescription = LocHelper.GetLocalizedMechanic("mech_gutterflow_tap");
				EmporiumEntry.Instance.faucet.DisableTag("IS_TOOLTIP_DISABLED_TAG");
				EmporiumEntry.Instance.ApplyFaucetState(tapUnlocked: true);
			}
		},
		{
			"RUSTWATER_TAP",
			delegate
			{
				EmporiumEntry.Instance.faucet.shortDescription = LocHelper.GetLocalizedMechanic("mech_rustwater_tap");
				EmporiumEntry.Instance.faucet.DisableTag("IS_TOOLTIP_DISABLED_TAG");
				EmporiumEntry.Instance.ApplyFaucetState(tapUnlocked: true);
			}
		},
		{
			"RUINED_MACHINE_UNLOCK",
			delegate
			{
				PlayerStore.Instance.QueueFuturClient("retired_junker_intro", 1);
				PlayerStore.Instance.QueueFuturClient("machine_scrapper", 1);
			}
		},
		{
			"RETIRED_FARMER",
			delegate
			{
				PlayerStore.Instance.QueueFuturClient("retired_farmer", 1);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateOldHydroponic1());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("hydroponic_guide"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("fillerweed_seed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("fillerweed_seed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("nutrifruit_seed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("nutrifruit_seed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("nutrifruit_seed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("nutrient_tablet"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("nutrient_tablet"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("water_ration_small"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("water_ration_small"));
			}
		},
		{
			"RETIRED_RANCHER",
			delegate
			{
				PlayerStore.Instance.QueueFuturClient("retired_rancher", 1);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, AnimalCage.SpawnCageWithDispenser());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateRat("MALE", 5));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateRat("FEMALE", 5));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateRat("MALE", 3));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateRat("FEMALE", 3));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateRat("MALE", 1));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateRat("FEMALE", 1));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("kitchen_cleaver"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("labeler_blue"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("marking_pen"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateNutrifruitSmall());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("processed_meat"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("processed_meat"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("processed_meat"));
			}
		},
		{
			"GUN_PERMIT_I",
			delegate
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("sign_firearm"));
				Shotgun.ReturnIfLost();
			}
		},
		{
			"GUN_PERMIT_III",
			delegate
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("sign_firearm_heavy"));
			}
		},
		{
			"RETIRED_GUNSMITH",
			delegate
			{
				PlayerStore.Instance.QueueFuturClient("retired_gunsmith", 1);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("silencer"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("rds"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("mag_22"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("mag_22"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("mag_22_high"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("laser_toy"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("silencer_makeshift"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("laser"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("internal_springs_improved"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("gun_part"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("screwdriver"));
				GameItem gameItem4 = ItemFeatureHelper.SetShabby(PreBuiltItemHelper.CreatePrinter());
				GraphUtils.TryAcceptAll(gameItem4, PreBuiltItemHelper.EnergyCredit(100));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, gameItem4);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("printer_chip_pistol")));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("printer_plastic"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("printer_plastic"));
			}
		},
		{
			"RETIRED_CHEMIST",
			delegate
			{
				PlayerStore.Instance.QueueFuturClient("retired_chemist", 1);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("chemistry_guide"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("chem_scanner")));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("blender")));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("heating_plate")));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemFeatureHelper.SetShabby(PreBuiltItemHelper.CreateEvaporator(40)));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("chem_finisher")));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("1ml_dropper"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("5ml_dropper"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("25ml_cylinder"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("100ml_cylinder"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("bottled_water"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("bottled_water"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("common_chemical"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("coffee_bean"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("water_pitcher"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("kotton_fabric"));
			}
		},
		{
			"WATER_TRADER",
			delegate
			{
				PlayerStore.Instance.QueueFuturClient("storyWaterVendor", 1);
				PlayerStore.Instance.QueueFuturClient("story_water_buyer", 1);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("water_guide"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.FullWaterTestStripBox());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.FullWaterPurificationTabletBox());
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("water_pitcher"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("water_filter"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("sign_water"));
				GameItem gameItem3 = DirectoryMaster.Item("portable_water_purifier");
				DurabilityHelper.SetCurrentDurability(gameItem3, 75);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, gameItem3);
			}
		},
		{
			"MARKETPLACE_REQUESTS",
			delegate
			{
				StoreUIManager.Instance.pcScreenObject.SetActive(value: true);
				PlayerStore.Instance.marketplaceUnlockedDay = StoreStation.Instance.dayCounter;
				StoreRequest.ActivateRequest(StoreRequestList.GeorgioToiletPaper());
				StoreRequest.HandleUpperLevelRequest();
				StoreRequest.HandleLowerLevelRequest(force: true);
				StorePoster.LoadStorePoster();
			}
		},
		{
			"JACKSON1",
			delegate
			{
				PlayerStore.Instance.storeClientData.jacksonInvestment += 10;
			}
		},
		{
			"JACKSON2",
			delegate
			{
				PlayerStore.Instance.storeClientData.jacksonInvestment += 20;
			}
		},
		{
			"JACKSON_SPECIAL_STOCK",
			delegate
			{
				VanillaAfterhourEvents.OnSpecialStockUnlocked();
			}
		},
		{
			"RENOVATION1",
			delegate
			{
				PlayerStore.Instance.baseStoreAttractiveness += 150;
			}
		},
		{
			"RENOVATION2",
			delegate
			{
				PlayerStore.Instance.baseStoreAttractiveness += 150;
			}
		},
		{
			"RENOVATION3",
			delegate
			{
				PlayerStore.Instance.baseStoreAttractiveness += 150;
			}
		},
		{
			"AQUACULTURE_II",
			delegate
			{
				string[] array = new string[2] { "MALE", "FEMALE" };
				foreach (string sex in array)
				{
					GameItem gameItem2 = AquacultureItems.SpawnFish(FishSpecies.Eel, sex, 0);
					if (gameItem2 != null)
					{
						gameItem2.unitValue = 100L;
						GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, gameItem2);
					}
				}
			}
		},
		{
			"AQUACULTURE",
			delegate
			{
				PlayerStore.Instance.QueueFuturClient("retired_aquafarmer", 1);
				GameItem gameItem = ItemSpawner.Spawn("ras");
				GraphUtils.TryAcceptAll(gameItem, ItemSpawner.Spawn("ras_water_tank"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, gameItem);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, AquacultureItems.SpawnFertilizedRoe(FishSpecies.Tilapia));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.EnergyCredit(50));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, WaterPremadeHelper.AccurateRustWater("water_jug"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("industrial_fish_feed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("industrial_fish_feed"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("kitchen_knife"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, ItemSpawner.Spawn("marking_pen"));
			}
		},
		{
			"WINEMAKER",
			delegate
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("wine_yeast"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("wine_yeast"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("wine_yeast"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("wine_bottle"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("wine_bottle"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("wine_bottle"));
				GameItem item = DirectoryMaster.Item("wine_berry");
				WineHelper.SetBerryCondition(item, 1);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, item);
				GameItem item2 = DirectoryMaster.Item("wine_berry");
				WineHelper.SetBerryCondition(item2, -1, 2);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, item2);
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("water_ration"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("magnifier"));
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("wine_book"));
			}
		},
		{
			"DOSSIER_UPGRADE",
			delegate
			{
				EmporiumEntry.Instance.UpgradeDossier();
			}
		},
		{
			"TRASHCAN_UPGRADE",
			delegate
			{
				EmporiumEntry.Instance.UpgradeTrashcan();
			}
		},
		{
			"VENDING_MACHINE",
			delegate
			{
				if (!PlayerStore.Instance.isVendingMachineUnlocked)
				{
					VendingMachineHelper.UnlockVendingMachine();
				}
			}
		},
		{
			"VENDING_FOUNTAIN",
			delegate
			{
				if (!PlayerStore.Instance.isVendingFountainUnlocked)
				{
					VendingFountainHelper.UnlockVendingFountain();
				}
			}
		},
		{
			"PLANT_DNA",
			delegate
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, PreBuiltItemHelper.CreateDnaManipulator());
				for (int i = 0; i < 4; i++)
				{
					GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("plant_dna_disk"));
				}
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, DirectoryMaster.Item("gene_scanner"));
			}
		},
		{
			"HYPERTECH_SMUGGLER",
			delegate
			{
				StoreService hypertechSmugglerService = StoreService.GetHypertechSmugglerService();
				if (hypertechSmugglerService != null)
				{
					hypertechSmugglerService.introduced = true;
					hypertechSmugglerService.unlocked = true;
					hypertechSmugglerService.currentCooldown = hypertechSmugglerService.totalCooldown + 1;
				}
				PlayerStore.Instance.QueueFuturClient("hypertech_smuggler", 1);
			}
		}
	};

	private const bool AQUACULTURE_IN_BUILD = true;

	private const bool PLANT_DNA_IN_BUILD = true;

	private const bool VENDING_IN_BUILD = true;

	public static NetworkUpgrade CreateBrewer()
	{
		return new NetworkUpgrade("BREWER")
		{
			subtitle = "Introduction",
			cost = 200,
			state = 1,
			recommended = true
		};
	}

	public static NetworkUpgrade CreateChemist()
	{
		return new NetworkUpgrade("CHEMIST")
		{
			subtitle = "Introduction",
			cost = 250,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreatePharma()
	{
		return new NetworkUpgrade("PHARMA")
		{
			subtitle = "Introduction",
			cost = 300,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateMiner()
	{
		return new NetworkUpgrade("MINER")
		{
			subtitle = "Introduction",
			cost = 250,
			state = 1
		};
	}

	private static NetworkUpgrade CreateShareholder(string id, string introductionId, int cost, bool lockInDemo)
	{
		return new NetworkUpgrade(id)
		{
			subtitle = "Introduction",
			cost = cost,
			state = 1,
			lockInDemo = lockInDemo,
			prerequisite = { introductionId },
			iconKey = introductionId
		};
	}

	public static NetworkUpgrade CreateBrewerShareholder()
	{
		return CreateShareholder("BREWER_SHAREHOLDER", "BREWER", 1450, lockInDemo: false);
	}

	public static NetworkUpgrade CreateMinerShareholder()
	{
		return CreateShareholder("MINER_SHAREHOLDER", "MINER", 900, lockInDemo: false);
	}

	public static NetworkUpgrade CreateChemistShareholder()
	{
		return CreateShareholder("CHEMIST_SHAREHOLDER", "CHEMIST", 1440, lockInDemo: true);
	}

	public static NetworkUpgrade CreatePharmaShareholder()
	{
		return CreateShareholder("PHARMA_SHAREHOLDER", "PHARMA", 2500, lockInDemo: true);
	}

	public static NetworkUpgrade CreateLandlordGrace()
	{
		return new NetworkUpgrade("LANDLORD_GRACE")
		{
			subtitle = "Favor",
			cost = 100,
			isRepeatable = true,
			state = 2,
			cooldownDuration = 15,
			heldUntilUsed = true
		};
	}

	public static NetworkUpgrade CreatePrisonGrace()
	{
		return new NetworkUpgrade("SEC_GRACE")
		{
			subtitle = "Favor",
			cost = 250,
			isRepeatable = true,
			state = 1
		};
	}

	public static NetworkUpgrade CreateInspectionInformation()
	{
		return new NetworkUpgrade("INSPECTION_INFORMANT")
		{
			subtitle = "Informant",
			cost = 300,
			state = 1
		};
	}

	public static NetworkUpgrade CreateFixer()
	{
		return new NetworkUpgrade("WILD_FIXER")
		{
			subtitle = "Informant",
			cost = 350,
			state = 1
		};
	}

	public static NetworkUpgrade CreateFixer2()
	{
		return new NetworkUpgrade("WILD_FIXER2")
		{
			subtitle = "Informant",
			cost = 450,
			state = 1,
			prerequisite = { "WILD_FIXER" },
			titleProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_WILD_FIXER2_title"),
			descriptionProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_WILD_FIXER2_desc")
		};
	}

	public static NetworkUpgrade CreateFixer3()
	{
		return new NetworkUpgrade("WILD_FIXER3")
		{
			subtitle = "Informant",
			cost = 550,
			state = 1,
			prerequisite = { "WILD_FIXER2" },
			titleProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_WILD_FIXER3_title"),
			descriptionProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_WILD_FIXER3_desc")
		};
	}

	public static NetworkUpgrade CreateEvidenceRemoval()
	{
		return new NetworkUpgrade("EVIDENCE_REMOVAL")
		{
			subtitle = "Informant",
			cost = 500,
			cooldownDuration = 14,
			state = 1
		};
	}

	public static NetworkUpgrade CreateBuyReferal()
	{
		return new NetworkUpgrade("BUY_REFERAL")
		{
			subtitle = "Referral",
			recommended = true,
			cost = 200,
			state = 1
		};
	}

	public static NetworkUpgrade CreateSellReferal()
	{
		return new NetworkUpgrade("SELL_REFERAL")
		{
			subtitle = "Referral",
			cost = 200,
			state = 1
		};
	}

	public static NetworkUpgrade CreateCriminelNetwork()
	{
		return new NetworkUpgrade("CRIMINEL_NETWORK")
		{
			subtitle = "Referral",
			cost = 350,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreatePowerHijack()
	{
		return new NetworkUpgrade("POWER_HIJACK")
		{
			subtitle = "Service",
			cost = 300,
			state = 1
		};
	}

	public static NetworkUpgrade CreatePowerHijack2()
	{
		return new NetworkUpgrade("POWER_HIJACK2")
		{
			subtitle = "Service",
			cost = 600,
			state = 1,
			prerequisite = { "POWER_HIJACK" },
			titleProvider = () => LocHelper.GetLocalizedOrDefault("NetworkUpgrade", "network_POWER_HIJACK2_title", "Power Line Hijack II"),
			descriptionProvider = () => LocHelper.GetLocalizedOrDefault("NetworkUpgrade", "network_POWER_HIJACK2_desc", "Wild's crew splices a bigger solar array into your line. It now carries part of your Current Converters' load: they cost {0}% less to run.", 25)
		};
	}

	public static NetworkUpgrade CreatePowerHijack3()
	{
		return new NetworkUpgrade("POWER_HIJACK3")
		{
			subtitle = "Service",
			cost = 1000,
			state = 1,
			prerequisite = { "POWER_HIJACK2" },
			titleProvider = () => LocHelper.GetLocalizedOrDefault("NetworkUpgrade", "network_POWER_HIJACK3_title", "Power Line Hijack III"),
			descriptionProvider = () => LocHelper.GetLocalizedOrDefault("NetworkUpgrade", "network_POWER_HIJACK3_desc", "Wild's crew taps a whole rooftop of panels for you. Your Current Converters now cost {0}% less to run in total.", 50)
		};
	}

	public static NetworkUpgrade CreateCommercialPower()
	{
		return new NetworkUpgrade("COMMERCIAL_POWER")
		{
			subtitle = "Service",
			cost = 350,
			state = 1
		};
	}

	public static NetworkUpgrade CreateShowcaseI()
	{
		return new NetworkUpgrade("SHOWCASE_I")
		{
			subtitle = "Store",
			cost = 200,
			state = 1,
			recommended = true
		};
	}

	public static NetworkUpgrade CreateShowcaseII()
	{
		return new NetworkUpgrade("SHOWCASE_II")
		{
			subtitle = "Store",
			cost = 500,
			state = 1,
			prerequisite = { "SHOWCASE_I" },
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateGutterflowTap()
	{
		return new NetworkUpgrade("GUTTERFLOW_TAP")
		{
			subtitle = "Water",
			cost = 250,
			state = 1,
			prerequisite = { "WATER_TRADER" },
			lockInDemo = false
		};
	}

	public static NetworkUpgrade CreateRustwaterTap()
	{
		return new NetworkUpgrade("RUSTWATER_TAP")
		{
			subtitle = "Water",
			cost = 350,
			state = 1,
			prerequisite = { "GUTTERFLOW_TAP" },
			lockInDemo = false
		};
	}

	public static NetworkUpgrade CreateRuinedMachineUnlock()
	{
		return new NetworkUpgrade("RUINED_MACHINE_UNLOCK")
		{
			subtitle = "Referral",
			cost = 300,
			state = 1,
			lockInDemo = true,
			hidden = true
		};
	}

	public static NetworkUpgrade CreateRetiredFarmerUnlock()
	{
		return new NetworkUpgrade("RETIRED_FARMER")
		{
			subtitle = "Hydroponic",
			cost = 350,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateRetiredGunsmithUnlock()
	{
		return new NetworkUpgrade("RETIRED_GUNSMITH")
		{
			subtitle = "Gunsmith",
			cost = 650,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateGunPermitI()
	{
		return new NetworkUpgrade("GUN_PERMIT_I")
		{
			subtitle = "Gunsmith",
			cost = 300,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateGunPermitII()
	{
		return new NetworkUpgrade("GUN_PERMIT_II")
		{
			subtitle = "Gunsmith",
			cost = 600,
			state = 1,
			prerequisite = { "GUN_PERMIT_I" },
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateGunPermitIII()
	{
		return new NetworkUpgrade("GUN_PERMIT_III")
		{
			subtitle = "Gunsmith",
			cost = 900,
			state = 1,
			prerequisite = { "GUN_PERMIT_II" },
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateRetiredChemistUnlock()
	{
		return new NetworkUpgrade("RETIRED_CHEMIST")
		{
			subtitle = "Referral",
			cost = 1000,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateRetiredRancherUnlock()
	{
		return new NetworkUpgrade("RETIRED_RANCHER")
		{
			subtitle = "Referral",
			cost = 750,
			state = 1,
			lockInDemo = false
		};
	}

	public static NetworkUpgrade CreateWaterTraderUnlock()
	{
		return new NetworkUpgrade("WATER_TRADER")
		{
			subtitle = "Water",
			cost = 300,
			state = 1,
			lockInDemo = false,
			recommended = true
		};
	}

	public static NetworkUpgrade CreatePlantDnaUnlock()
	{
		return new NetworkUpgrade("PLANT_DNA")
		{
			prerequisite = { "RETIRED_FARMER" },
			subtitle = "Hydroponic",
			cost = 1250,
			state = 1,
			lockInDemo = false
		};
	}

	public static NetworkUpgrade CreateDossierUpgrade()
	{
		return new NetworkUpgrade("DOSSIER_UPGRADE")
		{
			subtitle = "Store",
			cost = 300,
			state = 1
		};
	}

	public static NetworkUpgrade CreateTrashcanUpgrade()
	{
		return new NetworkUpgrade("TRASHCAN_UPGRADE")
		{
			subtitle = "Store",
			cost = 300,
			state = 1
		};
	}

	public static NetworkUpgrade CreateVendingMachineUpgrade()
	{
		return new NetworkUpgrade("VENDING_MACHINE")
		{
			subtitle = "Store",
			cost = 1200,
			state = 1,
			lockInDemo = false
		};
	}

	public static NetworkUpgrade CreateVendingFountainUpgrade()
	{
		return new NetworkUpgrade("VENDING_FOUNTAIN")
		{
			subtitle = "Store",
			cost = 1200,
			state = 1,
			lockInDemo = false
		};
	}

	public static NetworkUpgrade CreateAquacultureIIUnlock()
	{
		return new NetworkUpgrade("AQUACULTURE_II")
		{
			prerequisite = { "AQUACULTURE" },
			subtitle = "Aquaculture",
			cost = 1200,
			state = 1,
			lockInDemo = false
		};
	}

	public static NetworkUpgrade CreateAquacultureUnlock()
	{
		return new NetworkUpgrade("AQUACULTURE")
		{
			prerequisite = { "WATER_TRADER" },
			subtitle = "Aquaculture",
			cost = 400,
			state = 1,
			lockInDemo = false,
			recommended = true
		};
	}

	public static NetworkUpgrade CreateWinemakerUnlock()
	{
		return new NetworkUpgrade("WINEMAKER")
		{
			subtitle = "Referral",
			cost = 200,
			state = 1,
			recommended = true
		};
	}

	public static NetworkUpgrade CreateMarketplaceRequestsUnlock()
	{
		return new NetworkUpgrade("MARKETPLACE_REQUESTS")
		{
			subtitle = "Service",
			cost = 250,
			state = 1,
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateJacksonInvestment1()
	{
		return new NetworkUpgrade("JACKSON1")
		{
			subtitle = "Supplier",
			cost = 400,
			state = 1
		};
	}

	public static NetworkUpgrade CreateJacksonSpecialStock()
	{
		return new NetworkUpgrade("JACKSON_SPECIAL_STOCK")
		{
			subtitle = "Supplier",
			cost = 1250,
			state = 1,
			prerequisite = { "JACKSON2" },
			lockInDemo = true,
			iconKey = "JACKSON2"
		};
	}

	public static NetworkUpgrade CreateJacksonInvestment2()
	{
		return new NetworkUpgrade("JACKSON2")
		{
			subtitle = "Supplier",
			cost = 650,
			state = 1,
			prerequisite = { "JACKSON1" },
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateRenovation1()
	{
		return new NetworkUpgrade("RENOVATION1")
		{
			subtitle = "Store",
			cost = 500,
			state = 1
		};
	}

	public static NetworkUpgrade CreateRenovation2()
	{
		return new NetworkUpgrade("RENOVATION2")
		{
			subtitle = "Store",
			cost = 750,
			state = 1,
			prerequisite = { "RENOVATION1" }
		};
	}

	public static NetworkUpgrade CreateRenovation3()
	{
		return new NetworkUpgrade("RENOVATION3")
		{
			subtitle = "Store",
			cost = 1000,
			state = 1,
			prerequisite = { "RENOVATION2" },
			lockInDemo = true
		};
	}

	public static NetworkUpgrade CreateHypertechSmugglerReferral()
	{
		return new NetworkUpgrade("HYPERTECH_SMUGGLER")
		{
			subtitle = "Referral",
			cost = 2500,
			state = 1,
			lockInDemo = true,
			tab = "Advanced",
			recommended = true,
			titleProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_HYPERTECH_SMUGGLER_title"),
			descriptionProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_HYPERTECH_SMUGGLER_desc")
		};
	}

	public static NetworkUpgrade CreateEndorsement1()
	{
		NetworkUpgrade networkUpgrade = CreateEndorsement("ENDORSEMENT1", 2500, null);
		networkUpgrade.titleProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_ENDORSEMENT1_title");
		networkUpgrade.descriptionProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_ENDORSEMENT1_desc", 25);
		return networkUpgrade;
	}

	public static NetworkUpgrade CreateEndorsement2()
	{
		NetworkUpgrade networkUpgrade = CreateEndorsement("ENDORSEMENT2", 5000, "ENDORSEMENT1");
		networkUpgrade.titleProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_ENDORSEMENT2_title");
		networkUpgrade.descriptionProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_ENDORSEMENT2_desc", 25);
		return networkUpgrade;
	}

	public static NetworkUpgrade CreateEndorsement3()
	{
		NetworkUpgrade networkUpgrade = CreateEndorsement("ENDORSEMENT3", 10000, "ENDORSEMENT2");
		networkUpgrade.titleProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_ENDORSEMENT3_title");
		networkUpgrade.descriptionProvider = () => LocHelper.GetLocalizedNetworkUpgrade("network_ENDORSEMENT3_desc", 25);
		return networkUpgrade;
	}

	private static NetworkUpgrade CreateEndorsement(string id, int cost, string prerequisite)
	{
		NetworkUpgrade networkUpgrade = new NetworkUpgrade(id);
		networkUpgrade.subtitle = "Endorsement";
		networkUpgrade.cost = cost;
		networkUpgrade.state = 1;
		networkUpgrade.tab = "Advanced";
		if (prerequisite != null)
		{
			networkUpgrade.prerequisite.Add(prerequisite);
		}
		return networkUpgrade;
	}
}
