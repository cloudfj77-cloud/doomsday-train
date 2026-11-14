using System;

public static class PlantHarvest
{
	public struct SizeSplit
	{
		public int small;

		public int normal;

		public int large;

		public int huge;

		public int Total => small + normal + large + huge;
	}

	public static bool isHarvesting;

	public const int HEALTH_ALL_SMALL = 15;

	public const int HEALTH_MOSTLY_SMALL = 30;

	public const int HEALTH_SOME_SMALL = 50;

	public const int HEALTH_ALL_NORMAL = 70;

	public const int HEALTH_SOME_LARGE = 80;

	public const int HEALTH_MOSTLY_LARGE = 90;

	public const int HEALTH_ALL_LARGE = 95;

	public const int HEALTH_SOME_HUGE = 99;

	public const int HUGE_QUALITY_THRESHOLD = 50;

	public static SizeSplit SplitByHealth(int count, int health, bool reduceCount, bool allowHuge, int qualityBonus = 0)
	{
		SizeSplit result = default(SizeSplit);
		if (health <= 15)
		{
			if (reduceCount)
			{
				count = RNG.MultiplyAndRound(count, 0.5);
			}
			result.small = count;
		}
		else if (health <= 30)
		{
			if (reduceCount)
			{
				count = RNG.MultiplyAndRound(count, 0.65);
			}
			result.small = (int)((double)count * 0.7);
			result.normal = count - result.small;
		}
		else if (health <= 50)
		{
			if (reduceCount)
			{
				count = RNG.MultiplyAndRound(count, 0.8);
			}
			result.small = (int)((double)count * 0.3);
			result.normal = count - result.small;
		}
		else if (health <= 70)
		{
			if (reduceCount)
			{
				count = RNG.MultiplyAndRound(count, 0.9);
			}
			result.normal = count;
		}
		else if (health <= 80)
		{
			result.normal = (int)((double)count * 0.8);
			result.large = count - result.normal;
		}
		else if (health <= 90)
		{
			result.normal = (int)((double)count * 0.3);
			result.large = count - result.normal;
		}
		else if (!allowHuge || health <= 95)
		{
			result.large = count;
		}
		else if (health <= 99 && qualityBonus <= 50)
		{
			result.large = (int)((double)count * 0.5);
			result.huge = count - result.large;
		}
		else
		{
			result.huge = Math.Max(count, 0);
		}
		return result;
	}

	public static void OutputSplit(PlantHarvestContext ctx, SizeSplit split, Func<GameItem> small, Func<GameItem> normal, Func<GameItem> large, Func<GameItem> huge = null)
	{
		for (int i = 0; i < split.small; i++)
		{
			ctx.Output(small());
		}
		for (int j = 0; j < split.normal; j++)
		{
			ctx.Output(normal());
		}
		for (int k = 0; k < split.large; k++)
		{
			ctx.Output(large());
		}
		if (huge != null)
		{
			for (int l = 0; l < split.huge; l++)
			{
				ctx.Output(huge());
			}
		}
	}

	public static void DefaultHarvest(PlantHarvestContext ctx)
	{
		ctx.OutputById(ctx.plant.harvestItemId, Math.Max(1, ctx.harvestCount));
	}

	public static void Output(PlantHarvestContext ctx, GameItem item)
	{
		if (item == null)
		{
			return;
		}
		string plantId = HydroponicHelper.GetPlantId(ctx.seed);
		if (item.IsTag("SEED_ITEM_TAG") && HydroponicHelper.GetPlantId(item).Equals(plantId))
		{
			OutputOwnSeed(ctx, item, plantId);
			return;
		}
		if (!item.IsTag("SEED_ITEM_TAG"))
		{
			PlantGeneticsHelper.RunHarvestHooks(ctx.seed, item, ctx);
		}
		Deliver(ctx, item);
	}

	private static void Deliver(PlantHarvestContext ctx, GameItem item)
	{
		if (GraphUtils.TryAcceptAll(ctx.output, item) > 0 && !HopperHelper.TryRouteOverflow(ctx.output, item))
		{
			Debug.LogWarning("PlantHarvest: output grid full, " + item.identifier + " spilled into the ship inventory");
			GraphUtils.EmporiumTryAdd(item);
		}
	}

	private static void OutputOwnSeed(PlantHarvestContext ctx, GameItem seed, string plantId)
	{
		int num = SeedCopies(ctx.seed);
		if (num <= 0)
		{
			seed.Destroy();
			return;
		}
		bool flag = ctx.plant != null && ctx.plant.HarvestIsOwnSeed;
		for (int i = 0; i < num; i++)
		{
			GameItem gameItem = ((i == 0) ? seed : DirectoryMaster.Item(plantId));
			if (gameItem != null)
			{
				PlantGeneticsHelper.InheritTraits(ctx.seed, gameItem);
				if (flag)
				{
					PlantGeneticsHelper.RunHarvestHooks(ctx.seed, gameItem, ctx, seedProduceOnly: true);
				}
				Deliver(ctx, gameItem);
			}
		}
	}

	private static int SeedCopies(GameItem parentSeed)
	{
		float statMultiplier = PlantGeneticsHelper.GetStatMultiplier(parentSeed, "SEED_YIELD");
		int num = (int)statMultiplier;
		float num2 = statMultiplier - (float)num;
		return num + (RNG.Roll(num2 * 100f) ? 1 : 0);
	}
}
