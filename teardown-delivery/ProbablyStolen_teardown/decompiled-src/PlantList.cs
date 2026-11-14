using System;

public static class PlantList
{
	public const string NUTRIFRUIT_SEED = "nutrifruit_seed";

	public const string KOTTON_SEED = "kotton_seed";

	public const string DREAM_CAP_SEED = "dream_cap_seed";

	public const string COFFEE_BEAN = "coffee_bean";

	public const string FILLERWEED_SEED = "fillerweed_seed";

	public const string HYDROREED_SEED = "hydroreed_seed";

	public const string BLOOMBERRY_SEED = "bloomberry_seed";

	public const int FILLERWEED_MODULE_BONUS = 4;

	public const int NUTRIFRUIT_SEED_RETURN = 3;

	public const int NUTRIFRUIT_INFERTILE_RANK = 10;

	public const string NUTRIFRUIT_SEED_ATLAS = "Items/items_hydroponic_plant";

	public const string NUTRIFRUIT_FREED_SEED_SPRITE = "nutrifruit_modified_seed";

	public const int NUTRIFRUIT_SEED_VALUE = 40;

	public const int NUTRIFRUIT_FREED_SEED_VALUE = 8;

	public const int NUTRIFRUIT_OLD_SEED_VALUE = 4;

	public const int KOTTON_SECOND_SEED_CHANCE = 50;

	public static void Register()
	{
		PlantRegistry.Add(new PlantDefinition("nutrifruit_seed", "Nutrifruit", "nutrifruit", 23, 5).Nutriment(150, 100, 50).Water(100).ReturnsSeeds()
			.Innate("complex_genome", 2)
			.Innate("fast_grower", 2)
			.Innate("nutritious")
			.Innate("bountiful", 2)
			.Innate("hardy")
			.Innate("infertile", 10)
			.OnHarvest(HarvestNutrifruit));
		PlantRegistry.Add(new PlantDefinition("kotton_seed", "Kotton", "kotton_fiber", 32, 9).Nutriment(350, 150, 75).Contaminant(1.26).Water(176)
			.ReturnsSeeds()
			.Innate("bountiful", 3)
			.Innate("thirsty", 2)
			.Innate("frail", 2)
			.Innate("slow_grower")
			.OnHarvest(HarvestKotton));
		PlantRegistry.Add(new PlantDefinition("dream_cap_seed", "Dream Cap", "dream_cap", 28, 8).Nutriment(250, 75, 30).Contaminant(1.01).ReturnsSeeds()
			.Innate(PlantTraitList.ProductionId("dream_cap_mush"))
			.Innate("weak")
			.Innate("fertile")
			.OnGrowTick(TickDreamCap)
			.OnHarvest(HarvestDreamCap));
		PlantRegistry.Add(new PlantDefinition("coffee_bean", "Coffee", "coffee_bean", 21, 8).Nutriment(250, 75, 30).Contaminant(1.01).Innate(PlantTraitList.ProductionId("caffeine"))
			.Innate(PlantTraitList.ProductionId("coffee_mush"))
			.Innate("potent")
			.OnHarvest(HarvestCoffeeBean));
		PlantRegistry.Add(new PlantDefinition("fillerweed_seed", "Fillerweed", "fillerweed", 11, 4).Nutriment(100, 100, 75).Contaminant(0.0).Water(287)
			.ReturnsSeeds()
			.Innate("simple_genome", 2)
			.Innate("edible")
			.Innate("unappetizing")
			.Innate("hardy", 3)
			.Innate("bountiful", 2)
			.Innate("fast_grower", 2)
			.Innate("thirsty", 3)
			.Innate("nutrient_poor", 2)
			.OnHarvest(HarvestFillerweed));
		PlantRegistry.Add(new PlantDefinition("hydroreed_seed", "Hydroreed", "hydroreed", 18, 0).Nutriment(100, 100, 25).Contaminant(0.0).Water(20)
			.ReturnsSeeds()
			.DurableProduce()
			.Innate("purifying", 4)
			.Innate("dense")
			.Innate("hardy")
			.OnGrowTick(TickHydroreed)
			.OnHarvest(HarvestHydroreed));
		PlantRegistry.Add(new PlantDefinition("bloomberry_seed", "Bloomberry", "bloomberry", 18, 8).Nutriment(200, 75, 50).Contaminant(1.25).Refruit(10)
			.Innate("perennial")
			.Innate("edible")
			.OnHarvest(HarvestBloomberry));
	}

	public static bool IsNutrifruitFreed()
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.storeClientData != null)
		{
			return PlayerStore.Instance.storeClientData.nutrifruitSeedsFreed;
		}
		return false;
	}

	public static void RefreshNutrifruitStrain(GameItem seed)
	{
		if (seed != null && !(HydroponicHelper.GetPlantId(seed) != "nutrifruit_seed"))
		{
			bool flag = PlantGeneticsHelper.GetTraitRank(seed, "infertile") > 0;
			string text = ((IsNutrifruitFreed() && !flag) ? "nutrifruit_modified_seed" : "nutrifruit_seed");
			if (seed.spritePath != text)
			{
				seed.SetSpriteAndShape("Items/items_hydroponic_plant", text);
			}
			if (!IsNutrifruitFreed())
			{
				seed.unitValue = 40L;
			}
			else
			{
				seed.unitValue = (flag ? 4 : 8);
			}
		}
	}

	public static void FreeNutrifruit()
	{
		if (PlayerStore.Instance == null)
		{
			return;
		}
		PlayerStore.Instance.storeClientData.nutrifruitSeedsFreed = true;
		foreach (GameItem allItem in EmporiumEntry.Instance.GetAllItems())
		{
			RefreshNutrifruitStrain(allItem);
		}
	}

	private static void HarvestNutrifruit(PlantHarvestContext ctx)
	{
		ctx.OutputById("nutrifruit_seed", 3);
		PlantHarvest.SizeSplit split = PlantHarvest.SplitByHealth(ctx.harvestCount + ctx.CountModule("farm_module_nutrifruit"), qualityBonus: ModuleHelper.GetAjustedQuality(ctx.hydroponic), health: ctx.health, reduceCount: false, allowHuge: true);
		PlantHarvest.OutputSplit(ctx, split, PreBuiltItemHelper.CreateNutrifruitSmall, PreBuiltItemHelper.CreateNutrifruitNormal, PreBuiltItemHelper.CreateNutrifruitLarge, delegate
		{
			AchievementTracker.Instance.CheckHugeNutrifruit();
			return PreBuiltItemHelper.CreateNutrifruitHuge();
		});
	}

	public static int KottonSeedReturn(int health)
	{
		if (health <= 50)
		{
			return 1;
		}
		if (health <= 80)
		{
			if (!RNG.Roll(50))
			{
				return 1;
			}
			return 2;
		}
		return 2;
	}

	private static void HarvestKotton(PlantHarvestContext ctx)
	{
		PlantHarvest.SizeSplit split = PlantHarvest.SplitByHealth(ctx.harvestCount, ctx.health, reduceCount: true, allowHuge: false);
		ctx.OutputById("kotton_seed", KottonSeedReturn(ctx.health));
		if (ctx.HasModule("farm_module_kotton_loom"))
		{
			ctx.OutputById("kotton_fabric", split.Total);
		}
		else
		{
			PlantHarvest.OutputSplit(ctx, split, PreBuiltItemHelper.CreateSmallKotton, PreBuiltItemHelper.CreateKotton, PreBuiltItemHelper.CreateLargeKotton);
		}
	}

	private static void TickDreamCap(GameItem seed, GameItem hydroponic)
	{
		foreach (GameItem module in MachineHelper.GetModules(hydroponic))
		{
			if (module.identifier.Equals("farm_module_dream_cap"))
			{
				ContrabandHelper.RemoveContrabandStatus(seed);
			}
		}
	}

	private static void HarvestDreamCap(PlantHarvestContext ctx)
	{
		PlantHarvest.SizeSplit split = PlantHarvest.SplitByHealth(ctx.harvestCount + ctx.CountModule("farm_module_dream_cap") * 2, ctx.health, reduceCount: true, allowHuge: false);
		ctx.OutputById("dream_cap_seed");
		bool isComestible = ctx.HasModule("farm_module_dream_cap");
		bool flag = ctx.HasModule("farm_module_nightmare_cap");
		if (!isComestible && flag)
		{
			ctx.OutputById("nightmare_cap", split.Total);
			return;
		}
		PlantHarvest.OutputSplit(ctx, split, () => PreBuiltItemHelper.CreateSmallDreamCap(isComestible), () => PreBuiltItemHelper.CreateDreamCap(isComestible), () => PreBuiltItemHelper.CreateLargeDreamCap(isComestible));
	}

	private static void HarvestCoffeeBean(PlantHarvestContext ctx)
	{
		int num = ctx.harvestCount;
		if (ctx.health <= 15)
		{
			num = RNG.MultiplyAndRound(num, 0.5);
		}
		else if (ctx.health <= 30)
		{
			num = RNG.MultiplyAndRound(num, 0.6);
		}
		else if (ctx.health <= 50)
		{
			num = RNG.MultiplyAndRound(num, 0.7);
		}
		else if (ctx.health <= 70)
		{
			num = RNG.MultiplyAndRound(num, 0.8);
		}
		ctx.OutputById("coffee_bean", Math.Max(1, num));
	}

	private static void HarvestFillerweed(PlantHarvestContext ctx)
	{
		bool flag = ctx.HasModule("farm_module_fillerweed_production");
		int num = ctx.harvestCount;
		if (flag)
		{
			num += 4;
		}
		for (int i = 0; i < Math.Max(1, num - 1); i++)
		{
			ctx.Output(PreBuiltItemHelper.CreateFillerweed());
		}
		ctx.Output(PreBuiltItemHelper.CreateFillerweedSmall());
		if (!flag)
		{
			ctx.OutputById("fillerweed_seed");
			if (RNG.Roll(50))
			{
				ctx.OutputById("fillerweed_seed");
			}
		}
	}

	private static void TickHydroreed(GameItem seed, GameItem hydroponic)
	{
		seed.SetSpriteAndShape("Items/items_hydroponic_plant", "hydroreed_p");
	}

	private static void HarvestHydroreed(PlantHarvestContext ctx)
	{
		ctx.OutputById("hydroreed_filter", ctx.CountModule("farm_module_hydroreed_filter"));
		ctx.OutputById("hydroreed_seed");
		if (RNG.Roll(50))
		{
			ctx.OutputById("hydroreed_seed");
		}
	}

	private static void HarvestBloomberry(PlantHarvestContext ctx)
	{
		for (int i = 0; i < Math.Max(1, ctx.harvestCount); i++)
		{
			GameItem item = DirectoryMaster.Item("wine_berry");
			int num = RollBloomberryCondition(ctx.health);
			int visualVariant = ((num == -1) ? RNG.GetRandomInt(0, 2) : 0);
			WineHelper.SetBerryCondition(item, num, visualVariant);
			ctx.Output(item);
		}
	}

	private static int RollBloomberryCondition(int score)
	{
		int randomInt = RNG.GetRandomInt(0, 99);
		if (score <= 25)
		{
			if (randomInt >= 55)
			{
				return -1;
			}
			return -2;
		}
		if (score <= 55)
		{
			if (randomInt >= 25)
			{
				if (randomInt >= 70)
				{
					return 1;
				}
				return -1;
			}
			return -2;
		}
		if (score <= 85)
		{
			if (randomInt >= 25)
			{
				if (randomInt >= 70)
				{
					return 2;
				}
				return 1;
			}
			return -1;
		}
		if (randomInt >= 40)
		{
			return 2;
		}
		return 1;
	}
}
