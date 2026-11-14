using System.Collections.Generic;
using UnityEngine;

public static class PlantWaterQuality
{
	public const float PURIFYING_SHARE_PER_CYCLE = 0.1f;

	public const float PURIFYING_MAX_SHARE = 0.95f;

	public const int PURIFYING_MIN_REMOVAL_UL = 1000;

	public const float POLLUTING_SHARE_PER_CYCLE = 0.001f;

	private static readonly (string liquid, float share)[] POLLUTION = new(string, float)[3]
	{
		("organic_waste", 0.5f),
		("microbe", 0.35f),
		("physical_contaminant", 0.15f)
	};

	public static float PurifyingShare(int rank)
	{
		return Mathf.Min(0.95f, 0.1f * PlantGeneticsHelper.RankScale(rank));
	}

	public static float PollutingShare(int rank)
	{
		return 0.001f * PlantGeneticsHelper.RankScale(rank);
	}

	public static int PurifyingFloor(Liquid liquid, int rank)
	{
		return liquid.baseMaxPurityFromMachine / Mathf.Max(1, rank);
	}

	private static bool IsPlantFilterable(Liquid liquid)
	{
		if (liquid.canBeRemovedByPurifier && liquid.basePurifierRemoveAmount > 0)
		{
			return liquid.identifier != "mineral";
		}
		return false;
	}

	private static IEnumerable<Liquid> PlantFilterable()
	{
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (IsPlantFilterable(liquid))
			{
				yield return liquid;
			}
		}
	}

	public static void Purify(GameItem container, int rank)
	{
		if (container == null || rank <= 0 || !container.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int water = Part(container, "water");
		if (water <= 0)
		{
			return;
		}
		float num = PurifyingShare(rank);
		int removed = 0;
		foreach (Liquid item in PlantFilterable())
		{
			int num2 = Part(container, item.identifier);
			if (num2 > 0)
			{
				int volumeToRemove = Mathf.Max(1000, Mathf.RoundToInt((float)num2 * num));
				removed += WaterHelper.RemoveContaminantFromContainer(container, item.identifier, volumeToRemove, PurifyingFloor(item, rank));
			}
		}
		if (removed > 0)
		{
			container.ModifyTag(Liquid.GetCurrentPartFromId("water"), delegate(TagState state)
			{
				state.Enable().SetInt(water + removed);
			});
			WaterFeatureHelper.UpdateWaterFeature(container, isExpose: false);
		}
	}

	public static float PurifyingCeilingPercent(int rank)
	{
		int num = 0;
		foreach (Liquid item in PlantFilterable())
		{
			num += PurifyingFloor(item, rank);
		}
		return 100f - (float)num / 100f;
	}

	public static string DescribePurifying(int rank)
	{
		return PlantGeneticsText.EffectPurifying(Mathf.RoundToInt(PurifyingShare(rank) * 100f).ToString(), PurifyingCeilingPercent(rank).ToString("0.0#"));
	}

	public static void Pollute(GameItem container, int rank)
	{
		if (container == null || rank <= 0 || !container.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int water = Part(container, "water");
		if (water <= 0)
		{
			return;
		}
		int num = Mathf.RoundToInt((float)water * PollutingShare(rank));
		if (num <= 0)
		{
			return;
		}
		List<(string, int)> list = new List<(string, int)>();
		int moved = 0;
		(string, float)[] pOLLUTION = POLLUTION;
		for (int i = 0; i < pOLLUTION.Length; i++)
		{
			(string, float) tuple = pOLLUTION[i];
			string item = tuple.Item1;
			float item2 = tuple.Item2;
			int num2 = Mathf.Min((int)((float)num * item2), water - moved);
			if (num2 > 0)
			{
				list.Add((item, num2));
				moved += num2;
			}
		}
		if (moved <= 0)
		{
			return;
		}
		container.ModifyTag(Liquid.GetCurrentPartFromId("water"), delegate(TagState state)
		{
			state.Enable().SetInt(water - moved);
		});
		foreach (var (liquidId, addAmount) in list)
		{
			WaterHelper.AddLiquid(container, liquidId, addAmount);
		}
		WaterFeatureHelper.UpdateWaterFeature(container, isExpose: false);
	}

	public static string DescribePolluting(int rank)
	{
		return PlantGeneticsText.EffectPolluting((PollutingShare(rank) * 100f).ToString("0.##"));
	}

	private static int Part(GameItem container, string liquidId)
	{
		return container.GetTagReadonly(Liquid.GetCurrentPartFromId(liquidId)).GetInt();
	}
}
