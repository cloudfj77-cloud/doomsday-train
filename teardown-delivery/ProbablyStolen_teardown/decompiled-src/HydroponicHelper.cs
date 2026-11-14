using System;
using UnityEngine;

public class HydroponicHelper
{
	public const string SEED_ITEM_TAG = "SEED_ITEM_TAG";

	public const string PLANT_ID_STRING = "PLANT_ID_STRING";

	public const string REQUIRED_PROGRESS_INT = "REQUIRED_PROGRESS";

	public const string HARVEST_QUANTITY_INT = "HARVEST_QUANTITY";

	public const string HARVEST_ITEM_ID = "HARVEST_ITEM_ID";

	public const string REQUIRED_NUTRIMENT_LEVEL_TAG = "REQUIRED_NUTRIMENT_LEVEL_TAG";

	public const string REQUIRED_NUTRIMENT_RANGE_TAG = "REQUIRED_NUTRIMENT_RANGE_TAG";

	public const string NUTRIMENT_USE_RATE = "NUTRIMENT_USE_RATE";

	public const string WATER_USE_RATE = "WATER_USE_RATE";

	public const string CONTAMINANT_EFFECT_MULT = "CONTAMINANT_EFFECT_MULT";

	public const string GROWING_TAG = "GROWING_TAG";

	public const string GROWING_PROGRESS_INT = "GROWING_PROGRESS";

	public const string ASSOCIATED_HYDROPONIC_ID = "ASSOCIATED_HYDROPONIC_ID";

	public const string PLANT_HEALTH_INT = "PLANT_HEALTH_INT";

	public const string PERENNIAL_TAG = "PERENNIAL_TAG";

	public const string PERENNIAL_ESTABLISHED_INT = "PERENNIAL_ESTABLISHED_INT";

	public const int HEALTH_SCALE = 100;

	public const int REFRUIT_FALLBACK_PERCENT = 50;

	public static void InitSeedItem(GameItem item, string plantId)
	{
		PlantDefinition plantDefinition = PlantRegistry.Get(plantId);
		if (plantDefinition == null)
		{
			Debug.LogError("HydroponicHelper.InitSeedItem: unknown plant '" + plantId + "'");
			return;
		}
		item.EnableTag("SEED_ITEM_TAG");
		item.SetType("SEED_ITEM_TAG");
		item.ModifyTag("PLANT_ID_STRING", delegate(TagState state)
		{
			state.Enable().SetString(plantId);
		});
		StampBaseStats(item, plantDefinition.requiredProgress, plantDefinition.harvestQuantity, plantDefinition.harvestItemId, plantDefinition.nutrimentLevel, plantDefinition.nutrimentRange, plantDefinition.mineralUseRate, plantDefinition.contaminantEffect, plantDefinition.waterUseRate);
		PlantGeneticsHelper.InitInnateTraits(item, plantDefinition);
		MachineProgressHelper.InitProgressSourceItem(item, plantDefinition.requiredProgress, useDefaultTooltip: false);
	}

	public static void InitSeedItem(GameItem item, int requredProgress, int harvestQuantity, string harvestId, int nutrimentLevel = 50, int nutrimentRange = 50, int mineralUseRate = 50, double contaminantEffect = 1.0, int waterUseRate = 100)
	{
		item.EnableTag("SEED_ITEM_TAG");
		item.SetType("SEED_ITEM_TAG");
		StampBaseStats(item, requredProgress, harvestQuantity, harvestId, nutrimentLevel, nutrimentRange, mineralUseRate, contaminantEffect, waterUseRate);
		MachineProgressHelper.InitProgressSourceItem(item, requredProgress, useDefaultTooltip: false);
	}

	private static void StampBaseStats(GameItem item, int requiredProgress, int harvestQuantity, string harvestId, int nutrimentLevel, int nutrimentRange, int mineralUseRate, double contaminantEffect, int waterUseRate)
	{
		item.ModifyTag("REQUIRED_PROGRESS", delegate(TagState state)
		{
			state.Enable().SetInt(requiredProgress);
		});
		item.ModifyTag("HARVEST_QUANTITY", delegate(TagState state)
		{
			state.Enable().SetInt(harvestQuantity);
		});
		item.ModifyTag("HARVEST_ITEM_ID", delegate(TagState state)
		{
			state.Enable().SetString(harvestId);
		});
		item.ModifyTag("REQUIRED_NUTRIMENT_LEVEL_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(nutrimentLevel);
		});
		item.ModifyTag("REQUIRED_NUTRIMENT_RANGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(nutrimentRange);
		});
		item.ModifyTag("NUTRIMENT_USE_RATE", delegate(TagState state)
		{
			state.Enable().SetInt(mineralUseRate);
		});
		item.ModifyTag("WATER_USE_RATE", delegate(TagState state)
		{
			state.Enable().SetInt(waterUseRate);
		});
		item.ModifyTag("CONTAMINANT_EFFECT_MULT", delegate(TagState state)
		{
			state.Enable().SetDouble(contaminantEffect);
		});
	}

	public static string GetPlantId(GameItem seed)
	{
		if (seed == null || !seed.IsTag("PLANT_ID_STRING"))
		{
			return "";
		}
		return seed.GetTagReadonly("PLANT_ID_STRING").GetString();
	}

	public static PlantDefinition GetPlant(GameItem seed)
	{
		return PlantRegistry.Get(GetPlantId(seed));
	}

	public static string GetHarvestItemId(GameItem seed)
	{
		PlantDefinition plant = GetPlant(seed);
		if (plant != null)
		{
			return plant.harvestItemId;
		}
		if (!seed.IsTag("HARVEST_ITEM_ID"))
		{
			return "";
		}
		return seed.GetTagReadonly("HARVEST_ITEM_ID").GetString();
	}

	private static int BaseInt(GameItem seed, string tag, int fallback)
	{
		if (seed == null || !seed.IsTag(tag))
		{
			return fallback;
		}
		return seed.GetTagReadonly(tag).GetInt();
	}

	public static int GetRequiredProgress(GameItem seed)
	{
		PlantDefinition plant = GetPlant(seed);
		int num = plant?.requiredProgress ?? BaseInt(seed, "REQUIRED_PROGRESS", 10);
		if (IsPerennialEstablished(seed))
		{
			num = GetRefruitBaseProgress(plant, num);
		}
		float statMultiplier = PlantGeneticsHelper.GetStatMultiplier(seed, "GROWTH_SPEED");
		return Math.Max(1, Mathf.RoundToInt((float)num / statMultiplier));
	}

	private static int GetRefruitBaseProgress(PlantDefinition plant, int fullWork)
	{
		if (plant != null && plant.refruitProgress > 0)
		{
			return plant.refruitProgress;
		}
		return Math.Max(1, Mathf.RoundToInt((float)(fullWork * 50) / 100f));
	}

	public static int GetBaseHarvestQuantity(GameItem seed)
	{
		return (int)Math.Round((float)(GetPlant(seed)?.harvestQuantity ?? BaseInt(seed, "HARVEST_QUANTITY", 1)) * PlantGeneticsHelper.GetStatMultiplier(seed, "HARVEST_QUANTITY"), MidpointRounding.AwayFromZero);
	}

	public static int GetWaterUsePerCycle(GameItem seed)
	{
		return Mathf.RoundToInt((float)(GetPlant(seed)?.waterUseRate ?? BaseInt(seed, "WATER_USE_RATE", 100)) * PlantGeneticsHelper.GetStatMultiplier(seed, "WATER_USE"));
	}

	public static int GetNutrimentLevel(GameItem seed)
	{
		return GetPlant(seed)?.nutrimentLevel ?? BaseInt(seed, "REQUIRED_NUTRIMENT_LEVEL_TAG", 50);
	}

	public static int GetNutrimentRange(GameItem seed)
	{
		return Mathf.RoundToInt((float)(GetPlant(seed)?.nutrimentRange ?? BaseInt(seed, "REQUIRED_NUTRIMENT_RANGE_TAG", 50)) * PlantGeneticsHelper.GetStatMultiplier(seed, "NUTRIMENT_RANGE"));
	}

	public static int GetMineralUseRate(GameItem seed)
	{
		return GetPlant(seed)?.mineralUseRate ?? BaseInt(seed, "NUTRIMENT_USE_RATE", 50);
	}

	public static double GetContaminantEffect(GameItem seed)
	{
		PlantDefinition plant = GetPlant(seed);
		if (plant == null)
		{
			if (seed == null || !seed.IsTag("CONTAMINANT_EFFECT_MULT"))
			{
				return 1.0;
			}
			return seed.GetTagReadonly("CONTAMINANT_EFFECT_MULT").GetDouble();
		}
		return plant.contaminantEffect;
	}

	public static double GetContaminantSensitivity(GameItem seed)
	{
		return PlantGeneticsHelper.GetStatMultiplier(seed, "CONTAMINANT_SENSITIVITY");
	}

	public static int GetStartingHealthPercent(GameItem seed)
	{
		return Mathf.Clamp(Mathf.RoundToInt((float)(GetPlant(seed)?.startingHealth ?? 100) * PlantGeneticsHelper.GetStatMultiplier(seed, "PLANT_HEALTH")), 1, 100);
	}

	public static int GetHealthPercent(GameItem seed)
	{
		return BaseInt(seed, "PLANT_HEALTH_INT", 0) / 100;
	}

	public static int GetHarvestCount(GameItem seed)
	{
		int baseHarvestQuantity = GetBaseHarvestQuantity(seed);
		GameItem associatedHydroponic = GetAssociatedHydroponic(seed);
		if (associatedHydroponic == null)
		{
			return baseHarvestQuantity;
		}
		int ajustedQuality = ModuleHelper.GetAjustedQuality(associatedHydroponic);
		return (int)Math.Round((double)baseHarvestQuantity + (double)baseHarvestQuantity * ((double)ajustedQuality / 100.0), MidpointRounding.AwayFromZero);
	}

	public static void StartGrowing(GameItem seed, GameItem hydroponic)
	{
		seed.EnableTag("GROWING_TAG");
		seed.ModifyTag("GROWING_PROGRESS", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		seed.ModifyTag("PLANT_HEALTH_INT", delegate(TagState state)
		{
			state.Enable().SetInt(GetStartingHealthPercent(seed) * 100);
		});
		seed.ModifyTag("ASSOCIATED_HYDROPONIC_ID", delegate(TagState state)
		{
			state.Enable().SetInt(hydroponic.GetUniqueID());
		});
		if (IsPerennial(seed))
		{
			seed.ModifyTag("PERENNIAL_ESTABLISHED_INT", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		StartGrowCycle(seed, hydroponic);
		PlantGeneticsHelper.RunStartGrowingHooks(seed);
	}

	private static void StartGrowCycle(GameItem seed, GameItem hydroponic)
	{
		int required = GetRequiredProgress(seed);
		seed.ModifyTag("REQUIRED_PROGRESS", delegate(TagState state)
		{
			state.Enable().SetInt(required);
		});
		seed.ModifyTag("PROGRESS_ITEM_TARGET_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(required);
		});
		seed.ModifyTag("GROWING_PROGRESS", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		MachineProgressHelper.StartProgressTypeMachine(hydroponic, seed);
		MachineHelper.LockModuleInv(hydroponic);
	}

	public static void ContinueGrowing(GameItem growingSeed, int progress, GameItem hydroponic)
	{
		int currentProgress = growingSeed.GetTagReadonly("GROWING_PROGRESS").GetInt();
		int maxProgress = growingSeed.GetTagReadonly("REQUIRED_PROGRESS").GetInt();
		for (int i = 0; i < progress; i++)
		{
			HandleNutriment(growingSeed);
			HandleContaminant(growingSeed);
		}
		MachineHydroponic.HandleWaterConsumption(hydroponic);
		HandleSmugglerModule(growingSeed);
		GetPlant(growingSeed)?.onGrowTick?.Invoke(growingSeed, hydroponic);
		PlantGeneticsHelper.RunGrowTickHooks(growingSeed, hydroponic);
		growingSeed.ModifyTag("GROWING_PROGRESS", delegate(TagState state)
		{
			state.Enable().SetInt(Math.Min(currentProgress + progress, maxProgress));
		});
		if (!MachineProgressHelper.IsProgressTypeMachineFinished(hydroponic))
		{
			return;
		}
		GameGridInventory outputSlot = MachineHydroponic.GetOutputSlot(hydroponic);
		GameItem childItem = MachineHydroponic.GetInputSlot(hydroponic).childItem;
		outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
		FinishGrowing(childItem);
		outputSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		if (childItem != null && IsPerennial(childItem))
		{
			StartNextFruitingCycle(childItem, hydroponic);
			return;
		}
		MachineProgressHelper.FinishProgressTypeMachine(hydroponic);
		if (childItem != null)
		{
			childItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => true;
			childItem.Destroy();
		}
	}

	public static void FinishGrowing(GameItem growingSeed)
	{
		if (growingSeed == null)
		{
			return;
		}
		GameItem associatedHydroponic = GetAssociatedHydroponic(growingSeed);
		if (associatedHydroponic == null)
		{
			return;
		}
		PlantDefinition plant = GetPlant(growingSeed);
		if (plant != null)
		{
			PlantHarvestContext plantHarvestContext = new PlantHarvestContext
			{
				plant = plant,
				seed = growingSeed,
				hydroponic = associatedHydroponic,
				output = MachineHydroponic.GetOutputSlot(associatedHydroponic),
				modules = MachineHelper.GetModules(associatedHydroponic),
				health = GetHealthPercent(growingSeed),
				harvestCount = GetHarvestCount(growingSeed)
			};
			PlantHarvest.isHarvesting = true;
			try
			{
				if (plant.onHarvest != null)
				{
					plant.onHarvest(plantHarvestContext);
				}
				else
				{
					PlantHarvest.DefaultHarvest(plantHarvestContext);
				}
			}
			finally
			{
				PlantHarvest.isHarvesting = false;
			}
		}
		MachineHelper.UnlockModuleInv(associatedHydroponic);
	}

	public static GameItem GetAssociatedHydroponic(GameItem seed)
	{
		if (seed == null || !seed.IsTag("GROWING_TAG"))
		{
			return null;
		}
		return EmporiumEntry.Instance.FindItemByUniqueID(seed.GetTagReadonly("ASSOCIATED_HYDROPONIC_ID").GetInt());
	}

	private static void HandleSmugglerModule(GameItem growingSeed)
	{
		GameItem associatedHydroponic = GetAssociatedHydroponic(growingSeed);
		if (associatedHydroponic == null)
		{
			return;
		}
		foreach (GameItem module in MachineHelper.GetModules(associatedHydroponic))
		{
			if (module.identifier.Equals("farm_module_smuggler"))
			{
				ContrabandHelper.RemoveContrabandStatus(growingSeed);
			}
		}
	}

	public static bool IsPerennial(GameItem seed)
	{
		if (seed != null)
		{
			if (!PlantGeneticsHelper.IsPerennial(seed))
			{
				return seed.IsTag("PERENNIAL_TAG");
			}
			return true;
		}
		return false;
	}

	public static bool IsPerennialEstablished(GameItem seed)
	{
		if (IsPerennial(seed) && seed.IsTag("PERENNIAL_ESTABLISHED_INT"))
		{
			return seed.GetTagReadonly("PERENNIAL_ESTABLISHED_INT").GetInt() != 0;
		}
		return false;
	}

	public static void StartNextFruitingCycle(GameItem seed, GameItem hydroponic)
	{
		seed.ModifyTag("PERENNIAL_ESTABLISHED_INT", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		seed.ModifyTag("PLANT_HEALTH_INT", delegate(TagState state)
		{
			state.Enable().SetInt(GetStartingHealthPercent(seed) * 100);
		});
		StartGrowCycle(seed, hydroponic);
	}

	public static bool IsRipening(GameItem seed)
	{
		int @int = seed.GetTagReadonly("REQUIRED_PROGRESS").GetInt();
		if (@int <= 0)
		{
			return false;
		}
		return seed.GetTagReadonly("GROWING_PROGRESS").GetInt() >= @int * 4 / 5;
	}

	public static void RemovePlantFromHydroponic(GameItem hydroponic)
	{
		if (hydroponic == null)
		{
			return;
		}
		try
		{
			GameItem gameItem = MachineHydroponic.GetInputSlot(hydroponic)?.childItem;
			if (gameItem != null)
			{
				MachineHelper.UnlockModuleInv(hydroponic);
				MachineProgressHelper.FinishProgressTypeMachine(hydroponic);
				gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => true;
				gameItem.Destroy();
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("RemovePlantFromHydroponic skipped a malformed hydroponic: " + ex.Message);
		}
	}

	private static bool CanHatchetRemovePlant(GameItem s, GameItem t)
	{
		if (t == null || !t.IsTag("HYDROPONIC_MACHINE_TAG"))
		{
			return false;
		}
		if (!GeneralHelper.IsItemOwned(s, t))
		{
			return false;
		}
		try
		{
			return MachineHydroponic.GetInputSlot(t)?.childItem != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void InitHatchetPlantRemoval(GameItem item)
	{
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanHatchetRemovePlant(s, t)));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t?.IsTag("HYDROPONIC_MACHINE_TAG") ?? false));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t != null && t.IsTag("HYDROPONIC_MACHINE_TAG"))
			{
				RemovePlantFromHydroponic(t);
			}
		});
	}

	public static void CreateSeedItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("SEED_ITEM_TAG"))
		{
			return;
		}
		PlantDefinition plant = GetPlant(item);
		bool showBase = plant != null && PlantGeneticsHelper.IsScannerOn();
		int baseValue = ((plant != null) ? (IsPerennialEstablished(item) ? GetRefruitBaseProgress(plant, plant.requiredProgress) : plant.requiredProgress) : 0);
		AddSeedStatLine(builder, LocHelper.GetLocalizedMechanic("mech_seed_required_work", GetRequiredProgress(item)), baseValue, showBase);
		AddSeedStatLine(builder, LocHelper.GetLocalizedMechanic("mech_seed_harvest_quantity", GetBaseHarvestQuantity(item)), plant?.harvestQuantity ?? 0, showBase);
		AddSeedStatLine(builder, LocHelper.GetLocalizedMechanic("mech_seed_water_use_rate", GetWaterUsePerCycle(item)), plant?.waterUseRate ?? 0, showBase);
		if (item.IsTag("GROWING_TAG"))
		{
			GameItem associatedHydroponic = GetAssociatedHydroponic(item);
			if (associatedHydroponic != null)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_seed_status", MachineHydroponic.GetGrowStatus(associatedHydroponic)), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			int @int = item.GetTagReadonly("GROWING_PROGRESS").GetInt();
			int int2 = item.GetTagReadonly("REQUIRED_PROGRESS").GetInt();
			builder.AddProgressBar((float)@int / (float)Math.Max(1, int2), 10, RenderHandler.ColorPalette.LightGreen, RenderHandler.ColorPalette.VeryDarkGreen, null, " ");
			builder.AddLine("(" + @int + "/" + int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
	}

	private static void AddSeedStatLine(RichTextBuilder builder, string effectiveText, int baseValue, bool showBase)
	{
		if (!showBase)
		{
			builder.AddLine(effectiveText, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			return;
		}
		builder.Add(effectiveText, RenderHandler.ColorPalette.LightCyan, " ");
		builder.AddLine("(Base: " + baseValue + ")", ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
	}

	public static int ApplyBonusModuleYield(int yield, GameItem system, GameInventory invModule)
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType(invModule, (GameItem item) => item.IsTag("MODULE_TAG")))
		{
			if (item.identifier.Equals("system_nutrifruit"))
			{
				yield++;
			}
		}
		return yield;
	}

	public static void HandleNutriment(GameItem growingSeed)
	{
		GameItem associatedHydroponic = GetAssociatedHydroponic(growingSeed);
		if (associatedHydroponic == null)
		{
			return;
		}
		GameItem waterSlotItem = MachineHydroponic.GetWaterSlotItem(associatedHydroponic);
		if (waterSlotItem == null)
		{
			return;
		}
		int nutrimentLevel = GetNutrimentLevel(growingSeed);
		int nutrimentRange = GetNutrimentRange(growingSeed);
		int num = nutrimentLevel - nutrimentRange;
		int num2 = nutrimentLevel + nutrimentRange;
		int contaminantPercentage = WaterHelper.GetContaminantPercentage(waterSlotItem, "mineral");
		if (contaminantPercentage < num)
		{
			growingSeed.ModifyTag("PLANT_HEALTH_INT", delegate(TagState state)
			{
				state.Enable().SetInt(state.GetInt() - 100);
			});
		}
		else if (contaminantPercentage > num2)
		{
			growingSeed.ModifyTag("PLANT_HEALTH_INT", delegate(TagState state)
			{
				state.Enable().SetInt(state.GetInt() - 50);
			});
		}
		WaterHelper.RemoveContaminantFromContainer(waterSlotItem, "mineral", GetMineralUseRate(growingSeed), 0);
	}

	public static void HandleContaminant(GameItem growingSeed)
	{
		GameItem associatedHydroponic = GetAssociatedHydroponic(growingSeed);
		if (associatedHydroponic != null)
		{
			GameItem waterSlotItem = MachineHydroponic.GetWaterSlotItem(associatedHydroponic);
			if (waterSlotItem != null)
			{
				double contaminantEffect = GetContaminantEffect(growingSeed);
				double contaminantSensitivity = GetContaminantSensitivity(growingSeed);
				ApplyContaminantEffect(growingSeed, waterSlotItem, "heavy_metal", contaminantEffect, contaminantSensitivity);
				ApplyContaminantEffect(growingSeed, waterSlotItem, "chemical_contaminant", contaminantEffect, contaminantSensitivity);
				ApplyContaminantEffect(growingSeed, waterSlotItem, "microbe", contaminantEffect, contaminantSensitivity);
				ApplyContaminantEffect(growingSeed, waterSlotItem, "organic_waste", contaminantEffect, contaminantSensitivity);
				ApplyContaminantEffect(growingSeed, waterSlotItem, "acid", contaminantEffect, contaminantSensitivity);
				ApplyContaminantEffect(growingSeed, waterSlotItem, "base", contaminantEffect, contaminantSensitivity);
			}
		}
	}

	private static void ApplyContaminantEffect(GameItem growingSeed, GameItem waterContainer, string contaminantId, double contaminantEffect, double sensitivity)
	{
		int num = (int)Math.Min(1000000.0, Math.Round((double)WaterHelper.GetContaminantPercentage(waterContainer, contaminantId) * sensitivity));
		int num2 = ((num <= 40) ? ((num <= 25) ? ((num <= 15) ? ((num > 10) ? 15 : 0) : ((num > 20) ? 30 : 20)) : ((num > 30) ? 60 : 40)) : ((num <= 100) ? ((num > 60) ? 160 : 80) : ((num > 200) ? 640 : 320)));
		int healthReduction = num2;
		healthReduction = (int)((double)healthReduction * contaminantEffect);
		if (healthReduction != 0)
		{
			growingSeed.ModifyTag("PLANT_HEALTH_INT", delegate(TagState state)
			{
				state.Enable().SetInt(state.GetInt() - healthReduction);
			});
		}
	}

	public static bool IsGrowingSeed(GameItem item)
	{
		if (item.IsTag("SEED_ITEM_TAG"))
		{
			return item.IsTag("GROWING_TAG");
		}
		return false;
	}
}
