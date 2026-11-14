using System;

public class ModuleDirectory : ItemDirectory
{
	public const int NUTRIFRUIT_MODULE_BONUS_YIELD = 1;

	private const int PRINTER_CHIP_COUNT_MAKESHIFT_RDS = 4;

	private const int PRINTER_CHIP_COUNT_TOY_LASER = 4;

	private const int PRINTER_CHIP_COUNT_SMALL_BOTTLE = 6;

	private const int PRINTER_CHIP_COUNT_RDS = 2;

	private const int PRINTER_CHIP_COUNT_LASER_SIGHT = 2;

	private const int PRINTER_CHIP_COUNT_SILENCER = 2;

	private const int PRINTER_CHIP_COUNT_22_MAG = 2;

	private const int PRINTER_CHIP_COUNT_22_MAG_EXT = 2;

	private const int PRINTER_CHIP_COUNT_22_MAG_HIGH = 2;

	private const int PRINTER_CHIP_COUNT_10_MAG = 2;

	private const int PRINTER_CHIP_COUNT_10_MAG_EXT = 1;

	private const int PRINTER_CHIP_COUNT_10_MAG_HIGH = 1;

	private const int PRINTER_CHIP_COUNT_PISTOL_DOUBLE = 2;

	private const int PRINTER_CHIP_COUNT_BOX_CUTTER = 3;

	private const int PRINTER_CHIP_COUNT_KITCHEN_KNIFE = 4;

	private const int PRINTER_CHIP_COUNT_HATCHET = 4;

	private const int PRINTER_CHIP_COUNT_KITCHEN_CLEAVER = 4;

	private const int PRINTER_CHIP_COUNT_COMBAT_KNIFE = 5;

	private const int PRINTER_CHIP_COUNT_COMBAT_MACHETE = 3;

	private const int BLAST_FURNACE_INPUT_COUNT = 3;

	private const int BLAST_FURNACE_OUTPUT_COUNT = 2;

	private const int MODULE_EXTRACTOR_USE_COUNT = 6;

	private const int ALARM_TRANSMITTER_REPUTATION_GAIN = 1;

	public const int HYDROREED_FILTER_YIELD_BONUS = 1;

	private const int JUNK_MODULE_INPUT = 6;

	private const int JUNK_MODULE_OUTPUT = 4;

	public const int DREAM_CAP_NEUTRALIZER_YIELD_BONUS = 2;

	private const int PRINTER_CHIP_COUNT_INTERNAL_SPRINGS_IMPROVED = 4;

	protected override void InitDirectory()
	{
		Add("blank_module", () => BlankModule());
		Add("system_module_performance", () => SystemPerformanceModule());
		Add("system_module_efficiancy", () => SystemEfficiencyModule());
		Add("system_module_quality", () => SystemQualityModule());
		Add("system_module_overclock", () => SystemOverclockModule());
		Add("system_module_eco", () => SystemEcoModule());
		Add("system_module_fineness", () => SystemFinenessModule());
		Add("system_module_streamlining", () => SystemStreamliningModule());
		Add("system_module_ruined", () => SystemRuinedModule());
		Add("system_module_corrupt", () => SystemCorruptModule());
		Add("farm_module_nutrifruit", () => FarmModuleNutrifruit());
		Add("farm_module_dream_cap", () => FarmModuleDreamCapNeutralizer());
		Add("farm_module_kotton_loom", () => FarmModuleKottonLoom());
		Add("farm_module_nightmare_cap", () => FarmModuleDreamCapNightmare());
		Add("farm_module_fillerweed_production", () => FarmModuleFillerweedProduction());
		Add("farm_module_hydroreed_filter", () => FarmModuleHydroreedFilter());
		Add("farm_module_smuggler", () => FarmModuleSmuggler());
		Add("evaporator_module_solvent_recovery", () => EvaporatorSolventRecovery());
		Add("dna_manipulator_module_stabilizer", () => DnaManipulatorStabilizer());
		Add("dna_manipulator_module_positive_filter", () => DnaManipulatorPositiveFilter());
		Add("dna_manipulator_module_negative_filter", () => DnaManipulatorNegativeFilter());
		Add("dna_manipulator_module_precision", () => DnaManipulatorPrecision());
		Add("printer_module_metal", () => PrinterModuleMetal());
		Add("system_uncapped_neural_core", () => SystemUncappedNeuralCoreModule());
		Add("alarm_module_transmitter", () => AlarmModuleTransmitter());
		Add("furnace_module_blast", () => FurnaceModuleBlast());
		Add("furnace_module_junk", () => FurnaceModuleJunk());
		Add("system_capped_neural_core", () => SystemcappedNeuralCoreModule());
		Add("crypto_module_sec", () => CrypoModuleSec());
		Add("crypto_module_cmd", () => CrypoModuleCmd());
		Add("crypto_module_med", () => CrypoModuleMed());
		Add("crypto_module_eng", () => CrypoModuleEng());
		Add("crypto_module_sup", () => CrypoModuleSup());
		Add("crypto_module_ser", () => CrypoModuleSer());
		Add("printer_chip_pistol", () => PrinterChipPistol());
		Add("printer_chip_pistol_double", () => PrinterChipPistolDouble());
		Add("printer_chip_makeshift_rds", () => PrinterChipMakeshiftRDS());
		Add("printer_chip_makeshift_toy_laser", () => PrinterChipMakeshiftToyLaser());
		Add("printer_chip_small_bottle", () => PrinterChipSmallBottle());
		Add("printer_chip_internal_springs_improved", () => PrinterChipInternalSpringsImproved());
		Add("printer_chip_rds", () => PrinterChipRDS());
		Add("printer_chip_laser_sight", () => PrinterChipLaserSight());
		Add("printer_chip_silencer", () => PrinterChipSilencer());
		Add("printer_chip_22_mag", () => PrinterChip22Mag());
		Add("printer_chip_22_mag_ext", () => PrinterChip22MagExt());
		Add("printer_chip_22_mag_high", () => PrinterChip22MagHigh());
		Add("printer_chip_10_mag", () => PrinterChip10Mag());
		Add("printer_chip_10_mag_ext", () => PrinterChip10MagExt());
		Add("printer_chip_10_mag_high", () => PrinterChip10MagHigh());
		Add("printer_chip_handmade_pistol_frame", () => PrinterChipHandmadePistolFrame());
		Add("printer_chip_heavy_handmade_pistol_frame", () => PrinterChipHeavyHandmadePistolFrame());
		Add("printer_chip_gun_part", () => PrinterChipGunPart());
		Add("printer_module_cnc", () => PrinterModuleCNC());
		MachineProgressHelper.SetMachineTagDisplayName("Metal Printing Module", () => LocHelper.GetLocalizedItem("item_printer_module_metal_name"));
		MachineProgressHelper.SetMachineTagDisplayName("CNC Module", () => LocHelper.GetLocalizedOrDefault("Item", "item_printer_module_cnc_name", "Computer Numerical Control Module"));
		Add("printer_chip_box_cutter", () => CreateMeleeChip("item_printer_chip_box_cutter_name", "3D Printer Chip (x{0} Box Cutter)", "box_cutter", 3, 150, 34, (id: "scrap_metal", count: 2), ("printer_plastic", 1)));
		Add("printer_chip_kitchen_knife", () => CreateMeleeChip("item_printer_chip_kitchen_knife_name", "3D Printer Chip (x{0} Kitchen Knife)", "kitchen_knife", 4, 200, 64, (id: "scrap_metal", count: 3), ("printer_plastic", 1)));
		Add("printer_chip_hatchet", () => CreateMeleeChip("item_printer_chip_hatchet_name", "3D Printer Chip (x{0} Hatchet)", "hatchet", 4, 250, 75, (id: "scrap_metal", count: 4), ("printer_plastic", 1)));
		Add("printer_chip_kitchen_cleaver", () => CreateMeleeChip("item_printer_chip_kitchen_cleaver_name", "3D Printer Chip (x{0} Meat Cleaver)", "kitchen_cleaver", 4, 300, 82, (id: "scrap_metal", count: 5), ("printer_plastic", 2)));
		Add("printer_chip_combat_knife", () => CreateMeleeChip("item_printer_chip_combat_knife_name", "3D Printer Chip (x{0} Combat Knife)", "combat_knife", 5, 380, 98, (id: "metal_ingot", count: 2), null, requiresMetalPrinting: true));
		Add("printer_chip_combat_machete", () => CreateMeleeChip("item_printer_chip_combat_machete_name", "3D Printer Chip (x{0} Combat Machete)", "combat_machete", 3, 480, 120, (id: "metal_ingot", count: 3), null, requiresMetalPrinting: true));
		Add("printer_module_melee_batch", () => PrinterModuleMeleeBatch());
		Add("module_extractor", () => ModuleExtractor());
		Add("module_extractor_advanced", () => ModuleExtractorAdvanced());
		Add("module_bay_expansion_kit", () => ModuleBayExpansionKit());
		Add("chem_module", () => ChemModule());
		Add("node_small", () => NodeSmall());
		Add("node_medium", () => NodeMedium());
	}

	private GameItem BlankModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_white");
		gameItem.name = LocHelper.GetLocalizedItem("item_blank_module_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_blank_module_desc");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 60L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL");
		return gameItem;
	}

	private GameItem SystemPerformanceModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_red");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_performance_name");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 70L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 16);
		return gameItem;
	}

	private GameItem SystemEfficiencyModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_green");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_efficiancy_name");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 70L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 0, 16);
		return gameItem;
	}

	private GameItem SystemQualityModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_yellow");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_quality_name");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 70L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 0, 0, 8);
		return gameItem;
	}

	private GameItem SystemOverclockModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_red_2");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_overclock_name");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 100L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 36, -44);
		return gameItem;
	}

	private GameItem SystemEcoModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_green_2");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_eco_name");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 100L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", -16, 32);
		return gameItem;
	}

	private GameItem SystemStreamliningModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_green_3");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_streamlining_name");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 125L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 8, 25, -16);
		return gameItem;
	}

	private GameItem SystemFinenessModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_yellow_2");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_fineness_name");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 100L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", -8, -15, 25);
		return gameItem;
	}

	private GameItem SystemRuinedModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_ruined");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_ruined_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_system_module_ruined_desc");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 30L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL");
		return gameItem;
	}

	private GameItem SystemCorruptModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_corrupt");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_module_corrupt_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_system_module_corrupt_desc");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 45L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", -8, -5);
		return gameItem;
	}

	private GameItem SystemUncappedNeuralCoreModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_black_5");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_uncapped_neural_core_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_system_uncapped_neural_core_desc");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 1500L;
		ContrabandHelper.InitContrabandItem(gameItem, 4);
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 200, -100, 100);
		return gameItem;
	}

	private GameItem SystemcappedNeuralCoreModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_black_4");
		gameItem.name = LocHelper.GetLocalizedItem("item_system_capped_neural_core_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_system_capped_neural_core_desc");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 250L;
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 100, -100, 50);
		return gameItem;
	}

	private GameItem EquipmentPerformanceModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_yellow_2");
		gameItem.name = "Equipment Performance Module";
		gameItem.unitValue = 100L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 30, 0, 0, isShipModule: false);
		return gameItem;
	}

	private GameItem EquipmentEfficiencyModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_blue_2");
		gameItem.name = "Equipment Efficiency Module";
		gameItem.unitValue = 100L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 0, 20, 0, isShipModule: false);
		return gameItem;
	}

	private GameItem EquipmentOverclockModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_red_2");
		gameItem.name = "Equipment Overclock Module";
		gameItem.unitValue = 100L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", 65, -40, 0, isShipModule: false);
		return gameItem;
	}

	private GameItem EquipmentEcoModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_green_2");
		gameItem.name = "Equipment Eco Module";
		gameItem.unitValue = 100L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_UNIVERSAL", -30, 40, 0, isShipModule: false);
		return gameItem;
	}

	private static bool MayChipSwap(GameItem source, GameItem target)
	{
		if (source == null || target == null || source == target)
		{
			return false;
		}
		if (!source.IsTag("DESEQUENCER_TARGET_ID_TAG"))
		{
			return false;
		}
		if (!target.IsTag("DESEQUENCER_TARGET_ID_TAG"))
		{
			return false;
		}
		if (source.IsGameItemType("MACHINE") || target.IsGameItemType("MACHINE"))
		{
			return false;
		}
		if (target.MayStack(source))
		{
			return false;
		}
		if (source.parentInventory == null || target.parentInventory == null)
		{
			return false;
		}
		if (source.parentInventory == target.parentInventory)
		{
			return false;
		}
		return true;
	}

	private static void InitChipSwap(GameItem item)
	{
		item.canUseOutsideCombat = true;
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, new Func<GameItem, GameItem, bool>(MayChipSwap));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, new Func<GameItem, GameItem, bool>(MayChipSwap));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem source, GameItem target)
		{
			if (MayChipSwap(source, target))
			{
				GraphUtils.SwapItemsInPlace(source, target);
			}
		});
	}

	private GameItem CrypoModuleCmd()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "cmd_module");
		gameItem.name = LocHelper.GetLocalizedItem("item_crypto_module_cmd_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_crypto_module_cmd_desc");
		gameItem.unitValue = 350L;
		ContrabandHelper.InitContrabandItem(gameItem, 4);
		gameItem.ModifyTag("DESEQUENCER_TARGET_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString("cmd_keycard");
		});
		MachineProgressHelper.InitProgressSourceItem(gameItem, 150);
		InitChipSwap(gameItem);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem CrypoModuleSec()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "sec_module");
		gameItem.name = LocHelper.GetLocalizedItem("item_crypto_module_sec_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_crypto_module_sec_desc");
		gameItem.unitValue = 300L;
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		gameItem.ModifyTag("DESEQUENCER_TARGET_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString("sec_keycard");
		});
		MachineProgressHelper.InitProgressSourceItem(gameItem, 125);
		InitChipSwap(gameItem);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem CrypoModuleMed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "med_module");
		gameItem.name = LocHelper.GetLocalizedItem("item_crypto_module_med_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_crypto_module_med_desc");
		gameItem.unitValue = 250L;
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.ModifyTag("DESEQUENCER_TARGET_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString("med_keycard");
		});
		MachineProgressHelper.InitProgressSourceItem(gameItem, 100);
		InitChipSwap(gameItem);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem CrypoModuleEng()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "eng_module");
		gameItem.name = LocHelper.GetLocalizedItem("item_crypto_module_eng_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_crypto_module_eng_desc");
		gameItem.unitValue = 250L;
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.ModifyTag("DESEQUENCER_TARGET_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString("eng_keycard");
		});
		MachineProgressHelper.InitProgressSourceItem(gameItem, 100);
		InitChipSwap(gameItem);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem CrypoModuleSup()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "sup_module");
		gameItem.name = LocHelper.GetLocalizedItem("item_crypto_module_sup_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_crypto_module_sup_desc");
		gameItem.unitValue = 200L;
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.ModifyTag("DESEQUENCER_TARGET_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString("sup_keycard");
		});
		MachineProgressHelper.InitProgressSourceItem(gameItem, 75);
		InitChipSwap(gameItem);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem CrypoModuleSer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "ser_module");
		gameItem.name = LocHelper.GetLocalizedItem("item_crypto_module_ser_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_crypto_module_ser_desc");
		gameItem.unitValue = 200L;
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.ModifyTag("DESEQUENCER_TARGET_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString("ser_keycard");
		});
		MachineProgressHelper.InitProgressSourceItem(gameItem, 75);
		InitChipSwap(gameItem);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipPistolDouble()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip_pistol");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_printer_chip_pistol_double_name", "3D Printer Chip (x{0} .22 Pistol)", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 400L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "printer_plastic", 3);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 60, "printed_gun", useDefaultTooltip: true, 2);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipPistol()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip_pistol");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_pistol_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 250L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "printer_plastic", 2);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 50, "printed_gun");
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipInternalSpringsImproved()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip2");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_internal_springs_improved_name", 4);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 150L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "scrap_metal", 2);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 20, "internal_springs_improved", useDefaultTooltip: true, 4);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipMakeshiftRDS()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip2");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_makeshift_rds_name", 4);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 200L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetDualAdditionalRequiredItem(gameItem, (itemID: "printer_plastic", count: 1), (itemID: "common_electronic", count: 1));
		MachineProgressHelper.InitProgressSourceItem(gameItem, 20, "rds_makeshift", useDefaultTooltip: true, 4);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipMakeshiftToyLaser()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip2");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_makeshift_toy_laser_name", 4);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 200L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetDualAdditionalRequiredItem(gameItem, (itemID: "printer_plastic", count: 1), (itemID: "common_electronic", count: 1));
		MachineProgressHelper.InitProgressSourceItem(gameItem, 20, "laser_toy", useDefaultTooltip: true, 4);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipSmallBottle()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip2");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_small_bottle_name", 6);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 100L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "printer_plastic", 1);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 10, "small_bottled_water", useDefaultTooltip: true, 6);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipRDS()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip4");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_rds_name", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 300L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetDualAdditionalRequiredItem(gameItem, (itemID: "scrap_metal", count: 3), (itemID: "common_electronic", count: 1));
		MachineProgressHelper.InitProgressSourceItem(gameItem, 24, "rds", useDefaultTooltip: true, 2);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipLaserSight()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip4");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_laser_sight_name", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 300L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetDualAdditionalRequiredItem(gameItem, (itemID: "scrap_metal", count: 2), (itemID: "common_electronic", count: 1));
		MachineProgressHelper.InitProgressSourceItem(gameItem, 24, "laser", useDefaultTooltip: true, 2);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipSilencer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip4");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_silencer_name", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 300L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "scrap_metal", 5);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 24, "silencer", useDefaultTooltip: true, 2);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChip22Mag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip3");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_22_mag_name", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 250L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "printer_plastic", 1);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 24, "mag_22", useDefaultTooltip: true, 2);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChip22MagHigh()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip3");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_22_mag_high_name", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 300L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "printer_plastic", 2);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 36, "mag_22_high", useDefaultTooltip: true, 2);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChip22MagExt()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip3");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_22_mag_ext_name", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 350L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "printer_plastic", 3);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 48, "mag_22_ext", useDefaultTooltip: true, 2);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChip10Mag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip4");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_10_mag_name", 2);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 300L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "scrap_metal", 3);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 36, "mag_10mm", useDefaultTooltip: true, 2, "Metal Printing Module");
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChip10MagHigh()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip4");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_10_mag_high_name", 1);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 350L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "scrap_metal", 2);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 48, "mag_10mm_high", useDefaultTooltip: true, 1, "Metal Printing Module");
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChip10MagExt()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip4");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_chip_10_mag_ext_name", 1);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = 400L;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "scrap_metal", 3);
		MachineProgressHelper.InitProgressSourceItem(gameItem, 60, "mag_10mm_ext", useDefaultTooltip: true, 1, "Metal Printing Module");
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem FarmModuleSmuggler()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module6");
		gameItem.name = LocHelper.GetLocalizedItem("item_farm_module_smuggler_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_farm_module_smuggler_desc");
		gameItem.unitValue = 100L;
		gameItem.SetGameItemType("HYDROPONIC_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_HYDROPONIC", 0, 0, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_remove_contraband"));
		return gameItem;
	}

	private GameItem EvaporatorSolventRecovery()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module1");
		gameItem.name = LocHelper.GetLocalizedItem("item_evaporator_module_solvent_recovery_name");
		gameItem.unitValue = 275L;
		gameItem.SetGameItemType("EVAPORATOR_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_EVAPORATOR", 0, -25, 50, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_solvent_only"));
		return gameItem;
	}

	private GameItem DnaManipulatorStabilizer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module1");
		gameItem.name = PlantGeneticsText.StabilizerName();
		gameItem.shortDescription = PlantGeneticsText.StabilizerDescription();
		gameItem.unitValue = 500L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_DNA_MANIPULATOR", -50, -25, 0, isShipModule: true, PlantGeneticsText.StabilizerEffect());
		return gameItem;
	}

	private GameItem DnaManipulatorPositiveFilter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module1");
		gameItem.name = PlantGeneticsText.PositiveFilterName();
		gameItem.shortDescription = PlantGeneticsText.PositiveFilterDescription();
		gameItem.unitValue = 500L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_DNA_MANIPULATOR", 0, -25, 0, isShipModule: true, PlantGeneticsText.PositiveFilterEffect());
		return gameItem;
	}

	private GameItem DnaManipulatorNegativeFilter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module1");
		gameItem.name = PlantGeneticsText.NegativeFilterName();
		gameItem.shortDescription = PlantGeneticsText.NegativeFilterDescription();
		gameItem.unitValue = 500L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_DNA_MANIPULATOR", 0, -25, 0, isShipModule: true, PlantGeneticsText.NegativeFilterEffect());
		return gameItem;
	}

	private GameItem DnaManipulatorPrecision()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module1");
		gameItem.unitValue = 1000L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_DNA_MANIPULATOR", -75, -75, 0, isShipModule: true, PlantGeneticsText.PrecisionEffect());
		MachineDnaManipulator.SetupPrecisionModule(gameItem);
		return gameItem;
	}

	private GameItem CreateMeleeChip(string nameKey, string englishName, string targetId, int count, int price, int progress, (string id, int count) first, (string id, int count)? second, bool requiresMetalPrinting = false)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip6");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", nameKey, englishName, count);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = price;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		gameItem.EnableTag("PRINTER_MELEE_CHIP_TAG");
		if (second.HasValue)
		{
			MachineProgressHelper.SetDualAdditionalRequiredItem(gameItem, first, second.Value);
		}
		else
		{
			MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, first.id, first.count);
		}
		MachineProgressHelper.InitProgressSourceItem(gameItem, progress, targetId, useDefaultTooltip: true, count, requiresMetalPrinting ? "Metal Printing Module" : null);
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterModuleMeleeBatch()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module2");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_printer_module_melee_batch_name", "Melee Batch Module");
		gameItem.shortDescription = LocHelper.GetLocalizedOrDefault("Item", "item_printer_module_melee_batch_desc", "A printer module that adds one more weapon to every melee weapon print.");
		gameItem.unitValue = 250L;
		gameItem.SetGameItemType("PRINTER_MODULE");
		gameItem.EnableTag("PRINTER_MELEE_BATCH_MODULE_TAG");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_PRINTER", -50, -50, 0, isShipModule: true, LocHelper.GetLocalizedOrDefault("Item", "module_effect_melee_batch", "Printed Melee Weapon Output +{0}", 1));
		return gameItem;
	}

	private GameItem CreateCncChip(string nameKey, string englishName, string targetId, int count, int price, int progress, bool framePrint, bool gunPartPrint)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "printer_chip_pistol");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", nameKey, englishName, count);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_chip_desc");
		gameItem.unitValue = price;
		gameItem.EnableTag("PRINTER_CHIP_TAG");
		if (framePrint)
		{
			gameItem.EnableTag("PRINTER_FRAME_CHIP_TAG");
		}
		if (gunPartPrint)
		{
			gameItem.EnableTag("PRINTER_GUN_PART_CHIP_TAG");
		}
		MachineProgressHelper.InitProgressSourceItem(gameItem, progress, targetId, useDefaultTooltip: true, count, "Metal Printing Module");
		MachineProgressHelper.AddRequiredMachineTag(gameItem, "CNC Module");
		AudioHelper.InitSmallModule(gameItem);
		return gameItem;
	}

	private GameItem PrinterChipHandmadePistolFrame()
	{
		GameItem gameItem = CreateCncChip("item_printer_chip_handmade_pistol_frame_name", "Nestfire Compact Frame 3D Printer Chip", "handmade_pistol_frame", 1, 350, 60, framePrint: true, gunPartPrint: false);
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "metal_ingot", 1);
		return gameItem;
	}

	private GameItem PrinterChipHeavyHandmadePistolFrame()
	{
		GameItem gameItem = CreateCncChip("item_printer_chip_heavy_handmade_pistol_frame_name", "Ironcraft 10 Frame 3D Printer Chip", "heavy_handmade_pistol_frame", 1, 450, 70, framePrint: true, gunPartPrint: false);
		MachineProgressHelper.SetDualAdditionalRequiredItem(gameItem, (itemID: "metal_ingot", count: 1), (itemID: "scrap_metal", count: 1));
		return gameItem;
	}

	private GameItem PrinterChipGunPart()
	{
		GameItem gameItem = CreateCncChip("item_printer_chip_gun_part_name", "Gun Part 3D Printer Chip", "gun_part", 3, 300, 30, framePrint: false, gunPartPrint: true);
		MachineProgressHelper.SetSingleAdditionalRequiredItem(gameItem, "metal_ingot", 1);
		return gameItem;
	}

	private GameItem PrinterModuleCNC()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module3");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_printer_module_cnc_name", "Computer Numerical Control Module");
		gameItem.shortDescription = LocHelper.GetLocalizedOrDefault("Item", "item_printer_module_cnc_desc", "A module that allows the printer to use gun frame and gun part chips, together with the Metal Printing Module.");
		gameItem.unitValue = 400L;
		gameItem.SetGameItemType("PRINTER_MODULE");
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			if (MachineHelper.IsModuleBay(inventory))
			{
				inventory.GetParentItem().EnableTag("CNC Module");
			}
		});
		gameItem.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			if (MachineHelper.IsModuleBay(inventory))
			{
				inventory.GetParentItem().DisableTag("CNC Module");
			}
		});
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_PRINTER", -25, -25, 0, isShipModule: true, LocHelper.GetLocalizedOrDefault("Item", "module_effect_cnc", "Computer Numerical Control"));
		return gameItem;
	}

	private GameItem PrinterModuleMetal()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module1");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_module_metal_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_module_metal_desc");
		gameItem.unitValue = 300L;
		gameItem.SetGameItemType("PRINTER_MODULE");
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			if (MachineHelper.IsModuleBay(inventory))
			{
				inventory.GetParentItem().EnableTag("Metal Printing Module");
			}
		});
		gameItem.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			if (MachineHelper.IsModuleBay(inventory))
			{
				inventory.GetParentItem().DisableTag("Metal Printing Module");
			}
		});
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_PRINTER", 0, -50, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_metal_printing"));
		return gameItem;
	}

	private GameItem AlarmModuleTransmitter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module_l7");
		gameItem.name = LocHelper.GetLocalizedItem("item_alarm_module_transmitter_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_alarm_module_transmitter_desc", StoreReputation.GetFactionNameByID("FACTION_SECURITY"));
		gameItem.ModifyTag("TRANSMITTER_TARGET_TAG", delegate(TagState state)
		{
			state.SetString("FACTION_SECURITY");
		});
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_alarm_module_transmitter_flavor");
		gameItem.unitValue = 100L;
		gameItem.SetGameItemType("ALARM_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_ALARM", 0, -50, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_alarm_reputation", 1));
		gameItem.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onToggleSlotItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			MachineAlarm.ToggleTransmitter(item);
		};
		return gameItem;
	}

	private GameItem FurnaceModuleBlast()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module_l8");
		gameItem.name = LocHelper.GetLocalizedItem("item_furnace_module_blast_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_furnace_module_blast_desc", 3, 2);
		gameItem.unitValue = 150L;
		gameItem.SetGameItemType("MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_FURNACE", 0, -100, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_blast_furnace"));
		return gameItem;
	}

	private GameItem FurnaceModuleJunk()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "long_module3");
		gameItem.name = LocHelper.GetLocalizedItem("item_furnace_module_junk_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_furnace_module_junk_desc");
		gameItem.unitValue = 120L;
		gameItem.SetGameItemType("MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_FURNACE", 0, 0, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_junk_convert", 6, 4));
		return gameItem;
	}

	private GameItem FarmModuleHydroreedFilter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module_l6");
		gameItem.name = LocHelper.GetLocalizedItem("item_farm_module_hydroreed_filter_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_farm_module_hydroreed_filter_desc");
		gameItem.unitValue = 105L;
		gameItem.SetGameItemType("HYDROPONIC_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_HYDROPONIC", 0, -15, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_hydroreed_yield", 1));
		return gameItem;
	}

	private GameItem FarmModuleFillerweedProduction()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module_l5");
		gameItem.name = LocHelper.GetLocalizedItem("item_farm_module_fillerweed_production_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_farm_module_fillerweed_production_desc");
		gameItem.unitValue = 115L;
		gameItem.SetGameItemType("HYDROPONIC_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_HYDROPONIC", 0, -25, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_fillerweed_yield", 4));
		return gameItem;
	}

	private GameItem FarmModuleDreamCapNightmare()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module5");
		gameItem.name = LocHelper.GetLocalizedItem("item_farm_module_nightmare_cap_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_farm_module_nightmare_cap_desc");
		gameItem.unitValue = 150L;
		gameItem.SetGameItemType("HYDROPONIC_MODULE");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_HYDROPONIC", -33, -50, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_nightmare_cap"));
		return gameItem;
	}

	private GameItem FarmModuleDreamCapNeutralizer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module_l4");
		gameItem.name = LocHelper.GetLocalizedItem("item_farm_module_dream_cap_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_farm_module_dream_cap_desc");
		gameItem.unitValue = 150L;
		gameItem.SetGameItemType("HYDROPONIC_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_HYDROPONIC", 15, 15, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_dream_cap_neutralizer", 2));
		return gameItem;
	}

	private GameItem FarmModuleNutrifruit()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module_l2");
		gameItem.name = LocHelper.GetLocalizedItem("item_farm_module_nutrifruit_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_farm_module_nutrifruit_desc");
		gameItem.unitValue = 150L;
		gameItem.SetGameItemType("HYDROPONIC_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_HYDROPONIC", -10, -10, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_nutrifruit_yield", 1));
		return gameItem;
	}

	private GameItem FarmModuleKottonLoom()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "farm_module_l1");
		gameItem.name = LocHelper.GetLocalizedItem("item_farm_module_kotton_loom_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_farm_module_kotton_loom_desc");
		gameItem.unitValue = 200L;
		gameItem.SetGameItemType("HYDROPONIC_MODULE");
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_HYDROPONIC", -50, -100, 0, isShipModule: true, LocHelper.GetLocalizedItem("module_effect_kotton_fabric"));
		return gameItem;
	}

	private GameItem ModuleExtractor()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_extractor");
		gameItem.name = LocHelper.GetLocalizedItem("item_module_extractor_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_module_extractor_desc", 6);
		gameItem.unitValue = 100L;
		gameItem.SetGameItemType("TOOL");
		AudioHelper.InitSmallMachine(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => UseCountHelper.CanUse(s) && t.IsTag("MODULE_STUCK_TAG") && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => UseCountHelper.CanUse(s) && t.IsTag("MODULE_STUCK_TAG") && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (UseCountHelper.CanUse(s))
			{
				ModuleHelper.SetModuleUnstuck(t);
				EmporiumAudioPlayer.Instance.ratchet2.Play();
				UseCountHelper.Use(s);
			}
		});
		UseCountHelper.InitUseCountItem(gameItem, 6, transferable: false, destroyOnEmpty: false, useAutoValue: true, 40, 10);
		return gameItem;
	}

	private GameItem ModuleExtractorAdvanced()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "module_extractor_adv");
		gameItem.name = LocHelper.GetLocalizedItem("item_module_extractor_advanced_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_module_extractor_advanced_desc");
		gameItem.unitValue = 350L;
		gameItem.SetGameItemType("TOOL");
		AudioHelper.InitSmallMachine(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("MODULE_STUCK_TAG") && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("MODULE_STUCK_TAG") && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			ModuleHelper.SetModuleUnstuck(t);
			EmporiumAudioPlayer.Instance.ratchet2.Play();
		});
		return gameItem;
	}

	private GameItem ModuleBayExpansionKit()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "hydro_upgrade_kit");
		gameItem.name = LocHelper.GetLocalizedItem("item_module_bay_expansion_kit_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_module_bay_expansion_kit_desc");
		gameItem.unitValue = 150L;
		gameItem.SetGameItemType("TOOL");
		AudioHelper.InitModule(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayItemTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("STANDARD_MACHINE_TAG") && MachineHelper.CanExpand(t) && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)));
		gameItem.canItemTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canItemTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("STANDARD_MACHINE_TAG") && MachineHelper.CanExpand(t) && GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s)));
		gameItem.onItemTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onItemTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			MachineHelper.ExpandModule(MachineHelper.GetModuleInv(t), t);
			t.unitValue += 100L;
			StartingPerkContent.ConsumeUpgradeMaterial(s);
		});
		return gameItem;
	}

	private GameItem ChemModule()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "chem_module");
		gameItem.name = LocHelper.GetLocalizedItem("item_chem_module_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_chem_module_desc");
		gameItem.SetGameItemType("MODULE");
		gameItem.unitValue = 250L;
		ModuleHelper.InitModuleItem(gameItem, "MODULE_TYPE_CHEM");
		return gameItem;
	}

	private GameItem NodeSmall()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "node_s");
		gameItem.name = LocHelper.GetLocalizedItem("item_node_small_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_node_desc");
		gameItem.unitValue = 50L;
		ModuleEffectHelper.InitNode(gameItem);
		return gameItem;
	}

	private GameItem NodeMedium()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "node_m");
		gameItem.name = LocHelper.GetLocalizedItem("item_node_medium_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_node_desc");
		ModuleEffectHelper.InitNode(gameItem);
		gameItem.unitValue = 50L;
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			inventory.GetParentItem()?.IsGameItemType("MACHINE");
		});
		gameItem.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			inventory.GetParentItem()?.IsGameItemType("MACHINE");
		});
		return gameItem;
	}
}
