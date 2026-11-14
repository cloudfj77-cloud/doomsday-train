using System.Collections.Generic;

public class ModuleHelper
{
	public const string ADDITIONAL_EFFECT_STRING = "ADDITIONAL_EFFECT_STRING";

	public const string MODULE_STUCK_TAG = "MODULE_STUCK_TAG";

	public const string MODULE_TAG = "MODULE_TAG";

	public const string SHIP_MODULE_TAG = "SHIP_MODULE_TAG";

	public const string MODULE_TYPE = "MODULE_TYPE";

	public const string MODULE_TYPE_PREFIX = "MODULE_TYPE_";

	public const string MODULE_TYPE_UNIVERSAL = "MODULE_TYPE_UNIVERSAL";

	public const string MODULE_TYPE_HYDROPONIC = "MODULE_TYPE_HYDROPONIC";

	public const string MODULE_TYPE_ALARM = "MODULE_TYPE_ALARM";

	public const string MODULE_TYPE_FURNACE = "MODULE_TYPE_FURNACE";

	public const string MODULE_TYPE_NAVIGATION = "MODULE_TYPE_NAVIGATION";

	public const string MODULE_TYPE_HULL = "MODULE_TYPE_HULL";

	public const string MODULE_TYPE_PRINTER = "MODULE_TYPE_PRINTER";

	public const string MODULE_TYPE_CHEM = "MODULE_TYPE_CHEM";

	public const string MODULE_TYPE_EVAPORATOR = "MODULE_TYPE_EVAPORATOR";

	public const string MODULE_TYPE_SCAV_SCANNER = "MODULE_TYPE_SCAV_SCANNER";

	public const string MODULE_TYPE_DNA_MANIPULATOR = "MODULE_TYPE_DNA_MANIPULATOR";

	public const string EQUIPMENT_MODULE_TAG = "EQUIPMENT_MODULE_TAG";

	public const string BONUS_PERCENTAGE_PERFORMANCE_INT = "BONUS_PERCENTAGE_PERFORMANCE_INT";

	public const string BONUS_PERCENTAGE_EFFICIENCY_INT = "BONUS_PERCENTAGE_EFFICIENCY_INT";

	public const string BONUS_PERCENTAGE_QUALITY_INT = "BONUS_PERCENTAGE_QUALITY_INT";

	public const string TEMP_PERCENTAGE_PERFORMANCE_INT = "TEMP_PERCENTAGE_PERFORMANCE_INT";

	public const string TEMP_PERCENTAGE_EFFICIENCY_INT = "TEMP_PERCENTAGE_EFFICIENCY_INT";

	public const string TEMP_PERCENTAGE_QUALITY_INT = "TEMP_PERCENTAGE_QUALITY_INT";

	public const string IS_RECEIVING_BONUS_FROM_MODULE_BOOL = "IS_RECEIVING_BONUS_FROM_MODULE_BOOL";

	public const string TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT = "TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT";

	public const string TOTAL_PERCENTAGE_POWER_BONUS_INT = "TOTAL_PERCENTAGE_POWER_BONUS_INT";

	public const string TOTAL_PERCENTAGE_QUALITY_BONUS_INT = "TOTAL_PERCENTAGE_QUALITY_BONUS_INT";

	public const string DETECTION_ENCOUNTER_NAME = "DETECTION_ENCOUNTER_NAME";

	public const string DETECTION_ENCOUNTER_VALUE_INT = "DETECTION_ENCOUNTER_VALUE_INT";

	public const string MODULE_TIER_INT = "MODULE_TIER_INT";

	public const int MODULE_EXTRACT_COST = 10;

	public static bool IsModuleTypeTag(string tag)
	{
		return tag?.StartsWith("MODULE_TYPE_") ?? false;
	}

	public static void InitModuleItem(GameItem item, string moduleType, int bonusPercentagePerformance = 0, int bonusPercentageEfficiency = 0, int bonusPercentageQuality = 0, bool isShipModule = true, string additionalEffectDisplay = "", int tier = 1)
	{
		item.SetGameItemType("MODULE");
		item.EnableTag("MODULE_TAG");
		item.EnableTag(moduleType);
		item.SetType(moduleType);
		item.ModifyTag("MODULE_TYPE", delegate(TagState state)
		{
			state.Enable().SetString(moduleType);
		});
		item.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(bonusPercentagePerformance);
		});
		item.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(bonusPercentageEfficiency);
		});
		item.ModifyTag("BONUS_PERCENTAGE_QUALITY_INT", delegate(TagState state)
		{
			state.Enable().SetInt(bonusPercentageQuality);
		});
		item.ModifyTag("MODULE_TIER_INT", delegate(TagState state)
		{
			state.Enable().SetInt(tier);
		});
		if (isShipModule)
		{
			item.EnableTag("SHIP_MODULE_TAG");
		}
		else
		{
			item.EnableTag("EQUIPMENT_MODULE_TAG");
		}
		AudioHelper.InitModule(item);
		if (!additionalEffectDisplay.Equals(""))
		{
			item.ModifyTag("ADDITIONAL_EFFECT_STRING", delegate(TagState state)
			{
				state.Enable().SetString(additionalEffectDisplay);
			});
		}
		item.mayInventoryRemoveItemFunc = (GameItem item, GameInventory _) => !item.IsTag("MODULE_STUCK_TAG");
		ModuleEffectHelper.ResetAllTempStat(item);
	}

	public static void SetBaseStatMult(GameItem item, float mult, float valueMult = 1f)
	{
		int bonusPercentagePerformance = item.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt();
		int bonusPercentageEfficiency = item.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt();
		int bonusPercentageQuality = item.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt();
		bonusPercentagePerformance = (int)((float)bonusPercentagePerformance * mult);
		bonusPercentageEfficiency = (int)((float)bonusPercentageEfficiency * mult);
		bonusPercentageQuality = (int)((float)bonusPercentageQuality * mult);
		item.ModifyTag("BONUS_PERCENTAGE_PERFORMANCE_INT", delegate(TagState state)
		{
			state.SetInt(bonusPercentagePerformance);
		});
		item.ModifyTag("BONUS_PERCENTAGE_EFFICIENCY_INT", delegate(TagState state)
		{
			state.SetInt(bonusPercentageEfficiency);
		});
		item.ModifyTag("BONUS_PERCENTAGE_QUALITY_INT", delegate(TagState state)
		{
			state.SetInt(bonusPercentageQuality);
		});
		int num = (int)((float)item.unitValue * valueMult);
		item.unitValue = num;
		ModuleEffectHelper.ResetAllTempStat(item);
	}

	private static void AddModuleStatLine(RichTextBuilder builder, string statName, int baseValue, int tempValue)
	{
		if (baseValue != 0 || tempValue != 0)
		{
			string arg = ((tempValue > 0) ? "+" : "");
			RenderHandler.ColorPalette color = ((tempValue >= 0) ? RenderHandler.ColorPalette.LightCyan : RenderHandler.ColorPalette.LightOrange);
			if (baseValue == tempValue)
			{
				builder.AddLine(LocHelper.BulletLine($"{statName} {arg}{tempValue}%"), ignoreEmpty: true, color);
				return;
			}
			string arg2 = ((baseValue > 0) ? "+" : "");
			builder.Add(LocHelper.BulletLine($"{statName} {arg}{tempValue}%"), color);
			string localizedMechanic = LocHelper.GetLocalizedMechanic("mech_base_stat");
			builder.AddLine($" ({localizedMechanic}: {arg2}{baseValue}%)", ignoreEmpty: true, color);
		}
	}

	public static void CreateModuleTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("MODULE_TAG"))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_module_effect_header"), ignoreEmpty: true, RenderHandler.ColorPalette.Cyan);
			AddModuleStatLine(builder, LocHelper.GetLocalizedMechanic("mech_module_stat_performance"), item.GetTagReadonly("BONUS_PERCENTAGE_PERFORMANCE_INT").GetInt(), item.GetTagReadonly("TEMP_PERCENTAGE_PERFORMANCE_INT").GetInt());
			AddModuleStatLine(builder, LocHelper.GetLocalizedMechanic("mech_module_stat_efficiency"), item.GetTagReadonly("BONUS_PERCENTAGE_EFFICIENCY_INT").GetInt(), item.GetTagReadonly("TEMP_PERCENTAGE_EFFICIENCY_INT").GetInt());
			AddModuleStatLine(builder, LocHelper.GetLocalizedMechanic("mech_module_stat_quality"), item.GetTagReadonly("BONUS_PERCENTAGE_QUALITY_INT").GetInt(), item.GetTagReadonly("TEMP_PERCENTAGE_QUALITY_INT").GetInt());
			if (item.IsTag("ADDITIONAL_EFFECT_STRING"))
			{
				builder.AddLine(LocHelper.BulletLines(item.GetTagReadonly("ADDITIONAL_EFFECT_STRING").GetString()), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (item.IsTag("MODULE_STUCK_TAG"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_module_stuck"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			ModuleEffectHelper.CreateModuleEffectTooltip(builder, item);
		}
	}

	public static void ComputeModuleEffect(GameInventory invModule, GameItem item, GameItem system)
	{
		ComputeModuleEffect(invModule.childItems, system);
	}

	public static int SumQualityBonus(List<GameItem> activeModules)
	{
		int num = 0;
		foreach (GameItem activeModule in activeModules)
		{
			if (activeModule.IsTag("MODULE_TAG") || activeModule.IsGameItemType("NODE"))
			{
				num += activeModule.GetTagReadonly("TEMP_PERCENTAGE_QUALITY_INT").GetInt();
			}
		}
		return num;
	}

	public static void ComputeModuleEffect(List<GameItem> activeModules, GameItem system)
	{
		int totalBonusPercentagePerformance = 0;
		int totalBonusPercentageEfficiency = 0;
		int totalBonusPercentageQuality = 0;
		bool isBenefitFromModule = false;
		foreach (GameItem activeModule in activeModules)
		{
			if (activeModule.IsTag("MODULE_TAG") || activeModule.IsGameItemType("NODE"))
			{
				totalBonusPercentagePerformance += activeModule.GetTagReadonly("TEMP_PERCENTAGE_PERFORMANCE_INT").GetInt();
				totalBonusPercentageEfficiency += activeModule.GetTagReadonly("TEMP_PERCENTAGE_EFFICIENCY_INT").GetInt();
				totalBonusPercentageQuality += activeModule.GetTagReadonly("TEMP_PERCENTAGE_QUALITY_INT").GetInt();
				isBenefitFromModule = true;
			}
		}
		system.ModifyTag("IS_RECEIVING_BONUS_FROM_MODULE_BOOL", delegate(TagState state)
		{
			state.SetBool(isBenefitFromModule);
		});
		system.ModifyTag("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT", delegate(TagState state)
		{
			state.SetInt(totalBonusPercentagePerformance);
		});
		system.ModifyTag("TOTAL_PERCENTAGE_POWER_BONUS_INT", delegate(TagState state)
		{
			state.SetInt(totalBonusPercentageEfficiency);
		});
		system.ModifyTag("TOTAL_PERCENTAGE_QUALITY_BONUS_INT", delegate(TagState state)
		{
			state.SetInt(totalBonusPercentageQuality);
		});
	}

	public static void ApplyBasicModuleEffect(GameInventory invModule, GameItem item, GameItem system)
	{
		int @int = system.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
		int int2 = system.GetTagReadonly("TOTAL_PERCENTAGE_POWER_BONUS_INT").GetInt();
		system.GetTagReadonly("TOTAL_PERCENTAGE_QUALITY_BONUS_INT").GetInt();
		int int3 = system.GetTagReadonly("BASE_PROCESSING_SPEED_TAG").GetInt();
		int int4 = system.GetTagReadonly("MACHINERY_DRAWN_POWER_BASE").GetInt();
		int newSpeed = int3 + int3 * @int / 100;
		if (system.IsTag("MACHINE_PERFORMANCE_RATIO_INT"))
		{
			int int5 = system.GetTagReadonly("MACHINE_PERFORMANCE_RATIO_INT").GetInt();
			newSpeed = (int)((float)int3 + (float)(int3 * @int / int5) / 100f);
		}
		int newPowerUse = int4 - int4 * int2 / 100;
		if (system.IsTag("MACHINE_EFFICIENCY_RATIO_INT"))
		{
			int int6 = system.GetTagReadonly("MACHINE_EFFICIENCY_RATIO_INT").GetInt();
			if (int6 > 0)
			{
				newPowerUse = (int)((float)int4 - (float)(int4 * int2 / int6) / 100f);
			}
		}
		if (system.IsTag("MACHINE_PERFORMANCE_REDUCES_POWER_TAG") && system.IsTag("MACHINE_PERFORMANCE_RATIO_INT"))
		{
			int int7 = system.GetTagReadonly("MACHINE_PERFORMANCE_RATIO_INT").GetInt();
			if (int7 > 0)
			{
				newPowerUse -= (int)((float)(int4 * @int / int7) / 100f);
			}
		}
		if (newPowerUse < 1)
		{
			newPowerUse = 1;
		}
		system.ModifyTag("CURRENT_PROCESSING_SPEED_TAG", delegate(TagState state)
		{
			state.SetInt(newSpeed);
		});
		system.ModifyTag("machinery_drawn_power", delegate(TagState state)
		{
			state.SetInt(newPowerUse);
		});
	}

	public static int GetAjustedQuality(GameItem machine)
	{
		int @int = machine.GetTagReadonly("MACHINE_QUALITY_RATIO_INT").GetInt();
		if (@int <= 0)
		{
			return 0;
		}
		return machine.GetTagReadonly("TOTAL_PERCENTAGE_QUALITY_BONUS_INT").GetInt() / @int;
	}

	public static void ApplyModifiedPower(GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG"))
		{
			int @int = item.GetTagReadonly("MACHINERY_DRAWN_POWER_BASE").GetInt();
			int int2 = item.GetTagReadonly("TOTAL_PERCENTAGE_POWER_BONUS_INT").GetInt();
			int newPowerUse = @int - @int * int2 / 100;
			item.ModifyTag("machinery_drawn_power", delegate(TagState state)
			{
				state.SetInt(newPowerUse);
			});
		}
	}

	public static void ApplyEquipmentModifiedPower(GameItem item)
	{
		if (item.IsTag(EquipmentHelper.EQUIPMENT_TAG))
		{
			int @int = item.GetTagReadonly(EquipmentHelper.EQUIPMENT_DRAW_CONSUMPTION_BACKUP_INT).GetInt();
			int int2 = item.GetTagReadonly("TOTAL_PERCENTAGE_POWER_BONUS_INT").GetInt();
			int newPowerUse = @int - @int * int2 / 100;
			item.ModifyTag(EquipmentHelper.EQUIPMENT_DRAW_CONSUMPTION_INT, delegate(TagState state)
			{
				state.SetInt(newPowerUse);
			});
		}
	}

	public static void ApplyPerformanceNavEffect(GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG_NAVIGATION"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int navAlt = @int / 50 + 2;
			if (navAlt >= 5)
			{
				navAlt = 5;
			}
			item.ModifyTag("NAVIGATION_ALTERNATIF_INT", delegate(TagState state)
			{
				state.SetInt(navAlt);
			});
		}
	}

	public static void ApplyPerformancePropulsionEffect(GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG_PROPULSION"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int int2 = item.GetTagReadonly("BASE_PROPULSION_SPEED_INT").GetInt();
			int newSpeed = int2 + int2 * @int / 100;
			item.ModifyTag("PROPULSION_SPEED_INT", delegate(TagState state)
			{
				state.SetInt(newSpeed);
			});
		}
	}

	public static void ApplyEfficiencyPropulsionEffect(GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG_PROPULSION"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_POWER_BONUS_INT").GetInt();
			int int2 = item.GetTagReadonly("BASE_PROPULSION_FUEL_DRAIN_INT").GetInt();
			int newFuelConsumption = int2 - int2 * @int / 100;
			item.ModifyTag("PROPULSION_FUEL_DRAIN_INT", delegate(TagState state)
			{
				state.SetInt(newFuelConsumption);
			});
		}
	}

	public static void ApplyPerformanceWaterRecyclerEffect(GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG_SERVICE"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int num = 50;
			int newPercentage = num + @int;
			if (newPercentage > 100)
			{
				newPercentage = 100;
			}
			item.ModifyTag("WATER_RECYCLER_CURRENT_EFFICIENCY_INT", delegate(TagState state)
			{
				state.SetInt(newPercentage);
			});
		}
	}

	public static void ApplyPerformanceAutonomousMedicalEffect(GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG_SERVICE"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int num = 5;
			int newHealRate = num + num * @int / 100;
			item.ModifyTag("WATER_RECYCLER_CURRENT_EFFICIENCY_INT", delegate(TagState state)
			{
				state.SetInt(newHealRate);
			});
		}
	}

	public static void ApplyPerformanceHullRepairEffect(GameItem item)
	{
		if (item.IsTag("HULL_REPAIR_SYSTEM_TAG"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int num = 2;
			int newRepairRate = num + num * @int / 100;
			item.ModifyTag("HULL_REPAIR_RATE_INT", delegate(TagState state)
			{
				state.SetInt(newRepairRate);
			});
		}
	}

	public static void ApplyPerformanceArmorConditionerEffect(GameItem item)
	{
		if (item.IsTag("SYSTEM_TAG_INDUSTRIAL"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int num = 5;
			int newHealRate = num + num * @int / 100;
			item.ModifyTag("ARMOR_CONDITIONER_REPAIR_RATE_INT", delegate(TagState state)
			{
				state.SetInt(newHealRate);
			});
		}
	}

	public static void ApplyPerformanceMiningDrillEffect(GameItem item)
	{
		if (item.IsTag("MINING_DRILL_SYSTEM"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int num = 100;
			int newHealRate = num + num * @int / 100;
			item.ModifyTag("CURRENT_MINING_POWER_INT", delegate(TagState state)
			{
				state.SetInt(newHealRate);
			});
		}
	}

	public static void ApplyPerformanceSalvageGantryEffect(GameItem item)
	{
		if (item.IsTag("SALVAGE_GANTRY_SYSTEM"))
		{
			int @int = item.GetTagReadonly("TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT").GetInt();
			int num = 100;
			int newHealRate = num + num * @int / 100;
			item.ModifyTag("CURRENT_SALVAGE_POWER_INT", delegate(TagState state)
			{
				state.SetInt(newHealRate);
			});
		}
	}

	public static void AutonomousMedicalHeal(GameCharacterItem gci, int healRate)
	{
		if (gci.HaveEffect("bleed_heavy"))
		{
			gci.RemoveEffectByID("bleed_heavy");
		}
		else if (gci.HaveEffect("bleed_light"))
		{
			gci.RemoveEffectByID("bleed_light");
		}
		else if (gci.health != gci.maxHealth)
		{
			gci.HealEntity(healRate);
		}
	}

	public static bool CanAutoMedicalHeal(GameCharacterItem gci)
	{
		if (gci.HaveEffect("bleed_heavy"))
		{
			return true;
		}
		if (gci.HaveEffect("bleed_light"))
		{
			return true;
		}
		if (gci.health != gci.maxHealth)
		{
			return true;
		}
		return false;
	}

	public static bool IsAutoMedicalDone(List<GameCharacterItem> list)
	{
		foreach (GameCharacterItem item in list)
		{
			if (CanAutoMedicalHeal(item))
			{
				return false;
			}
		}
		return true;
	}

	public static void InitNavigationDetectionModule(GameItem item, string eventId, int modifiedOdd)
	{
		item.EnableTag("DETECTION_ENCOUNTER_NAME");
		item.ModifyTag("DETECTION_ENCOUNTER_NAME", delegate(TagState state)
		{
			state.SetString(eventId);
		});
		item.ModifyTag("DETECTION_ENCOUNTER_VALUE_INT", delegate(TagState state)
		{
			state.SetInt(modifiedOdd);
		});
	}

	public static void SetModuleStuck(GameItem gameItem)
	{
		if (gameItem.IsTag("MODULE_TAG"))
		{
			gameItem.EnableTag("MODULE_STUCK_TAG");
			gameItem.AddItemFeature(ItemFeatureList.ModuleStuckFeature());
		}
	}

	public static void SetModuleUnstuck(GameItem gameItem)
	{
		if (gameItem.IsTag("MODULE_TAG"))
		{
			gameItem.DisableTag("MODULE_STUCK_TAG");
			gameItem.RemoveItemFeatureByID("moduleStuck");
		}
	}

	public static void AddStuckRuinedModule(GameItem target)
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_ruined");
		GraphUtils.TryAcceptAll(target, gameItem);
		SetModuleStuck(gameItem);
	}

	public static void AddStuckCorruptedModule(GameItem target)
	{
		GameItem gameItem = DirectoryMaster.Item("system_module_corrupt");
		GraphUtils.TryAcceptAll(target, gameItem);
		SetModuleStuck(gameItem);
	}

	public static void AddRandomStuckNormalModule(GameItem target)
	{
		GameItem gameItem = ItemSpawner.Spawn(GetRandomModuleId());
		GraphUtils.TryAcceptAll(target, gameItem);
		SetModuleStuck(gameItem);
	}

	public static string GetRandomModuleId()
	{
		if (RNG.Roll(23))
		{
			return "random_performance_module";
		}
		if (RNG.Roll(23))
		{
			return "random_efficiency_module";
		}
		if (RNG.Roll(23))
		{
			return "random_quality_module";
		}
		if (RNG.Roll(10))
		{
			return "system_module_overclock";
		}
		if (RNG.Roll(10))
		{
			return "system_module_eco";
		}
		if (RNG.Roll(10))
		{
			return "system_module_fineness";
		}
		return "random_performance_module";
	}

	public static void RestoreModule(GameItem gameItem, GameItem electronicPart)
	{
		if (gameItem.IsTag("MODULE_TAG"))
		{
			electronicPart.Destroy();
			GameInventory parentInventory = gameItem.parentInventory;
			gameItem.Destroy();
			if (RNG.Roll(33))
			{
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("system_module_performance"));
			}
			else if (RNG.Roll(33))
			{
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("system_module_efficiancy"));
			}
			else
			{
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("system_module_quality"));
			}
		}
	}

	public static void RepairExtractor(GameItem extractor, GameItem electronicPart)
	{
		if (extractor.identifier.Equals("module_extractor"))
		{
			UseCountHelper.ModMaxUseCount(extractor, -1);
			UseCountHelper.ReplenishUseCount(extractor, 4);
			electronicPart.Destroy();
		}
	}

	public static bool CanElectronicPartTarget(GameItem electronicPart, GameItem target)
	{
		if (!GeneralHelper.IsItemOwned(electronicPart) || !GeneralHelper.IsItemOwned(target))
		{
			return false;
		}
		if (target.identifier.Equals("module_extractor"))
		{
			int @int = target.GetTagReadonly("MAX_USE_COUNT_INT").GetInt();
			int int2 = target.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
			if (@int == int2)
			{
				return false;
			}
			return true;
		}
		if (!target.IsTag("MODULE_STUCK_TAG") && target.IsTag("MODULE_TAG") && target.identifier.Equals("system_module_corrupt"))
		{
			return true;
		}
		if (target.identifier.Equals("vending_machine_core") && VendingMachineHelper.CanUpgradedAttractiveness(GeneralHelper.GetParentItem(target)))
		{
			return true;
		}
		if (target.identifier.Equals("vending_fountain_core") && VendingMachineHelper.CanUpgradedAttractiveness(GeneralHelper.GetParentItem(target)))
		{
			return true;
		}
		if (target.identifier.Equals("metal_scanner") && ScavHelper.CanUpgradeScavScanner(target))
		{
			return true;
		}
		if (target.identifier.Equals("vacuum_robot") && HusbandryHelper.CanUpgradeVacuumRobot(target))
		{
			return true;
		}
		if (target.IsTag("FRIDGE_TAG") && MachineFridge.CanUpgradePowerEfficiency(target))
		{
			return true;
		}
		if (MachineBottlePrinter.CanUpgradePrinter(target))
		{
			return true;
		}
		if (CurrentConverterHelper.CanUpgradeEfficiency(target))
		{
			return true;
		}
		if (target.IsTag("RAS_TAG") && MachineRAS.CanUpgradePowerEfficiency(target))
		{
			return true;
		}
		if (ReactorHelper.CanUpgrade(target))
		{
			return true;
		}
		return false;
	}
}
