using UnityEngine;

public static class PlantTraitList
{
	public const string EDIBLE = "edible";

	public const string FAST_GROWER = "fast_grower";

	public const string SLOW_GROWER = "slow_grower";

	public const string BOUNTIFUL = "bountiful";

	public const string SPARSE = "sparse";

	public const string DELICIOUS = "delicious";

	public const string UNAPPETIZING = "unappetizing";

	public const string DROUGHT_RESISTANT = "drought_resistant";

	public const string THIRSTY = "thirsty";

	public const string HARDY = "hardy";

	public const string FRAIL = "frail";

	public const string NUTRITIOUS = "nutritious";

	public const string NUTRIENT_POOR = "nutrient_poor";

	public const string FERTILE = "fertile";

	public const string INFERTILE = "infertile";

	public const string SIMPLE_GENOME = "simple_genome";

	public const string COMPLEX_GENOME = "complex_genome";

	public const string PERENNIAL = "perennial";

	public const string PURIFYING = "purifying";

	public const string POLLUTING = "polluting";

	public const string DENSE = "dense";

	public const string POTENT = "potent";

	public const string WEAK = "weak";

	public const string PRODUCTION_PREFIX = "production_";

	public const string GROUP_GROWTH = "growth";

	public const string GROUP_YIELD = "yield";

	public const string GROUP_TASTE = "taste";

	public const string GROUP_WATER = "water";

	public const string GROUP_RESILIENCE = "resilience";

	public const string GROUP_NUTRITION = "nutrition";

	public const string GROUP_SEEDS = "seeds";

	public const string GROUP_GENOME = "genome";

	public const string GROUP_WATER_QUALITY = "water_quality";

	public const string GROUP_FIBRE = "fibre";

	public const float DENSE_PERCENT = 25f;

	public const string GROUP_POTENCY = "potency";

	public const float POTENT_PERCENT = 25f;

	public const float PRODUCTION_CAFFEINE_ML = 2f;

	public const float PRODUCTION_COFFEE_MUSH_ML = 10f;

	public const float PRODUCTION_DREAM_CAP_MUSH_ML = 2f;

	public const int DREAM_CAP_CONTRABAND_LEVEL = 2;

	public const string NEUTRALIZER_MODULE_ID = "farm_module_dream_cap";

	public const string NIGHTMARE_MODULE_ID = "farm_module_nightmare_cap";

	public const int GENOME_COMPLEXITY_PER_RANK = 4;

	public const int EDIBLE_CALORIES = 100;

	public const string TASTE_FEATURE_ID = "plant_taste";

	public const string EDIBLE_PRODUCE_TAG = "PLANT_EDIBLE_PRODUCE_TAG";

	public const string TASTE_PERCENT_INT = "PLANT_TASTE_PERCENT_INT";

	public const string TASTE_DISPLAY_STRING = "PLANT_TASTE_DISPLAY_STRING";

	public const float FAST_GROWER_PERCENT = 25f;

	public const float BOUNTIFUL_PERCENT = 20f;

	public const float DELICIOUS_PRICE_PERCENT = 15f;

	public const float DROUGHT_RESISTANT_PERCENT = -25f;

	public const float HARDY_PERCENT = -30f;

	public const float NUTRITIOUS_PERCENT = 25f;

	public const float FERTILE_PERCENT = 25f;

	public const float PERENNIAL_HARVEST_PERCENT = -25f;

	public const float PERENNIAL_GROWTH_PERCENT = -20f;

	public const float PERENNIAL_SEED_PERCENT = -50f;

	public static string ProductionId(string liquidId)
	{
		return "production_" + liquidId;
	}

	public static void Register()
	{
		PlantTraitRegistry.Add(new PlantTrait("edible", "Edible", 2, "Everything this plant produces can be eaten.").Exclusive("edible").OnHarvestItem(MakeEdible));
		PlantTraitRegistry.Add(new PlantTrait("nutritious", "Nutritious", 3, "Produce carries more calories.").Stat("CALORIES", 25f).Unlimited().Exclusive("nutrition")
			.OnHarvestItem(ScaleCalories));
		PlantTraitRegistry.Add(new PlantTrait("nutrient_poor", "Nutrient-Poor", -2, "Produce carries fewer calories.").Stat("CALORIES", -25f).Unlimited().Exclusive("nutrition")
			.OnHarvestItem(ScaleCalories));
		PlantTraitRegistry.Add(new PlantTrait("fast_grower", "Fast Grower", 4, "Grows faster.").Stat("GROWTH_SPEED", 25f).Unlimited().Exclusive("growth"));
		PlantTraitRegistry.Add(new PlantTrait("slow_grower", "Slow Grower", -3, "Grows slower.").Stat("GROWTH_SPEED", -25f).Unlimited().Exclusive("growth"));
		PlantTraitRegistry.Add(new PlantTrait("bountiful", "Bountiful", 4, "Yields more produce.").Stat("HARVEST_QUANTITY", 20f).Unlimited().Exclusive("yield"));
		PlantTraitRegistry.Add(new PlantTrait("sparse", "Sparse", -3, "Yields less produce.").Stat("HARVEST_QUANTITY", -20f).Unlimited().Exclusive("yield"));
		PlantTraitRegistry.Add(new PlantTrait("delicious", "Delicious", 3, "Edible produce sells for more.").Unlimited().Exclusive("taste").Describe((int rank) => PlantGeneticsText.EffectSellPrice(SignedPercent(PlantGeneticsHelper.RankedPercent(15f, rank))))
			.OnHarvestItem(delegate(PlantTraitContext ctx)
			{
				AddTasteFeature(ctx, "delicious", 15f);
			}));
		PlantTraitRegistry.Add(new PlantTrait("unappetizing", "Unappetizing", -2, "Edible produce sells for less.").Unlimited().Exclusive("taste").Describe((int rank) => PlantGeneticsText.EffectSellPrice(SignedPercent(PlantGeneticsHelper.RankedPercent(-15f, rank))))
			.OnHarvestItem(delegate(PlantTraitContext ctx)
			{
				AddTasteFeature(ctx, "unappetizing", -15f);
			}));
		PlantTraitRegistry.Add(new PlantTrait("drought_resistant", "Drought Resistant", 2, "Drinks less water.").Stat("WATER_USE", -25f).Unlimited().Exclusive("water"));
		PlantTraitRegistry.Add(new PlantTrait("thirsty", "Thirsty", -1, "Drinks more water.").Stat("WATER_USE", 25f).Unlimited().Exclusive("water"));
		PlantTraitRegistry.Add(new PlantTrait("hardy", "Hardy", 3, "Less sensitive to contaminants in the water.").Stat("CONTAMINANT_SENSITIVITY", -30f).Unlimited().Exclusive("resilience"));
		PlantTraitRegistry.Add(new PlantTrait("frail", "Frail", -2, "More sensitive to contaminants in the water.").Stat("CONTAMINANT_SENSITIVITY", 30f).Unlimited().Exclusive("resilience"));
		PlantTraitRegistry.Add(new PlantTrait("fertile", "Fertile", 2, "Gives back more seeds when harvested.").Stat("SEED_YIELD", 25f).Unlimited().Exclusive("seeds")
			.RequiresCrop((PlantDefinition plant) => plant.returnsSeeds));
		PlantTraitRegistry.Add(new PlantTrait("infertile", "Infertile", -1, "Gives back fewer seeds when harvested.").Stat("SEED_YIELD", -25f).Unlimited().Exclusive("seeds")
			.RequiresCrop((PlantDefinition plant) => plant.returnsSeeds));
		PlantTraitRegistry.Add(new PlantTrait("simple_genome", "Simple Genome", 0, "An uncomplicated genome: edits are easier.").ComplexityOffset(-4).Unlimited().Exclusive("genome")
			.Locked());
		PlantTraitRegistry.Add(new PlantTrait("complex_genome", "Complex Genome", 0, "A tangled genome: edits are harder.").ComplexityOffset(4).Unlimited().Exclusive("genome")
			.Locked());
		PlantTraitRegistry.Add(new PlantTrait("perennial", "Perennial", 5, "Survives its harvest and fruits again.").Perennial().Stat("HARVEST_QUANTITY", -25f).Stat("GROWTH_SPEED", -20f)
			.Stat("SEED_YIELD", -50f)
			.IncompatibleWith("infertile"));
		PlantTraitRegistry.Add(new PlantTrait("purifying", "Purifying", 3, "Cleans the water it grows in.").Unlimited().Exclusive("water_quality").OnGrowTick(delegate(GameItem seed, GameItem hydroponic, int rank)
		{
			PlantWaterQuality.Purify(MachineHydroponic.GetWaterSlotItem(hydroponic), rank);
		})
			.Describe(PlantWaterQuality.DescribePurifying));
		PlantTraitRegistry.Add(new PlantTrait("polluting", "Polluting", -2, "Fouls the water it grows in.").Unlimited().Exclusive("water_quality").OnGrowTick(delegate(GameItem seed, GameItem hydroponic, int rank)
		{
			PlantWaterQuality.Pollute(MachineHydroponic.GetWaterSlotItem(hydroponic), rank);
		})
			.Describe(PlantWaterQuality.DescribePolluting));
		PlantTraitRegistry.Add(new PlantTrait("dense", "Dense", 3, "Harvested items last longer.").Stat("DURABILITY", 25f).Unlimited().Exclusive("fibre")
			.RequiresCrop((PlantDefinition plant) => plant.producesDurableGoods)
			.OnHarvestItem(ScaleDurability));
		PlantTraitRegistry.Add(Production("caffeine", 2f, 3));
		PlantTraitRegistry.Add(Production("coffee_mush", 10f, 2));
		PlantTraitRegistry.Add(Production("dream_cap_mush", 2f, 4, "farm_module_dream_cap", 2).ConvertedBy("farm_module_nightmare_cap", "nightmare_cap_mush"));
		PlantTraitRegistry.Add(new PlantTrait("potent", "Potent", 3, "More of every active ingredient.").Unlimited().Exclusive("potency").AppliesToSeedProduce()
			.RequiresSeed(PlantGeneticsHelper.HasActiveLiquid)
			.OnHarvestItem(delegate(PlantTraitContext ctx)
			{
				ScaleActives(ctx, 25f);
			})
			.Describe((int rank) => PlantGeneticsText.EffectActiveIngredients(SignedPercent(PlantGeneticsHelper.RankedPercent(25f, rank)))));
		PlantTraitRegistry.Add(new PlantTrait("weak", "Weak", -2, "Less of every active ingredient.").Unlimited().Exclusive("potency").AppliesToSeedProduce()
			.RequiresSeed(PlantGeneticsHelper.HasActiveLiquid)
			.OnHarvestItem(delegate(PlantTraitContext ctx)
			{
				ScaleActives(ctx, -25f);
			})
			.Describe((int rank) => PlantGeneticsText.EffectActiveIngredients(SignedPercent(PlantGeneticsHelper.RankedPercent(-25f, rank)))));
	}

	private static PlantTrait Production(string liquidId, float mlAtRankOne, int complexity, string neutralizerModuleId = null, int contrabandLevel = 0)
	{
		return new PlantTrait(ProductionId(liquidId), liquidId + " Production", complexity).Named(() => PlantGeneticsText.ProductionName(LiquidName()), () => PlantGeneticsText.ProductionDescription(LiquidName())).Infuses(liquidId).Unlimited()
			.AppliesToSeedProduce()
			.OnHarvestItem(delegate(PlantTraitContext ctx)
			{
				Infuse(ctx, liquidId, mlAtRankOne, neutralizerModuleId, contrabandLevel);
			})
			.Describe((int rank) => (contrabandLevel <= 0) ? PlantGeneticsText.EffectProduction(ProducedMl(mlAtRankOne, rank).ToString("0.#"), LiquidName()) : PlantGeneticsText.EffectProductionContraband(ProducedMl(mlAtRankOne, rank).ToString("0.#"), LiquidName()));
		string LiquidName()
		{
			Liquid liquidFromId = Liquid.GetLiquidFromId(liquidId);
			if (liquidFromId == null)
			{
				return liquidId;
			}
			return liquidFromId.displayName;
		}
	}

	private static float ProducedMl(float mlAtRankOne, int rank)
	{
		return mlAtRankOne * PlantGeneticsHelper.RankScale(rank);
	}

	private static string SignedPercent(float percent)
	{
		int num = Mathf.RoundToInt(percent);
		return ((num >= 0) ? "+" : "") + num + "%";
	}

	private static void Infuse(PlantTraitContext ctx, string liquidId, float mlAtRankOne, string neutralizerModuleId, int contrabandLevel)
	{
		GameItem item = ctx.item;
		if (neutralizerModuleId != null && ctx.HasModule(neutralizerModuleId))
		{
			return;
		}
		foreach (var (moduleId, text) in ctx.trait.conversions)
		{
			if (ctx.HasModule(moduleId))
			{
				liquidId = text;
				break;
			}
		}
		if (!item.IsTag("CHEM_ITEM_TAG"))
		{
			ChemicalHelper.InitChemItem(item, isBlendable: true);
		}
		ChemicalHelper.AddChemContentUL(item, liquidId, Mathf.RoundToInt(ProducedMl(mlAtRankOne, ctx.rank) * 1000f));
		if (contrabandLevel > 0 && !item.IsTag("CONTRABAND_ITEM_TAG"))
		{
			ContrabandHelper.InitContrabandItem(item, contrabandLevel);
		}
	}

	private static void ScaleActives(PlantTraitContext ctx, float percentAtRankOne)
	{
		float multiplier = 1f + ctx.RankedPercent(percentAtRankOne) / 100f;
		foreach (string activeLiquid in ctx.activeLiquids)
		{
			string currentPartFromId = Liquid.GetCurrentPartFromId(activeLiquid);
			if (ctx.item.IsTag(currentPartFromId))
			{
				ctx.item.ModifyTag(currentPartFromId, delegate(TagState state)
				{
					state.Enable().SetInt(Mathf.RoundToInt((float)state.GetInt() * multiplier));
				});
			}
		}
	}

	private static void ScaleDurability(PlantTraitContext ctx)
	{
		GameItem item = ctx.item;
		if (item.IsTag("DURABILITY_TAG"))
		{
			float num = 1f + ctx.trait.GetStatPercent("DURABILITY", ctx.rank) / 100f;
			int num2 = Mathf.Max(1, Mathf.RoundToInt((float)DurabilityHelper.GetMaxDurability(item) * num));
			DurabilityHelper.SetDurability(item, num2, num2, num2);
		}
	}

	private static void MakeEdible(PlantTraitContext ctx)
	{
		GameItem item = ctx.item;
		if (!item.IsGameItemType("FOOD"))
		{
			item.EnableTag("PLANT_EDIBLE_PRODUCE_TAG");
			item.SetGameItemType("FOOD");
			FoodItemHelper.InitFoodItem(item, 100, isCanned: false);
		}
	}

	private static void ScaleCalories(PlantTraitContext ctx)
	{
		GameItem item = ctx.item;
		if (item.IsTag("CALORIE_VALUE_TAG"))
		{
			float multiplier = 1f + ctx.trait.GetStatPercent("CALORIES", ctx.rank) / 100f;
			item.ModifyTag("CALORIE_VALUE_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(Mathf.Max(1, Mathf.RoundToInt((float)state.GetInt() * multiplier)));
			});
		}
	}

	public static void LoadEdibleProduce(GameItem item)
	{
		if (item.IsTag("PLANT_EDIBLE_PRODUCE_TAG"))
		{
			item.SetGameItemType("FOOD");
			if (!item.IsTag("CALORIE_VALUE_TAG"))
			{
				FoodItemHelper.InitFoodItem(item, 100, isCanned: false);
			}
		}
	}

	private static void AddTasteFeature(PlantTraitContext ctx, string traitId, float percentAtRankOne)
	{
		GameItem item = ctx.item;
		if (!item.IsGameItemType("FOOD"))
		{
			return;
		}
		int percent = Mathf.RoundToInt(ctx.RankedPercent(percentAtRankOne));
		if (percent != 0)
		{
			item.ModifyTag("PLANT_TASTE_PERCENT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(percent);
			});
			item.ModifyTag("PLANT_TASTE_DISPLAY_STRING", delegate(TagState state)
			{
				state.Enable().SetString(traitId);
			});
			SetTasteFeature(item, TasteDisplay(traitId), percent);
		}
	}

	private static string TasteDisplay(string stored)
	{
		PlantTrait plantTrait = PlantTraitRegistry.Get(stored);
		if (plantTrait == null)
		{
			return stored;
		}
		return plantTrait.displayName;
	}

	public static void LoadTasteFeature(GameItem item)
	{
		if (!item.IsTag("PLANT_TASTE_PERCENT_INT"))
		{
			item.RemoveItemFeatureByID("plant_taste");
		}
		else
		{
			SetTasteFeature(item, TasteDisplay(item.GetTagReadonly("PLANT_TASTE_DISPLAY_STRING").GetString()), item.GetTagReadonly("PLANT_TASTE_PERCENT_INT").GetInt());
		}
	}

	private static void SetTasteFeature(GameItem item, string display, int percent)
	{
		item.RemoveItemFeatureByID("plant_taste");
		ItemFeature itemFeature = new ItemFeature(ItemFeature.FeatureType.Normale);
		itemFeature.identifier = "plant_taste";
		itemFeature.category = "plant_taste";
		itemFeature.valueStage = ItemFeature.ValueStage.Innate;
		itemFeature.SetPublicDisplay(display);
		itemFeature.SetActualDisplay(display);
		itemFeature.isFeatureMatch = true;
		itemFeature.SetValueModifier(percent);
		itemFeature.SetPreExposedValueMod(percent);
		item.AddItemFeature(itemFeature);
	}
}
