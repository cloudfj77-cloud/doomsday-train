using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class PlantGeneticsHelper
{
	public const string TRAIT_TAG_PREFIX = "PLANT_TRAIT_";

	public const string GENETIC_HEAT_INT = "GENETIC_HEAT_INT";

	public const string DNA_DISK_TAG = "DNA_DISK_TAG";

	public const string DNA_DISK_TRAIT_STRING = "DNA_DISK_TRAIT_STRING";

	public const string DNA_DISK_RANK_INT = "DNA_DISK_RANK_INT";

	public const string DNA_DISK_USES_INT = "DNA_DISK_USES_INT";

	public const int HEAT_PER_OPERATION = 2;

	public const int OPERATION_BASE_PROGRESS = 20;

	public const int OPERATION_PROGRESS_PER_COMPLEXITY = 4;

	public const int OPERATION_BASE_SUCCESS = 95;

	public const int OPERATION_SUCCESS_PENALTY_PER_COMPLEXITY = 5;

	public const int OPERATION_MIN_SUCCESS = 5;

	public const float RANK_EXPONENT = 0.75f;

	public const float MIN_RANKED_PERCENT = -99.99f;

	public const float MIN_STAT_MULTIPLIER = 0.000100016594f;

	private static readonly int[] ROMAN_VALUES = new int[13]
	{
		1000, 900, 500, 400, 100, 90, 50, 40, 10, 9,
		5, 4, 1
	};

	private static readonly string[] ROMAN_SYMBOLS = new string[13]
	{
		"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX",
		"V", "IV", "I"
	};

	public const int SCANNER_TOOLTIP_WIDTH = 60;

	public static string DISK_BLANK_NAME => PlantGeneticsText.DiskName();

	public static float RankScale(int rank)
	{
		if (rank <= 0)
		{
			return 0f;
		}
		return Mathf.Pow(rank, 0.75f);
	}

	public static float RankedPercent(float percentAtRankOne, int rank)
	{
		if (rank <= 0)
		{
			return 0f;
		}
		return Mathf.Max(-99.99f, percentAtRankOne * RankScale(rank));
	}

	public static string RankLabel(int rank)
	{
		if (rank <= 0 || rank >= 4000)
		{
			return rank.ToString();
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < ROMAN_VALUES.Length; i++)
		{
			while (rank >= ROMAN_VALUES[i])
			{
				stringBuilder.Append(ROMAN_SYMBOLS[i]);
				rank -= ROMAN_VALUES[i];
			}
		}
		return stringBuilder.ToString();
	}

	public static string TraitTag(string traitId)
	{
		return "PLANT_TRAIT_" + traitId;
	}

	public static int GetTraitRank(GameItem item, string traitId)
	{
		if (item == null || !item.IsTag(TraitTag(traitId)))
		{
			return 0;
		}
		return item.GetTagReadonly(TraitTag(traitId)).GetInt();
	}

	public static void SetTraitRank(GameItem item, string traitId, int rank)
	{
		if (rank <= 0)
		{
			item.DisableTag(TraitTag(traitId));
		}
		else
		{
			item.ModifyTag(TraitTag(traitId), delegate(TagState state)
			{
				state.Enable().SetInt(rank);
			});
		}
		RefreshModifiedType(item);
	}

	public static bool IsModified(GameItem seed)
	{
		PlantDefinition plant = HydroponicHelper.GetPlant(seed);
		if (plant == null)
		{
			return false;
		}
		bool flag = plant.id == "nutrifruit_seed" && PlantList.IsNutrifruitFreed();
		foreach (PlantTrait item in PlantTraitRegistry.All)
		{
			if ((!flag || !(item.id == "infertile")) && GetTraitRank(seed, item.id) != plant.GetInnateRank(item.id))
			{
				return true;
			}
		}
		return false;
	}

	public static void RefreshModifiedType(GameItem seed)
	{
		if (seed != null && seed.IsTag("SEED_ITEM_TAG"))
		{
			if (IsModified(seed))
			{
				seed.SetGameItemType("MODIFIED_SEED");
			}
			else
			{
				seed.RemoveGameItemType("MODIFIED_SEED");
			}
			PlantList.RefreshNutrifruitStrain(seed);
		}
	}

	public static List<(PlantTrait trait, int rank)> GetTraits(GameItem item)
	{
		List<(PlantTrait, int)> list = new List<(PlantTrait, int)>();
		if (item == null)
		{
			return list;
		}
		foreach (PlantTrait item2 in PlantTraitRegistry.All)
		{
			int traitRank = GetTraitRank(item, item2.id);
			if (traitRank > 0)
			{
				list.Add((item2, traitRank));
			}
		}
		return list;
	}

	public static bool HasAnyTrait(GameItem item)
	{
		return GetTraits(item).Count > 0;
	}

	public static void InitInnateTraits(GameItem seed, PlantDefinition plant)
	{
		if (plant == null)
		{
			return;
		}
		foreach (KeyValuePair<string, int> innateTrait in plant.innateTraits)
		{
			SetTraitRank(seed, innateTrait.Key, innateTrait.Value);
		}
		ClearHeat(seed);
	}

	public static float GetStatMultiplier(GameItem seed, string stat)
	{
		float num = 1f;
		foreach (var trait in GetTraits(seed))
		{
			PlantTrait item = trait.trait;
			int item2 = trait.rank;
			num *= 1f + item.GetStatPercent(stat, item2) / 100f;
		}
		return Mathf.Max(0.000100016594f, num);
	}

	public static int GetComplexity(GameItem seed)
	{
		PlantDefinition plant = HydroponicHelper.GetPlant(seed);
		int num = 0;
		foreach (PlantTrait item in PlantTraitRegistry.All)
		{
			int traitRank = GetTraitRank(seed, item.id);
			int num2 = plant?.GetInnateRank(item.id) ?? 0;
			int num3 = traitRank - num2;
			num = ((num3 < 0) ? (num + Math.Abs(item.complexity) * -num3) : (num + item.complexity * num3));
			num += item.complexityOffsetPerRank * traitRank;
		}
		return num;
	}

	public static int GetHeat(GameItem seed)
	{
		if (seed == null || !seed.IsTag("GENETIC_HEAT_INT"))
		{
			return 0;
		}
		return seed.GetTagReadonly("GENETIC_HEAT_INT").GetInt();
	}

	public static void AddHeat(GameItem seed, PlantTrait trait)
	{
		int gain = 2 + ((trait != null) ? Math.Abs(trait.complexity) : 0);
		seed.ModifyTag("GENETIC_HEAT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(state.GetInt() + gain);
		});
	}

	public static void ClearHeat(GameItem seed)
	{
		seed.ModifyTag("GENETIC_HEAT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
	}

	public static int GetEffectiveComplexity(GameItem seed)
	{
		return GetComplexity(seed) + GetHeat(seed);
	}

	public static int GetOperationProgress(int effectiveComplexity)
	{
		return 20 + 4 * Math.Max(0, effectiveComplexity);
	}

	public static int GetSuccessChance(int effectiveComplexity, int qualityBonus)
	{
		return Mathf.Clamp(95 - 5 * Math.Max(0, effectiveComplexity) + qualityBonus, 5, 100);
	}

	public static bool CanApplyTrait(GameItem seed, PlantTrait trait, out string reason)
	{
		reason = "";
		if (seed == null || trait == null)
		{
			reason = PlantGeneticsText.BlockNothingToApply();
			return false;
		}
		if (trait.locked)
		{
			reason = PlantGeneticsText.BlockLockedApply(trait.displayName);
			return false;
		}
		if (!IsTraitUsable(seed, trait, out reason))
		{
			return false;
		}
		PlantTrait incompatibleTrait = GetIncompatibleTrait(seed, trait);
		if (incompatibleTrait != null)
		{
			reason = PlantGeneticsText.BlockIncompatible(trait.displayName, incompatibleTrait.displayName);
			return false;
		}
		if (GetTraitRank(seed, trait.id) >= trait.maxRank)
		{
			reason = PlantGeneticsText.BlockMaxRank(trait.displayName);
			return false;
		}
		PlantTrait exclusiveTrait = GetExclusiveTrait(seed, trait);
		if (exclusiveTrait != null)
		{
			reason = PlantGeneticsText.BlockExclusive(exclusiveTrait.displayName, trait.displayName);
			return false;
		}
		return true;
	}

	public static PlantTrait GetExclusiveTrait(GameItem seed, PlantTrait trait)
	{
		if (string.IsNullOrEmpty(trait.exclusiveGroup))
		{
			return null;
		}
		foreach (var trait2 in GetTraits(seed))
		{
			PlantTrait item = trait2.trait;
			if (!item.id.Equals(trait.id) && trait.exclusiveGroup.Equals(item.exclusiveGroup))
			{
				return item;
			}
		}
		return null;
	}

	public static bool IsTraitCompatible(PlantDefinition plant, PlantTrait trait, out string reason)
	{
		reason = "";
		if (plant == null || trait == null)
		{
			return true;
		}
		bool num = trait.ActsOnProduce && plant.HarvestIsOwnSeed && !trait.appliesToSeedProduce;
		bool flag = trait.cropRequirement != null && !trait.cropRequirement(plant);
		if (num || flag)
		{
			reason = PlantGeneticsText.BlockNoEffectOnCrop(trait.displayName, plant.displayName);
			return false;
		}
		return true;
	}

	public static PlantTrait GetIncompatibleTrait(GameItem seed, PlantTrait trait)
	{
		foreach (var trait2 in GetTraits(seed))
		{
			PlantTrait item = trait2.trait;
			if (!item.id.Equals(trait.id) && (trait.IsIncompatibleWith(item.id) || item.IsIncompatibleWith(trait.id)))
			{
				return item;
			}
		}
		return null;
	}

	public static bool IsDiskCompatibleWithSeed(GameItem disk, GameItem seed)
	{
		return GetDiskIncompatibilityReason(disk, seed).Length == 0;
	}

	public static string GetDiskIncompatibilityReason(GameItem disk, GameItem seed)
	{
		PlantTrait diskTrait = GetDiskTrait(disk);
		if (diskTrait == null || seed == null)
		{
			return "";
		}
		if (!IsTraitUsable(seed, diskTrait, out var reason))
		{
			return reason;
		}
		PlantTrait incompatibleTrait = GetIncompatibleTrait(seed, diskTrait);
		if (incompatibleTrait != null)
		{
			return PlantGeneticsText.BlockIncompatible(diskTrait.displayName, incompatibleTrait.displayName);
		}
		PlantTrait exclusiveTrait = GetExclusiveTrait(seed, diskTrait);
		if (exclusiveTrait != null)
		{
			return PlantGeneticsText.BlockExclusive(exclusiveTrait.displayName, diskTrait.displayName);
		}
		return "";
	}

	public static List<string> GetActiveLiquids(GameItem seed)
	{
		List<string> list = new List<string>();
		foreach (var trait in GetTraits(seed))
		{
			foreach (string item in trait.trait.ProducedLiquids())
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static bool HasActiveLiquid(GameItem seed)
	{
		return GetActiveLiquids(seed).Count > 0;
	}

	public static bool IsTraitUsable(GameItem seed, PlantTrait trait, out string reason)
	{
		if (!IsTraitCompatible(HydroponicHelper.GetPlant(seed), trait, out reason))
		{
			return false;
		}
		if (seed != null && trait.seedRequirement != null && !trait.seedRequirement(seed))
		{
			reason = PlantGeneticsText.BlockNothingToActOn(trait.displayName);
			return false;
		}
		return true;
	}

	public static bool IsPerennial(GameItem seed)
	{
		foreach (var trait in GetTraits(seed))
		{
			if (trait.trait.perennial)
			{
				return true;
			}
		}
		return false;
	}

	public static int ApplyTrait(GameItem seed, PlantTrait trait, int ranks = 1)
	{
		int num = GetTraitRank(seed, trait.id) + Math.Max(1, ranks);
		if (!trait.IsUnlimited)
		{
			num = Math.Min(num, trait.maxRank);
		}
		SetTraitRank(seed, trait.id, num);
		return num;
	}

	public static int RemoveTraitRank(GameItem seed, PlantTrait trait)
	{
		int num = Math.Max(0, GetTraitRank(seed, trait.id) - 1);
		SetTraitRank(seed, trait.id, num);
		return num;
	}

	public static List<PlantTrait> GetExtractableTraits(GameItem seed, Func<PlantTrait, bool> filter = null)
	{
		List<PlantTrait> list = new List<PlantTrait>();
		foreach (var trait in GetTraits(seed))
		{
			PlantTrait item = trait.trait;
			if (!item.locked && (filter == null || filter(item)))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static bool HasExtractableTrait(GameItem seed, Func<PlantTrait, bool> filter = null)
	{
		return GetExtractableTraits(seed, filter).Count > 0;
	}

	public static PlantTrait PickRandomTrait(GameItem seed, Func<PlantTrait, bool> filter = null)
	{
		List<PlantTrait> extractableTraits = GetExtractableTraits(seed, filter);
		if (extractableTraits.Count == 0)
		{
			return null;
		}
		return extractableTraits[RNG.GetRandomInt(0, extractableTraits.Count - 1)];
	}

	public static void InheritTraits(GameItem parentSeed, GameItem childSeed)
	{
		foreach (PlantTrait item in PlantTraitRegistry.All)
		{
			SetTraitRank(childSeed, item.id, GetTraitRank(parentSeed, item.id));
		}
		ClearHeat(childSeed);
	}

	public static void RunHarvestHooks(GameItem seed, GameItem item, PlantHarvestContext harvest = null, bool seedProduceOnly = false)
	{
		List<string> activeLiquids = GetActiveLiquids(seed);
		foreach (var (plantTrait, rank) in GetTraits(seed))
		{
			if (plantTrait.onHarvestItem != null && (!seedProduceOnly || plantTrait.appliesToSeedProduce))
			{
				plantTrait.onHarvestItem(new PlantTraitContext
				{
					trait = plantTrait,
					seed = seed,
					item = item,
					rank = rank,
					harvest = harvest,
					activeLiquids = activeLiquids
				});
			}
		}
	}

	public static void ApplyFactoryGenetics(GameItem item, string cropId, params string[] simulatedModuleIds)
	{
		if (item == null || PlantHarvest.isHarvesting)
		{
			return;
		}
		PlantDefinition plantDefinition = PlantRegistry.Get(cropId);
		if (plantDefinition == null)
		{
			return;
		}
		bool harvestIsOwnSeed = plantDefinition.HarvestIsOwnSeed;
		List<string> list = new List<string>();
		foreach (PlantTrait item2 in PlantTraitRegistry.All)
		{
			if (plantDefinition.GetInnateRank(item2.id) <= 0)
			{
				continue;
			}
			foreach (string item3 in item2.ProducedLiquids())
			{
				if (!list.Contains(item3))
				{
					list.Add(item3);
				}
			}
		}
		List<string> simulatedModules = new List<string>(simulatedModuleIds);
		foreach (PlantTrait item4 in PlantTraitRegistry.All)
		{
			int innateRank = plantDefinition.GetInnateRank(item4.id);
			if (innateRank > 0 && item4.onHarvestItem != null && (!harvestIsOwnSeed || item4.appliesToSeedProduce))
			{
				item4.onHarvestItem(new PlantTraitContext
				{
					trait = item4,
					seed = null,
					item = item,
					rank = innateRank,
					harvest = null,
					simulatedModules = simulatedModules,
					activeLiquids = list
				});
			}
		}
	}

	public static void RunStartGrowingHooks(GameItem seed)
	{
		foreach (var (plantTrait, arg) in GetTraits(seed))
		{
			plantTrait.onStartGrowing?.Invoke(seed, arg);
		}
	}

	public static void RunGrowTickHooks(GameItem seed, GameItem hydroponic)
	{
		foreach (var (plantTrait, arg) in GetTraits(seed))
		{
			plantTrait.onGrowTick?.Invoke(seed, hydroponic, arg);
		}
	}

	public static void InitDisk(GameItem item)
	{
		item.EnableTag("DNA_DISK_TAG");
		item.ModifyTag("DNA_DISK_TRAIT_STRING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		UpdateDiskDisplay(item);
	}

	public static bool IsDisk(GameItem item)
	{
		return item?.IsTag("DNA_DISK_TAG") ?? false;
	}

	public static bool IsBlankDisk(GameItem item)
	{
		if (IsDisk(item))
		{
			return GetDiskTrait(item) == null;
		}
		return false;
	}

	public static bool IsWrittenDisk(GameItem item)
	{
		if (IsDisk(item))
		{
			return GetDiskTrait(item) != null;
		}
		return false;
	}

	public static PlantTrait GetDiskTrait(GameItem disk)
	{
		if (!IsDisk(disk))
		{
			return null;
		}
		string @string = disk.GetTagReadonly("DNA_DISK_TRAIT_STRING").GetString();
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		return PlantTraitRegistry.Get(@string);
	}

	public static void WriteDisk(GameItem disk, PlantTrait trait, int rank = 1, int uses = 1)
	{
		disk.ModifyTag("DNA_DISK_TRAIT_STRING", delegate(TagState state)
		{
			state.Enable().SetString((trait != null) ? trait.id : "");
		});
		disk.ModifyTag("DNA_DISK_RANK_INT", delegate(TagState state)
		{
			state.Enable().SetInt((trait != null) ? Math.Max(1, rank) : 0);
		});
		disk.ModifyTag("DNA_DISK_USES_INT", delegate(TagState state)
		{
			state.Enable().SetInt((trait != null) ? Math.Max(1, uses) : 0);
		});
		UpdateDiskDisplay(disk);
	}

	public static int GetDiskRank(GameItem disk)
	{
		if (!IsWrittenDisk(disk) || !disk.IsTag("DNA_DISK_RANK_INT"))
		{
			return 1;
		}
		return Math.Max(1, disk.GetTagReadonly("DNA_DISK_RANK_INT").GetInt());
	}

	public static int GetDiskUses(GameItem disk)
	{
		if (!IsWrittenDisk(disk) || !disk.IsTag("DNA_DISK_USES_INT"))
		{
			return 1;
		}
		return Math.Max(1, disk.GetTagReadonly("DNA_DISK_USES_INT").GetInt());
	}

	public static void AddDiskUses(GameItem disk, int amount)
	{
		if (IsWrittenDisk(disk) && amount > 0)
		{
			int uses = GetDiskUses(disk) + amount;
			disk.ModifyTag("DNA_DISK_USES_INT", delegate(TagState state)
			{
				state.Enable().SetInt(uses);
			});
			UpdateDiskDisplay(disk);
		}
	}

	public static int ExtractionUses(GameItem seed, PlantTrait trait)
	{
		PlantDefinition plant = HydroponicHelper.GetPlant(seed);
		if (plant == null || trait == null || plant.GetInnateRank(trait.id) <= 0)
		{
			return 1;
		}
		return Math.Max(1, GetTraitRank(seed, trait.id));
	}

	public static bool CanExtractOntoDisk(GameItem seed, GameItem disk, out string reason)
	{
		reason = "";
		if (seed == null || disk == null || !IsDisk(disk))
		{
			return true;
		}
		PlantTrait diskTrait = GetDiskTrait(disk);
		if (diskTrait == null)
		{
			return true;
		}
		if (diskTrait.locked)
		{
			reason = PlantGeneticsText.BlockLockedExtract(diskTrait.displayName);
			return false;
		}
		if (GetTraitRank(seed, diskTrait.id) <= 0)
		{
			reason = PlantGeneticsText.BlockRefillMissing(diskTrait.displayName);
			return false;
		}
		return true;
	}

	public static bool SpendDiskUse(GameItem disk)
	{
		int left = GetDiskUses(disk) - 1;
		if (left <= 0)
		{
			return true;
		}
		disk.ModifyTag("DNA_DISK_USES_INT", delegate(TagState state)
		{
			state.Enable().SetInt(left);
		});
		UpdateDiskDisplay(disk);
		return false;
	}

	public static void UpdateDiskDisplay(GameItem disk)
	{
		PlantTrait diskTrait = GetDiskTrait(disk);
		disk.SetSpriteAndShape("Items/items_hydroponic_plant", MachineDnaManipulator.DiskSpriteFor(diskTrait));
		if (diskTrait == null)
		{
			disk.name = DISK_BLANK_NAME;
			disk.shortDescription = PlantGeneticsText.DiskBlankDescription();
		}
		else
		{
			int diskRank = GetDiskRank(disk);
			string traitLabel = ((diskRank > 1) ? PlantGeneticsText.TraitWithRank(diskTrait.displayName, RankLabel(diskRank)) : diskTrait.displayName);
			disk.name = PlantGeneticsText.NamedWithTrait(DISK_BLANK_NAME, traitLabel);
			disk.shortDescription = "";
		}
		disk.Validate();
	}

	public static bool IsScannerOn()
	{
		if (PlayerStore.Instance != null)
		{
			return PlayerStore.Instance.isGeneScannerScanning;
		}
		return false;
	}

	public static int GetTooltipWidth(GameItem item, int defaultWidth)
	{
		if (item == null || !item.IsTag("SEED_ITEM_TAG"))
		{
			return defaultWidth;
		}
		if (HydroponicHelper.GetPlant(item) == null || !IsScannerOn())
		{
			return defaultWidth;
		}
		return Math.Max(defaultWidth, 60);
	}

	public static void CreateSeedGeneticsTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item == null || !item.IsTag("SEED_ITEM_TAG") || HydroponicHelper.GetPlant(item) == null || !IsScannerOn())
		{
			return;
		}
		PlantDefinition plant = HydroponicHelper.GetPlant(item);
		List<(PlantTrait trait, int rank)> traits = GetTraits(item);
		int heat = GetHeat(item);
		builder.AddLine((heat > 0) ? PlantGeneticsText.ReadoutHeaderHeat(GetComplexity(item), heat) : PlantGeneticsText.ReadoutHeader(GetComplexity(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightPurple);
		if (traits.Count == 0)
		{
			builder.AddLine(PlantGeneticsText.ReadoutNoTraits(), ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
		}
		foreach (var item4 in traits)
		{
			PlantTrait item2 = item4.trait;
			int item3 = item4.rank;
			bool flag = plant.GetInnateRank(item2.id) >= item3;
			RenderHandler.ColorPalette color = (item2.IsNegative ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.LightGreen);
			string reason;
			bool num = IsTraitUsable(item, item2, out reason);
			string traitLabel = ((item2.maxRank > 1) ? PlantGeneticsText.TraitWithRank(item2.displayName, RankLabel(item3)) : item2.displayName);
			if (item2.locked)
			{
				traitLabel = PlantGeneticsText.OriginLocked(traitLabel);
			}
			else if (flag)
			{
				traitLabel = PlantGeneticsText.OriginInnate(traitLabel);
			}
			if (!num)
			{
				traitLabel = PlantGeneticsText.NoEffectSuffix(traitLabel);
			}
			builder.AddLine(PlantGeneticsText.ReadoutTraitLine(traitLabel, DescribeEffect(item2, item3)), ignoreEmpty: true, color);
		}
		foreach (KeyValuePair<string, int> innateTrait in plant.innateTraits)
		{
			PlantTrait plantTrait = PlantTraitRegistry.Get(innateTrait.Key);
			if (plantTrait != null && GetTraitRank(item, plantTrait.id) < innateTrait.Value)
			{
				builder.AddLine(PlantGeneticsText.ReadoutInnateRemoved(plantTrait.displayName), ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
			}
		}
	}

	public static string DescribeEffect(PlantTrait trait, int rank)
	{
		if (trait.complexityOffsetPerRank != 0)
		{
			int num = trait.complexityOffsetPerRank * Math.Max(1, rank);
			return PlantGeneticsText.EffectComplexity(((num > 0) ? "+" : "") + num);
		}
		if (trait.describeRank != null)
		{
			return trait.describeRank(Math.Max(1, rank));
		}
		if (trait.statPercentPerRank.Count == 0)
		{
			return trait.description;
		}
		if (trait.perennial)
		{
			return trait.description + " " + DescribeStatPercents(trait, rank);
		}
		return DescribeStatPercents(trait, rank);
	}

	private static string DescribeStatPercents(PlantTrait trait, int rank)
	{
		return string.Join(", ", DescribeStatPercentLines(trait, rank));
	}

	private static List<string> DescribeStatPercentLines(PlantTrait trait, int rank)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, float> item in trait.statPercentPerRank)
		{
			float statPercent = trait.GetStatPercent(item.Key, rank);
			int num = Mathf.RoundToInt(statPercent);
			string text = ((num <= -100) ? statPercent.ToString("0.##") : num.ToString());
			list.Add(PlantGeneticsText.StatPercent(PlantStat.DisplayName(item.Key), ((num >= 0) ? "+" : "") + text));
		}
		return list;
	}

	public static List<string> DescribeEffectLines(PlantTrait trait, int rank)
	{
		if (trait.perennial && trait.describeRank == null && trait.complexityOffsetPerRank == 0 && trait.statPercentPerRank.Count > 0)
		{
			List<string> list = new List<string>();
			list.Add(trait.description);
			list.AddRange(DescribeStatPercentLines(trait, rank));
			return list;
		}
		return new List<string> { DescribeEffect(trait, rank) };
	}

	public static void CreateDiskTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!IsDisk(item))
		{
			return;
		}
		PlantTrait diskTrait = GetDiskTrait(item);
		if (diskTrait == null)
		{
			builder.AddLine(PlantGeneticsText.DiskBlankTooltip(), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			return;
		}
		RenderHandler.ColorPalette color = (diskTrait.IsNegative ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.LightGreen);
		foreach (string item2 in DescribeEffectLines(diskTrait, GetDiskRank(item)))
		{
			builder.AddLine(item2, ignoreEmpty: true, color);
		}
		int diskUses = GetDiskUses(item);
		if (diskUses > 1)
		{
			builder.AddLine(PlantGeneticsText.DiskUsesRemaining(diskUses), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		builder.AddLine(PlantGeneticsText.TraitComplexity(((diskTrait.complexity >= 0) ? "+" : "") + diskTrait.complexity), ignoreEmpty: true, RenderHandler.ColorPalette.LightPurple);
		if (!diskTrait.IsUnlimited && diskTrait.maxRank > 1)
		{
			builder.AddLine(PlantGeneticsText.TraitStacksTo(RankLabel(diskTrait.maxRank)), ignoreEmpty: true, RenderHandler.ColorPalette.LightPurple);
		}
		if (!string.IsNullOrEmpty(diskTrait.exclusiveGroup))
		{
			builder.AddLine(PlantGeneticsText.TraitExclusiveWith(ExclusiveTraitNames(diskTrait)), ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
		}
		string text = IncompatibleCropNames(diskTrait);
		if (text.Length > 0)
		{
			builder.AddLine(PlantGeneticsText.TraitNoEffectOn(text), ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
		}
	}

	private static string IncompatibleCropNames(PlantTrait trait)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (PlantDefinition item in PlantRegistry.All)
		{
			if (!IsTraitCompatible(item, trait, out var _))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(item.displayName);
			}
		}
		return stringBuilder.ToString();
	}

	private static string ExclusiveTraitNames(PlantTrait trait)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (PlantTrait item in PlantTraitRegistry.All)
		{
			if (!item.id.Equals(trait.id) && trait.exclusiveGroup.Equals(item.exclusiveGroup))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(item.displayName);
			}
		}
		if (stringBuilder.Length <= 0)
		{
			return PlantGeneticsText.None();
		}
		return stringBuilder.ToString();
	}
}
