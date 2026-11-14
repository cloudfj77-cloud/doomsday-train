using System;
using System.Collections.Generic;
using System.Linq;

public class PreBuiltItemHelper
{
	public const int WATER_FILTER_DURABILITY = 400;

	public static Dictionary<string, Func<GameItem>> preBuiltItems = new Dictionary<string, Func<GameItem>>
	{
		{
			"loot_crate_evidence",
			() => LootCrateEvidence()
		},
		{
			"loot_crate_medical",
			() => LootCrateMedical()
		},
		{
			"loot_crate_security",
			() => LootCrateSecurity()
		},
		{
			"loot_crate_service",
			() => LootCrateService()
		},
		{
			"loot_crate_engineering",
			() => LootCrateEngineering()
		},
		{
			"full_water_test_strip_box",
			() => FullWaterTestStripBox()
		},
		{
			"full_beer",
			() => FullBeer()
		},
		{
			"full_water_purification_tablet_box",
			() => FullWaterPurificationTabletBox()
		},
		{
			"full_nutrient_tablet_box",
			() => FullNutrientTabletBox()
		},
		{
			"full_nutrifruit_seed_packet",
			() => FullNutrifruitSeedPacket()
		},
		{
			"random_performance_module",
			() => RandomPerformanceModule()
		},
		{
			"random_efficiency_module",
			() => RandomEfficiencyModule()
		},
		{
			"random_quality_module",
			() => RandomQualityModule()
		},
		{
			"random_overclock_module",
			() => RandomOverclockModule()
		},
		{
			"random_eco_module",
			() => RandomEcoModule()
		},
		{
			"random_fineness_module",
			() => RandomFinenessModule()
		},
		{
			"portable_water_purifier_used",
			() => UsedPortableWaterPurifier()
		},
		{
			"random_node",
			() => RandomNode()
		}
	};

	public const string LOOT_CRATE_ID_STRING = "LOOT_CRATE_ID_STRING";

	public const string LOOT_CRATE_SCANNED_TAG = "LOOT_CRATE_SCANNED_TAG";

	private static readonly Dictionary<string, Action<GameItem, float>> crateFillers = new Dictionary<string, Action<GameItem, float>>
	{
		{ "loot_crate_evidence", FillEvidence },
		{ "loot_crate_medical", FillMedical },
		{ "loot_crate_security", FillSecurity },
		{ "loot_crate_service", FillService },
		{ "loot_crate_engineering", FillEngineering }
	};

	private static readonly LocalizedLines ThreatMessages = LocalizedLines.Mechanic("note_ghostwater_threat_1", "note_ghostwater_threat_2", "note_ghostwater_threat_3", "note_ghostwater_threat_4", "note_ghostwater_threat_5", "note_ghostwater_threat_6");

	private static readonly LocalizedLines AmmoThreatMessages = LocalizedLines.Mechanic("note_ammo_threat_1");

	private static readonly LocalizedLines SecretAdmirerMessages = LocalizedLines.Mechanic("note_admirer_1", "note_admirer_2", "note_admirer_3", "note_admirer_4", "note_admirer_5", "note_admirer_6", "note_admirer_7");

	public static GameItem CreatePaperWithText(string text, string title = "Note")
	{
		GameItem gameItem = DirectoryMaster.Item("slip");
		gameItem.name = title;
		gameItem.ModifyTag("SLIP_TITLE", delegate(TagState state)
		{
			state.Enable().SetString(title);
		});
		gameItem.ModifyTag("SLIP_BODY", delegate(TagState state)
		{
			state.Enable().SetString(text);
		});
		return gameItem;
	}

	public static GameItem FullWaterTestStripBox()
	{
		GameItem gameItem = DirectoryMaster.Item("water_test_container");
		for (int i = 0; i < 10; i++)
		{
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("water_test_strip"));
		}
		return gameItem;
	}

	public static GameItem FullBeer()
	{
		GameItem gameItem = DirectoryMaster.Item("beerCase");
		for (int i = 0; i < 8; i++)
		{
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("red_beer"));
		}
		return gameItem;
	}

	public static GameItem FullWaterPurificationTabletBox()
	{
		GameItem gameItem = DirectoryMaster.Item("water_tablet_container");
		for (int i = 0; i < 9; i++)
		{
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("water_tablet"));
		}
		return gameItem;
	}

	public static GameItem FullNutrientTabletBox()
	{
		GameItem gameItem = DirectoryMaster.Item("nutrient_tablet_container");
		for (int i = 0; i < 9; i++)
		{
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("nutrient_tablet"));
		}
		return gameItem;
	}

	public static GameItem FullNutrifruitSeedPacket()
	{
		GameItem gameItem = DirectoryMaster.Item("nutrifruit_seed_packet");
		for (int i = 0; i < 9; i++)
		{
			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("nutrifruit_seed"));
		}
		return gameItem;
	}

	public static GameItem EnergyCredit(int charge)
	{
		GameItem gameItem = DirectoryMaster.Item("energy_credit");
		PowerHelper.SetPowerSourceAt(gameItem, charge);
		return gameItem;
	}

	public static GameItem EnergyCreditExt(int charge = 250)
	{
		GameItem gameItem = DirectoryMaster.Item("energy_credit_ext");
		PowerHelper.SetPowerSourceAt(gameItem, charge);
		return gameItem;
	}

	public static GameItem EnergyCredit(int min, int max)
	{
		GameItem gameItem = DirectoryMaster.Item("energy_credit");
		PowerHelper.SetPowerSourceAt(gameItem, RNG.GetRandomInt(min, max));
		return gameItem;
	}

	public static GameItem BatteryDisposable(int charge = 1000)
	{
		GameItem gameItem = DirectoryMaster.Item("energy_credit_dis");
		PowerHelper.SetPowerSourceAt(gameItem, charge);
		return gameItem;
	}

	public static GameItem BatteryDisposable(int min, int max)
	{
		GameItem gameItem = DirectoryMaster.Item("energy_credit_dis");
		PowerHelper.SetPowerSourceAt(gameItem, RNG.GetRandomInt(min, max));
		return gameItem;
	}

	public static GameItem WaterFilter(int min, int max)
	{
		GameItem gameItem = DirectoryMaster.Item("water_filter");
		DurabilityHelper.SetDurability(gameItem, RNG.GetRandomInt(min, max), 400, 400);
		return gameItem;
	}

	public static GameItem WaterFilter()
	{
		GameItem gameItem = DirectoryMaster.Item("water_filter");
		DurabilityHelper.SetDurability(gameItem, 400, 400, 400);
		return gameItem;
	}

	public static GameItem FakeWaterFilter(int min, int max)
	{
		GameItem gameItem = DirectoryMaster.Item("water_filter");
		DurabilityHelper.SetDurability(gameItem, 100, RNG.GetRandomInt(min, max), 100, 100);
		return gameItem;
	}

	public static GameItem UsedPortableWaterPurifier()
	{
		GameItem gameItem = DirectoryMaster.Item("portable_water_purifier");
		DurabilityHelper.SetCurrentDurability(gameItem, RNG.GetRandomInt(15, 50));
		return gameItem;
	}

	public static GameItem RealIngotPurity(string purity)
	{
		GameItem gameItem = DirectoryMaster.Item("metal_ingot");
		IngotPurityHelper.SetPurity(gameItem, purity);
		return gameItem;
	}

	public static GameItem LootCrateEvidence()
	{
		return CreateLootCrate("loot_crate_evidence", "evidence_box");
	}

	public static GameItem LootCrateMedical()
	{
		return CreateLootCrate("loot_crate_medical", "med_box");
	}

	public static GameItem LootCrateSecurity()
	{
		return CreateLootCrate("loot_crate_security", "sec_box");
	}

	public static GameItem LootCrateService()
	{
		return CreateLootCrate("loot_crate_service", "service_box");
	}

	public static GameItem LootCrateEngineering()
	{
		return CreateLootCrate("loot_crate_engineering", "eng_box");
	}

	private static GameItem CreateLootCrate(string crateId, string boxId)
	{
		GameItem gameItem = DirectoryMaster.Item(boxId);
		gameItem.ModifyTag("LOOT_CRATE_ID_STRING", delegate(TagState state)
		{
			state.Enable().SetString(crateId);
		});
		LotteryHelper.StampSpawnLuck(gameItem);
		crateFillers[crateId](gameItem, GetLuckMult(gameItem));
		LockHelper.LockUpContainer(gameItem);
		return gameItem;
	}

	public static float GetLuckMult(GameItem crate)
	{
		return 1f + (float)LotteryHelper.GetSpawnLuck(crate) / 100f;
	}

	private static bool LuckRoll(int chance, float luckMult)
	{
		return RNG.Roll((float)chance * luckMult);
	}

	private static void TryAdd(GameItem crate, GameItem item)
	{
		if (item != null && GraphUtils.TryAcceptAll(crate, item) > 0)
		{
			item.Destroy();
		}
	}

	public static void MarkCrateScanned(GameItem crate)
	{
		if (crate.IsTag("LOOT_CRATE_ID_STRING"))
		{
			crate.EnableTag("LOOT_CRATE_SCANNED_TAG");
		}
	}

	public static void TryRefillForBetterLuck(GameItem crate)
	{
		if (!crate.IsTag("LOOT_CRATE_ID_STRING") || crate.IsTag("LOOT_CRATE_SCANNED_TAG"))
		{
			return;
		}
		int luck = HealthData.GetLuck();
		if (luck <= LotteryHelper.GetSpawnLuck(crate))
		{
			return;
		}
		string @string = crate.GetTagReadonly("LOOT_CRATE_ID_STRING").GetString();
		if (!crateFillers.ContainsKey(@string) || crate.contentWindow == null || !(crate.contentWindow.child is GameInventory gameInventory))
		{
			return;
		}
		foreach (GameItem item in gameInventory.childItems.ToList())
		{
			item.Destroy();
		}
		gameInventory.mayInventoryAddItemFunc = null;
		LotteryHelper.SetSpawnLuck(crate, luck);
		crateFillers[@string](crate, GetLuckMult(crate));
		LockHelper.SetChildBehindLock(crate);
		Debug.Log("Loot crate " + @string + " refilled at unlock with luck " + luck);
	}

	private static void FillEvidence(GameItem gameItem, float luckMult)
	{
		if (LuckRoll(20, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("current_converter"));
		}
		if (LuckRoll(4, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("skincare_cream"));
		}
		if (LuckRoll(6, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("bottled_water_premium"));
		}
		if (LuckRoll(4, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("neuroactive_perfume"));
		}
		if (LuckRoll(6, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pheromone_perfume"));
		}
		if (LuckRoll(6, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("turbo_booster_adv"));
		}
		if (LuckRoll(6, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("blue_blood_bag"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("scav_token"));
		}
		if (LuckRoll(6, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("water_filter_adv"));
		}
		if (LuckRoll(3, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("portable_water_purifier"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, CreateRealCounterfeitInjector());
		}
		if (LuckRoll(35, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_alt_injector"));
		}
		if (LuckRoll(35, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_alt_injector"));
		}
		if (LuckRoll(5, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("bottled_water_premium"));
		}
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, CreateHeavyHandmadeGun(ItemFeatureList.EquipmentCondition.Poor));
		}
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, CreateHandmadeGun(ItemFeatureList.EquipmentCondition.Poor));
		}
		if (LuckRoll(15, luckMult))
		{
			GameItem gameItem2 = StartingPerkContent.CreateUnownedDegradedCassette();
			if (gameItem2 != null)
			{
				TryAdd(gameItem, gameItem2);
			}
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("kitchen_knife"));
		}
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("combat_knife"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("glass_shard_shiv"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("kitchen_cleaver"));
		}
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("hatchet"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pipe_weapon"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_injector"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("raw_meat"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("sec_keycard"));
		}
		if (LuckRoll(20, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("med_keycard"));
		}
		if (LuckRoll(20, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("eng_keycard"));
		}
		if (LuckRoll(10, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("cmd_keycard"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_chemical"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_chemical"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("restricted_chemical"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_electronic"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("scratcher"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("processed_cheese"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("unlicensed_phagimycin_pill"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("unlicensed_phagimycin_pill"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("unlicensed_phagimycin_pill"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("unlicensed_phagimycin_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("dream_dust"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("dream_dust"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("dream_dust"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("dream_dust"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("zyanide_pill"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("zyanide_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("fanta_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("fanta_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("fanta_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("fanta_pill"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("small_pistol_ammo"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("small_pistol_ammo_p"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("small_pistol_ammo_nl"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo_p"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo_nl"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("nudka"));
		}
		if (LuckRoll(50, luckMult) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			TryAdd(gameItem, DirectoryMaster.Item("wine_yeast_red"));
		}
		if (LuckRoll(50, luckMult) && NetworkUpgrade.IsWinemakerUnlocked())
		{
			TryAdd(gameItem, DirectoryMaster.Item("wine_yeast_red"));
		}
	}

	private static void FillMedical(GameItem gameItem, float luckMult)
	{
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("med_keycard"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("med_keycard"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("black_injector"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("black_injector"));
		}
		TryAdd(gameItem, CreateRealGenuineInjector());
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("surgery_tool"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_chemical"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_chemical"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_chemical"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("restricted_chemical"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_medical"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("salve"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("rubbing_alcohol"));
		}
		TryAdd(gameItem, EnergyCredit(RNG.GetRandomInt(25, 50)));
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("bandage_item"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("bandage_item"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("bandage_item"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("bandage_item"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("blood_bag"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("blue_blood_bag"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("galaxy_blend"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("galaxy_blend"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_injector"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_injector"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_injector"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("phagimycin_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("phagimycin_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("phagimycin_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("phagimycin_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("phagimycin_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("fanta_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("fanta_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("fanta_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("smelling_salt"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("smelling_salt"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("smelling_salt"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("birth_control_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("birth_control_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("birth_control_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("caffeine_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("caffeine_pill"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("caffeine_pill"));
		}
	}

	private static void FillSecurity(GameItem gameItem, float luckMult)
	{
		if (LuckRoll(20, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("glock_receiver"));
		}
		if (LuckRoll(20, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("stun_gun"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("tazer"));
		}
		if (LuckRoll(20, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("current_converter"));
		}
		TryAdd(gameItem, EnergyCredit(RNG.GetRandomInt(50, 100)));
		if (LuckRoll(35, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("stun_baton"));
		}
		if (LuckRoll(35, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("stun_baton"));
		}
		if (LuckRoll(35, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("combat_knife"));
		}
		if (LuckRoll(35, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("combat_knife"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_injector"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_injector"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("pink_injector"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("sec_keycard"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("sec_keycard"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("flashbang_grenade"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("flashbang_grenade"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("flashlight"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("smoke_grenade"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("smoke_grenade"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo_p"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo_nl"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo_p"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("heavy_pistol_ammo_nl"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("topical_bandage_item"));
		}
		if (LuckRoll(55, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("topical_bandage_item"));
		}
	}

	private static void FillService(GameItem gameItem, float luckMult)
	{
		if (LuckRoll(20, luckMult))
		{
			GameItem gameItem2 = StartingPerkContent.CreateUnownedDegradedCassette();
			if (gameItem2 != null)
			{
				TryAdd(gameItem, gameItem2);
			}
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("current_converter"));
		}
		if (LuckRoll(8, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("neuroactive_perfume"));
		}
		if (LuckRoll(66, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("ser_keycard"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("ser_keycard"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("aquascan"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("uv_filter"));
		}
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("water_filter_adv"));
		}
		if (LuckRoll(8, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("portable_water_purifier"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("toilet_paper"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("toilet_paper"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("toilet_paper"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("processed_cheese"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("toothpaste"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, WaterFilter());
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, WaterFilter());
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, WaterFilter());
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, FullNutrifruitSeedPacket());
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, FullWaterTestStripBox());
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, FullWaterPurificationTabletBox());
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, EnergyCredit(100));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, EnergyCredit(33));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("box_cutter"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("kitchen_knife"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("hatchet"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("paper_towel"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("paper_towel"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("shampoo"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("shampoo"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("caffeine_pill"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("caffeine_pill"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("caffeine_pill"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("kotton_seed"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("kotton_seed"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("kotton_seed"));
		}
		if (LuckRoll(75, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("hydroreed_seed"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("hydroreed_seed"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("module_bay_expansion_kit"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("nutrient_tablet"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("nutrient_tablet"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("nutrient_tablet"));
		}
		TryAdd(gameItem, ItemSpawner.SpawnFromTable("packedFoodTable"));
		TryAdd(gameItem, ItemSpawner.SpawnFromTable("packedFoodTable"));
	}

	private static void FillEngineering(GameItem gameItem, float luckMult)
	{
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("energy_credit_breeder"));
		}
		if (LuckRoll(4, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("system_capped_neural_core"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("module_bay_expansion_kit"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("module_bay_expansion_kit"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("current_converter"));
		}
		TryAdd(gameItem, EnergyCredit(100));
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, EnergyCredit(RNG.GetRandomInt(0, 100)));
		}
		TryAdd(gameItem, EnergyCreditExt(RNG.GetRandomInt(0, 125)));
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("screwdriver"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("wire_cutter"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("crowbar"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("welder"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("module_extractor"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("module_extractor"));
		}
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("module_extractor_advanced"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("turbo_booster"));
		}
		if (LuckRoll(25, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("turbo_booster"));
		}
		if (LuckRoll(8, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("turbo_booster_adv"));
		}
		if (LuckRoll(12, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("advanced_flux_agent"));
		}
		if (LuckRoll(15, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("furnace_module_blast"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("eng_keycard"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("eng_keycard"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("random_performance_module"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("random_efficiency_module"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("random_quality_module"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("random_node"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("random_node"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("system_module_overclock"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("system_module_eco"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, ItemSpawner.Spawn("system_module_fineness"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_electronic"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_electronic"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("common_electronic"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("nuts_metal"));
		}
		if (LuckRoll(33, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("nuts_metal"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("flux_agent"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("flux_agent"));
		}
		if (LuckRoll(50, luckMult))
		{
			TryAdd(gameItem, DirectoryMaster.Item("flux_agent"));
		}
	}

	public static GameItem Medkit()
	{
		GameItem gameItem = DirectoryMaster.Item("firstaid_box");
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("pink_injector"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("phagimycin_pill"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("phagimycin_pill"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("smelling_salt"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("hemostatic_bandage_item"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("rubbing_alcohol"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("salve"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("pure_white_injector"));
		GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("pure_white_injector"));
		return gameItem;
	}

	public static GameItem GenuineCigarette()
	{
		GameItem gameItem = DirectoryMaster.Item("packet_red_cigarette");
		InsCigaretteHelper.SetGenuine(gameItem);
		gameItem.AddItemFeature(ItemFeatureList.CigaretteAuthenticity());
		return gameItem;
	}

	public static GameItem FakeCigarette()
	{
		GameItem gameItem = DirectoryMaster.Item("packet_red_cigarette");
		InsCigaretteHelper.SetConterfeit(gameItem);
		gameItem.AddItemFeature(ItemFeatureList.CigaretteFakeAuthenticity());
		return gameItem;
	}

	public static GameItem CounterfeitCigarette()
	{
		GameItem gameItem = DirectoryMaster.Item("packet_red_cigarette");
		InsCigaretteHelper.SetConterfeit(gameItem);
		gameItem.AddItemFeature(ItemFeatureList.CigaretteCounterfeit());
		return gameItem;
	}

	public static GameItem FakeCounterfeitCigarette()
	{
		GameItem gameItem = DirectoryMaster.Item("packet_red_cigarette");
		InsCigaretteHelper.SetGenuine(gameItem);
		gameItem.AddItemFeature(ItemFeatureList.CigaretteFakeCounterfeit());
		return gameItem;
	}

	public static GameItem FakeCigaretteStory()
	{
		GameItem gameItem = DirectoryMaster.Item("packet_red_cigarette");
		InsCigaretteHelper.SetConterfeitStory(gameItem);
		gameItem.AddItemFeature(ItemFeatureList.CigaretteFakeAuthenticity());
		return gameItem;
	}

	public static GameItem RandomCigarette()
	{
		GameItem gameItem = DirectoryMaster.Item("packet_red_cigarette");
		int randomInt = RNG.GetRandomInt(0, 100);
		if (!PlayerStore.Instance.isVisualInspectionIntroduced)
		{
			if (randomInt <= 30)
			{
				Debug.Log("Creating Genuine");
				InsCigaretteHelper.SetGenuine(gameItem);
				gameItem.AddItemFeature(ItemFeatureList.CigaretteAuthenticity());
			}
			else
			{
				Debug.Log("Creating Counterfeit");
				InsCigaretteHelper.SetConterfeit(gameItem);
				gameItem.AddItemFeature(ItemFeatureList.CigaretteCounterfeit());
			}
		}
		else if (randomInt <= 20)
		{
			InsCigaretteHelper.SetConterfeit(gameItem);
			gameItem.AddItemFeature(ItemFeatureList.CigaretteCounterfeit());
		}
		else if (randomInt <= 60)
		{
			InsCigaretteHelper.SetConterfeit(gameItem);
			gameItem.AddItemFeature(ItemFeatureList.CigaretteFakeAuthenticity());
		}
		else if (randomInt <= 90)
		{
			InsCigaretteHelper.SetGenuine(gameItem);
			gameItem.AddItemFeature(ItemFeatureList.CigaretteAuthenticity());
		}
		else
		{
			InsCigaretteHelper.SetGenuine(gameItem);
			gameItem.AddItemFeature(ItemFeatureList.CigaretteFakeCounterfeit());
		}
		return gameItem;
	}

	public static GameItem CreatePlasmaFull(int customerValue)
	{
		GameItem gameItem = DirectoryMaster.Item("plasma_fuel");
		PlasmaHelper.InitPlasmaItem(gameItem);
		gameItem.unitValue = customerValue;
		AppraisalHelper.SetDefaultAppraisalValue(gameItem, customerValue);
		return gameItem;
	}

	public static GameItem CreatePlasmaRandom(int customerValue)
	{
		GameItem gameItem = DirectoryMaster.Item("plasma_fuel");
		PlasmaHelper.InitPlasmaItem(gameItem);
		gameItem.unitValue = customerValue;
		if (RNG.Roll(6))
		{
			PlasmaHelper.InitLabPlasma(gameItem);
		}
		else if (RNG.Roll(25))
		{
			PlasmaHelper.InitLLPlasma(gameItem);
		}
		else if (RNG.Roll(20))
		{
			PlasmaHelper.InitRepublicPlasma(gameItem);
		}
		else if (RNG.Roll(20))
		{
			PlasmaHelper.InitZhongHuaPlasma(gameItem);
		}
		else
		{
			PlasmaHelper.InitRandomPlasma(gameItem);
		}
		AppraisalHelper.SetDefaultAppraisalValue(gameItem, customerValue);
		return gameItem;
	}

	public static GameItem CreateRibwichRandom()
	{
		GameItem gameItem = DirectoryMaster.Item("ribwich");
		RibwichHelper.Init(gameItem);
		if (RNG.Roll(10))
		{
			RibwichHelper.InitOpened(gameItem);
		}
		else if (RNG.Roll(15))
		{
			RibwichHelper.InitCounterfeit(gameItem);
		}
		else if (RNG.Roll(5))
		{
			RibwichHelper.InitCorrectDoubleMeat(gameItem);
		}
		else if (RNG.Roll(15))
		{
			RibwichHelper.InitCorrectDeluxeEdition(gameItem);
		}
		else
		{
			RibwichHelper.InitCorrectNormal(gameItem);
		}
		return gameItem;
	}

	public static GameItem CreateHandRandom()
	{
		GameItem gameItem = DirectoryMaster.Item("hand");
		AppHandHelper.InitAppHandItem(gameItem);
		if (RNG.Roll(10))
		{
			AppHandHelper.InitPremium(gameItem);
		}
		else if (RNG.Roll(20))
		{
			AppHandHelper.InitCombat(gameItem);
		}
		else if (RNG.Roll(25))
		{
			AppHandHelper.InitJunk(gameItem);
		}
		else
		{
			AppHandHelper.InitRandom(gameItem);
		}
		return gameItem;
	}

	public static GameItem CreateStamp()
	{
		GameItem gameItem = DirectoryMaster.Item("food_stamp");
		InsStampHelper.Init(gameItem);
		gameItem.AddItemFeature(ItemFeatureList.StampAuthenticity());
		return gameItem;
	}

	public static GameItem CreateFakeStamp()
	{
		GameItem gameItem = DirectoryMaster.Item("food_stamp");
		int amount = 40;
		if (RNG.Roll(33))
		{
			amount = 80;
		}
		if (RNG.Roll(25))
		{
			amount = 120;
		}
		if (RNG.Roll(20))
		{
			amount = 160;
		}
		if (RNG.Roll(15))
		{
			amount = 200;
		}
		InsStampHelper.Init(gameItem);
		InsStampHelper.SetCounterfeit(gameItem, amount);
		gameItem.AddItemFeature(ItemFeatureList.StampFakeAuthenticity());
		return gameItem;
	}

	public static GameItem CreateRealStamp()
	{
		GameItem gameItem = DirectoryMaster.Item("food_stamp");
		int amount = 40;
		if (RNG.Roll(33))
		{
			amount = 80;
		}
		if (RNG.Roll(25))
		{
			amount = 120;
		}
		if (RNG.Roll(20))
		{
			amount = 160;
		}
		if (RNG.Roll(15))
		{
			amount = 200;
		}
		InsStampHelper.Init(gameItem);
		InsStampHelper.SetGenuine(gameItem, amount);
		gameItem.AddItemFeature(ItemFeatureList.StampAuthenticity());
		return gameItem;
	}

	public static GameItem CreateRealStamp(int amount)
	{
		GameItem gameItem = DirectoryMaster.Item("food_stamp");
		InsStampHelper.Init(gameItem);
		InsStampHelper.SetGenuine(gameItem, amount);
		gameItem.AddItemFeature(ItemFeatureList.StampAuthenticity());
		return gameItem;
	}

	public static GameItem CreateRealStamp80()
	{
		GameItem gameItem = DirectoryMaster.Item("food_stamp");
		int amount = 80;
		InsStampHelper.Init(gameItem);
		InsStampHelper.SetGenuine(gameItem, amount);
		gameItem.AddItemFeature(ItemFeatureList.StampAuthenticity());
		return gameItem;
	}

	public static GameItem CreateRealGenuineInjector()
	{
		GameItem gameItem = InsInjectorHelper.CreateRealInjector();
		gameItem.AddItemFeature(ItemFeatureList.InjectorFeature(ItemFeatureList.InjectorConditionType.Genuine, ItemFeatureList.InjectorConditionType.Genuine));
		return gameItem;
	}

	public static GameItem CreateRealExpiredInjector()
	{
		GameItem gameItem = InsInjectorHelper.CreateExpiredInjector();
		gameItem.AddItemFeature(ItemFeatureList.InjectorFeature(ItemFeatureList.InjectorConditionType.Expired, ItemFeatureList.InjectorConditionType.Expired));
		return gameItem;
	}

	public static GameItem CreateRealCounterfeitInjector()
	{
		GameItem gameItem = InsInjectorHelper.CreateCounterfeit();
		gameItem.AddItemFeature(ItemFeatureList.InjectorFeature(ItemFeatureList.InjectorConditionType.Counterfeit, ItemFeatureList.InjectorConditionType.Counterfeit));
		return gameItem;
	}

	public static GameItem CreateRealUnusableInjector()
	{
		GameItem gameItem = InsInjectorHelper.CreateUnusableInjector();
		gameItem.AddItemFeature(ItemFeatureList.InjectorFeature(ItemFeatureList.InjectorConditionType.Unusable, ItemFeatureList.InjectorConditionType.Unusable));
		return gameItem;
	}

	public static GameItem CreateInjector(ItemFeatureList.InjectorConditionType fake, ItemFeatureList.InjectorConditionType real)
	{
		GameItem gameItem = real switch
		{
			ItemFeatureList.InjectorConditionType.Genuine => InsInjectorHelper.CreateRealInjector(), 
			ItemFeatureList.InjectorConditionType.Expired => InsInjectorHelper.CreateExpiredInjector(), 
			ItemFeatureList.InjectorConditionType.Counterfeit => InsInjectorHelper.CreateCounterfeit(), 
			_ => InsInjectorHelper.CreateUnusableInjector(), 
		};
		gameItem.AddItemFeature(ItemFeatureList.InjectorFeature(fake, real));
		return gameItem;
	}

	public static GameItem CreateRandomInjector()
	{
		if (RNG.Roll(20))
		{
			return CreateInjector(ItemFeatureList.InjectorConditionType.Genuine, ItemFeatureList.InjectorConditionType.Expired);
		}
		if (RNG.Roll(25))
		{
			return CreateInjector(ItemFeatureList.InjectorConditionType.Genuine, ItemFeatureList.InjectorConditionType.Unusable);
		}
		if (RNG.Roll(30))
		{
			return CreateInjector(ItemFeatureList.InjectorConditionType.Genuine, ItemFeatureList.InjectorConditionType.Counterfeit);
		}
		return CreateInjector(ItemFeatureList.InjectorConditionType.Genuine, ItemFeatureList.InjectorConditionType.Genuine);
	}

	public static GameItem CreateRandomStamp()
	{
		if (RNG.Roll(75))
		{
			return CreateRealStamp();
		}
		return CreateFakeStamp();
	}

	public static GameItem CreateAgeWell()
	{
		GameItem gameItem = DirectoryMaster.Item("wine_rack");
		GameItem item = MachineWineRack.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateWaterPurifier()
	{
		GameItem gameItem = DirectoryMaster.Item("water_purifier");
		GameItem item = MachinePurifier.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateHydroponic()
	{
		GameItem gameItem = DirectoryMaster.Item("hydroponic");
		GameItem item = MachineHydroponic.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateDnaManipulator()
	{
		GameItem gameItem = DirectoryMaster.Item("plant_dna_manipulator");
		GameItem item = MachineDnaManipulator.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreatePrinter()
	{
		GameItem gameItem = DirectoryMaster.Item("printer");
		GameItem item = MachinePrinter.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateMoistureFarm()
	{
		GameItem gameItem = DirectoryMaster.Item("moisture_farm");
		GameItem item = MachineMoistureFarm.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateFurnace()
	{
		GameItem gameItem = DirectoryMaster.Item("furnace");
		GameItem item = MachineFurnace.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateAlarm()
	{
		GameItem gameItem = DirectoryMaster.Item("security_alarm");
		GameItem item = MachineAlarm.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateProjector()
	{
		GameItem gameItem = DirectoryMaster.Item("mirage_projector");
		GameItem item = MachineProjector.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateDealMaker()
	{
		GameItem gameItem = DirectoryMaster.Item("deal_maker");
		GameItem item = MachineDealMaker.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		return gameItem;
	}

	public static GameItem CreateEvaporator(int powerCellCapacity = 0)
	{
		GameItem gameItem = DirectoryMaster.Item("evaporator");
		GameItem item = MachineEvaporator.CreateNote();
		GraphUtils.TryAcceptAll(gameItem, item);
		if (powerCellCapacity != 0)
		{
			GraphUtils.TryAcceptAll(gameItem, EnergyCredit(powerCellCapacity));
		}
		return gameItem;
	}

	public static GameItem CreateHandnote(string body, string title = "", string sign = "", int font = 1)
	{
		GameItem gameItem = DirectoryMaster.Item("handnote");
		gameItem.ModifyTag("HANDNOTE_TITLE", delegate(TagState state)
		{
			state.Enable().SetString(title);
		});
		gameItem.ModifyTag("HANDNOTE_BODY", delegate(TagState state)
		{
			state.Enable().SetString(body);
		});
		gameItem.ModifyTag("HANDNOTE_SIGN", delegate(TagState state)
		{
			state.Enable().SetString(sign);
		});
		gameItem.ModifyTag("HANDNOTE_FONT", delegate(TagState state)
		{
			state.Enable().SetInt(font);
		});
		return gameItem;
	}

	public static GameItem CreateHandNoteFromList(List<string> lines, string title = "", string sign = "", int font = 1)
	{
		return CreateHandnote(string.Join(" ", lines), title, sign, font);
	}

	public static GameItem CreateRandomGhostwaterThreatNote()
	{
		return CreateHandnote(ThreatMessages[RNG.GetRandomIntExculsive(0, ThreatMessages.Length)]);
	}

	public static GameItem CreateRandomAmmoThreatNote()
	{
		return CreateHandnote(AmmoThreatMessages[RNG.GetRandomIntExculsive(0, AmmoThreatMessages.Length)]);
	}

	public static GameItem CreateSecretAdmirerNote()
	{
		return CreateHandnote(SecretAdmirerMessages[RNG.GetRandomIntExculsive(0, SecretAdmirerMessages.Length)]);
	}

	private static string SizedName(string sizeKey, string baseKey)
	{
		return LocHelper.GetLocalizedMechanic(sizeKey, LocHelper.GetLocalizedItem(baseKey));
	}

	public static GameItem CreateNutrifruitNormal()
	{
		GameItem gameItem = DirectoryMaster.Item("nutrifruit");
		gameItem.name = LocHelper.GetLocalizedItem("item_nutrifruit_name");
		gameItem.unitValue = 12L;
		return gameItem;
	}

	public static GameItem CreateNutrifruitSmall()
	{
		GameItem gameItem = DirectoryMaster.Item("nutrifruit");
		gameItem.name = SizedName("item_size_small_of", "item_nutrifruit_name");
		gameItem.unitValue = 8L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_s");
		ChemicalCompositionHelper.SetPotatoComposition(gameItem, 150f);
		FoodItemHelper.SetCalories(gameItem, 144);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "nutrifruit_seed");
		return gameItem;
	}

	public static GameItem CreateNutrifruitLarge()
	{
		GameItem gameItem = DirectoryMaster.Item("nutrifruit");
		gameItem.name = SizedName("item_size_large_of", "item_nutrifruit_name");
		gameItem.unitValue = 18L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_l");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemicalCompositionHelper.SetPotatoComposition(gameItem, 400f);
		FoodItemHelper.SetCalories(gameItem, 384);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "nutrifruit_seed");
		return gameItem;
	}

	public static GameItem CreateNutrifruitHuge()
	{
		GameItem gameItem = DirectoryMaster.Item("nutrifruit");
		gameItem.name = SizedName("item_size_huge_of", "item_nutrifruit_name");
		gameItem.unitValue = 32L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_xl");
		ChemicalCompositionHelper.SetPotatoComposition(gameItem, 800f);
		FoodItemHelper.SetCalories(gameItem, 768);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "nutrifruit_seed");
		return gameItem;
	}

	public static GameItem CreateDreamCap(bool isComestible = false)
	{
		GameItem gameItem = DirectoryMaster.Item("dream_cap");
		gameItem.name = LocHelper.GetLocalizedItem("item_dream_cap_name");
		gameItem.unitValue = 23L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "dream_cap");
		if (isComestible)
		{
			gameItem.unitValue = 19L;
			gameItem.name = LocHelper.GetLocalizedItem("item_meal_cap_name");
			gameItem.shortDescription = LocHelper.GetLocalizedItem("item_meal_cap_desc");
			gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "meal_cap");
			gameItem.SetGameItemType("FOOD");
			gameItem.RemoveGameItemType("CHEMICAL_SUPPLIES");
			ContrabandHelper.RemoveContrabandStatus(gameItem);
		}
		ChemicalCompositionHelper.SetDreamCapComposition(gameItem, 32.5f, isComestible);
		if (!isComestible)
		{
			PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "dream_cap_seed");
		}
		return gameItem;
	}

	public static GameItem CreateLargeDreamCap(bool isComestible = false)
	{
		GameItem gameItem = DirectoryMaster.Item("dream_cap");
		gameItem.name = SizedName("item_size_large_of", "item_dream_cap_name");
		gameItem.unitValue = 31L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "dream_cap_l");
		if (isComestible)
		{
			gameItem.unitValue = 27L;
			gameItem.name = SizedName("item_size_large_of", "item_meal_cap_name");
			gameItem.shortDescription = LocHelper.GetLocalizedItem("item_meal_cap_desc");
			gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "meal_cap_l");
			gameItem.SetGameItemType("FOOD");
			gameItem.RemoveGameItemType("CHEMICAL_SUPPLIES");
			ContrabandHelper.RemoveContrabandStatus(gameItem);
		}
		ChemicalCompositionHelper.SetDreamCapComposition(gameItem, 75f, isComestible);
		if (!isComestible)
		{
			PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "dream_cap_seed");
		}
		return gameItem;
	}

	public static GameItem CreateSmallDreamCap(bool isComestible = false)
	{
		GameItem gameItem = DirectoryMaster.Item("dream_cap");
		gameItem.name = SizedName("item_size_small_of", "item_dream_cap_name");
		gameItem.unitValue = 17L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "dream_cap_s");
		if (isComestible)
		{
			gameItem.unitValue = 13L;
			gameItem.name = SizedName("item_size_small_of", "item_meal_cap_name");
			gameItem.shortDescription = LocHelper.GetLocalizedItem("item_meal_cap_desc");
			gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "meal_cap_s");
			gameItem.SetGameItemType("FOOD");
			gameItem.RemoveGameItemType("CHEMICAL_SUPPLIES");
			ContrabandHelper.RemoveContrabandStatus(gameItem);
		}
		ChemicalCompositionHelper.SetDreamCapComposition(gameItem, 20.5f, isComestible);
		if (!isComestible)
		{
			PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "dream_cap_seed");
		}
		return gameItem;
	}

	public static GameItem CreateSmallKotton()
	{
		GameItem gameItem = DirectoryMaster.Item("kotton_fiber");
		gameItem.name = SizedName("item_size_small_of", "item_kotton_fiber_name");
		gameItem.unitValue = 36L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "kotton_plant_s");
		gameItem.SetGameItemType("MATERIAL");
		return gameItem;
	}

	public static GameItem CreateKotton()
	{
		GameItem gameItem = DirectoryMaster.Item("kotton_fiber");
		gameItem.name = LocHelper.GetLocalizedItem("item_kotton_fiber_name");
		gameItem.unitValue = 46L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "kotton_plant");
		gameItem.SetGameItemType("MATERIAL");
		return gameItem;
	}

	public static GameItem CreateLargeKotton()
	{
		GameItem gameItem = DirectoryMaster.Item("kotton_fiber");
		gameItem.name = SizedName("item_size_large_of", "item_kotton_fiber_name");
		gameItem.unitValue = 60L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "kotton_plant_l");
		gameItem.SetGameItemType("MATERIAL");
		return gameItem;
	}

	public static GameItem CreateFillerweed()
	{
		GameItem gameItem = DirectoryMaster.Item("fillerweed");
		gameItem.name = LocHelper.GetLocalizedItem("item_fillerweed_name");
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "fillerweed");
		return gameItem;
	}

	public static GameItem CreateFillerweedSmall()
	{
		GameItem gameItem = DirectoryMaster.Item("fillerweed");
		gameItem.name = SizedName("item_size_small_of", "item_fillerweed_name");
		gameItem.unitValue = 7L;
		gameItem.SetSpriteAndShape("Items/items_hydroponic_plant", "fillerweed_s");
		FoodItemHelper.InitFoodItem(gameItem, 155, isCanned: false);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "fillerweed_seed");
		return gameItem;
	}

	public static GameItem CreateBrokenAlarm()
	{
		GameItem gameItem = DirectoryMaster.Item("security_alarm");
		gameItem.name = gameItem.name + " (" + LocHelper.GetLocalizedMechanic("mech_item_ruined") + ")";
		gameItem.unitValue = 95L;
		gameItem.backupUnitValue = 95L;
		gameItem.shortDescription = LocHelper.GetLocalizedMechanic("mech_item_broken_machine_desc");
		gameItem.SetSpriteAndShape("Items/items_broken_machine", "broken_alarm");
		MachineBrokenHelper.InitBrokenMachine(gameItem);
		MachineBrokenHelper.AddBrokenMachineItem(gameItem);
		return gameItem;
	}

	public static GameItem CreateBrokenFurnace()
	{
		GameItem gameItem = DirectoryMaster.Item("furnace");
		gameItem.name = gameItem.name + " (" + LocHelper.GetLocalizedMechanic("mech_item_ruined") + ")";
		gameItem.shortDescription = LocHelper.GetLocalizedMechanic("mech_item_broken_machine_desc");
		gameItem.unitValue = 135L;
		gameItem.backupUnitValue = 135L;
		gameItem.SetSpriteAndShape("Items/items_broken_machine", "broken_furnace");
		MachineBrokenHelper.InitBrokenMachine(gameItem);
		gameItem.ModifyTag("BROKEN_MACHINE_WIRE_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(4);
		});
		gameItem.ModifyTag("BROKEN_MACHINE_SCREW_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(4);
		});
		gameItem.ModifyTag("BROKEN_MACHINE_WELD_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(5);
		});
		if (RNG.Roll(50))
		{
			GraphUtils.TryAcceptAll(gameItem, EnergyCredit(RNG.GetRandomInt(1, 25)));
		}
		else if (RNG.Roll(25))
		{
			GraphUtils.TryAcceptAll(gameItem, EnergyCredit(RNG.GetRandomInt(25, 50)));
		}
		if (RNG.Roll(66))
		{
			ModuleHelper.AddStuckCorruptedModule(gameItem);
		}
		if (RNG.Roll(40))
		{
			ModuleHelper.AddStuckCorruptedModule(gameItem);
		}
		if (RNG.Roll(20))
		{
			ModuleHelper.AddRandomStuckNormalModule(gameItem);
		}
		if (RNG.Roll(66))
		{
			ModuleHelper.AddStuckRuinedModule(gameItem);
		}
		return gameItem;
	}

	public static GameItem CreateBrokenBrokenMoistureFarm()
	{
		GameItem gameItem = DirectoryMaster.Item("moisture_farm");
		gameItem.name = gameItem.name + " (" + LocHelper.GetLocalizedMechanic("mech_item_ruined") + ")";
		gameItem.unitValue = 195L;
		gameItem.backupUnitValue = 195L;
		gameItem.shortDescription = LocHelper.GetLocalizedMechanic("mech_item_broken_machine_desc");
		gameItem.SetSpriteAndShape("Items/items_broken_machine", "broken_moisture_farm");
		MachineBrokenHelper.InitBrokenMachine(gameItem);
		MachineHelper.GetSlot(gameItem, 2).onSlotAddItemEndFunc = delegate
		{
		};
		MachineHelper.GetSlot(gameItem, 2).onSlotAddItemFunc = delegate
		{
		};
		MachineHelper.GetSlot(gameItem, 2).onSlotRemoveItemEndFunc = delegate
		{
		};
		MachineHelper.GetSlot(gameItem, 2).onSlotRemoveItemFunc = delegate
		{
		};
		gameItem.ModifyTag("BROKEN_MACHINE_WIRE_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(5);
		});
		gameItem.ModifyTag("BROKEN_MACHINE_SCREW_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(5);
		});
		gameItem.ModifyTag("BROKEN_MACHINE_WELD_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(7);
		});
		if (RNG.Roll(50))
		{
			GraphUtils.TryAcceptAll(gameItem, EnergyCredit(RNG.GetRandomInt(1, 25)));
		}
		else if (RNG.Roll(25))
		{
			GraphUtils.TryAcceptAll(gameItem, EnergyCredit(RNG.GetRandomInt(25, 50)));
		}
		if (RNG.Roll(75))
		{
			ModuleHelper.AddStuckCorruptedModule(gameItem);
		}
		if (RNG.Roll(50))
		{
			ModuleHelper.AddStuckCorruptedModule(gameItem);
		}
		if (RNG.Roll(25))
		{
			ModuleHelper.AddRandomStuckNormalModule(gameItem);
		}
		if (RNG.Roll(50))
		{
			ModuleHelper.AddStuckRuinedModule(gameItem);
		}
		return gameItem;
	}

	public static GameItem CreateNutrientSolution()
	{
		GameItem gameItem = DirectoryMaster.Item("mini_bottle");
		gameItem.name = LocHelper.GetLocalizedMechanic("mech_item_nutrient_solution");
		WaterHelper.FillWithNutrientSolution(gameItem);
		return gameItem;
	}

	public static GameItem CreatePoorHandmadeGun()
	{
		GameItem gameItem = DirectoryMaster.Item("handmade_pistol");
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Poor);
		return gameItem;
	}

	public static GameItem CreateHandmadeGun(ItemFeatureList.EquipmentCondition equipmentCondition = ItemFeatureList.EquipmentCondition.Good)
	{
		GameItem gameItem = DirectoryMaster.Item("handmade_pistol");
		GunHelper.SetEquipmentCondition(gameItem, equipmentCondition);
		return gameItem;
	}

	public static GameItem CreateHeavyHandmadeGun(ItemFeatureList.EquipmentCondition equipmentCondition = ItemFeatureList.EquipmentCondition.Good)
	{
		GameItem gameItem = DirectoryMaster.Item("heavy_handmade_pistol");
		GunHelper.SetEquipmentCondition(gameItem, equipmentCondition);
		return gameItem;
	}

	public static GameItem CreateTacticalRevolver(ItemFeatureList.EquipmentCondition equipmentCondition = ItemFeatureList.EquipmentCondition.Good)
	{
		GameItem gameItem = DirectoryMaster.Item("revolver_short_tac");
		GunHelper.SetEquipmentCondition(gameItem, equipmentCondition);
		return gameItem;
	}

	public static GameItem CreatePoliceHandgun(ItemFeatureList.EquipmentCondition equipmentCondition = ItemFeatureList.EquipmentCondition.Good)
	{
		GameItem gameItem = DirectoryMaster.Item("glock_receiver");
		GunHelper.SetEquipmentCondition(gameItem, equipmentCondition);
		return gameItem;
	}

	public static GameItem CreatePoliceSMG(ItemFeatureList.EquipmentCondition equipmentCondition = ItemFeatureList.EquipmentCondition.Good)
	{
		GameItem gameItem = DirectoryMaster.Item("smg");
		GunHelper.SetEquipmentCondition(gameItem, equipmentCondition);
		return gameItem;
	}

	public static GameItem CreatePoliceShotgun(ItemFeatureList.EquipmentCondition equipmentCondition = ItemFeatureList.EquipmentCondition.Good)
	{
		GameItem gameItem = DirectoryMaster.Item("shotgun");
		GunHelper.SetEquipmentCondition(gameItem, equipmentCondition);
		return gameItem;
	}

	public static GameItem CreateRevolver(ItemFeatureList.EquipmentCondition equipmentCondition = ItemFeatureList.EquipmentCondition.Good)
	{
		GameItem gameItem = DirectoryMaster.Item("revolver");
		GunHelper.SetEquipmentCondition(gameItem, equipmentCondition);
		return gameItem;
	}

	public static GameItem FitGunMod(GameItem gun, string modId)
	{
		GameItem gameItem = DirectoryMaster.Item(modId);
		if (gun == null || gameItem == null)
		{
			return gun;
		}
		bool own = GeneralHelper.IsItemOwned(gun);
		GeneralHelper.SetItemOwned(gun);
		GraphUtils.TryAcceptAllOrDestroy(gun, gameItem, "gun mod did not fit the seller's gun");
		GeneralHelper.SetItemOwned(gun, own);
		return gun;
	}

	public static GameItem CreatePoorSMG()
	{
		GameItem gameItem = DirectoryMaster.Item("smg");
		GunHelper.SetEquipmentCondition(gameItem, ItemFeatureList.EquipmentCondition.Poor);
		return gameItem;
	}

	public static GameItem CreateChemicalBottle(string liquidId)
	{
		GameItem gameItem = DirectoryMaster.Item("mini_bottle");
		gameItem.name = LocHelper.GetLocalizedMechanic("mech_item_chemical_bottle", Liquid.GetLiquidFromId(liquidId).displayName);
		WaterHelper.AddLiquid(gameItem, liquidId, 250000);
		return gameItem;
	}

	public static GameItem CreateOldHydroponic1()
	{
		GameItem gameItem = CreateHydroponic();
		GraphUtils.TryAcceptAll(gameItem, EnergyCredit(100));
		gameItem.name = LocHelper.GetLocalizedMechanic("mech_item_old_hydroponic");
		GameItem gameItem2 = DirectoryMaster.Item("large_bottled_water");
		WaterHelper.FillWithGhostwater(gameItem2);
		GraphUtils.TryAcceptAll(gameItem, gameItem2);
		GameItem gameItem3 = DirectoryMaster.Item("system_module_corrupt");
		GraphUtils.TryAcceptAll(gameItem, gameItem3);
		ModuleHelper.SetModuleStuck(gameItem3);
		gameItem3.name = LocHelper.GetLocalizedMechanic("mech_item_glitched_module");
		gameItem3.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(15);
		});
		gameItem3.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(-15);
		});
		GameItem gameItem4 = DirectoryMaster.Item("system_module_corrupt");
		GraphUtils.TryAcceptAll(gameItem, gameItem4);
		ModuleHelper.SetModuleStuck(gameItem4);
		gameItem4.name = LocHelper.GetLocalizedMechanic("mech_item_glitched_module");
		gameItem4.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(15);
		});
		gameItem4.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(-15);
		});
		GameItem gameItem5 = DirectoryMaster.Item("system_module_corrupt");
		GraphUtils.TryAcceptAll(gameItem, gameItem5);
		ModuleHelper.SetModuleStuck(gameItem5);
		gameItem5.name = LocHelper.GetLocalizedMechanic("mech_item_glitched_module");
		gameItem5.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(15);
		});
		gameItem5.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(-15);
		});
		GameItem gameItem6 = DirectoryMaster.Item("system_module_ruined");
		GraphUtils.TryAcceptAll(gameItem, gameItem6);
		ModuleHelper.SetModuleStuck(gameItem6);
		return gameItem;
	}

	public static GameItem CreateOldGunPrinter()
	{
		GameItem gameItem = ItemFeatureHelper.SetShabby(CreatePrinter());
		GraphUtils.TryAcceptAll(gameItem, EnergyCredit(100));
		GraphUtils.TryAcceptAll(gameItem, ItemFeatureHelper.SetShabby(DirectoryMaster.Item("printer_chip_pistol")));
		GameItem gameItem2 = DirectoryMaster.Item("system_module_eco");
		GraphUtils.TryAcceptAll(gameItem, gameItem2);
		ModuleHelper.SetModuleStuck(gameItem2);
		GameItem gameItem3 = DirectoryMaster.Item("system_module_corrupt");
		gameItem3.name = LocHelper.GetLocalizedMechanic("mech_item_glitched_module");
		gameItem3.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(15);
		});
		gameItem3.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(-15);
		});
		GraphUtils.TryAcceptAll(gameItem, gameItem3);
		ModuleHelper.SetModuleStuck(gameItem3);
		return gameItem;
	}

	public static GameItem RandomPerformanceModule()
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_performance");
		SetBasicModuleShape(gameItem);
		ModuleEffectHelper.InitRandomEffect(gameItem);
		return gameItem;
	}

	public static GameItem RandomEfficiencyModule()
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_efficiancy");
		SetBasicModuleShape(gameItem);
		ModuleEffectHelper.InitRandomEffect(gameItem);
		return gameItem;
	}

	public static GameItem RandomQualityModule()
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_quality");
		SetBasicModuleShape(gameItem);
		ModuleEffectHelper.InitRandomEffect(gameItem);
		return gameItem;
	}

	public static void SetBasicModuleShape(GameItem module)
	{
		string text = "";
		int randomInt = RNG.GetRandomInt(0, 100);
		if (randomInt <= 25)
		{
			text = "_l";
			ModuleHelper.SetBaseStatMult(module, 1f, 0.92f);
		}
		else if (randomInt <= 50)
		{
			text = "_t";
			ModuleHelper.SetBaseStatMult(module, 1f, 0.92f);
		}
		else if (randomInt <= 75)
		{
			text = "_c";
			ModuleHelper.SetBaseStatMult(module, 0.75f, 0.85f);
		}
		module.SetSpriteAndShape("Items/items_module", module.spritePath + text);
	}

	public static GameItem RandomOverclockModule()
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_overclock");
		SetAdvancedModuleShape(gameItem);
		ModuleEffectHelper.InitRandomEffect(gameItem);
		return gameItem;
	}

	public static GameItem RandomEcoModule()
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_eco");
		SetAdvancedModuleShape(gameItem);
		ModuleEffectHelper.InitRandomEffect(gameItem);
		return gameItem;
	}

	public static GameItem RandomFinenessModule()
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_fineness");
		SetAdvancedModuleShape(gameItem);
		ModuleEffectHelper.InitRandomEffect(gameItem);
		return gameItem;
	}

	public static GameItem RandomNode()
	{
		GameItem gameItem = DirectoryMaster.Item("node_medium");
		ModuleEffectHelper.InitRandomEffect(gameItem);
		return gameItem;
	}

	public static void SetAdvancedModuleShape(GameItem module)
	{
		string text = "";
		int randomInt = RNG.GetRandomInt(0, 100);
		if (randomInt <= 33)
		{
			text = "_s";
			ModuleHelper.SetBaseStatMult(module, 1.25f, 1.15f);
		}
		else if (randomInt <= 66)
		{
			text = "_h";
			ModuleHelper.SetBaseStatMult(module, 1.25f, 1.15f);
		}
		module.SetSpriteAndShape("Items/items_module", module.spritePath + text);
	}

	public static GameItem CreateRat(string sex, int age, bool defaultStats = false)
	{
		GameItem gameItem = DirectoryMaster.Item("rat");
		gameItem.ModifyTag("ANIMAL_SEXE_TAG", delegate(TagState state)
		{
			state.Enable().SetString(sex);
		});
		gameItem.ModifyTag("ANIMAL_AGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(age);
		});
		if (defaultStats)
		{
			GeneticHelper.SetToDefault(gameItem, AnimalSpecies.Rat);
		}
		float weight = HusbandryHelper.SimulateWeightAtAge(gameItem, AnimalSpecies.Rat, age);
		gameItem.ModifyTag("ANIMAL_WEIGHT_TAG", delegate(TagState state)
		{
			state.Enable().SetFloat(weight);
		});
		HusbandryHelper.TryChangeSprite(gameItem);
		return gameItem;
	}
}
