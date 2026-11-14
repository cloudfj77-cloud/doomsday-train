using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreClientListSpec
{
	public const int MODULE_CHANCE_PER_PERMIT = 15;

	public const int METAL_CHIP_CHANCE_PER_PERMIT = 10;

	private const int AQUAFARMER_EQUIPMENT_CHANCE_AQUACULTURE_II = 25;

	public const string SCALPER_ID = "scalper";

	private static List<string> chemicalReturnKeys = new List<string> { "dialog_chem_return_general_1", "dialog_chem_return_general_2", "dialog_chem_return_general_3", "dialog_chem_return_general_4", "dialog_chem_return_general_5", "dialog_chem_return_general_6", "dialog_chem_return_general_7" };

	private static List<string> chemicalReturnSubstanceKeys = new List<string>
	{
		"dialog_chem_return_substance_1", "dialog_chem_return_substance_2", "dialog_chem_return_substance_3", "dialog_chem_return_substance_4", "dialog_chem_return_substance_5", "dialog_chem_return_substance_6", "dialog_chem_return_substance_7", "dialog_chem_return_substance_8", "dialog_chem_return_substance_9", "dialog_chem_return_substance_10",
		"dialog_chem_return_substance_11", "dialog_chem_return_substance_12"
	};

	private static readonly LocalizedLines chemicalReturnFollowups = LocalizedLines.Dialogue("dialog_chem_return_followup_1", "dialog_chem_return_followup_2", "dialog_chem_return_followup_3", "dialog_chem_return_followup_4", "dialog_chem_return_followup_5", "dialog_chem_return_followup_6", "dialog_chem_return_followup_7");

	private static readonly LocalizedLines reactionMild = LocalizedLines.Dialogue("dialog_reaction_mild_1", "dialog_reaction_mild_2", "dialog_reaction_mild_3", "dialog_reaction_mild_4", "dialog_reaction_mild_5");

	private static readonly LocalizedLines reactionStrong = LocalizedLines.Dialogue("dialog_reaction_strong_1", "dialog_reaction_strong_2", "dialog_reaction_strong_3");

	private static readonly LocalizedLines reactionExtreme = LocalizedLines.Dialogue("dialog_reaction_extreme_1", "dialog_reaction_extreme_2", "dialog_reaction_extreme_3");

	public static StoreClient CreateRetiredGunsmith()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_gunsmith";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_gunsmith");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientItemFeatureBuying.Add("premiumBuy15");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreatePrinter());
			PlayerStore.Instance.AddDirectSellingItemToTable("screwdriver");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_plastic");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_makeshift_rds");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_makeshift_toy_laser");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_small_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_pistol");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_internal_springs_improved");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_box_cutter");
			PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_kitchen_knife");
			if (StoreStation.Instance.dayCounter >= 20)
			{
				if (RNG.Roll(40))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_rds");
				}
				if (RNG.Roll(40))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_laser_sight");
				}
				if (RNG.Roll(40))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_silencer");
				}
				if (RNG.Roll(40))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_hatchet");
				}
				if (RNG.Roll(40))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_kitchen_cleaver");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_pistol_double");
				}
			}
			else
			{
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_rds");
				}
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_laser_sight");
				}
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_silencer");
				}
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_hatchet");
				}
				if (RNG.Roll(15))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_kitchen_cleaver");
				}
			}
			int permitSteps = (PermitHelper.IsGunPermitII() ? 1 : 0) + (PermitHelper.IsGunPermitIII() ? 1 : 0);
			if (RNG.Roll(Chance(33, 15)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_module_metal");
			}
			if (RNG.Roll(Chance(25, 15)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_module_melee_batch");
			}
			if (RNG.Roll(Chance(25, 15)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_module_cnc");
			}
			if (RNG.Roll(Chance(20, 10)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_combat_knife");
			}
			if (RNG.Roll(Chance(15, 10)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_combat_machete");
			}
			if (RNG.Roll(Chance(20, 10)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_handmade_pistol_frame");
			}
			if (RNG.Roll(Chance(15, 10)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_heavy_handmade_pistol_frame");
			}
			if (RNG.Roll(Chance(25, 10)))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_gun_part");
			}
			if (RNG.Roll(33))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_22_mag");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_22_mag_ext");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_22_mag_high");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_10_mag");
			}
			if (RNG.Roll(20))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_10_mag_ext");
			}
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("printer_chip_10_mag_high");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("silencer2");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("laser2");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("compensator");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_22_high");
			}
			if (RNG.Roll(22))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_high");
			}
			if (RNG.Roll(18))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("mag_10mm_ext");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("internal_springs_improved");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("trigger_group");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("match_barrel");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("pistol_switch");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("ammo_display");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("grip_gel");
			}
			int Chance(int baseChance, int perPermit)
			{
				return baseChance + permitSteps * perPermit;
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRetiredFarmer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_farmer";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_farmer");
		storeClient.spriteName = SpriteDict.GetRetiredFarmerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientItemFeatureBuying.Add("premiumBuy15");
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateHydroponic());
			PlayerStore.Instance.AddDirectSellingItemToTable("hydroponic_guide");
			PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("dream_cap_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("kotton_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("coffee_bean");
			PlayerStore.Instance.AddDirectSellingItemToTable("coffee_bean");
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrient_tablet");
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrient_tablet");
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrient_tablet");
			PlayerStore.Instance.AddDirectSellingItemToTable("fillerweed_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("fillerweed_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("hydroreed_seed");
			PlayerStore.Instance.AddDirectSellingItemToTable("hydroreed_seed");
			if (NetworkUpgrade.IsRetiredFarmerUnlocked())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("bloomberry_seed");
				PlayerStore.Instance.AddDirectSellingItemToTable("bloomberry_seed");
			}
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullNutrientTabletBox());
			PlayerStore.Instance.AddDirectSellingItemToTable("plant_analyser");
			PlayerStore.Instance.AddDirectSellingItemToTable("aquascan");
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullNutrifruitSeedPacket());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
			PlayerStore.Instance.AddDirectSellingItemToTable("water_pitcher");
			if (RNG.Roll(66))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_nutrifruit");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_dream_cap");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_kotton_loom");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_nightmare_cap");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_fillerweed_production");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_hydroreed_filter");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("farm_module_smuggler");
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	private static GameItem SpawnBigFishForSale(int pick, int slot)
	{
		string sex = ((slot % 2 == 0) ? "MALE" : "FEMALE");
		switch (pick)
		{
		case 0:
		{
			GameItem gameItem = AquacultureItems.SpawnFish(FishSpecies.Eel, sex, 0);
			if (gameItem != null)
			{
				gameItem.unitValue = 100L;
			}
			return gameItem;
		}
		case 1:
		{
			GameItem gameItem = AquacultureItems.SpawnFish(FishSpecies.ElectricEel, sex, 0);
			if (gameItem != null)
			{
				gameItem.unitValue = 100L;
			}
			return gameItem;
		}
		default:
		{
			GameItem gameItem = AquacultureItems.SpawnFish(FishSpecies.Sturgeon, "FEMALE", FishSpecies.Sturgeon.stageStartDays[3]);
			if (gameItem != null)
			{
				gameItem.unitValue = 500L;
			}
			return gameItem;
		}
		}
	}

	public static StoreClient CreateRetiredAquafarmer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_aquafarmer";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_aquafarmer");
		storeClient.spriteName = SpriteDict.GetRetiredAquafarmerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientItemFeatureBuying.Add("premiumBuy15");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).SetEndAction(delegate
		{
			bool aquacultureII = NetworkUpgrade.IsAquacultureIIUnlocked();
			if (aquacultureII)
			{
				for (int i = 0; i < 3; i++)
				{
					GameItem gameItem = SpawnBigFishForSale(RNG.GetRandomInt(0, 2), i);
					if (gameItem != null)
					{
						PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
					}
				}
			}
			if (StockEquipment())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(MachineRAS.SpawnRASWithEmptyTank());
			}
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("water_filter"));
			if (StockEquipment())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("livestock_feed_dispenser"));
			}
			if (StockEquipment())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("auto_butcher"));
			}
			if (StockEquipment())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("marking_pen"));
			}
			if (StockEquipment())
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("kitchen_cleaver"));
			}
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("gutterflow_pellet"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("gutterflow_pellet"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("industrial_fish_feed"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("industrial_fish_feed"));
			FishSpecies[] array = new FishSpecies[4]
			{
				FishSpecies.Tilapia,
				FishSpecies.Tilapia,
				FishSpecies.GutterTilapia,
				FishSpecies.GraftTilapia
			};
			for (int j = 0; j < array.Length; j++)
			{
				GameItem gameItem2 = AquacultureItems.SpawnFertilizedRoe(array[j]);
				if (gameItem2 != null)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(gameItem2);
				}
			}
			bool StockEquipment()
			{
				if (aquacultureII)
				{
					return RNG.Roll(25);
				}
				return true;
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRetiredRancher()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_rancher";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_rancher");
		storeClient.spriteName = SpriteDict.GetRandomFarmerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientItemFeatureBuying.Add("premiumBuy15");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("animal_cage"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("livestock_feed_dispenser"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("auto_butcher"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("marking_pen"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("kitchen_cleaver"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("labeler_blue"));
			PlayerStore.Instance.AddDirectSellingItemToTable("vacuum_robot");
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("serum_red"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("serum_green"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("gene_scanner"));
			PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("wire_cutter"));
			GameItem gameItem = PreBuiltItemHelper.CreateRat("MALE", 5);
			GameItem gameItem2 = PreBuiltItemHelper.CreateRat("FEMALE", 5);
			gameItem.unitValue = 120L;
			gameItem2.unitValue = 120L;
			PlayerStore.Instance.AddDirectSellingItemToTable(gameItem);
			PlayerStore.Instance.AddDirectSellingItemToTable(gameItem2);
			if (RNG.Roll(25))
			{
				GameItem gameItem3 = PreBuiltItemHelper.CreateRat("MALE", 5);
				gameItem3.unitValue = 500L;
				float multiplier = (float)RNG.GetRandomDouble(1.25, 1.5);
				GeneticHelper.SetTrait(gameItem3, HusbandryHelper.GetSpecies(gameItem3), "ANIMAL_GROWTH_RATE_TAG", multiplier);
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem3);
			}
			if (RNG.Roll(25))
			{
				GameItem gameItem4 = PreBuiltItemHelper.CreateRat("FEMALE", 5);
				gameItem4.unitValue = 500L;
				float multiplier2 = (float)RNG.GetRandomDouble(1.25, 1.5);
				GeneticHelper.SetTrait(gameItem4, HusbandryHelper.GetSpecies(gameItem4), "ANIMAL_GROWTH_RATE_TAG", multiplier2);
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem4);
			}
			if (RNG.Roll(25))
			{
				GameItem gameItem5 = PreBuiltItemHelper.CreateRat("MALE", 5);
				gameItem5.unitValue = 500L;
				float multiplier3 = (float)RNG.GetRandomDouble(1.25, 1.5);
				GeneticHelper.SetTrait(gameItem5, HusbandryHelper.GetSpecies(gameItem5), "ANIMAL_LITTER_SIZE_TAG", multiplier3);
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem5);
			}
			if (RNG.Roll(25))
			{
				GameItem gameItem6 = PreBuiltItemHelper.CreateRat("FEMALE", 5);
				gameItem6.unitValue = 500L;
				float multiplier4 = (float)RNG.GetRandomDouble(1.25, 1.5);
				GeneticHelper.SetTrait(gameItem6, HusbandryHelper.GetSpecies(gameItem6), "ANIMAL_LITTER_SIZE_TAG", multiplier4);
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem6);
			}
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateNutrifruitSmall());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateNutrifruitSmall());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateNutrifruitSmall());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateNutrifruitSmall());
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRetiredWaterMerchant()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_water_merchant";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_water_merchant");
		storeClient.spriteName = SpriteDict.GetRandomLowerLevelSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientItemFeatureBuying.Add("premiumBuy15");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateWaterPurifier());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateMoistureFarm());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
			if (RNG.Roll(75))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterTestStripBox());
			}
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
			if (RNG.Roll(75))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
			}
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.WaterFilter());
			}
			PlayerStore.Instance.AddDirectSellingItemToTable("water_pitcher");
			PlayerStore.Instance.AddDirectSellingItemToTable("water_tank");
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateEvaporator());
			if (RNG.Roll(15))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_filter_adv");
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRetiredWinemaker()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_winemaker";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_winemaker");
		storeClient.spriteName = SpriteDict.GetRandomFarmerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientItemFeatureBuying.Add("premiumBuy15");
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_bottle");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
			PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_superyeast");
			}
			else
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
			}
			if (RNG.Roll(25))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_superyeast");
			}
			else
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast");
			}
			if (RNG.Roll(5))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_yeast_infinite");
			}
			PlayerStore.Instance.AddDirectSellingItemToTable("water_ration");
			PlayerStore.Instance.AddDirectSellingItemToTable("water_ration");
			for (int i = 0; i < 8; i++)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wine_berry");
			}
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRetiredJunker()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_junker";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_junker");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateFurnace());
				if (PlayerStore.Instance.isBrokenMachineIntroduced)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor");
					PlayerStore.Instance.AddDirectSellingItemToTable("wire_cutter");
					PlayerStore.Instance.AddDirectSellingItemToTable("screwdriver");
					PlayerStore.Instance.AddDirectSellingItemToTable("welder");
				}
				PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
				if (RNG.Roll(75))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
				}
				if (RNG.Roll(50))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
				}
				if (RNG.Roll(25))
				{
					PlayerStore.Instance.AddDirectSellingItemToTable("flux_agent");
				}
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRetiredJunkerIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_junker_intro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_junker");
		storeClient.spriteName = SpriteDict.GetRandomScavSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitScav(storeClient);
		storeClient.dismissable = false;
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.isBrokenMachineIntroduced = true;
				PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor", isOwend: true);
				PlayerStore.Instance.AddDirectSellingItemToTable("welder", isOwend: true);
			})
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_5"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_6"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_7"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_8"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_9"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_10"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_junker_intro_11"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateRetiredChemist()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "retired_chemist";
		storeClient.nonShootable = true;
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_retired_chemist");
		storeClient.spriteName = SpriteDict.GetRetiredChemistSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		storeClient.clientItemFeatureBuying.Add("premiumBuy15");
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_retired_generic_1")).SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blender");
			PlayerStore.Instance.AddDirectSellingItemToTable("heating_plate");
			PlayerStore.Instance.AddDirectSellingItemToTable("evaporator");
			PlayerStore.Instance.AddDirectSellingItemToTable("chem_finisher");
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("storage_bay_chem");
			}
			if (RNG.Roll(75))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("evaporator_module_solvent_recovery");
			}
			PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical");
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("restricted_chemical");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("chem_scanner");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("1ml_dropper");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("5ml_dropper");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("25ml_cylinder");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("50ml_cylinder");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("100ml_cylinder");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("module_extractor");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_filter");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_filter");
			}
			if (RNG.Roll(75))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("kotton_fabric");
			}
			if (RNG.Roll(50))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_pitcher");
			}
			if (RNG.Roll(75))
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("water_filter");
			}
			PlayerStore.Instance.AddDirectSellingItemToTable("chem_module");
			if (RNG.Roll(50))
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
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
			PlayerStore.Instance.AddDirectSellingItemToTable("chemistry_guide");
		});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateBlackmailerIntro()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "blackmailer_intro";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_blackmailer");
		storeClient.spriteName = SpriteDict.GetBlackmailerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.dismissable = false;
		storeClient.nonDismissableArrestable = true;
		storeClient.SetWanted(150, "", isStealthWanted: true);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_intro_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_intro_2"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_intro_3"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_intro_4"))
			.NextDialogue()
			.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_intro_5"))
			.SetEndAction(delegate
			{
				DialogUIManager.Instance.CurrentClientLeave(0f);
			});
		storeClient.onCaptureAction = delegate
		{
			TriggerBlackmailerCapture();
		};
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_blackmailer_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_blackmailer_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateScalper()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "scalper";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_scalper");
		storeClient.spriteName = SpriteDict.GetShadyMerchantSpriteName();
		storeClient.clientFaction = "FACTION_BLACK_MARKET";
		storeClient.clientIntent = StoreClient.ClientIntent.SELL;
		Dialogue dialogue;
		if (PlayerStore.Instance.storeClientData.scalperFirstVisitDay < 0)
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scalper_first_1")).SetStartAction(delegate
			{
				if (PlayerStore.Instance.storeClientData.scalperFirstVisitDay < 0)
				{
					PlayerStore.Instance.storeClientData.scalperFirstVisitDay = StoreStation.Instance.dayCounter;
				}
			});
			dialogue = storeClient.mainDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scalper_first_2"));
		}
		else
		{
			storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scalper_1"));
			dialogue = storeClient.mainDialogue.NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scalper_2"));
		}
		dialogue.SetEndAction(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealGenuineInjector());
		});
		storeClient.onItemAddedToWeighted = delegate(GameItem item)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				ItemFeature itemFeature = ItemFeatureList.ScalperMarkup(GetScalperMarkup());
				if (itemFeature != null)
				{
					item.AddItemFeature(itemFeature);
				}
			}
		};
		storeClient.AddBasicDialog();
		storeClient.allDoneDialogue = new Dialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_scalper_alldone")).SetEndAction(delegate
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		});
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static int GetScalperMarkup()
	{
		int scalperFirstVisitDay = PlayerStore.Instance.storeClientData.scalperFirstVisitDay;
		int num = ((scalperFirstVisitDay >= 0) ? Mathf.Max(0, StoreStation.Instance.dayCounter - scalperFirstVisitDay) : 0);
		return Difficulty.Rules.scalperMarkupBase + Mathf.FloorToInt(Difficulty.Rules.scalperMarkupPerDay * (float)num);
	}

	public static StoreClient CreateBlackmailer()
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "blackmailer";
		storeClient.displayName = LocHelper.GetLocalizedClientName("name_blackmailer");
		storeClient.spriteName = SpriteDict.GetBlackmailerSpriteName();
		storeClient.clientFaction = "FACTION_LOWER_LEVEL";
		StoreClientFactionSetup.InitLowerLevelCitizen(storeClient);
		storeClient.dismissable = false;
		storeClient.nonDismissableArrestable = true;
		storeClient.SetWanted(150, "", isStealthWanted: true);
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue dialogue = new Dialogue();
		Dialogue dialogue2 = new Dialogue();
		Dialogue dialogue3 = new Dialogue();
		dialogue3.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_cost_line", GetBlackmailCost().ToString()));
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_blackmailer_choice_pay")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_blackmailer_pay_accept"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_blackmailer_pay_accept_2"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.ModCash(-GetBlackmailCost());
				DialogUIManager.Instance.CurrentClientLeave(0f);
			});
		dialogue2.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_blackmailer_choice_refuse")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_blackmailer_pay_refuse_client"))
			.SetEndAction(delegate
			{
				TriggerBlackmailerCapture();
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_blackmailer_2"))
			.nextDialogue = dialogue3;
		dialogue3.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, delegate
		{
		}, () => PlayerStore.Instance.GetCash() >= GetBlackmailCost()).AddChoice(DirectoryMaster.Item("$"), "", "", dialogue2, delegate
		{
		});
		storeClient.onCaptureAction = delegate
		{
			TriggerBlackmailerCapture();
		};
		string title = "CLIENT";
		string title2 = "SEC";
		storeClient.customOnArrestDialogue = new Dialogue();
		storeClient.customOnArrestDialogue.SetText(title, LocHelper.GetLocalizedDialogue("dialog_blackmailer_arrest_client")).NextDialogue().SetText(title2, LocHelper.GetLocalizedDialogue("dialog_blackmailer_arrest_sec"))
			.SetEndAction(delegate
			{
				StoreUIManager.Instance.OnArrestDialoguePlayed();
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static void TriggerBlackmailerCapture()
	{
		StoreReputation.ModLLReputation(-30);
		StoreReputation.ModRevReputation(-30);
		StoreReputation.ModSecReputation(-30);
		StoreReputation.ModULReputation(-30);
		StoreReputation.ModNCReputation(-30);
		StartingPerk.RemovePerk("blackmail");
	}

	public static int GetBlackmailCost()
	{
		return 40 + StoreStation.Instance.dayCounter;
	}

	public static float GetRelatedItemMultipler(int feedBackLevel)
	{
		if (feedBackLevel >= 5)
		{
			return 1f;
		}
		if (feedBackLevel >= 4)
		{
			return 0.75f;
		}
		if (feedBackLevel >= 3)
		{
			return 0.5f;
		}
		if (feedBackLevel >= 2)
		{
			return 0.25f;
		}
		if (feedBackLevel >= 1)
		{
			return 0.1f;
		}
		return 0f;
	}

	public static StoreClient CreateChemicalFeedbackClient(SoldItem soldItem, List<SoldItem> relatedItems, List<string> lines, bool isAskingCompensation, int reputationLoss)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "chemical_feedback_client";
		storeClient.displayName = soldItem.clientName;
		storeClient.spriteName = soldItem.clientSprite;
		storeClient.clientFaction = soldItem.factionId;
		storeClient.cooldown = 0;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		Dialogue option1 = new Dialogue();
		Dialogue dialogue = new Dialogue();
		Dialogue lastDialogue = new Dialogue();
		storeClient.dismissable = false;
		int repLost = reputationLoss;
		float num = 0f;
		float num2 = 0f;
		foreach (SoldItem relatedItem in relatedItems)
		{
			if (relatedItem != soldItem)
			{
				num += (float)relatedItem.soldPrice * GetRelatedItemMultipler(relatedItem.feedbackLevel);
				num2 += (float)reputationLoss * GetRelatedItemMultipler(relatedItem.feedbackLevel);
			}
		}
		int refundAmount;
		if (isAskingCompensation)
		{
			lastDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_chem_feedback_refund_compensation"));
			refundAmount = soldItem.soldPrice * 2;
			refundAmount = Math.Max(refundAmount, 50 + Math.Min(StoreStation.Instance.dayCounter, 55));
		}
		else
		{
			lastDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_chem_feedback_refund_only"));
			refundAmount = soldItem.soldPrice;
			refundAmount = Math.Max(refundAmount, 25 + Math.Min(StoreStation.Instance.dayCounter, 55));
		}
		refundAmount += (int)num;
		repLost += (int)num2;
		repLost = Math.Max(repLost, -20);
		option1.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_chem_feedback_choice_accept", refundAmount)).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_chem_feedback_good"))
			.NextDialogue()
			.SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_chem_feedback_goodbye"))
			.SetEndAction(delegate
			{
				PlayerStore.Instance.ModCash(-refundAmount);
				DialogUIManager.Instance.CurrentClientLeave();
			});
		dialogue.SetText("PLAYER", LocHelper.GetLocalizedDialogue("dialog_chem_feedback_choice_refuse", repLost)).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedDialogue("dialog_chem_feedback_refuse_client"))
			.SetEndAction(delegate
			{
				StoreReputation.ModReputation(soldItem.factionId, repLost);
				DialogUIManager.Instance.CurrentClientLeave();
			});
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_chem_feedback_returning"));
		Dialogue dialogue2 = storeClient.mainDialogue;
		foreach (string line in lines)
		{
			dialogue2.NextDialogue().SetText(storeClient.displayName, line);
			dialogue2 = dialogue2.nextDialogue;
		}
		bool isRefundChoiceAdded = false;
		dialogue2.SetEndAction(delegate
		{
			if (!isRefundChoiceAdded)
			{
				isRefundChoiceAdded = true;
				lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", option1, delegate
				{
				}, () => PlayerStore.Instance.playerCash >= refundAmount);
			}
		}).nextDialogue = lastDialogue;
		lastDialogue.AddChoice(DirectoryMaster.Item("$"), "", "", dialogue, delegate
		{
		});
		return storeClient;
	}

	public static StoreClient CreateChemicalReturningClient(SoldItem soldItem)
	{
		GameItem gameItem = soldItem.GetGameItem();
		string name = gameItem.name;
		string localizedDialogue = LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(chemicalReturnKeys), name);
		StoreClient storeClient = new StoreClient();
		storeClient.CanReturnAsBuyer = false;
		storeClient.identifier = "chemical_returning_client";
		storeClient.displayName = soldItem.clientName;
		storeClient.spriteName = soldItem.clientSprite;
		storeClient.clientFaction = soldItem.factionId;
		storeClient.cooldown = 0;
		storeClient.clientIntent = StoreClient.ClientIntent.BUY;
		storeClient.dismissable = true;
		int budget = Math.Max(50 + StoreStation.Instance.dayCounter / 5, soldItem.soldPrice * 3);
		storeClient.SetBudget(budget);
		storeClient.clientBuyingIdList.Add(soldItem.itemId);
		if (gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			storeClient.clientBuyingTagList.Add("WATER");
		}
		if (gameItem.IsGameItemType("SUBSTANCE"))
		{
			localizedDialogue = LocHelper.GetLocalizedDialogue(RNG.GetRandomElement(chemicalReturnSubstanceKeys), name);
		}
		storeClient.mainDialogue.SetText(storeClient.displayName, localizedDialogue).NextDialogue().SetText(storeClient.displayName, GetLineFromEuphoriaLevel(gameItem))
			.NextDialogue()
			.SetText(storeClient.displayName, RNG.GetRandomElement(chemicalReturnFollowups))
			.SetEndAction(delegate
			{
			});
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static string GetLineFromEuphoriaLevel(GameItem item)
	{
		switch (ChemicalFeedbackHelper.GetEuphoriaLevel(item))
		{
		case 0:
		case 1:
			Debug.Log("This should not be happening, LEVEL 0 OR 1 don't return as a returning client");
			return LocHelper.GetLocalizedDialogue("dialog_reaction_good");
		case 2:
			return RNG.GetRandomElement(reactionMild);
		case 3:
			return RNG.GetRandomElement(reactionStrong);
		case 4:
			return RNG.GetRandomElement(reactionExtreme);
		default:
			return LocHelper.GetLocalizedDialogue("dialog_reaction_fallback");
		}
	}

	public static StoreClient CreateHirelingClient(Hireling hireling)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "hireling";
		storeClient.nonShootable = true;
		storeClient.displayName = hireling.displayName;
		storeClient.spriteName = hireling.spriteId;
		storeClient.hirelingData = hireling;
		storeClient.clientIntent = StoreClient.ClientIntent.EXPEDITION;
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hireling_ready_1")).NextDialogue().SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hireling_ready_2"))
			.SetEndAction(delegate
			{
				ExpeditionHelper.SetHirelingEquipmentOnCounter(hireling.id);
			});
		storeClient.OnDismiss = delegate
		{
			ExpeditionHelper.RemoveHirelingEquipmentFromStore(hireling.id);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}

	public static StoreClient CreateHirelingClientReturn(Hireling hireling)
	{
		StoreClient storeClient = new StoreClient();
		storeClient.identifier = "hireling_return";
		storeClient.nonShootable = true;
		storeClient.displayName = hireling.displayName;
		storeClient.spriteName = hireling.spriteId;
		storeClient.hirelingData = hireling;
		storeClient.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		string localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_hireling_return_healthy");
		if (hireling.IsInjured())
		{
			localizedDialogue = LocHelper.GetLocalizedDialogue("dialog_hireling_return_injured");
		}
		storeClient.mainDialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedDialogue("dialog_hireling_return_1")).NextDialogue().SetText(storeClient.displayName, localizedDialogue)
			.SetEndAction(delegate
			{
				ExpeditionHelper.SetHirelingEquipmentOnCounter(hireling.id);
				hireling.OnExpeditionReported();
			});
		storeClient.OnDismiss = delegate
		{
			ExpeditionHelper.RemoveHirelingEquipmentFromStore(hireling.id);
		};
		storeClient.AddBasicDialog();
		storeClient.CompleteClientCreation();
		return storeClient;
	}
}
